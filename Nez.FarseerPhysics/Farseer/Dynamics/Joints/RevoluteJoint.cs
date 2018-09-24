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
	/// <summary>
	/// A revolute joint constrains to bodies to share a common point while they
	/// are free to rotate about the point. The relative rotation about the shared
	/// point is the joint angle. You can limit the relative rotation with
	/// a joint limit that specifies a lower and upper angle. You can use a motor
	/// to drive the relative rotation about the shared point. A maximum motor torque
	/// is provided so that infinite forces are not generated.
	/// </summary>
	public class RevoluteJoint : Joint
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

		public override vec2 worldAnchorA
		{
			get { return bodyA.getWorldPoint( localAnchorA ); }
			set { localAnchorA = bodyA.getLocalPoint( value ); }
		}

		public override vec2 worldAnchorB
		{
			get { return bodyB.getWorldPoint( localAnchorB ); }
			set { localAnchorB = bodyB.getLocalPoint( value ); }
		}

		/// <summary>
		/// The referance angle computed as BodyB angle minus BodyA angle.
		/// </summary>
		public float referenceAngle
		{
			get { return _referenceAngle; }
			set
			{
				wakeBodies();
				_referenceAngle = value;
			}
		}

		/// <summary>
		/// Get the current joint angle in radians.
		/// </summary>
		public float jointAngle
		{
			get { return bodyB._sweep.a - bodyA._sweep.a - referenceAngle; }
		}

		/// <summary>
		/// Get the current joint angle speed in radians per second.
		/// </summary>
		public float jointSpeed
		{
			get { return bodyB._angularVelocity - bodyA._angularVelocity; }
		}

		/// <summary>
		/// Is the joint limit enabled?
		/// </summary>
		/// <value><c>true</c> if [limit enabled]; otherwise, <c>false</c>.</value>
		public bool limitEnabled
		{
			get { return _enableLimit; }
			set
			{
				if( _enableLimit != value )
				{
					wakeBodies();
					_enableLimit = value;
					_impulse.Z = 0.0f;
				}
			}
		}

		/// <summary>
		/// Get the lower joint limit in radians.
		/// </summary>
		public float lowerLimit
		{
			get { return _lowerAngle; }
			set
			{
				if( _lowerAngle != value )
				{
					wakeBodies();
					_lowerAngle = value;
					_impulse.Z = 0.0f;
				}
			}
		}

		/// <summary>
		/// Get the upper joint limit in radians.
		/// </summary>
		public float upperLimit
		{
			get { return _upperAngle; }
			set
			{
				if( _upperAngle != value )
				{
					wakeBodies();
					_upperAngle = value;
					_impulse.Z = 0.0f;
				}
			}
		}

		/// <summary>
		/// Is the joint motor enabled?
		/// </summary>
		/// <value><c>true</c> if [motor enabled]; otherwise, <c>false</c>.</value>
		public bool motorEnabled
		{
			get { return _enableMotor; }
			set
			{
				wakeBodies();
				_enableMotor = value;
			}
		}

		/// <summary>
		/// Get or set the motor speed in radians per second.
		/// </summary>
		public float motorSpeed
		{
			set
			{
				wakeBodies();
				_motorSpeed = value;
			}
			get { return _motorSpeed; }
		}

		/// <summary>
		/// Get or set the maximum motor torque, usually in N-m.
		/// </summary>
		public float maxMotorTorque
		{
			set
			{
				wakeBodies();
				_maxMotorTorque = value;
			}
			get { return _maxMotorTorque; }
		}

		/// <summary>
		/// Get or set the current motor impulse, usually in N-m.
		/// </summary>
		public float motorImpulse
		{
			get { return _motorImpulse; }
			set
			{
				wakeBodies();
				_motorImpulse = value;
			}
		}

		// Solver shared
		Vector3 _impulse;
		float _motorImpulse;

		bool _enableMotor;
		float _maxMotorTorque;
		float _motorSpeed;

		bool _enableLimit;
		float _referenceAngle;
		float _lowerAngle;
		float _upperAngle;

		// Solver temp
		int _indexA;
		int _indexB;
		vec2 _rA;
		vec2 _rB;
		vec2 _localCenterA;
		vec2 _localCenterB;
		float _invMassA;
		float _invMassB;
		float _invIA;
		float _invIB;
		Mat33 _mass;            // effective mass for point-to-point constraint.
		float _motorMass;       // effective mass for motor/limit angular constraint.
		LimitState _limitState;

		#endregion


		internal RevoluteJoint()
		{
			jointType = JointType.Revolute;
		}

		/// <summary>
		/// Constructor of RevoluteJoint. 
		/// </summary>
		/// <param name="bodyA">The first body.</param>
		/// <param name="bodyB">The second body.</param>
		/// <param name="anchorA">The first body anchor.</param>
		/// <param name="anchorB">The second anchor.</param>
		/// <param name="useWorldCoordinates">Set to true if you are using world coordinates as anchors.</param>
		public RevoluteJoint( Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
			: base( bodyA, bodyB )
		{
			jointType = JointType.Revolute;

			if( useWorldCoordinates )
			{
				localAnchorA = base.bodyA.getLocalPoint( anchorA );
				localAnchorB = base.bodyB.getLocalPoint( anchorB );
			}
			else
			{
				localAnchorA = anchorA;
				localAnchorB = anchorB;
			}

			referenceAngle = base.bodyB.rotation - base.bodyA.rotation;

			_impulse = Vector3.Zero;
			_limitState = LimitState.Inactive;
		}

		/// <summary>
		/// Constructor of RevoluteJoint. 
		/// </summary>
		/// <param name="bodyA">The first body.</param>
		/// <param name="bodyB">The second body.</param>
		/// <param name="anchor">The shared anchor.</param>
		/// <param name="useWorldCoordinates"></param>
		public RevoluteJoint( Body bodyA, Body bodyB, vec2 anchor, bool useWorldCoordinates = false )
			: this( bodyA, bodyB, anchor, anchor, useWorldCoordinates )
		{
		}

		/// <summary>
		/// Set the joint limits, usually in meters.
		/// </summary>
		/// <param name="lower">The lower limit</param>
		/// <param name="upper">The upper limit</param>
		public void setLimits( float lower, float upper )
		{
			if( lower != _lowerAngle || upper != _upperAngle )
			{
				wakeBodies();
				_upperAngle = upper;
				_lowerAngle = lower;
				_impulse.Z = 0.0f;
			}
		}

		/// <summary>
		/// Gets the motor torque in N-m.
		/// </summary>
		/// <param name="invDt">The inverse delta time</param>
		public float getMotorTorque( float invDt )
		{
			return invDt * _motorImpulse;
		}

		public override vec2 getReactionForce( float invDt )
		{
			var p = new vec2( _impulse.X, _impulse.Y );
			return invDt * p;
		}

		public override float getReactionTorque( float invDt )
		{
			return invDt * _impulse.Z;
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

			float aA = data.positions[_indexA].a;
			vec2 vA = data.velocities[_indexA].v;
			float wA = data.velocities[_indexA].w;

			float aB = data.positions[_indexB].a;
			vec2 vB = data.velocities[_indexB].v;
			float wB = data.velocities[_indexB].w;

			Rot qA = new Rot( aA ), qB = new Rot( aB );

			_rA = MathUtils.mul( qA, localAnchorA - _localCenterA );
			_rB = MathUtils.mul( qB, localAnchorB - _localCenterB );

			// J = [-I -r1_skew I r2_skew]
			//     [ 0       -1 0       1]
			// r_skew = [-ry; rx]

			// Matlab
			// K = [ mA+r1y^2*iA+mB+r2y^2*iB,  -r1y*iA*r1x-r2y*iB*r2x,          -r1y*iA-r2y*iB]
			//     [  -r1y*iA*r1x-r2y*iB*r2x, mA+r1x^2*iA+mB+r2x^2*iB,           r1x*iA+r2x*iB]
			//     [          -r1y*iA-r2y*iB,           r1x*iA+r2x*iB,                   iA+iB]

			float mA = _invMassA, mB = _invMassB;
			float iA = _invIA, iB = _invIB;

			bool fixedRotation = ( iA + iB == 0.0f );

			_mass.ex.X = mA + mB + _rA.y * _rA.y * iA + _rB.y * _rB.y * iB;
			_mass.ey.X = -_rA.y * _rA.x * iA - _rB.y * _rB.x * iB;
			_mass.ez.X = -_rA.y * iA - _rB.y * iB;
			_mass.ex.Y = _mass.ey.X;
			_mass.ey.Y = mA + mB + _rA.x * _rA.x * iA + _rB.x * _rB.x * iB;
			_mass.ez.Y = _rA.x * iA + _rB.x * iB;
			_mass.ex.Z = _mass.ez.X;
			_mass.ey.Z = _mass.ez.Y;
			_mass.ez.Z = iA + iB;

			_motorMass = iA + iB;
			if( _motorMass > 0.0f )
			{
				_motorMass = 1.0f / _motorMass;
			}

			if( _enableMotor == false || fixedRotation )
			{
				_motorImpulse = 0.0f;
			}

			if( _enableLimit && fixedRotation == false )
			{
				float jointAngle = aB - aA - referenceAngle;
				if( Math.Abs( _upperAngle - _lowerAngle ) < 2.0f * Settings.angularSlop )
				{
					_limitState = LimitState.Equal;
				}
				else if( jointAngle <= _lowerAngle )
				{
					if( _limitState != LimitState.AtLower )
					{
						_impulse.Z = 0.0f;
					}
					_limitState = LimitState.AtLower;
				}
				else if( jointAngle >= _upperAngle )
				{
					if( _limitState != LimitState.AtUpper )
					{
						_impulse.Z = 0.0f;
					}
					_limitState = LimitState.AtUpper;
				}
				else
				{
					_limitState = LimitState.Inactive;
					_impulse.Z = 0.0f;
				}
			}
			else
			{
				_limitState = LimitState.Inactive;
			}

			if( Settings.enableWarmstarting )
			{
				// Scale impulses to support a variable time step.
				_impulse *= data.step.dtRatio;
				_motorImpulse *= data.step.dtRatio;

				vec2 P = new vec2( _impulse.X, _impulse.Y );

				vA -= mA * P;
				wA -= iA * ( MathUtils.cross( _rA, P ) + motorImpulse + _impulse.Z );

				vB += mB * P;
				wB += iB * ( MathUtils.cross( _rB, P ) + motorImpulse + _impulse.Z );
			}
			else
			{
				_impulse = Vector3.Zero;
				_motorImpulse = 0.0f;
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

			bool fixedRotation = ( iA + iB == 0.0f );

			// Solve motor constraint.
			if( _enableMotor && _limitState != LimitState.Equal && fixedRotation == false )
			{
				float Cdot = wB - wA - _motorSpeed;
				float impulse = _motorMass * ( -Cdot );
				float oldImpulse = _motorImpulse;
				float maxImpulse = data.step.dt * _maxMotorTorque;
				_motorImpulse = MathUtils.clamp( _motorImpulse + impulse, -maxImpulse, maxImpulse );
				impulse = _motorImpulse - oldImpulse;

				wA -= iA * impulse;
				wB += iB * impulse;
			}

			// Solve limit constraint.
			if( _enableLimit && _limitState != LimitState.Inactive && fixedRotation == false )
			{
				vec2 Cdot1 = vB + MathUtils.cross( wB, _rB ) - vA - MathUtils.cross( wA, _rA );
				float Cdot2 = wB - wA;
				Vector3 Cdot = new Vector3( Cdot1.x, Cdot1.y, Cdot2 );

				Vector3 impulse = -_mass.Solve33( Cdot );

				if( _limitState == LimitState.Equal )
				{
					_impulse += impulse;
				}
				else if( _limitState == LimitState.AtLower )
				{
					float newImpulse = _impulse.Z + impulse.Z;
					if( newImpulse < 0.0f )
					{
						vec2 rhs = -Cdot1 + _impulse.Z * new vec2( _mass.ez.X, _mass.ez.Y );
						vec2 reduced = _mass.Solve22( rhs );
						impulse.X = reduced.x;
						impulse.Y = reduced.y;
						impulse.Z = -_impulse.Z;
						_impulse.X += reduced.x;
						_impulse.Y += reduced.y;
						_impulse.Z = 0.0f;
					}
					else
					{
						_impulse += impulse;
					}
				}
				else if( _limitState == LimitState.AtUpper )
				{
					float newImpulse = _impulse.Z + impulse.Z;
					if( newImpulse > 0.0f )
					{
						vec2 rhs = -Cdot1 + _impulse.Z * new vec2( _mass.ez.X, _mass.ez.Y );
						vec2 reduced = _mass.Solve22( rhs );
						impulse.X = reduced.x;
						impulse.Y = reduced.y;
						impulse.Z = -_impulse.Z;
						_impulse.X += reduced.x;
						_impulse.Y += reduced.y;
						_impulse.Z = 0.0f;
					}
					else
					{
						_impulse += impulse;
					}
				}

				vec2 P = new vec2( impulse.X, impulse.Y );

				vA -= mA * P;
				wA -= iA * ( MathUtils.cross( _rA, P ) + impulse.Z );

				vB += mB * P;
				wB += iB * ( MathUtils.cross( _rB, P ) + impulse.Z );
			}
			else
			{
				// Solve point-to-point constraint
				vec2 Cdot = vB + MathUtils.cross( wB, _rB ) - vA - MathUtils.cross( wA, _rA );
				vec2 impulse = _mass.Solve22( -Cdot );

				_impulse.X += impulse.x;
				_impulse.Y += impulse.y;

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
			vec2 cA = data.positions[_indexA].c;
			float aA = data.positions[_indexA].a;
			vec2 cB = data.positions[_indexB].c;
			float aB = data.positions[_indexB].a;

			Rot qA = new Rot( aA ), qB = new Rot( aB );

			float angularError = 0.0f;
			float positionError;

			bool fixedRotation = ( _invIA + _invIB == 0.0f );

			// Solve angular limit constraint.
			if( _enableLimit && _limitState != LimitState.Inactive && fixedRotation == false )
			{
				float angle = aB - aA - referenceAngle;
				float limitImpulse = 0.0f;

				if( _limitState == LimitState.Equal )
				{
					// Prevent large angular corrections
					float C = MathUtils.clamp( angle - _lowerAngle, -Settings.maxAngularCorrection, Settings.maxAngularCorrection );
					limitImpulse = -_motorMass * C;
					angularError = Math.Abs( C );
				}
				else if( _limitState == LimitState.AtLower )
				{
					float C = angle - _lowerAngle;
					angularError = -C;

					// Prevent large angular corrections and allow some slop.
					C = MathUtils.clamp( C + Settings.angularSlop, -Settings.maxAngularCorrection, 0.0f );
					limitImpulse = -_motorMass * C;
				}
				else if( _limitState == LimitState.AtUpper )
				{
					float C = angle - _upperAngle;
					angularError = C;

					// Prevent large angular corrections and allow some slop.
					C = MathUtils.clamp( C - Settings.angularSlop, 0.0f, Settings.maxAngularCorrection );
					limitImpulse = -_motorMass * C;
				}

				aA -= _invIA * limitImpulse;
				aB += _invIB * limitImpulse;
			}

			// Solve point-to-point constraint.
			{
				qA.Set( aA );
				qB.Set( aB );
				vec2 rA = MathUtils.mul( qA, localAnchorA - _localCenterA );
				vec2 rB = MathUtils.mul( qB, localAnchorB - _localCenterB );

				vec2 C = cB + rB - cA - rA;
				positionError = C.Length();

				float mA = _invMassA, mB = _invMassB;
				float iA = _invIA, iB = _invIB;

				Mat22 K = new Mat22();
				K.ex.x = mA + mB + iA * rA.y * rA.y + iB * rB.y * rB.y;
				K.ex.y = -iA * rA.x * rA.y - iB * rB.x * rB.y;
				K.ey.x = K.ex.y;
				K.ey.y = mA + mB + iA * rA.x * rA.x + iB * rB.x * rB.x;

				vec2 impulse = -K.Solve( C );

				cA -= mA * impulse;
				aA -= iA * MathUtils.cross( rA, impulse );

				cB += mB * impulse;
				aB += iB * MathUtils.cross( rB, impulse );
			}

			data.positions[_indexA].c = cA;
			data.positions[_indexA].a = aA;
			data.positions[_indexB].c = cB;
			data.positions[_indexB].a = aB;

			return positionError <= Settings.linearSlop && angularError <= Settings.angularSlop;
		}
	}
}