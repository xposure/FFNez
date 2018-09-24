using System;
using Microsoft.Xna.Framework;


namespace Nez.PhysicsShapes
{
	public class Circle : Shape
	{
		public float radius;
		internal float _originalRadius;


		public Circle( float radius )
		{
			this.radius = radius;
			_originalRadius = radius;
		}


		#region Shape abstract methods

		/// <summary>
		/// internal hack used by Particles so they can reuse a Circle for all collision checks
		/// </summary>
		/// <param name="radius">Radius.</param>
		/// <param name="position">Position.</param>
		internal void recalculateBounds( float radius, vec2 position )
		{
			_originalRadius = radius;
			this.radius = radius;
			this.position = position;
			bounds = new RectangleF( position.x - radius, position.y - radius, radius * 2f, radius * 2f );
		}


		internal override void recalculateBounds( Collider collider )
		{
			// if we dont have rotation or dont care about TRS we use localOffset as the center so we'll start with that
			center = collider.localOffset;

			if( collider.shouldColliderScaleAndRotateWithTransform )
			{
				// we only scale lineraly being a circle so we'll use the max value
				var scale = collider.entity.transform.scale;
				var hasUnitScale = scale.x == 1 && scale.y == 1;
				var maxScale = Math.Max( scale.x, scale.y );
				radius = _originalRadius * maxScale;

				if( collider.entity.transform.rotation != 0 )
				{
					// to deal with rotation with an offset origin we just move our center in a circle around 0,0 with our offset making the 0 angle
					var offsetAngle = Mathf.atan2( collider.localOffset.y, collider.localOffset.x ) * Mathf.rad2Deg;
					var offsetLength = hasUnitScale ? collider._localOffsetLength : ( collider.localOffset * collider.entity.transform.scale ).Length;
					center = Mathf.pointOnCircle( vec2.Zero, offsetLength, collider.entity.transform.rotationDegrees + offsetAngle );
				}
			}

			position = collider.entity.transform.position + center;
			bounds = new RectangleF( position.x - radius, position.y - radius, radius * 2f, radius * 2f );
		}


		public override bool overlaps( Shape other )
		{
			CollisionResult result;

			// Box is only optimized for unrotated
			if( other is Box && ( other as Box ).isUnrotated )
				return Collisions.rectToCircle( ref other.bounds, position, radius );

			if( other is Circle )
				return Collisions.circleToCircle( position, radius, other.position, ( other as Circle ).radius );

			if( other is Polygon )
				return ShapeCollisions.circleToPolygon( this, other as Polygon, out result );

			throw new NotImplementedException( string.Format( "overlaps of Circle to {0} are not supported", other ) );
		}


		public override bool collidesWithShape( Shape other, out CollisionResult result )
		{
			if( other is Box && ( other as Box ).isUnrotated )
				return ShapeCollisions.circleToBox( this, other as Box, out result );

			if( other is Circle )
				return ShapeCollisions.circleToCircle( this, other as Circle, out result );

			if( other is Polygon )
				return ShapeCollisions.circleToPolygon( this, other as Polygon, out result );

			throw new NotImplementedException( string.Format( "Collisions of Circle to {0} are not supported", other ) );
		}


		public override bool collidesWithLine( vec2 start, vec2 end, out RaycastHit hit )
		{
			hit = new RaycastHit();
			return ShapeCollisions.lineToCircle( start, end, this, out hit );
		}


		/// <summary>
		/// Gets whether or not the provided point lie within the bounds of this <see cref="Circle"/>.
		/// </summary>
		/// <param name="point">the point</param>
		/// <returns><c>true</c> if the provided coordinates lie inside this <see cref="Circle"/>; <c>false</c> otherwise.</returns>
		public override bool containsPoint( vec2 point )
		{
			return ( ( point - position ).LengthSqr <= radius * radius );
		}

		#endregion


		/// <summary>
		/// Gets the point at the edge of this <see cref="Circle"/> from the provided angle
		/// </summary>
		/// <param name="angle">an angle in radians</param>
		/// <returns><see cref="vec2"/> representing the point on this <see cref="Circle"/>'s surface at the specified angle</returns>
		public vec2 getPointAlongEdge( float angle )
		{
			return new vec2( position.x + ( radius * Mathf.cos( angle ) ), position.y + ( radius * Mathf.sin( angle ) ) );
		}


		/// <summary>
		/// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Circle"/>.
		/// </summary>
		/// <param name="x">The x coordinate of the point to check for containment.</param>
		/// <param name="y">The y coordinate of the point to check for containment.</param>
		/// <returns><c>true</c> if the provided coordinates lie inside this <see cref="Circle"/>; <c>false</c> otherwise.</returns>
		public bool containsPoint( float x, float y )
		{
			return containsPoint( new vec2( x, y ) );
		}


		/// <summary>
		/// Gets whether or not the provided <see cref="vec2"/> lies within the bounds of this <see cref="Circle"/>.
		/// </summary>
		/// <param name="point">Point.</param>
		public bool containsPoint( ref vec2 point )
		{
			return ( point - position ).LengthSqr <= radius * radius;
		}


		public override bool pointCollidesWithShape( vec2 point, out CollisionResult result )
		{
			return ShapeCollisions.pointToCircle( point, this, out result );
		}

	}
}

