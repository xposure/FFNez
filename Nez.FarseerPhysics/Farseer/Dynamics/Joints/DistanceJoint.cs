/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using Nez.Common;
using Microsoft.Xna.Framework;

namespace Nez.Dynamics.Joints
{
	// 1-D rained system
	// m (v2 - v1) = lambda
	// v2 + (beta/h) * x1 + gamma * lambda = 0, gamma has units of inverse mass.
	// x2 = x1 + h * v2

	// 1-D mass-damper-spring system
	// m (v2 - v1) + h * d * v2 + h * k * 

	// C = norm(p2 - p1) - L
	// u = (p2 - p1) / norm(p2 - p1)
	// Cdot = dot(u, v2 + cross(w2, r2) - v1 - cross(w1, r1))
	// J = [-u -cross(r1, u) u cross(r2, u)]
	// K = J * invM * JT
	//   = invMass1 + invI1 * cross(r1, u)^2 + invMass2 + invI2 * cross(r2, u)^2

	/// <summary>
	/// A distance joint rains two points on two bodies
	/// to remain at a fixed distance from each other. You can view
	/// this as a massless, rigid rod.
	/// </summary>
	public class DistanceJoint : Joint
	{
		#region Properties/Fields

		/// <summary>
		/// The local anchor point relative to bodyA's origin.
		/// </summary>
		public vec2 localAnchorA;

		/// <summary>
		/// The local anchor point relative to bodyB's origin.
		/// </summary>
		public vec2 localAnchorB;

		public override sealed vec2 worldAnchorA
		{
			get { return bodyA.getWorldPoint( localAnchorA ); }
			set { Debug.Assert( false, "You can't set the world anchor on this joint type." ); }
		}

		public override sealed vec2 worldAnchorB
		{
			get { return bodyB.getWorldPoint( localAnchorB ); }
			set { Debug.Assert( false, "You can't set the world anchor on this joint type." ); }
		}

		/// <summary>
		/// The natural length between the anchor points.
		/// Manipulating the length can lead to non-physical behavior when the frequency is zero.
		/// </summary>
		public float length;

		/// <summary>
		/// The mass-spring-damper frequency in Hertz. A value of 0
		/// disables softness.
		/// </summary>
		public float frequency;

		/// <summary>
		/// The damping ratio. 0 = no damping, 1 = critical damping.
		/// </summary>
		public float dampingRatio;

		// Solver shared
		float _bias;
		float _gamma;
		float _impulse;

		// Solver temp
		int _indexA;
		int _indexB;
		vec2 _u;
		vec2 _rA;
		vec2 _rB;
		vec2 _localCenterA;
		vec2 _localCenterB;
		float _invMassA;
		float _invMassB;
		float _invIA;
		float _invIB;
		float _mass;

		#endregion


		internal DistanceJoint()
		{
			jointType = JointType.Distance;
		}

		/// <summary>
		/// This requires defining an
		/// anchor point on both bodies and the non-zero length of the
		/// distance joint. If you don't supply a length, the local anchor points
		/// is used so that the initial configuration can violate the constraint
		/// slightly. This helps when saving and loading a game.
		/// Warning Do not use a zero or short length.
		/// </summary>
		/// <param name="bodyA">The first body</param>
		/// <param name="bodyB">The second body</param>
		/// <param name="anchorA">The first body anchor</param>
		/// <param name="anchorB">The second body anchor</param>
		/// <param name="useWorldCoordinates">Set to true if you are using world coordinates as anchors.</param>
		public DistanceJoint( Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false ) : base( bodyA, bodyB )
		{
			jointType = JointType.Distance;

			if( useWorldCoordinates )
			{
				localAnchorA = bodyA.getLocalPoint( ref anchorA );
				localAnchorB = bodyB.getLocalPoint( ref anchorB );
				length = ( anchorB - anchorA ).Length();
			}
			else
			{
				localAnchorA = anchorA;
				localAnchorB = anchorB;
				length = ( base.bodyB.getWorldPoint( ref anchorB ) - base.bodyA.getWorldPoint( ref anchorA ) ).Length();
			}
		}

		/// <summary>
		/// Get the reaction force given the inverse time step. Unit is N.
		/// </summary>
		/// <param name="invDt"></param>
		/// <returns></returns>
		public override vec2 getReactionForce( float invDt )
		{
			vec2 F = ( invDt * _impulse ) * _u;
			return F;
		}

		/// <summary>
		/// Get the reaction torque given the inverse time step.
		/// Unit is N*m. This is always zero for a distance joint.
		/// </summary>
		/// <param name="invDt"></param>
		/// <returns></returns>
		public override float getReactionTorque( float invDt )
		{
			return 0.0f;
		}

		internal override void initVelocityConstraints( ref SolverData data )
		{
			_indexA = bodyA.islandIndex;
			_indexB = bodyB.islandIndex;
			_localCenterA = bodyA._sweep.localCenter;
			_localCenterB = bodyB._sweep.localCenter;
			_invMassA = bodyA._invMass;
			_invMassB = bodyB._invMass;
			_invIA = bodyA._invI;
			_invIB = bodyB._invI;

			vec2 cA = data.positions[_indexA].c;
			float aA = data.positions[_indexA].a;
			vec2 vA = data.velocities[_indexA].v;
			float wA = data.velocities[_indexA].w;

			vec2 cB = data.positions[_indexB].c;
			float aB = data.positions[_indexB].a;
			vec2 vB = data.velocities[_indexB].v;
			float wB = data.velocities[_indexB].w;

			Rot qA = new Rot( aA ), qB = new Rot( aB );

			_rA = MathUtils.mul( qA, localAnchorA - _localCenterA );
			_rB = MathUtils.mul( qB, localAnchorB - _localCenterB );
			_u = cB + _rB - cA - _rA;

			// Handle singularity.
			float length = _u.Length();
			if( length > Settings.linearSlop )
			{
				_u *= 1.0f / length;
			}
			else
			{
				_u = vec2.Zero;
			}

			float crAu = MathUtils.cross( _rA, _u );
			float crBu = MathUtils.cross( _rB, _u );
			float invMass = _invMassA + _invIA * crAu * crAu + _invMassB + _invIB * crBu * crBu;

			// Compute the effective mass matrix.
			_mass = invMass != 0.0f ? 1.0f / invMass : 0.0f;

			if( frequency > 0.0f )
			{
				float C = length - this.length;

				// Frequency
				float omega = 2.0f * Settings.pi * frequency;

				// Damping coefficient
				float d = 2.0f * _mass * dampingRatio * omega;

				// Spring stiffness
				float k = _mass * omega * omega;

				// magic formulas
				float h = data.step.dt;
				_gamma = h * ( d + h * k );
				_gamma = _gamma != 0.0f ? 1.0f / _gamma : 0.0f;
				_bias = C * h * k * _gamma;

				invMass += _gamma;
				_mass = invMass != 0.0f ? 1.0f / invMass : 0.0f;
			}
			else
			{
				_gamma = 0.0f;
				_bias = 0.0f;
			}

			if( Settings.enableWarmstarting )
			{
				// Scale the impulse to support a variable time step.
				_impulse *= data.step.dtRatio;

				vec2 P = _impulse * _u;
				vA -= _invMassA * P;
				wA -= _invIA * MathUtils.cross( _rA, P );
				vB += _invMassB * P;
				wB += _invIB * MathUtils.cross( _rB, P );
			}
			else
			{
				_impulse = 0.0f;
			}

			data.velocities[_indexA].v = vA;
			data.velocities[_indexA].w = wA;
			data.velocities[_indexB].v = vB;
			data.velocities[_indexB].w = wB;
		}

		internal override void solveVelocityConstraints( ref SolverData data )
		{
			vec2 vA = data.velocities[_indexA].v;
			float wA = data.velocities[_indexA].w;
			vec2 vB = data.velocities[_indexB].v;
			float wB = data.velocities[_indexB].w;

			// Cdot = dot(u, v + cross(w, r))
			vec2 vpA = vA + MathUtils.cross( wA, _rA );
			vec2 vpB = vB + MathUtils.cross( wB, _rB );
			float Cdot = vec2.Dot( _u, vpB - vpA );

			float impulse = -_mass * ( Cdot + _bias + _gamma * _impulse );
			_impulse += impulse;

			vec2 P = impulse * _u;
			vA -= _invMassA * P;
			wA -= _invIA * MathUtils.cross( _rA, P );
			vB += _invMassB * P;
			wB += _invIB * MathUtils.cross( _rB, P );

			data.velocities[_indexA].v = vA;
			data.velocities[_indexA].w = wA;
			data.velocities[_indexB].v = vB;
			data.velocities[_indexB].w = wB;

		}

		internal override bool solvePositionConstraints( ref SolverData data )
		{
			if( frequency > 0.0f )
			{
				// There is no position correction for soft distance constraints.
				return true;
			}

			var cA = data.positions[_indexA].c;
			var aA = data.positions[_indexA].a;
			var cB = data.positions[_indexB].c;
			var aB = data.positions[_indexB].a;

			Rot qA = new Rot( aA ), qB = new Rot( aB );

			var rA = MathUtils.mul( qA, localAnchorA - _localCenterA );
			var rB = MathUtils.mul( qB, localAnchorB - _localCenterB );
			var u = cB + rB - cA - rA;

			var length = u.Length();
            u.Normalize();
			//Nez.Vector2Ext.normalize( ref u );
			var C = length - this.length;
			C = MathUtils.clamp( C, -Settings.maxLinearCorrection, Settings.maxLinearCorrection );

			var impulse = -_mass * C;
			var P = impulse * u;

			cA -= _invMassA * P;
			aA -= _invIA * MathUtils.cross( rA, P );
			cB += _invMassB * P;
			aB += _invIB * MathUtils.cross( rB, P );

			data.positions[_indexA].c = cA;
			data.positions[_indexA].a = aA;
			data.positions[_indexB].c = cB;
			data.positions[_indexB].a = aB;

			return Math.Abs( C ) < Settings.linearSlop;
		}
	
	}
}