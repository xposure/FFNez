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
using Nez.Common;
using Microsoft.Xna.Framework;


namespace Nez.Dynamics.Joints
{
	// Limit:
	// C = norm(pB - pA) - L
	// u = (pB - pA) / norm(pB - pA)
	// Cdot = dot(u, vB + cross(wB, rB) - vA - cross(wA, rA))
	// J = [-u -cross(rA, u) u cross(rB, u)]
	// K = J * invM * JT
	//   = invMassA + invIA * cross(rA, u)^2 + invMassB + invIB * cross(rB, u)^2

	/// <summary>
	/// A rope joint enforces a maximum distance between two points on two bodies. It has no other effect.
	/// It can be used on ropes that are made up of several connected bodies, and if there is a need to support a heavy body.
	/// This joint is used for stabiliation of heavy objects on soft constraint joints.
	/// 
	/// Warning: if you attempt to change the maximum length during the simulation you will get some non-physical behavior.
	/// Use the DistanceJoint instead if you want to dynamically control the length.
	/// </summary>
	public class RopeJoint : Joint
	{
		#region Properties/Fields

		/// <summary>
		/// The local anchor point on BodyA
		/// </summary>
		public vec2 localAnchorA;

		/// <summary>
		/// The local anchor point on BodyB
		/// </summary>
		public vec2 localAnchorB;

		public override sealed vec2 worldAnchorA
		{
			get { return bodyA.getWorldPoint( localAnchorA ); }
			set { localAnchorA = bodyA.getLocalPoint( value ); }
		}

		public override sealed vec2 worldAnchorB
		{
			get { return bodyB.getWorldPoint( localAnchorB ); }
			set { localAnchorB = bodyB.getLocalPoint( value ); }
		}

		/// <summary>
		/// Get or set the maximum length of the rope.
		/// By default, it is the distance between the two anchor points.
		/// </summary>
		public float maxLength;

		/// <summary>
		/// Gets the state of the joint.
		/// </summary>
		public LimitState state { get; private set; }

		// Solver shared
		float _impulse;
		float _length;

		// Solver temp
		int _indexA;
		int _indexB;
		vec2 _localCenterA;
		vec2 _localCenterB;
		float _invMassA;
		private float _invMassB;
		float _invIA;
		float _invIB;
		float _mass;
		private vec2 _rA, _rB;
		vec2 _u;

		#endregion


		internal RopeJoint()
		{
			jointType = JointType.Rope;
		}

		/// <summary>
		/// Constructor for RopeJoint.
		/// </summary>
		/// <param name="bodyA">The first body</param>
		/// <param name="bodyB">The second body</param>
		/// <param name="anchorA">The anchor on the first body</param>
		/// <param name="anchorB">The anchor on the second body</param>
		/// <param name="useWorldCoordinates">Set to true if you are using world coordinates as anchors.</param>
		public RopeJoint( Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
			: base( bodyA, bodyB )
		{
			jointType = JointType.Rope;

			if( useWorldCoordinates )
			{
				localAnchorA = bodyA.getLocalPoint( anchorA );
				localAnchorB = bodyB.getLocalPoint( anchorB );
			}
			else
			{
				localAnchorA = anchorA;
				localAnchorB = anchorB;
			}

			//FPE feature: Setting default MaxLength
			vec2 d = worldAnchorB - worldAnchorA;
			maxLength = d.Length();
		}

		public override vec2 getReactionForce( float invDt )
		{
			return ( invDt * _impulse ) * _u;
		}

		public override float getReactionTorque( float invDt )
		{
			return 0;
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

			_length = _u.Length();

			float C = _length - maxLength;
			if( C > 0.0f )
			{
				state = LimitState.AtUpper;
			}
			else
			{
				state = LimitState.Inactive;
			}

			if( _length > Settings.linearSlop )
			{
				_u *= 1.0f / _length;
			}
			else
			{
				_u = vec2.Zero;
				_mass = 0.0f;
				_impulse = 0.0f;
				return;
			}

			// Compute effective mass.
			float crA = MathUtils.cross( _rA, _u );
			float crB = MathUtils.cross( _rB, _u );
			float invMass = _invMassA + _invIA * crA * crA + _invMassB + _invIB * crB * crB;

			_mass = invMass != 0.0f ? 1.0f / invMass : 0.0f;

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
			float C = _length - maxLength;
			float Cdot = vec2.Dot( _u, vpB - vpA );

			// Predictive constraint.
			if( C < 0.0f )
			{
				Cdot += data.step.inv_dt * C;
			}

			float impulse = -_mass * Cdot;
			float oldImpulse = _impulse;
			_impulse = Math.Min( 0.0f, _impulse + impulse );
			impulse = _impulse - oldImpulse;

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
			float C = length - maxLength;

			C = MathUtils.clamp( C, 0.0f, Settings.maxLinearCorrection );

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

			return length - maxLength < Settings.linearSlop;
		}
	
	}
}