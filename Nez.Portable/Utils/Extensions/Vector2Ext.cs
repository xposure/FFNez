#if FEATURE_UTILS
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;


namespace Atma
{
	public static class Vector2Ext
	{
		/// <summary>
		/// temporary workaround to vec2.Normalize screwing up the 0,0 vector
		/// </summary>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void normalize( ref vec2 vec )
		{
			var magnitude = Mathf.sqrt( ( vec.X * vec.X ) + ( vec.Y * vec.Y ) );
			if( magnitude > Mathf.epsilon )
				vec /= magnitude;
			else
				vec.X = vec.Y = 0;
		}


		/// <summary>
		/// temporary workaround to vec2.Normalize screwing up the 0,0 vector
		/// </summary>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 normalize( vec2 vec )
		{
			var magnitude = Mathf.sqrt( ( vec.X * vec.X ) + ( vec.Y * vec.Y ) );
			if( magnitude > Mathf.epsilon )
				vec /= magnitude;
			else
				vec.X = vec.Y = 0;

			return vec;
		}


		/// <summary>
		/// rounds the x and y values
		/// </summary>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 round( this vec2 vec )
		{
			return new vec2( Mathf.round( vec.X ), Mathf.round( vec.Y ) );
		}


		/// <summary>
		/// rounds the x and y values in place
		/// </summary>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void round( ref vec2 vec )
		{
			vec.X = Mathf.round( vec.X );
			vec.Y = Mathf.round( vec.Y );
		}


		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public void floor( ref vec2 val )
		{
			val.X = (int)val.X;
			val.Y = (int)val.Y;
		}


		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public vec2 floor( vec2 val )
		{
			return new vec2( (int)val.X, (int)val.Y );
		}


		/// <summary>
		/// returns a 0.5, 0.5 vector
		/// </summary>
		/// <returns>The vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 halfVector()
		{
			return new vec2( 0.5f, 0.5f );
		}


		/// <summary>
		/// compute the 2d pseudo cross product Dot( Perp( u ), v )
		/// </summary>
		/// <param name="u">U.</param>
		/// <param name="v">V.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float cross( vec2 u, vec2 v )
		{
			return u.Y * v.X - u.X * v.Y;
		}

		/// <summary>
		/// compute the 2d dot product dot(u, v)
		/// </summary>
		/// <param name="self">self</param>
		/// <param name="v">V.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float dot( this vec2 self, vec2 v )
		{
			return vec2.Dot(self, v);
		}


		/// <summary>
		/// returns the vector perpendicular to the passed in vectors
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 perpendicular( ref vec2 first, ref vec2 second )
		{
			return new vec2( -1f * ( second.Y - first.Y ), second.X - first.X );
		}


		/// <summary>
		/// returns the vector perpendicular to the passed in vectors
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 perpendicular( vec2 first, vec2 second )
		{
			return new vec2( -1f * ( second.Y - first.Y ), second.X - first.X );
		}


		/// <summary>
		/// flips the x/y values and inverts the y to get the perpendicular
		/// </summary>
		/// <param name="original">Original.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 perpendicular( vec2 original )
		{
			return new vec2( -original.Y, original.X );
		}


		/// <summary>
		/// returns the angle between the two vectors in degrees
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float angle( vec2 from, vec2 to )
		{
			normalize( ref from );
			normalize( ref to );
			return Mathf.acos( Mathf.clamp( vec2.Dot( from, to ), -1f, 1f ) ) * Mathf.rad2Deg;
		}


		/// <summary>
		/// returns the angle between left and right with self being the center point in degrees
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="left">V left.</param>
		/// <param name="right">V right.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float angleBetween( this vec2 self, vec2 left, vec2 right )
		{
			var one = left - self;
			var two = right - self;
			return angle( one, two );
		}

		/// <summary>
		/// returns a vector rotated around the origin by an angle in radians
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="angle">angle in radians.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 rotate( this vec2 self, float angle ) {
			var ca = Mathf.cos(angle);
			var sa = Mathf.sin(angle);
			return new vec2(ca * self.X - sa * self.Y, sa * self.X + ca * self.Y);
		}


		/// <summary>
		/// given two lines (ab and cd) finds the intersection point
		/// </summary>
		/// <returns>The ray intersection.</returns>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="c">C.</param>
		/// <param name="d">D.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool getRayIntersection( vec2 a, vec2 b, vec2 c, vec2 d, out vec2 intersection )
		{
			var dy1 = b.Y - a.Y;
			var dx1 = b.X - a.X;
			var dy2 = d.Y - c.Y;
			var dx2 = d.X - c.X;

			if( dy1 * dx2 == dy2 * dx1 )
			{
				intersection = new vec2( float.NaN, float.NaN );
				return false;
			}

			var x = ( ( c.Y - a.Y ) * dx1 * dx2 + dy1 * dx2 * a.X - dy2 * dx1 * c.X ) / ( dy1 * dx2 - dy2 * dx1 );
			var y = a.Y + ( dy1 / dx1 ) * ( x - a.X );

			intersection = new vec2( x, y );
			return true;
		}

		/// <summary>
		/// rounds and converts a vec2 to a Point
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Point roundToPoint( this vec2 vec )
		{
			var roundedVec = vec.round();
			return new Point((int) roundedVec.X, (int) roundedVec.Y);
		}

		/// <summary>
		/// floors and converts a vec2 to a Point
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Point floorToPoint( this vec2 vec )
		{
			return new Point(Mathf.floorToInt(vec.X), Mathf.floorToInt(vec.Y));
		}


		/// <summary>
		/// converts a vec2 to a vec3 with a 0 z-position
		/// </summary>
		/// <returns>The vector3.</returns>
		/// <param name="vec">Vec.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec3 toVector3( this vec2 vec )
		{
			return new vec3( vec, 0 );
		}


		/// <summary>
		/// checks if a triangle is CCW or CW
		/// </summary>
		/// <returns><c>true</c>, if triangle ccw was ised, <c>false</c> otherwise.</returns>
		/// <param name="a">The alpha component.</param>
		/// <param name="center">Center.</param>
		/// <param name="c">C.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool isTriangleCCW( vec2 a, vec2 center, vec2 c )
		{
			return cross( center - a, c - center ) < 0;
		}


		/// <summary>
		/// Creates a new <see cref="vec2"/> that contains a transformation of 2d-vector by the specified <see cref="mat4"/>.
		/// </summary>
		/// <param name="position">Source <see cref="vec2"/>.</param>
		/// <param name="matrix">The transformation <see cref="mat4"/>.</param>
		/// <returns>Transformed <see cref="vec2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static vec2 transform( vec2 position, Matrix2D matrix )
		{
			return new vec2( ( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) + matrix.M31, ( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) + matrix.M32 );
		}


		/// <summary>
		/// Creates a new <see cref="vec2"/> that contains a transformation of 2d-vector by the specified <see cref="mat4"/>.
		/// </summary>
		/// <param name="position">Source <see cref="vec2"/>.</param>
		/// <param name="matrix">The transformation <see cref="mat4"/>.</param>
		/// <param name="result">Transformed <see cref="vec2"/> as an output parameter.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void transform( ref vec2 position, ref Matrix2D matrix, out vec2 result )
		{
			var x = ( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) + matrix.M31;
			var y = ( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) + matrix.M32;
            result = new vec2();
			result.X = x;
			result.Y = y;
		}


		/// <summary>
		/// Apply transformation on vectors within array of <see cref="vec2"/> by the specified <see cref="mat4"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="sourceIndex">The starting index of transformation in the source array.</param>
		/// <param name="matrix">The transformation <see cref="mat4"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		/// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="vec2"/> should be written.</param>
		/// <param name="length">The number of vectors to be transformed.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void transform( vec2[] sourceArray, int sourceIndex, ref Matrix2D matrix, vec2[] destinationArray, int destinationIndex, int length )
		{
			for( var i = 0; i < length; i++ )
			{
				var position = sourceArray[sourceIndex + i];
				var destination = destinationArray[destinationIndex + i];
				destination.X = ( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) + matrix.M31;
				destination.Y = ( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) + matrix.M32;
				destinationArray[destinationIndex + i] = destination;
			}
		}


		/// <summary>
		/// Apply transformation on all vectors within array of <see cref="vec2"/> by the specified <see cref="mat4"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="matrix">The transformation <see cref="mat4"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void transform( vec2[] sourceArray, ref Matrix2D matrix, vec2[] destinationArray )
		{
			transform( sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length );
		}

	}
}

#endif
