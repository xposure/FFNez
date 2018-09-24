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

using System.Diagnostics;
using Nez.Common;
using Microsoft.Xna.Framework;


namespace Nez.Dynamics.Joints
{
	/// <summary>
	/// A motor joint is used to control the relative motion
	/// between two bodies. A typical usage is to control the movement
	/// of a dynamic body with respect to the ground.
	/// </summary>
	public class MotorJoint : Joint
	{
		#region Properties/Fields

		public override vec2 worldAnchorA
		{
			get { return bodyA.position; }
			set { Debug.Assert( false, "You can't set the world anchor on this joint type." ); }
		}

		public override vec2 worldAnchorB
		{
			get { return bodyB.position; }
			set { Debug.Assert( false, "You can't set the world anchor on this joint type." ); }
		}

		/// <summary>
		/// The maximum amount of force that can be applied to BodyA
		/// </summary>
		public float maxForce
		{
			set
			{
				Debug.Assert( MathUtils.isValid( value ) && value >= 0.0f );
				_maxForce = value;
			}
			get { return _maxForce; }
		}

		/// <summary>
		/// The maximum amount of torque that can be applied to BodyA
		/// </summary>
		public float maxTorque
		{
			set
			{
				Debug.Assert( MathUtils.isValid( value ) && value >= 0.0f );
				_maxTorque = value;
			}
			get { return _maxTorque; }
		}

		/// <summary>
		/// The linear (translation) offset.
		/// </summary>
		public vec2 linearOffset
		{
			set
			{
				if( _linearOffset.x != value.x || _linearOffset.y != value.y )
				{
					wakeBodies();
					_linearOffset = value;
				}
			}
			get { return _linearOffset; }
		}

		/// <summary>
		/// Get or set the angular offset.
		/// </summary>
		public float angularOffset
		{
			set
			{
				if( _angularOffset != value )
				{
					wakeBodies();
					_angularOffset = value;
				}
			}
			get { return _angularOffset; }
		}

		// FPE note: Used for serialization.
		internal float correctionFactor;

		// Solver shared
		vec2 _linearOffset;
		float _angularOffset;
		vec2 _linearImpulse;
		float _angularImpulse;
		float _maxForce;
		float _maxTorque;

		// Solver temp
		int _indexA;
		int _indexB;
		vec2 _rA;
		vec2 _rB;
		vec2 _localCenterA;
		vec2 _localCenterB;
		vec2 _linearError;
		float _angularError;
		float _invMassA;
		float _invMassB;
		float _invIA;
		float _invIB;
		Mat22 _linearMass;
		float _angularMass;

		#endregion


		internal MotorJoint()
		{
			jointType = JointType.Motor;
		}

		/// <summary>
		/// Constructor for MotorJoint.
		/// </summary>
		/// <param name="bodyA">The first body</param>
		/// <param name="bodyB">The second body</param>
		/// <param name="useWorldCoordinates">Set to true if you are using world coordinates as anchors.</param>
		public MotorJoint( Body bodyA, Body bodyB, bool useWorldCoordinates = false ) : base( bodyA, bodyB )
		{
			jointType = JointType.Motor;

			vec2 xB = base.bodyB.position;

			if( useWorldCoordinates )
				_linearOffset = base.bodyA.getLocalPoint( xB );
			else
				_linearOffset = xB;

			//Defaults
			_angularOffset = 0.0f;
			_maxForce = 1.0f;
			_maxTorque = 1.0f;
			correctionFactor = 0.3f;

			_angularOffset = base.bodyB.rotation - base.bodyA.rotation;
		}

		public override vec2 getReactionForce( float invDt )
		{
			return invDt * _linearImpulse;
		}

		public override float getReactionTorque( float invDt )
		{
			return invDt * _angularImpulse;
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

			Rot qA = new Rot( aA );
			Rot qB = new Rot( aB );

			// Compute the effective mass matrix.
			_rA = MathUtils.mul( qA, -_localCenterA );
			_rB = MathUtils.mul( qB, -_localCenterB );

			// J = [-I -r1_skew I r2_skew]
			//     [ 0       -1 0       1]
			// r_skew = [-ry; rx]

			// Matlab
			// K = [ mA+r1y^2*iA+mB+r2y^2*iB,  -r1y*iA*r1x-r2y*iB*r2x,          -r1y*iA-r2y*iB]
			//     [  -r1y*iA*r1x-r2y*iB*r2x, mA+r1x^2*iA+mB+r2x^2*iB,           r1x*iA+r2x*iB]
			//     [          -r1y*iA-r2y*iB,           r1x*iA+r2x*iB,                   iA+iB]

			float mA = _invMassA, mB = _invMassB;
			float iA = _invIA, iB = _invIB;

			Mat22 K = new Mat22();
			K.ex.x = mA + mB + iA * _rA.y * _rA.y + iB * _rB.y * _rB.y;
			K.ex.y = -iA * _rA.x * _rA.y - iB * _rB.x * _rB.y;
			K.ey.x = K.ex.y;
			K.ey.y = mA + mB + iA * _rA.x * _rA.x + iB * _rB.x * _rB.x;

			_linearMass = K.Inverse;

			_angularMass = iA + iB;
			if( _angularMass > 0.0f )
			{
				_angularMass = 1.0f / _angularMass;
			}

			_linearError = cB + _rB - cA - _rA - MathUtils.mul( qA, _linearOffset );
			_angularError = aB - aA - _angularOffset;

			if( Settings.enableWarmstarting )
			{
				// Scale impulses to support a variable time step.
				_linearImpulse *= data.step.dtRatio;
				_angularImpulse *= data.step.dtRatio;

				vec2 P = new vec2( _linearImpulse.x, _linearImpulse.y );

				vA -= mA * P;
				wA -= iA * ( MathUtils.cross( _rA, P ) + _angularImpulse );
				vB += mB * P;
				wB += iB * ( MathUtils.cross( _rB, P ) + _angularImpulse );
			}
			else
			{
				_linearImpulse = vec2.Zero;
				_angularImpulse = 0.0f;
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

			float mA = _invMassA, mB = _invMassB;
			float iA = _invIA, iB = _invIB;

			float h = data.step.dt;
			float inv_h = data.step.inv_dt;

			// Solve angular friction
			{
				float Cdot = wB - wA + inv_h * correctionFactor * _angularError;
				float impulse = -_angularMass * Cdot;

				float oldImpulse = _angularImpulse;
				float maxImpulse = h * _maxTorque;
				_angularImpulse = MathUtils.clamp( _angularImpulse + impulse, -maxImpulse, maxImpulse );
				impulse = _angularImpulse - oldImpulse;

				wA -= iA * impulse;
				wB += iB * impulse;
			}

			// Solve linear friction
			{
				var Cdot = vB + MathUtils.cross( wB, _rB ) - vA - MathUtils.cross( wA, _rA ) + inv_h * correctionFactor * _linearError;

				var impulse = -MathUtils.mul( ref _linearMass, ref Cdot );
				var oldImpulse = _linearImpulse;
				_linearImpulse += impulse;

				var maxImpulse = h * _maxForce;
				if( _linearImpulse.LengthSquared() > maxImpulse * maxImpulse )
				{
                    _linearImpulse.Normalize();
					//Nez.Vector2Ext.normalize( ref _linearImpulse );
					_linearImpulse *= maxImpulse;
				}

				impulse = _linearImpulse - oldImpulse;

				vA -= mA * impulse;
				wA -= iA * MathUtils.cross( _rA, impulse );

				vB += mB * impulse;
				wB += iB * MathUtils.cross( _rB, impulse );
			}

			data.velocities[_indexA].v = vA;
			data.velocities[_indexA].w = wA;
			data.velocities[_indexB].v = vB;
			data.velocities[_indexB].w = wB;
		}

		internal override bool solvePositionConstraints( ref SolverData data )
		{
			return true;
		}
	
	}
}