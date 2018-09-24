﻿using System;
using System.Collections.Generic;
using Nez.Collision;
using Nez.Dynamics;
using Microsoft.Xna.Framework;


namespace Nez.Common.PhysicsLogic
{
	/// <summary>
	/// Creates a simple explosion that ignores other bodies hiding behind static bodies.
	/// </summary>
	public sealed class SimpleExplosion : PhysicsLogic
	{
		/// <summary>
		/// This is the power used in the power function. A value of 1 means the force
		/// applied to bodies in the explosion is linear. A value of 2 means it is exponential.
		/// </summary>
		public float power;


		public SimpleExplosion( World world ) : base( world, PhysicsLogicType.Explosion )
		{
			power = 1; //linear
		}


		/// <summary>
		/// Activate the explosion at the specified position.
		/// </summary>
		/// <param name="pos">The position (center) of the explosion.</param>
		/// <param name="radius">The radius of the explosion.</param>
		/// <param name="force">The force applied</param>
		/// <param name="maxForce">A maximum amount of force. When force gets over this value, it will be equal to maxForce</param>
		/// <returns>A list of bodies and the amount of force that was applied to them.</returns>
		public Dictionary<Body, vec2> activate( vec2 pos, float radius, float force, float maxForce = float.MaxValue )
		{
			var affectedBodies = new HashSet<Body>();

			AABB aabb;
			aabb.lowerBound = pos - new vec2( radius );
			aabb.upperBound = pos + new vec2( radius );
			var radiusSquared = radius * radius;

			// Query the world for bodies within the radius.
			world.queryAABB( fixture =>
			{
				if( vec2.DistanceSquared( fixture.body.position, pos ) <= radiusSquared )
					 affectedBodies.Add( fixture.body );

				 return true;
			}, ref aabb );

			return applyImpulse( pos, radius, force, maxForce, affectedBodies );
		}


		Dictionary<Body, vec2> applyImpulse( vec2 pos, float radius, float force, float maxForce, HashSet<Body> overlappingBodies )
		{
			Dictionary<Body, vec2> forces = new Dictionary<Body, vec2>( overlappingBodies.Count );

			foreach( Body overlappingBody in overlappingBodies )
			{
				if( isActiveOn( overlappingBody ) )
				{
					var distance = vec2.Distance( pos, overlappingBody.position );
					var forcePercent = getPercent( distance, radius );

					var forceVector = pos - overlappingBody.position;
					forceVector *= 1f / (float)Math.Sqrt( forceVector.x * forceVector.x + forceVector.y * forceVector.y );
					forceVector *= MathHelper.Min( force * forcePercent, maxForce );
					forceVector *= -1;

					overlappingBody.applyLinearImpulse( forceVector );
					forces.Add( overlappingBody, forceVector );
				}
			}

			return forces;
		}


		float getPercent( float distance, float radius )
		{
			//(1-(distance/radius))^power-1
			float percent = (float)Math.Pow( 1 - ( ( distance - radius ) / radius ), power ) - 1;

			if( float.IsNaN( percent ) )
				return 0f;

			return MathHelper.Clamp( percent, 0f, 1f );
		}
	
	}
}