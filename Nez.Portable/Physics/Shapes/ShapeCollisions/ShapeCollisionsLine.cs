using Microsoft.Xna.Framework;


namespace Nez.PhysicsShapes
{
	public static partial class ShapeCollisions
	{
		public static bool lineToPoly( vec2 start, vec2 end, Polygon polygon, out RaycastHit hit )
		{
			hit = new RaycastHit();
			var normal = vec2.Zero;
			var intersectionPoint = vec2.Zero;
			var fraction = float.MaxValue;
			var hasIntersection = false;

			for( int j = polygon.points.Length - 1, i = 0; i < polygon.points.Length; j = i, i++ )
			{
				var edge1 = polygon.position + polygon.points[j];
				var edge2 = polygon.position + polygon.points[i];
				vec2 intersection;
				if( lineToLine( edge1, edge2, start, end, out intersection ) )
				{
					hasIntersection = true;

					// TODO: is this the correct and most efficient way to get the fraction?
					// check x fraction first. if it is NaN use y instead
					var distanceFraction = ( intersection.x - start.x ) / ( end.x - start.x );
					if( float.IsNaN( distanceFraction ) || float.IsInfinity( distanceFraction ) )
						distanceFraction = ( intersection.y - start.y ) / ( end.y - start.y );

					if( distanceFraction < fraction )
					{
						var edge = edge2 - edge1;
						normal = new vec2( edge.y, -edge.x );
						fraction = distanceFraction;
						intersectionPoint = intersection;
					}
				}
			}

			if( hasIntersection )
			{
				normal.Normalize();
				float distance;
				vec2.Distance( ref start, ref intersectionPoint, out distance );
				hit.setValues( fraction, distance, intersectionPoint, normal );
				return true;
			}

			return false;
		}


		public static bool lineToCircle( vec2 start, vec2 end, Circle s, out RaycastHit hit )
		{
			hit = new RaycastHit();

			// calculate the length here and normalize d separately since we will need it to get the fraction if we have a hit
			var lineLength = vec2.Distance( start, end );
			var d = ( end - start ) / lineLength;
			var m = start - s.position;
			var b = vec2.Dot( m, d );
			var c = vec2.Dot( m, m ) - s.radius * s.radius;

			// exit if r's origin outside of s (c > 0) and r pointing away from s (b > 0)
			if( c > 0f && b > 0f )
				return false;

			var discr = b * b - c;

			// a negative descriminant means the line misses the circle
			if( discr < 0 )
				return false;

			// ray intersects circle. calculate details now.
			hit.fraction = -b - Mathf.sqrt( discr );

			// if fraction is negative, ray started inside circle so clamp fraction to 0
			if( hit.fraction < 0 )
				hit.fraction = 0;

			hit.point = start + hit.fraction * d;
			vec2.Distance( ref start, ref hit.point, out hit.distance );
			hit.normal = vec2.Normalize( hit.point - s.position );
			hit.fraction = hit.distance / lineLength;

			return true;
		}


		public static bool lineToLine( vec2 a1, vec2 a2, vec2 b1, vec2 b2, out vec2 intersection )
		{
			intersection = vec2.Zero;

			var b = a2 - a1;
			var d = b2 - b1;
			var bDotDPerp = b.x * d.y - b.y * d.x;

			// if b dot d == 0, it means the lines are parallel so have infinite intersection points
			if( bDotDPerp == 0 )
				return false;

			var c = b1 - a1;
			var t = ( c.x * d.y - c.y * d.x ) / bDotDPerp;
			if( t < 0 || t > 1 )
				return false;

			var u = ( c.x * b.y - c.y * b.x ) / bDotDPerp;
			if( u < 0 || u > 1 )
				return false;

			intersection = a1 + t * b;

			return true;
		}

	}
}
