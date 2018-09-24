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
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace Nez.Common
{
	public static class MathUtils
	{
		public static float cross( ref vec2 a, ref vec2 b )
		{
			return a.x * b.y - a.y * b.x;
		}

		public static float cross( vec2 a, vec2 b )
		{
			return cross( ref a, ref b );
		}

		/// Perform the cross product on two vectors.
		public static Vector3 cross( Vector3 a, Vector3 b )
		{
			return new Vector3( a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X );
		}

		public static vec2 cross( vec2 a, float s )
		{
			return new vec2( s * a.y, -s * a.x );
		}

		public static vec2 cross( float s, vec2 a )
		{
			return new vec2( -s * a.y, s * a.x );
		}

		public static vec2 abs( vec2 v )
		{
			return new vec2( Math.Abs( v.x ), Math.Abs( v.y ) );
		}

		public static vec2 mul( ref Mat22 A, vec2 v )
		{
			return mul( ref A, ref v );
		}

		public static vec2 mul( ref Mat22 A, ref vec2 v )
		{
			return new vec2( A.ex.x * v.x + A.ey.x * v.y, A.ex.y * v.x + A.ey.y * v.y );
		}

		public static vec2 mul( ref Transform T, vec2 v )
		{
			return mul( ref T, ref v );
		}

		public static vec2 mul( ref Transform T, ref vec2 v )
		{
			float x = ( T.q.c * v.x - T.q.s * v.y ) + T.p.x;
			float y = ( T.q.s * v.x + T.q.c * v.y ) + T.p.y;

			return new vec2( x, y );
		}

		public static vec2 mulT( ref Mat22 A, vec2 v )
		{
			return mulT( ref A, ref v );
		}

		public static vec2 mulT( ref Mat22 A, ref vec2 v )
		{
			return new vec2( v.x * A.ex.x + v.y * A.ex.y, v.x * A.ey.x + v.y * A.ey.y );
		}

		public static vec2 mulT( ref Transform T, vec2 v )
		{
			return mulT( ref T, ref v );
		}

		public static vec2 mulT( ref Transform T, ref vec2 v )
		{
			float px = v.x - T.p.x;
			float py = v.y - T.p.y;
			float x = ( T.q.c * px + T.q.s * py );
			float y = ( -T.q.s * px + T.q.c * py );

			return new vec2( x, y );
		}

		// A^T * B
		public static void mulT( ref Mat22 A, ref Mat22 B, out Mat22 C )
		{
			C = new Mat22();
			C.ex.x = A.ex.x * B.ex.x + A.ex.y * B.ex.y;
			C.ex.y = A.ey.x * B.ex.x + A.ey.y * B.ex.y;
			C.ey.x = A.ex.x * B.ey.x + A.ex.y * B.ey.y;
			C.ey.y = A.ey.x * B.ey.x + A.ey.y * B.ey.y;
		}

		/// Multiply a matrix times a vector.
		public static Vector3 mul( Mat33 A, Vector3 v )
		{
			return v.X * A.ex + v.Y * A.ey + v.Z * A.ez;
		}

		// v2 = A.q.Rot(B.q.Rot(v1) + B.p) + A.p
		//    = (A.q * B.q).Rot(v1) + A.q.Rot(B.p) + A.p
		public static Transform mul( Transform A, Transform B )
		{
			Transform C = new Transform();
			C.q = mul( A.q, B.q );
			C.p = mul( A.q, B.p ) + A.p;
			return C;
		}

		// v2 = A.q' * (B.q * v1 + B.p - A.p)
		//    = A.q' * B.q * v1 + A.q' * (B.p - A.p)
		public static void mulT( ref Transform A, ref Transform B, out Transform C )
		{
			C = new Transform();
			C.q = mulT( A.q, B.q );
			C.p = mulT( A.q, B.p - A.p );
		}

		public static void Swap<T>( ref T a, ref T b )
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

		/// Multiply a matrix times a vector.
		public static vec2 mul22( Mat33 A, vec2 v )
		{
			return new vec2( A.ex.X * v.x + A.ey.X * v.y, A.ex.Y * v.x + A.ey.Y * v.y );
		}

		/// Multiply two rotations: q * r
		public static Rot mul( Rot q, Rot r )
		{
			// [qc -qs] * [rc -rs] = [qc*rc-qs*rs -qc*rs-qs*rc]
			// [qs  qc]   [rs  rc]   [qs*rc+qc*rs -qs*rs+qc*rc]
			// s = qs * rc + qc * rs
			// c = qc * rc - qs * rs
			Rot qr;
			qr.s = q.s * r.c + q.c * r.s;
			qr.c = q.c * r.c - q.s * r.s;
			return qr;
		}

		public static vec2 mulT( Transform T, vec2 v )
		{
			float px = v.x - T.p.x;
			float py = v.y - T.p.y;
			float x = ( T.q.c * px + T.q.s * py );
			float y = ( -T.q.s * px + T.q.c * py );

			return new vec2( x, y );
		}

		/// Transpose multiply two rotations: qT * r
		public static Rot mulT( Rot q, Rot r )
		{
			// [ qc qs] * [rc -rs] = [qc*rc+qs*rs -qc*rs+qs*rc]
			// [-qs qc]   [rs  rc]   [-qs*rc+qc*rs qs*rs+qc*rc]
			// s = qc * rs - qs * rc
			// c = qc * rc + qs * rs
			Rot qr;
			qr.s = q.c * r.s - q.s * r.c;
			qr.c = q.c * r.c + q.s * r.s;
			return qr;
		}

		// v2 = A.q' * (B.q * v1 + B.p - A.p)
		//    = A.q' * B.q * v1 + A.q' * (B.p - A.p)
		public static Transform mulT( Transform A, Transform B )
		{
			var C = new Transform();
			C.q = mulT( A.q, B.q );
			C.p = mulT( A.q, B.p - A.p );
			return C;
		}

		/// Rotate a vector
		public static vec2 mul( Rot q, vec2 v )
		{
			return new vec2( q.c * v.x - q.s * v.y, q.s * v.x + q.c * v.y );
		}

		/// Inverse rotate a vector
		public static vec2 mulT( Rot q, vec2 v )
		{
			return new vec2( q.c * v.x + q.s * v.y, -q.s * v.x + q.c * v.y );
		}

		/// Get the skew vector such that dot(skew_vec, other) == cross(vec, other)
		public static vec2 skew( vec2 input )
		{
			return new vec2( -input.y, input.x );
		}

		/// <summary>
		/// This function is used to ensure that a floating point number is
		/// not a NaN or infinity.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <returns>
		/// 	<c>true</c> if the specified x is valid; otherwise, <c>false</c>.
		/// </returns>
		public static bool isValid( float x )
		{
			if( float.IsNaN( x ) )
			{
				// NaN.
				return false;
			}

			return !float.IsInfinity( x );
		}

		public static bool isValid( this vec2 x )
		{
			return isValid( x.x ) && isValid( x.y );
		}

		/// <summary>
		/// This is a approximate yet fast inverse square-root.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <returns></returns>
		public static float invSqrt( float x )
		{
			FloatConverter convert = new FloatConverter();
			convert.x = x;
			float xhalf = 0.5f * x;
			convert.i = 0x5f3759df - ( convert.i >> 1 );
			x = convert.x;
			x = x * ( 1.5f - xhalf * x * x );
			return x;
		}

		public static int clamp( int a, int low, int high )
		{
			return Math.Max( low, Math.Min( a, high ) );
		}

		public static float clamp( float a, float low, float high )
		{
			return Math.Max( low, Math.Min( a, high ) );
		}

		public static vec2 clamp( vec2 a, vec2 low, vec2 high )
		{
			return vec2.Max( low, vec2.Min( a, high ) );
		}

		public static void cross( ref vec2 a, ref vec2 b, out float c )
		{
			c = a.x * b.y - a.y * b.x;
		}

		/// <summary>
		/// Return the angle between two vectors on a plane
		/// The angle is from vector 1 to vector 2, positive anticlockwise
		/// The result is between -pi -> pi
		/// </summary>
		public static double vectorAngle( ref vec2 p1, ref vec2 p2 )
		{
			double theta1 = Math.Atan2( p1.y, p1.x );
			double theta2 = Math.Atan2( p2.y, p2.x );
			double dtheta = theta2 - theta1;
			while( dtheta > Math.PI )
				dtheta -= ( 2 * Math.PI );
			while( dtheta < -Math.PI )
				dtheta += ( 2 * Math.PI );

			return ( dtheta );
		}

		/// Perform the dot product on two vectors.
		public static float dot( Vector3 a, Vector3 b )
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static double vectorAngle( vec2 p1, vec2 p2 )
		{
			return vectorAngle( ref p1, ref p2 );
		}

		/// <summary>
		/// Returns a positive number if c is to the left of the line going from a to b.
		/// </summary>
		/// <returns>Positive number if point is left, negative if point is right, 
		/// and 0 if points are collinear.</returns>
		public static float area( vec2 a, vec2 b, vec2 c )
		{
			return area( ref a, ref b, ref c );
		}

		/// <summary>
		/// Returns a positive number if c is to the left of the line going from a to b.
		/// </summary>
		/// <returns>Positive number if point is left, negative if point is right, 
		/// and 0 if points are collinear.</returns>
		public static float area( ref vec2 a, ref vec2 b, ref vec2 c )
		{
			return a.x * ( b.y - c.y ) + b.x * ( c.y - a.y ) + c.x * ( a.y - b.y );
		}

		/// <summary>
		/// Determines if three vertices are collinear (ie. on a straight line)
		/// </summary>
		/// <param name="a">First vertex</param>
		/// <param name="b">Second vertex</param>
		/// <param name="c">Third vertex</param>
		/// <param name="tolerance">The tolerance</param>
		/// <returns></returns>
		public static bool isCollinear( ref vec2 a, ref vec2 b, ref vec2 c, float tolerance = 0 )
		{
			return floatInRange( area( ref a, ref b, ref c ), -tolerance, tolerance );
		}

		public static void cross( float s, ref vec2 a, out vec2 b )
		{
			b = new vec2( -s * a.y, s * a.x );
		}

		public static bool floatEquals( float value1, float value2 )
		{
			return Math.Abs( value1 - value2 ) <= Settings.epsilon;
		}

		/// <summary>
		/// Checks if a floating point Value is equal to another,
		/// within a certain tolerance.
		/// </summary>
		/// <param name="value1">The first floating point Value.</param>
		/// <param name="value2">The second floating point Value.</param>
		/// <param name="delta">The floating point tolerance.</param>
		/// <returns>True if the values are "equal", false otherwise.</returns>
		public static bool floatEquals( float value1, float value2, float delta )
		{
			return floatInRange( value1, value2 - delta, value2 + delta );
		}

		/// <summary>
		/// Checks if a floating point Value is within a specified
		/// range of values (inclusive).
		/// </summary>
		/// <param name="value">The Value to check.</param>
		/// <param name="min">The minimum Value.</param>
		/// <param name="max">The maximum Value.</param>
		/// <returns>True if the Value is within the range specified,
		/// false otherwise.</returns>
		public static bool floatInRange( float value, float min, float max )
		{
			return ( value >= min && value <= max );
		}


		#region Nested type: FloatConverter

		[StructLayout( LayoutKind.Explicit )]
		private struct FloatConverter
		{
			[FieldOffset( 0 )]
			public float x;
			[FieldOffset( 0 )]
			public int i;
		}

		#endregion


		public static vec2 mul( ref Rot rot, vec2 axis )
		{
			return mul( rot, axis );
		}

		public static vec2 mulT( ref Rot rot, vec2 axis )
		{
			return mulT( rot, axis );
		}
	}


	/// <summary>
	/// A 2-by-2 matrix. Stored in column-major order.
	/// </summary>
	public struct Mat22
	{
		public vec2 ex, ey;

		/// <summary>
		/// Construct this matrix using columns.
		/// </summary>
		/// <param name="c1">The c1.</param>
		/// <param name="c2">The c2.</param>
		public Mat22( vec2 c1, vec2 c2 )
		{
			ex = c1;
			ey = c2;
		}

		/// <summary>
		/// Construct this matrix using scalars.
		/// </summary>
		/// <param name="a11">The a11.</param>
		/// <param name="a12">The a12.</param>
		/// <param name="a21">The a21.</param>
		/// <param name="a22">The a22.</param>
		public Mat22( float a11, float a12, float a21, float a22 )
		{
			ex = new vec2( a11, a21 );
			ey = new vec2( a12, a22 );
		}

		public Mat22 Inverse
		{
			get
			{
				float a = ex.x, b = ey.x, c = ex.y, d = ey.y;
				float det = a * d - b * c;
				if( det != 0.0f )
				{
					det = 1.0f / det;
				}

				Mat22 result = new Mat22();
				result.ex.x = det * d;
				result.ex.y = -det * c;

				result.ey.x = -det * b;
				result.ey.y = det * a;

				return result;
			}
		}

		/// <summary>
		/// Initialize this matrix using columns.
		/// </summary>
		/// <param name="c1">The c1.</param>
		/// <param name="c2">The c2.</param>
		public void Set( vec2 c1, vec2 c2 )
		{
			ex = c1;
			ey = c2;
		}

		/// <summary>
		/// Set this to the identity matrix.
		/// </summary>
		public void SetIdentity()
		{
			ex.x = 1.0f;
			ey.x = 0.0f;
			ex.y = 0.0f;
			ey.y = 1.0f;
		}

		/// <summary>
		/// Set this matrix to all zeros.
		/// </summary>
		public void SetZero()
		{
			ex.x = 0.0f;
			ey.x = 0.0f;
			ex.y = 0.0f;
			ey.y = 0.0f;
		}

		/// <summary>
		/// Solve A * x = b, where b is a column vector. This is more efficient
		/// than computing the inverse in one-shot cases.
		/// </summary>
		/// <param name="b">The b.</param>
		/// <returns></returns>
		public vec2 Solve( vec2 b )
		{
			float a11 = ex.x, a12 = ey.x, a21 = ex.y, a22 = ey.y;
			float det = a11 * a22 - a12 * a21;
			if( det != 0.0f )
			{
				det = 1.0f / det;
			}

			return new vec2( det * ( a22 * b.x - a12 * b.y ), det * ( a11 * b.y - a21 * b.x ) );
		}

		public static void Add( ref Mat22 A, ref Mat22 B, out Mat22 R )
		{
			R.ex = A.ex + B.ex;
			R.ey = A.ey + B.ey;
		}
	}


	/// <summary>
	/// A 3-by-3 matrix. Stored in column-major order.
	/// </summary>
	public struct Mat33
	{
		public Vector3 ex, ey, ez;

		/// <summary>
		/// Construct this matrix using columns.
		/// </summary>
		/// <param name="c1">The c1.</param>
		/// <param name="c2">The c2.</param>
		/// <param name="c3">The c3.</param>
		public Mat33( Vector3 c1, Vector3 c2, Vector3 c3 )
		{
			ex = c1;
			ey = c2;
			ez = c3;
		}

		/// <summary>
		/// Set this matrix to all zeros.
		/// </summary>
		public void SetZero()
		{
			ex = Vector3.Zero;
			ey = Vector3.Zero;
			ez = Vector3.Zero;
		}

		/// <summary>
		/// Solve A * x = b, where b is a column vector. This is more efficient
		/// than computing the inverse in one-shot cases.
		/// </summary>
		/// <param name="b">The b.</param>
		/// <returns></returns>
		public Vector3 Solve33( Vector3 b )
		{
			float det = Vector3.Dot( ex, Vector3.Cross( ey, ez ) );
			if( det != 0.0f )
			{
				det = 1.0f / det;
			}

			return new Vector3( det * Vector3.Dot( b, Vector3.Cross( ey, ez ) ), det * Vector3.Dot( ex, Vector3.Cross( b, ez ) ), det * Vector3.Dot( ex, Vector3.Cross( ey, b ) ) );
		}

		/// <summary>
		/// Solve A * x = b, where b is a column vector. This is more efficient
		/// than computing the inverse in one-shot cases. Solve only the upper
		/// 2-by-2 matrix equation.
		/// </summary>
		/// <param name="b">The b.</param>
		/// <returns></returns>
		public vec2 Solve22( vec2 b )
		{
			float a11 = ex.X, a12 = ey.X, a21 = ex.Y, a22 = ey.Y;
			float det = a11 * a22 - a12 * a21;

			if( det != 0.0f )
			{
				det = 1.0f / det;
			}

			return new vec2( det * ( a22 * b.x - a12 * b.y ), det * ( a11 * b.y - a21 * b.x ) );
		}

		/// Get the inverse of this matrix as a 2-by-2.
		/// Returns the zero matrix if singular.
		public void GetInverse22( ref Mat33 M )
		{
			float a = ex.X, b = ey.X, c = ex.Y, d = ey.Y;
			float det = a * d - b * c;
			if( det != 0.0f )
			{
				det = 1.0f / det;
			}

			M.ex.X = det * d; M.ey.X = -det * b; M.ex.Z = 0.0f;
			M.ex.Y = -det * c; M.ey.Y = det * a; M.ey.Z = 0.0f;
			M.ez.X = 0.0f; M.ez.Y = 0.0f; M.ez.Z = 0.0f;
		}

		/// Get the symmetric inverse of this matrix as a 3-by-3.
		/// Returns the zero matrix if singular.
		public void GetSymInverse33( ref Mat33 M )
		{
			float det = MathUtils.dot( ex, MathUtils.cross( ey, ez ) );
			if( det != 0.0f )
			{
				det = 1.0f / det;
			}

			float a11 = ex.X, a12 = ey.X, a13 = ez.X;
			float a22 = ey.Y, a23 = ez.Y;
			float a33 = ez.Z;

			M.ex.X = det * ( a22 * a33 - a23 * a23 );
			M.ex.Y = det * ( a13 * a23 - a12 * a33 );
			M.ex.Z = det * ( a12 * a23 - a13 * a22 );

			M.ey.X = M.ex.Y;
			M.ey.Y = det * ( a11 * a33 - a13 * a13 );
			M.ey.Z = det * ( a13 * a12 - a11 * a23 );

			M.ez.X = M.ex.Z;
			M.ez.Y = M.ey.Z;
			M.ez.Z = det * ( a11 * a22 - a12 * a12 );
		}
	}


	/// <summary>
	/// Rotation
	/// </summary>
	public struct Rot
	{
		/// Sine and cosine
		public float s, c;

		/// <summary>
		/// Initialize from an angle in radians
		/// </summary>
		/// <param name="angle">Angle in radians</param>
		public Rot( float angle )
		{
			// TODO_ERIN optimize
			s = (float)Math.Sin( angle );
			c = (float)Math.Cos( angle );
		}

		/// <summary>
		/// Set using an angle in radians.
		/// </summary>
		/// <param name="angle"></param>
		public void Set( float angle )
		{
			//FPE: Optimization
			if( angle == 0 )
			{
				s = 0;
				c = 1;
			}
			else
			{
				// TODO_ERIN optimize
				s = (float)Math.Sin( angle );
				c = (float)Math.Cos( angle );
			}
		}

		/// <summary>
		/// Set to the identity rotation
		/// </summary>
		public void SetIdentity()
		{
			s = 0.0f;
			c = 1.0f;
		}

		/// <summary>
		/// Get the angle in radians
		/// </summary>
		public float GetAngle()
		{
			return (float)Math.Atan2( s, c );
		}

		/// <summary>
		/// Get the x-axis
		/// </summary>
		public vec2 GetXAxis()
		{
			return new vec2( c, s );
		}

		/// <summary>
		/// Get the y-axis
		/// </summary>
		public vec2 GetYAxis()
		{
			return new vec2( -s, c );
		}
	}


	/// <summary>
	/// A transform contains translation and rotation. It is used to represent
	/// the position and orientation of rigid frames.
	/// </summary>
	public struct Transform
	{
		public vec2 p;
		public Rot q;

		/// <summary>
		/// Initialize using a position vector and a rotation matrix.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The r.</param>
		public Transform( ref vec2 position, ref Rot rotation )
		{
			p = position;
			q = rotation;
		}

		/// <summary>
		/// Set this to the identity transform.
		/// </summary>
		public void SetIdentity()
		{
			p = vec2.Zero;
			q.SetIdentity();
		}

		/// <summary>
		/// Set this based on the position and angle.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="angle">The angle.</param>
		public void Set( vec2 position, float angle )
		{
			p = position;
			q.Set( angle );
		}
	}


	/// <summary>
	/// This describes the motion of a body/shape for TOI computation.
	/// Shapes are defined with respect to the body origin, which may
	/// no coincide with the center of mass. However, to support dynamics
	/// we must interpolate the center of mass position.
	/// </summary>
	public struct Sweep
	{
		/// <summary>
		/// World angles
		/// </summary>
		public float a;

		public float a0;

		/// <summary>
		/// Fraction of the current time step in the range [0,1]
		/// c0 and a0 are the positions at alpha0.
		/// </summary>
		public float alpha0;

		/// <summary>
		/// Center world positions
		/// </summary>
		public vec2 c;

		public vec2 c0;

		/// <summary>
		/// Local center of mass position
		/// </summary>
		public vec2 localCenter;


		/// <summary>
		/// Get the interpolated transform at a specific time.
		/// </summary>
		/// <param name="xfb">The transform.</param>
		/// <param name="beta">beta is a factor in [0,1], where 0 indicates alpha0.</param>
		public void getTransform( out Transform xfb, float beta )
		{
			xfb = new Transform();
			xfb.p.x = ( 1.0f - beta ) * c0.x + beta * c.x;
			xfb.p.y = ( 1.0f - beta ) * c0.y + beta * c.y;
			var angle = ( 1.0f - beta ) * a0 + beta * a;
			xfb.q.Set( angle );

			// Shift to origin
			xfb.p -= MathUtils.mul( xfb.q, localCenter );
		}

		/// <summary>
		/// Advance the sweep forward, yielding a new initial state.
		/// </summary>
		/// <param name="alpha">new initial time..</param>
		public void advance( float alpha )
		{
			Debug.Assert( alpha0 < 1.0f );
			var beta = ( alpha - alpha0 ) / ( 1.0f - alpha0 );
			c0 += beta * ( c - c0 );
			a0 += beta * ( a - a0 );
			alpha0 = alpha;
		}

		/// <summary>
		/// Normalize the angles.
		/// </summary>
		public void normalize()
		{
			var d = MathHelper.TwoPi * (float)Math.Floor( a0 / MathHelper.TwoPi );
			a0 -= d;
			a -= d;
		}
	}

}