using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Nez.Collision;
using Microsoft.Xna.Framework;


namespace Nez.Common
{
	public enum PolygonError
	{
		/// <summary>
		/// There were no errors in the polygon
		/// </summary>
		NoError,

		/// <summary>
		/// Polygon must have between 3 and Settings.MaxPolygonVertices vertices.
		/// </summary>
		InvalidAmountOfVertices,

		/// <summary>
		/// Polygon must be simple. This means no overlapping edges.
		/// </summary>
		NotSimple,

		/// <summary>
		/// Polygon must have a counter clockwise winding.
		/// </summary>
		NotCounterClockWise,

		/// <summary>
		/// The polygon is concave, it needs to be convex.
		/// </summary>
		NotConvex,

		/// <summary>
		/// Polygon area is too small.
		/// </summary>
		AreaTooSmall,

		/// <summary>
		/// The polygon has a side that is too short.
		/// </summary>
		SideTooSmall
	}



	[DebuggerDisplay( "Count = {Count} Vertices = {ToString()}" )]
	public class Vertices : List<vec2>
	{
		internal bool attachedToBody;

		/// <summary>
		/// You can add holes to this collection.
		/// It will get respected by some of the triangulation algoithms, but otherwise not used.
		/// </summary>
		public List<Vertices> holes;


		public Vertices()
		{}

		public Vertices( int capacity ) : base( capacity )
		{}

		public Vertices( IEnumerable<vec2> vertices )
		{
			AddRange( vertices );
		}

		/// <summary>
		/// Gets the next index. Used for iterating all the edges with wrap-around.
		/// </summary>
		/// <param name="index">The current index</param>
		public int nextIndex( int index )
		{
			return ( index + 1 > Count - 1 ) ? 0 : index + 1;
		}

		/// <summary>
		/// Gets the next vertex. Used for iterating all the edges with wrap-around.
		/// </summary>
		/// <param name="index">The current index</param>
		public vec2 nextVertex( int index )
		{
			return this[nextIndex( index )];
		}

		/// <summary>
		/// Gets the previous index. Used for iterating all the edges with wrap-around.
		/// </summary>
		/// <param name="index">The current index</param>
		public int previousIndex( int index )
		{
			return index - 1 < 0 ? Count - 1 : index - 1;
		}

		/// <summary>
		/// Gets the previous vertex. Used for iterating all the edges with wrap-around.
		/// </summary>
		/// <param name="index">The current index</param>
		public vec2 previousVertex( int index )
		{
			return this[previousIndex( index )];
		}

		/// <summary>
		/// Gets the signed area.
		/// If the area is less than 0, it indicates that the polygon is clockwise winded.
		/// </summary>
		/// <returns>The signed area</returns>
		public float getSignedArea()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return 0;

			int i;
			float area = 0;

			for( i = 0; i < Count; i++ )
			{
				int j = ( i + 1 ) % Count;

				vec2 vi = this[i];
				vec2 vj = this[j];

				area += vi.x * vj.y;
				area -= vi.y * vj.x;
			}
			area /= 2.0f;
			return area;
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <returns></returns>
		public float getArea()
		{
			float area = getSignedArea();
			return ( area < 0 ? -area : area );
		}

		/// <summary>
		/// Gets the centroid.
		/// </summary>
		/// <returns></returns>
		public vec2 getCentroid()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return new vec2( float.NaN, float.NaN );

			// Same algorithm is used by Box2D
			vec2 c = vec2.Zero;
			float area = 0.0f;
			const float inv3 = 1.0f / 3.0f;

			for( int i = 0; i < Count; ++i )
			{
				// Triangle vertices.
				vec2 current = this[i];
				vec2 next = ( i + 1 < Count ? this[i + 1] : this[0] );

				float triangleArea = 0.5f * ( current.x * next.y - current.y * next.x );
				area += triangleArea;

				// Area weighted centroid
				c += triangleArea * inv3 * ( current + next );
			}

			// Centroid
			c *= 1.0f / area;
			return c;
		}

		/// <summary>
		/// Returns an AABB that fully contains this polygon.
		/// </summary>
		public AABB getAABB()
		{
			AABB aabb;
			vec2 lowerBound = new vec2( float.MaxValue, float.MaxValue );
			vec2 upperBound = new vec2( float.MinValue, float.MinValue );

			for( int i = 0; i < Count; ++i )
			{
				if( this[i].x < lowerBound.x )
				{
					lowerBound.x = this[i].x;
				}
				if( this[i].x > upperBound.x )
				{
					upperBound.x = this[i].x;
				}

				if( this[i].y < lowerBound.y )
				{
					lowerBound.y = this[i].y;
				}
				if( this[i].y > upperBound.y )
				{
					upperBound.y = this[i].y;
				}
			}

			aabb.lowerBound = lowerBound;
			aabb.upperBound = upperBound;

			return aabb;
		}

		/// <summary>
		/// Translates the vertices with the specified vector.
		/// </summary>
		/// <param name="value">The value.</param>
		public void translate( vec2 value )
		{
			translate( ref value );
		}

		/// <summary>
		/// Translates the vertices with the specified vector.
		/// </summary>
		/// <param name="value">The vector.</param>
		public void translate( ref vec2 value )
		{
			Debug.Assert( !attachedToBody, "Translating vertices that are used by a Body can result in unstable behavior. Use Body.Position instead." );

			for( int i = 0; i < Count; i++ )
				this[i] = vec2.Add( this[i], value );

			if( holes != null && holes.Count > 0 )
			{
				foreach( Vertices hole in holes )
				{
					hole.translate( ref value );
				}
			}
		}

		/// <summary>
		/// Scales the vertices with the specified vector.
		/// </summary>
		/// <param name="value">The Value.</param>
		public void scale( vec2 value )
		{
			scale( ref value );
		}

		/// <summary>
		/// Scales the vertices with the specified vector.
		/// </summary>
		/// <param name="value">The Value.</param>
		public void scale( ref vec2 value )
		{
			Debug.Assert( !attachedToBody, "Scaling vertices that are used by a Body can result in unstable behavior." );

			for( int i = 0; i < Count; i++ )
				this[i] = vec2.Multiply( this[i], value );

			if( holes != null && holes.Count > 0 )
			{
				foreach( Vertices hole in holes )
				{
					hole.scale( ref value );
				}
			}
		}

		/// <summary>
		/// Rotate the vertices with the defined value in radians.
		/// 
		/// Warning: Using this method on an active set of vertices of a Body, will cause problems with collisions. Use Body.Rotation instead.
		/// </summary>
		/// <param name="value">The amount to rotate by in radians.</param>
		public void rotate( float value )
		{
			Debug.Assert( !attachedToBody, "Rotating vertices that are used by a Body can result in unstable behavior." );

			var cos = (float)Math.Cos( value );
			var sin = (float)Math.Sin( value );

			for( var i = 0; i < Count; i++ )
			{
				var position = this[i];
				this[i] = new vec2( ( position.x * cos + position.y * -sin ), ( position.x * sin + position.y * cos ) );
			}

			if( holes != null && holes.Count > 0 )
			{
				foreach( Vertices hole in holes )
				{
					hole.rotate( value );
				}
			}
		}

		/// <summary>
		/// Determines whether the polygon is convex.
		/// O(n^2) running time.
		/// 
		/// Assumptions:
		/// - The polygon is in counter clockwise order
		/// - The polygon has no overlapping edges
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if it is convex; otherwise, <c>false</c>.
		/// </returns>
		public bool isConvex()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return false;

			//Triangles are always convex
			if( Count == 3 )
				return true;

			// Checks the polygon is convex and the interior is to the left of each edge.
			for( int i = 0; i < Count; ++i )
			{
				int next = i + 1 < Count ? i + 1 : 0;
				vec2 edge = this[next] - this[i];

				for( int j = 0; j < Count; ++j )
				{
					// Don't check vertices on the current edge.
					if( j == i || j == next )
						continue;

					vec2 r = this[j] - this[i];

					float s = edge.x * r.y - edge.y * r.x;

					if( s <= 0.0f )
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Indicates if the vertices are in counter clockwise order.
		/// Warning: If the area of the polygon is 0, it is unable to determine the winding.
		/// </summary>
		public bool isCounterClockWise()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return false;

			return ( getSignedArea() > 0.0f );
		}

		/// <summary>
		/// Forces the vertices to be counter clock wise order.
		/// </summary>
		public void forceCounterClockWise()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return;

			if( !isCounterClockWise() )
				Reverse();
		}

		/// <summary>
		/// Checks if the vertices forms an simple polygon by checking for edge crossings.
		/// </summary>
		public bool isSimple()
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if( Count < 3 )
				return false;

			for( int i = 0; i < Count; ++i )
			{
				vec2 a1 = this[i];
				vec2 a2 = nextVertex( i );
				for( int j = i + 1; j < Count; ++j )
				{
					vec2 b1 = this[j];
					vec2 b2 = nextVertex( j );

					vec2 temp;

					if( LineTools.lineIntersect2( ref a1, ref a2, ref b1, ref b2, out temp ) )
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if the polygon is valid for use in the engine.
		///
		/// Performs a full check, for simplicity, convexity,
		/// orientation, minimum angle, and volume.
		/// 
		/// From Eric Jordan's convex decomposition library
		/// </summary>
		/// <returns>PolygonError.NoError if there were no error.</returns>
		public PolygonError checkPolygon()
		{
			if( Count < 3 || Count > Settings.maxPolygonVertices )
				return PolygonError.InvalidAmountOfVertices;

			if( !isSimple() )
				return PolygonError.NotSimple;

			if( getArea() <= Settings.epsilon )
				return PolygonError.AreaTooSmall;

			if( !isConvex() )
				return PolygonError.NotConvex;

			//Check if the sides are of adequate length.
			for( int i = 0; i < Count; ++i )
			{
				int next = i + 1 < Count ? i + 1 : 0;
				vec2 edge = this[next] - this[i];
				if( edge.LengthSquared() <= Settings.epsilon * Settings.epsilon )
				{
					return PolygonError.SideTooSmall;
				}
			}

			if( !isCounterClockWise() )
				return PolygonError.NotCounterClockWise;

			return PolygonError.NoError;
		}

		/// <summary>
		/// Projects to axis.
		/// </summary>
		/// <param name="axis">The axis.</param>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		public void projectToAxis( ref vec2 axis, out float min, out float max )
		{
			// To project a point on an axis use the dot product
			float dotProduct = vec2.Dot( axis, this[0] );
			min = dotProduct;
			max = dotProduct;

			for( int i = 0; i < Count; i++ )
			{
				dotProduct = vec2.Dot( this[i], axis );
				if( dotProduct < min )
				{
					min = dotProduct;
				}
				else
				{
					if( dotProduct > max )
					{
						max = dotProduct;
					}
				}
			}
		}

		/// <summary>
		/// Winding number test for a point in a polygon.
		/// </summary>
		/// See more info about the algorithm here: http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm
		/// <param name="point">The point to be tested.</param>
		/// <returns>-1 if the winding number is zero and the point is outside
		/// the polygon, 1 if the point is inside the polygon, and 0 if the point
		/// is on the polygons edge.</returns>
		public int pointInPolygon( ref vec2 point )
		{
			// Winding number
			int wn = 0;

			// Iterate through polygon's edges
			for( int i = 0; i < Count; i++ )
			{
				// Get points
				vec2 p1 = this[i];
				vec2 p2 = this[nextIndex( i )];

				// Test if a point is directly on the edge
				vec2 edge = p2 - p1;
				float area = MathUtils.area( ref p1, ref p2, ref point );
				if( area == 0f && vec2.Dot( point - p1, edge ) >= 0f && vec2.Dot( point - p2, edge ) <= 0f )
				{
					return 0;
				}
				// Test edge for intersection with ray from point
				if( p1.y <= point.y )
				{
					if( p2.y > point.y && area > 0f )
					{
						++wn;
					}
				}
				else
				{
					if( p2.y <= point.y && area < 0f )
					{
						--wn;
					}
				}
			}
			return ( wn == 0 ? -1 : 1 );
		}

		/// <summary>
		/// Compute the sum of the angles made between the test point and each pair of points making up the polygon. 
		/// If this sum is 2pi then the point is an interior point, if 0 then the point is an exterior point. 
		/// ref: http://ozviz.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/  - Solution 2 
		/// </summary>
		public bool pointInPolygonAngle( ref vec2 point )
		{
			double angle = 0;

			// Iterate through polygon's edges
			for( int i = 0; i < Count; i++ )
			{
				// Get points
				vec2 p1 = this[i] - point;
				vec2 p2 = this[nextIndex( i )] - point;

				angle += MathUtils.vectorAngle( ref p1, ref p2 );
			}

			if( Math.Abs( angle ) < Math.PI )
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Transforms the polygon using the defined matrix.
		/// </summary>
		/// <param name="transform">The matrix to use as transformation.</param>
		public void transform( ref Matrix transform )
		{
			// Transform main polygon
			for( int i = 0; i < Count; i++ )
				this[i] = vec2.Transform( this[i], transform );

			// Transform holes
			if( holes != null && holes.Count > 0 )
			{
				for( int i = 0; i < holes.Count; i++ )
				{
					vec2[] temp = holes[i].ToArray();
					vec2.Transform( temp, ref transform, temp );

					holes[i] = new Vertices( temp );
				}
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for( int i = 0; i < Count; i++ )
			{
				builder.Append( this[i].ToString() );
				if( i < Count - 1 )
				{
					builder.Append( " " );
				}
			}
			return builder.ToString();
		}
	
	}
}