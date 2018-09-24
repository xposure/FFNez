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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Nez.Collision.Shapes;
using Nez.Common;
using Microsoft.Xna.Framework;


namespace Nez.Collision
{
	enum ContactFeatureType : byte
	{
		Vertex = 0,
		Face = 1,
	}

	/// <summary>
	/// The features that intersect to form the contact point
	/// This must be 4 bytes or less.
	/// </summary>
	public struct ContactFeature
	{
		/// <summary>
		/// Feature index on ShapeA
		/// </summary>
		public byte indexA;

		/// <summary>
		/// Feature index on ShapeB
		/// </summary>
		public byte indexB;

		/// <summary>
		/// The feature type on ShapeA
		/// </summary>
		public byte typeA;

		/// <summary>
		/// The feature type on ShapeB
		/// </summary>
		public byte typeB;
	}


	/// <summary>
	/// Contact ids to facilitate warm starting.
	/// </summary>
	[StructLayout( LayoutKind.Explicit )]
	public struct ContactID
	{
		/// <summary>
		/// The features that intersect to form the contact point
		/// </summary>
		[FieldOffset( 0 )]
		public ContactFeature features;

		/// <summary>
		/// Used to quickly compare contact ids.
		/// </summary>
		[FieldOffset( 0 )]
		public uint key;
	}


	/// <summary>
	/// A manifold point is a contact point belonging to a contact
	/// manifold. It holds details related to the geometry and dynamics
	/// of the contact points.
	/// The local point usage depends on the manifold type:
	/// -ShapeType.Circles: the local center of circleB
	/// -SeparationFunction.FaceA: the local center of cirlceB or the clip point of polygonB
	/// -SeparationFunction.FaceB: the clip point of polygonA
	/// This structure is stored across time steps, so we keep it small.
	/// Note: the impulses are used for internal caching and may not
	/// provide reliable contact forces, especially for high speed collisions.
	/// </summary>
	public struct ManifoldPoint
	{
		/// <summary>
		/// Uniquely identifies a contact point between two Shapes
		/// </summary>
		public ContactID id;

		/// <summary>
		/// Usage depends on manifold type
		/// </summary>
		public vec2 localPoint;

		/// <summary>
		/// The non-penetration impulse
		/// </summary>
		public float normalImpulse;

		/// <summary>
		/// The friction impulse
		/// </summary>
		public float tangentImpulse;
	}


	public enum ManifoldType
	{
		Circles,
		FaceA,
		FaceB
	}


	/// <summary>
	/// A manifold for two touching convex Shapes.
	/// Box2D supports multiple types of contact:
	/// - Clip point versus plane with radius
	/// - Point versus point with radius (circles)
	/// The local point usage depends on the manifold type:
	/// - ShapeType.Circles: the local center of circleA
	/// - SeparationFunction.FaceA: the center of faceA
	/// - SeparationFunction.FaceB: the center of faceB
	/// Similarly the local normal usage:
	/// - ShapeType.Circles: not used
	/// - SeparationFunction.FaceA: the normal on polygonA
	/// - SeparationFunction.FaceB: the normal on polygonB
	/// We store contacts in this way so that position correction can
	/// account for movement, which is critical for continuous physics.
	/// All contact scenarios must be expressed in one of these types.
	/// This structure is stored across time steps, so we keep it small.
	/// </summary>
	public struct Manifold
	{
		/// <summary>
		/// Not use for Type.SeparationFunction.Points
		/// </summary>
		public vec2 localNormal;

		/// <summary>
		/// Usage depends on manifold type
		/// </summary>
		public vec2 localPoint;

		/// <summary>
		/// The number of manifold points
		/// </summary>
		public int pointCount;

		/// <summary>
		/// The points of contact
		/// </summary>
		public FixedArray2<ManifoldPoint> points;

		public ManifoldType type;
	}


	/// <summary>
	/// This is used for determining the state of contact points.
	/// </summary>
	public enum PointState
	{
		/// <summary>
		/// Point does not exist
		/// </summary>
		Null,

		/// <summary>
		/// Point was added in the update
		/// </summary>
		Add,

		/// <summary>
		/// Point persisted across the update
		/// </summary>
		Persist,

		/// <summary>
		/// Point was removed in the update
		/// </summary>
		Remove,
	}


	/// <summary>
	/// Used for computing contact manifolds.
	/// </summary>
	public struct ClipVertex
	{
		public ContactID ID;
		public vec2 V;
	}


	/// <summary>
	/// Ray-cast input data.
	/// </summary>
	public struct RayCastInput
	{
		/// <summary>
		/// The ray extends from p1 to p1 + maxFraction * (p2 - p1).
		/// If you supply a max fraction of 1, the ray extends from p1 to p2.
		/// A max fraction of 0.5 makes the ray go from p1 and half way to p2.
		/// </summary>
		public float maxFraction;

		/// <summary>
		/// The starting point of the ray.
		/// </summary>
		public vec2 point1;

		/// <summary>
		/// The ending point of the ray.
		/// </summary>
		public vec2 point2;
	}


	/// <summary>
	/// Ray-cast output data. 
	/// </summary>
	public struct RayCastOutput
	{
		/// <summary>
		/// The ray hits at p1 + fraction * (p2 - p1), where p1 and p2 come from RayCastInput.
		/// Contains the actual fraction of the ray where it has the intersection point.
		/// </summary>
		public float fraction;

		/// <summary>
		/// The normal of the face of the shape the ray has hit.
		/// </summary>
		public vec2 normal;
	}


	/// <summary>
	/// An axis aligned bounding box.
	/// </summary>
	public struct AABB
	{
		#region Properties/Fields

		/// <summary>
		/// The lower vertex
		/// </summary>
		public vec2 lowerBound;

		/// <summary>
		/// The upper vertex
		/// </summary>
		public vec2 upperBound;

		public float width
		{
			get { return upperBound.x - lowerBound.x; }
		}

		public float height
		{
			get { return upperBound.y - lowerBound.y; }
		}

		/// <summary>
		/// Get the center of the AABB.
		/// </summary>
		public vec2 center
		{
			get { return 0.5f * ( lowerBound + upperBound ); }
		}

		/// <summary>
		/// Get the extents of the AABB (half-widths).
		/// </summary>
		public vec2 extents
		{
			get { return 0.5f * ( upperBound - lowerBound ); }
		}

		/// <summary>
		/// Get the perimeter length
		/// </summary>
		public float perimeter
		{
			get
			{
				float wx = upperBound.x - lowerBound.x;
				float wy = upperBound.y - lowerBound.y;
				return 2.0f * ( wx + wy );
			}
		}

		/// <summary>
		/// Gets the vertices of the AABB.
		/// </summary>
		/// <value>The corners of the AABB</value>
		public Vertices vertices
		{
			get
			{
				var vertices = new Vertices( 4 );
				vertices.Add( upperBound );
				vertices.Add( new vec2( upperBound.x, lowerBound.y ) );
				vertices.Add( lowerBound );
				vertices.Add( new vec2( lowerBound.x, upperBound.y ) );
				return vertices;
			}
		}

		/// <summary>
		/// First quadrant
		/// </summary>
		public AABB q1
		{
			get { return new AABB( center, upperBound ); }
		}

		/// <summary>
		/// Second quadrant
		/// </summary>
		public AABB q2
		{
			get { return new AABB( new vec2( lowerBound.x, center.y ), new vec2( center.x, upperBound.y ) ); }
		}

		/// <summary>
		/// Third quadrant
		/// </summary>
		public AABB q3
		{
			get { return new AABB( lowerBound, center ); }
		}

		/// <summary>
		/// Forth quadrant
		/// </summary>
		public AABB q4
		{
			get { return new AABB( new vec2( center.x, lowerBound.y ), new vec2( upperBound.x, center.y ) ); }
		}

		#endregion


		public AABB( vec2 min, vec2 max ) : this( ref min, ref max )
		{
		}

		public AABB( ref vec2 min, ref vec2 max )
		{
			lowerBound = min;
			upperBound = max;
		}

		public AABB( vec2 center, float width, float height )
		{
			lowerBound = center - new vec2( width / 2, height / 2 );
			upperBound = center + new vec2( width / 2, height / 2 );
		}

		/// <summary>
		/// Verify that the bounds are sorted. And the bounds are valid numbers (not NaN).
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </returns>
		public bool isValid()
		{
			vec2 d = upperBound - lowerBound;
			bool valid = d.x >= 0.0f && d.y >= 0.0f;
			valid = valid && lowerBound.isValid() && upperBound.isValid();
			return valid;
		}

		/// <summary>
		/// Combine an AABB into this one.
		/// </summary>
		/// <param name="aabb">The aabb.</param>
		public void combine( ref AABB aabb )
		{
			lowerBound = vec2.Min( lowerBound, aabb.lowerBound );
			upperBound = vec2.Max( upperBound, aabb.upperBound );
		}

		/// <summary>
		/// Combine two AABBs into this one.
		/// </summary>
		/// <param name="aabb1">The aabb1.</param>
		/// <param name="aabb2">The aabb2.</param>
		public void combine( ref AABB aabb1, ref AABB aabb2 )
		{
			lowerBound = vec2.Min( aabb1.lowerBound, aabb2.lowerBound );
			upperBound = vec2.Max( aabb1.upperBound, aabb2.upperBound );
		}

		/// <summary>
		/// Does this aabb contain the provided AABB.
		/// </summary>
		/// <param name="aabb">The aabb.</param>
		/// <returns>
		/// 	<c>true</c> if it contains the specified aabb; otherwise, <c>false</c>.
		/// </returns>
		public bool contains( ref AABB aabb )
		{
			bool result = true;
			result = result && lowerBound.x <= aabb.lowerBound.x;
			result = result && lowerBound.y <= aabb.lowerBound.y;
			result = result && aabb.upperBound.x <= upperBound.x;
			result = result && aabb.upperBound.y <= upperBound.y;
			return result;
		}

		/// <summary>
		/// Determines whether the AAABB contains the specified point.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns>
		/// 	<c>true</c> if it contains the specified point; otherwise, <c>false</c>.
		/// </returns>
		public bool contains( ref vec2 point )
		{
			//using epsilon to try and gaurd against float rounding errors.
			return ( point.x > ( lowerBound.x + Settings.epsilon ) && point.x < ( upperBound.x - Settings.epsilon ) &&
				   ( point.y > ( lowerBound.y + Settings.epsilon ) && point.y < ( upperBound.y - Settings.epsilon ) ) );
		}

		/// <summary>
		/// Test if the two AABBs overlap.
		/// </summary>
		/// <param name="a">The first AABB.</param>
		/// <param name="b">The second AABB.</param>
		/// <returns>True if they are overlapping.</returns>
		public static bool testOverlap( ref AABB a, ref AABB b )
		{
			vec2 d1 = b.lowerBound - a.upperBound;
			vec2 d2 = a.lowerBound - b.upperBound;

			if( d1.x > 0.0f || d1.y > 0.0f )
				return false;

			if( d2.x > 0.0f || d2.y > 0.0f )
				return false;

			return true;
		}

		/// <summary>
		/// Raycast against this AABB using the specificed points and maxfraction (found in input)
		/// </summary>
		/// <returns><c>true</c>, if cast was rayed, <c>false</c> otherwise.</returns>
		/// <param name="output">Output.</param>
		/// <param name="input">Input.</param>
		/// <param name="doInteriorCheck">If set to <c>true</c> do interior check.</param>
		public bool rayCast( out RayCastOutput output, ref RayCastInput input, bool doInteriorCheck = true )
		{
			// From Real-time Collision Detection, p179.

			output = new RayCastOutput();

			float tmin = -Settings.maxFloat;
			float tmax = Settings.maxFloat;

			vec2 p = input.point1;
			vec2 d = input.point2 - input.point1;
			vec2 absD = MathUtils.abs( d );

			vec2 normal = vec2.Zero;

			for( int i = 0; i < 2; ++i )
			{
				float absD_i = i == 0 ? absD.x : absD.y;
				float lowerBound_i = i == 0 ? lowerBound.x : lowerBound.y;
				float upperBound_i = i == 0 ? upperBound.x : upperBound.y;
				float p_i = i == 0 ? p.x : p.y;

				if( absD_i < Settings.epsilon )
				{
					// Parallel.
					if( p_i < lowerBound_i || upperBound_i < p_i )
					{
						return false;
					}
				}
				else
				{
					float d_i = i == 0 ? d.x : d.y;

					float inv_d = 1.0f / d_i;
					float t1 = ( lowerBound_i - p_i ) * inv_d;
					float t2 = ( upperBound_i - p_i ) * inv_d;

					// Sign of the normal vector.
					float s = -1.0f;

					if( t1 > t2 )
					{
						MathUtils.Swap( ref t1, ref t2 );
						s = 1.0f;
					}

					// Push the min up
					if( t1 > tmin )
					{
						if( i == 0 )
						{
							normal.x = s;
						}
						else
						{
							normal.y = s;
						}

						tmin = t1;
					}

					// Pull the max down
					tmax = Math.Min( tmax, t2 );

					if( tmin > tmax )
					{
						return false;
					}
				}
			}

			// Does the ray start inside the box?
			// Does the ray intersect beyond the max fraction?
			if( doInteriorCheck && ( tmin < 0.0f || input.maxFraction < tmin ) )
			{
				return false;
			}

			// Intersection.
			output.fraction = tmin;
			output.normal = normal;
			return true;
		}

	}


	/// <summary>
	/// This holds polygon B expressed in frame A.
	/// </summary>
	public class TempPolygon
	{
		public vec2[] vertices = new vec2[Settings.maxPolygonVertices];
		public vec2[] normals = new vec2[Settings.maxPolygonVertices];
		public int count;
	}


	/// <summary>
	/// This structure is used to keep track of the best separating axis.
	/// </summary>
	public struct EPAxis
	{
		public int index;
		public float separation;
		public EPAxisType type;
	}


	/// <summary>
	/// Reference face used for clipping
	/// </summary>
	public struct ReferenceFace
	{
		public int i1, i2;

		public vec2 v1, v2;

		public vec2 normal;

		public vec2 sideNormal1;
		public float sideOffset1;

		public vec2 sideNormal2;
		public float sideOffset2;
	}


	public enum EPAxisType
	{
		Unknown,
		EdgeA,
		EdgeB,
	}


	/// <summary>
	/// Collision methods
	/// </summary>
	public static class Collision
	{
		[ThreadStatic]
		static DistanceInput _input;

		/// <summary>
		/// Test overlap between the two shapes.
		/// </summary>
		/// <param name="shapeA">The first shape.</param>
		/// <param name="indexA">The index for the first shape.</param>
		/// <param name="shapeB">The second shape.</param>
		/// <param name="indexB">The index for the second shape.</param>
		/// <param name="xfA">The transform for the first shape.</param>
		/// <param name="xfB">The transform for the seconds shape.</param>
		/// <returns></returns>
		public static bool testOverlap( Shape shapeA, int indexA, Shape shapeB, int indexB, ref Transform xfA, ref Transform xfB )
		{
			_input = _input ?? new DistanceInput();
			_input.ProxyA.set( shapeA, indexA );
			_input.ProxyB.set( shapeB, indexB );
			_input.TransformA = xfA;
			_input.TransformB = xfB;
			_input.UseRadii = true;

			SimplexCache cache;
			DistanceOutput output;
			Distance.computeDistance( out output, out cache, _input );

			return output.Distance < 10.0f * Settings.epsilon;
		}

		public static void getPointStates( out FixedArray2<PointState> state1, out FixedArray2<PointState> state2, ref Manifold manifold1, ref Manifold manifold2 )
		{
			state1 = new FixedArray2<PointState>();
			state2 = new FixedArray2<PointState>();

			// Detect persists and removes.
			for( int i = 0; i < manifold1.pointCount; ++i )
			{
				ContactID id = manifold1.points[i].id;

				state1[i] = PointState.Remove;

				for( int j = 0; j < manifold2.pointCount; ++j )
				{
					if( manifold2.points[j].id.key == id.key )
					{
						state1[i] = PointState.Persist;
						break;
					}
				}
			}

			// Detect persists and adds.
			for( int i = 0; i < manifold2.pointCount; ++i )
			{
				ContactID id = manifold2.points[i].id;

				state2[i] = PointState.Add;

				for( int j = 0; j < manifold1.pointCount; ++j )
				{
					if( manifold1.points[j].id.key == id.key )
					{
						state2[i] = PointState.Persist;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Compute the collision manifold between two circles.
		/// </summary>
		public static void collideCircles( ref Manifold manifold, CircleShape circleA, ref Transform xfA, CircleShape circleB, ref Transform xfB )
		{
			manifold.pointCount = 0;

			var pA = MathUtils.mul( ref xfA, circleA.position );
			var pB = MathUtils.mul( ref xfB, circleB.position );

			var d = pB - pA;
			var distSqr = vec2.Dot( d, d );
			var radius = circleA.radius + circleB.radius;
			if( distSqr > radius * radius )
				return;

			manifold.type = ManifoldType.Circles;
			manifold.localPoint = circleA.position;
			manifold.localNormal = vec2.Zero;
			manifold.pointCount = 1;

			var p0 = manifold.points[0];
			p0.localPoint = circleB.position;
			p0.id.key = 0;

			manifold.points[0] = p0;
		}

		/// <summary>
		/// Compute the collision manifold between a polygon and a circle.
		/// </summary>
		/// <param name="manifold">The manifold.</param>
		/// <param name="polygonA">The polygon A.</param>
		/// <param name="xfA">The transform of A.</param>
		/// <param name="circleB">The circle B.</param>
		/// <param name="xfB">The transform of B.</param>
		public static void collidePolygonAndCircle( ref Manifold manifold, PolygonShape polygonA, ref Transform xfA, CircleShape circleB, ref Transform xfB )
		{
			manifold.pointCount = 0;

			// Compute circle position in the frame of the polygon.
			var c = MathUtils.mul( ref xfB, circleB.position );
			var cLocal = MathUtils.mulT( ref xfA, c );

			// Find the min separating edge.
			var normalIndex = 0;
			var separation = -Settings.maxFloat;
			var radius = polygonA.radius + circleB.radius;
			var vertexCount = polygonA.vertices.Count;

			for( int i = 0; i < vertexCount; ++i )
			{
				var value1 = polygonA.normals[i];
				var value2 = cLocal - polygonA.vertices[i];
				var s = value1.x * value2.X + value1.y * value2.Y;

				if( s > radius )
				{
					// Early out.
					return;
				}

				if( s > separation )
				{
					separation = s;
					normalIndex = i;
				}
			}

			// Vertices that subtend the incident face.
			var vertIndex1 = normalIndex;
			var vertIndex2 = vertIndex1 + 1 < vertexCount ? vertIndex1 + 1 : 0;
			var v1 = polygonA.vertices[vertIndex1];
			var v2 = polygonA.vertices[vertIndex2];

			// If the center is inside the polygon ...
			if( separation < Settings.epsilon )
			{
				manifold.pointCount = 1;
				manifold.type = ManifoldType.FaceA;
				manifold.localNormal = polygonA.normals[normalIndex];
				manifold.localPoint = 0.5f * ( v1 + v2 );

				var p0 = manifold.points[0];
				p0.localPoint = circleB.position;
				p0.id.key = 0;

				manifold.points[0] = p0;

				return;
			}

			// Compute barycentric coordinates
			var u1 = ( cLocal.X - v1.x ) * ( v2.x - v1.x ) + ( cLocal.Y - v1.y ) * ( v2.y - v1.y );
			var u2 = ( cLocal.X - v2.x ) * ( v1.x - v2.x ) + ( cLocal.Y - v2.y ) * ( v1.y - v2.y );

			if( u1 <= 0.0f )
			{
				var r = ( cLocal.X - v1.x ) * ( cLocal.X - v1.x ) + ( cLocal.Y - v1.y ) * ( cLocal.Y - v1.y );
				if( r > radius * radius )
					return;

				manifold.pointCount = 1;
				manifold.type = ManifoldType.FaceA;
				manifold.localNormal = cLocal - v1;
				float factor = 1f /
							   (float)
							   Math.Sqrt( manifold.localNormal.x * manifold.localNormal.x +
										 manifold.localNormal.y * manifold.localNormal.y );
				manifold.localNormal.x = manifold.localNormal.x * factor;
				manifold.localNormal.y = manifold.localNormal.y * factor;
				manifold.localPoint = v1;

				var p0b = manifold.points[0];
				p0b.localPoint = circleB.position;
				p0b.id.key = 0;

				manifold.points[0] = p0b;
			}
			else if( u2 <= 0.0f )
			{
				float r = ( cLocal.X - v2.x ) * ( cLocal.X - v2.x ) + ( cLocal.Y - v2.y ) * ( cLocal.Y - v2.y );
				if( r > radius * radius )
					return;

				manifold.pointCount = 1;
				manifold.type = ManifoldType.FaceA;
				manifold.localNormal = cLocal - v2;
				float factor = 1f /
							   (float)
							   Math.Sqrt( manifold.localNormal.x * manifold.localNormal.x +
										 manifold.localNormal.y * manifold.localNormal.y );
				manifold.localNormal.x = manifold.localNormal.x * factor;
				manifold.localNormal.y = manifold.localNormal.y * factor;
				manifold.localPoint = v2;

				var p0c = manifold.points[0];
				p0c.localPoint = circleB.position;
				p0c.id.key = 0;

				manifold.points[0] = p0c;
			}
			else
			{
				var faceCenter = 0.5f * ( v1 + v2 );
				var value1 = cLocal - faceCenter;
				var value2 = polygonA.normals[vertIndex1];
				var separation2 = value1.X * value2.x + value1.Y * value2.y;
				if( separation2 > radius )
					return;

				manifold.pointCount = 1;
				manifold.type = ManifoldType.FaceA;
				manifold.localNormal = polygonA.normals[vertIndex1];
				manifold.localPoint = faceCenter;

				var p0d = manifold.points[0];
				p0d.localPoint = circleB.position;
				p0d.id.key = 0;

				manifold.points[0] = p0d;
			}
		}

		/// <summary>
		/// Compute the collision manifold between two polygons.
		/// </summary>
		/// <param name="manifold">The manifold.</param>
		/// <param name="polyA">The poly A.</param>
		/// <param name="transformA">The transform A.</param>
		/// <param name="polyB">The poly B.</param>
		/// <param name="transformB">The transform B.</param>
		public static void collidePolygons( ref Manifold manifold, PolygonShape polyA, ref Transform transformA, PolygonShape polyB, ref Transform transformB )
		{
			manifold.pointCount = 0;
			var totalRadius = polyA.radius + polyB.radius;

			int edgeA = 0;
			var separationA = findMaxSeparation( out edgeA, polyA, ref transformA, polyB, ref transformB );
			if( separationA > totalRadius )
				return;

			int edgeB = 0;
			var separationB = findMaxSeparation( out edgeB, polyB, ref transformB, polyA, ref transformA );
			if( separationB > totalRadius )
				return;

			PolygonShape poly1; // reference polygon
			PolygonShape poly2; // incident polygon
			Transform xf1, xf2;
			int edge1; // reference edge
			bool flip;
			const float k_relativeTol = 0.98f;
			const float k_absoluteTol = 0.001f;

			if( separationB > k_relativeTol * separationA + k_absoluteTol )
			{
				poly1 = polyB;
				poly2 = polyA;
				xf1 = transformB;
				xf2 = transformA;
				edge1 = edgeB;
				manifold.type = ManifoldType.FaceB;
				flip = true;
			}
			else
			{
				poly1 = polyA;
				poly2 = polyB;
				xf1 = transformA;
				xf2 = transformB;
				edge1 = edgeA;
				manifold.type = ManifoldType.FaceA;
				flip = false;
			}

			FixedArray2<ClipVertex> incidentEdge;
			findIncidentEdge( out incidentEdge, poly1, ref xf1, edge1, poly2, ref xf2 );

			int count1 = poly1.vertices.Count;

			int iv1 = edge1;
			int iv2 = edge1 + 1 < count1 ? edge1 + 1 : 0;

			var v11 = poly1.vertices[iv1];
			var v12 = poly1.vertices[iv2];

			var localTangent = v12 - v11;
            localTangent.Normalize();
			//Nez.Vector2Ext.normalize( ref localTangent );

			var localNormal = new vec2( localTangent.y, -localTangent.x );
			var planePoint = 0.5f * ( v11 + v12 );

			var tangent = MathUtils.mul( xf1.q, localTangent );

			var normalx = tangent.Y;
			var normaly = -tangent.X;

			v11 = MathUtils.mul( ref xf1, v11 );
			v12 = MathUtils.mul( ref xf1, v12 );

			// Face offset.
			var frontOffset = normalx * v11.x + normaly * v11.y;

			// Side offsets, extended by polytope skin thickness.
			var sideOffset1 = -( tangent.X * v11.x + tangent.Y * v11.y ) + totalRadius;
			var sideOffset2 = tangent.X * v12.x + tangent.Y * v12.y + totalRadius;

			// Clip incident edge against extruded edge1 side edges.
			FixedArray2<ClipVertex> clipPoints1;
			FixedArray2<ClipVertex> clipPoints2;

			// Clip to box side 1
			var np = clipSegmentToLine( out clipPoints1, ref incidentEdge, -tangent, sideOffset1, iv1 );
			if( np < 2 )
				return;

			// Clip to negative box side 1
			np = clipSegmentToLine( out clipPoints2, ref clipPoints1, tangent, sideOffset2, iv2 );
			if( np < 2 )
				return;

			// Now clipPoints2 contains the clipped points.
			manifold.localNormal = localNormal;
			manifold.localPoint = planePoint;

			var pointCount = 0;
			for( var i = 0; i < Settings.maxManifoldPoints; ++i )
			{
				var value = clipPoints2[i].V;
				var separation = normalx * value.x + normaly * value.y - frontOffset;

				if( separation <= totalRadius )
				{
					var cp = manifold.points[pointCount];
					cp.localPoint = MathUtils.mulT( ref xf2, clipPoints2[i].V );
					cp.id = clipPoints2[i].ID;

					if( flip )
					{
						// Swap features
						var cf = cp.id.features;
						cp.id.features.indexA = cf.indexB;
						cp.id.features.indexB = cf.indexA;
						cp.id.features.typeA = cf.typeB;
						cp.id.features.typeB = cf.typeA;
					}

					manifold.points[pointCount] = cp;

					++pointCount;
				}
			}

			manifold.pointCount = pointCount;
		}

		/// <summary>
		/// Compute contact points for edge versus circle.
		/// This accounts for edge connectivity.
		/// </summary>
		/// <param name="manifold">The manifold.</param>
		/// <param name="edgeA">The edge A.</param>
		/// <param name="transformA">The transform A.</param>
		/// <param name="circleB">The circle B.</param>
		/// <param name="transformB">The transform B.</param>
		public static void collideEdgeAndCircle( ref Manifold manifold, EdgeShape edgeA, ref Transform transformA, CircleShape circleB, ref Transform transformB )
		{
			manifold.pointCount = 0;

			// Compute circle in frame of edge
			var Q = MathUtils.mulT( ref transformA, MathUtils.mul( ref transformB, ref circleB._position ) );

			vec2 A = edgeA.vertex1, B = edgeA.vertex2;
			var e = B - A;

			// Barycentric coordinates
			var u = vec2.Dot( e, B - Q );
			var v = vec2.Dot( e, Q - A );

			var radius = edgeA.radius + circleB.radius;

			ContactFeature cf;
			cf.indexB = 0;
			cf.typeB = (byte)ContactFeatureType.Vertex;

			vec2 P, d;

			// Region A
			if( v <= 0.0f )
			{
				P = A;
				d = Q - P;
				float dd;
				vec2.Dot( ref d, ref d, out dd );
				if( dd > radius * radius )
					return;

				// Is there an edge connected to A?
				if( edgeA.hasVertex0 )
				{
					var A1 = edgeA.vertex0;
					var B1 = A;
					var e1 = B1 - A1;
					var u1 = vec2.Dot( e1, B1 - Q );

					// Is the circle in Region AB of the previous edge?
					if( u1 > 0.0f )
						return;
				}

				cf.indexA = 0;
				cf.typeA = (byte)ContactFeatureType.Vertex;
				manifold.pointCount = 1;
				manifold.type = ManifoldType.Circles;
				manifold.localNormal = vec2.Zero;
				manifold.localPoint = P;

				var mp = new ManifoldPoint();
				mp.id.key = 0;
				mp.id.features = cf;
				mp.localPoint = circleB.position;
				manifold.points[0] = mp;
				return;
			}

			// Region B
			if( u <= 0.0f )
			{
				P = B;
				d = Q - P;
				float dd;
				vec2.Dot( ref d, ref d, out dd );
				if( dd > radius * radius )
					return;

				// Is there an edge connected to B?
				if( edgeA.hasVertex3 )
				{
					var B2 = edgeA.vertex3;
					var A2 = B;
					var e2 = B2 - A2;
					var v2 = vec2.Dot( e2, Q - A2 );

					// Is the circle in Region AB of the next edge?
					if( v2 > 0.0f )
						return;
				}

				cf.indexA = 1;
				cf.typeA = (byte)ContactFeatureType.Vertex;
				manifold.pointCount = 1;
				manifold.type = ManifoldType.Circles;
				manifold.localNormal = vec2.Zero;
				manifold.localPoint = P;

				var mp = new ManifoldPoint();
				mp.id.key = 0;
				mp.id.features = cf;
				mp.localPoint = circleB.position;
				manifold.points[0] = mp;
				return;
			}

			// Region AB
			float den;
			vec2.Dot( ref e, ref e, out den );
			Debug.Assert( den > 0.0f );
			P = ( 1.0f / den ) * ( u * A + v * B );
			d = Q - P;
			float dd2;
			vec2.Dot( ref d, ref d, out dd2 );
			if( dd2 > radius * radius )
				return;

			var n = new vec2( -e.y, e.x );
			if( vec2.Dot( n, Q - A ) < 0.0f )
				n = new vec2( -n.x, -n.y );
            n.Normalize();
			//Nez.Vector2Ext.normalize( ref n );

			cf.indexA = 0;
			cf.typeA = (byte)ContactFeatureType.Face;
			manifold.pointCount = 1;
			manifold.type = ManifoldType.FaceA;
			manifold.localNormal = n;
			manifold.localPoint = A;
			ManifoldPoint mp2 = new ManifoldPoint();
			mp2.id.key = 0;
			mp2.id.features = cf;
			mp2.localPoint = circleB.position;
			manifold.points[0] = mp2;
		}

		/// <summary>
		/// Collides and edge and a polygon, taking into account edge adjacency.
		/// </summary>
		/// <param name="manifold">The manifold.</param>
		/// <param name="edgeA">The edge A.</param>
		/// <param name="xfA">The xf A.</param>
		/// <param name="polygonB">The polygon B.</param>
		/// <param name="xfB">The xf B.</param>
		public static void collideEdgeAndPolygon( ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB )
		{
			var collider = new EPCollider();
			collider.collide( ref manifold, edgeA, ref xfA, polygonB, ref xfB );
		}


		class EPCollider
		{
			TempPolygon _polygonB = new TempPolygon();

			Transform _xf;
			vec2 _centroidB;
			vec2 _v0, _v1, _v2, _v3;
			vec2 _normal0, _normal1, _normal2;
			vec2 _normal;
			vec2 _lowerLimit, _upperLimit;
			float _radius;
			bool _front;

			public void collide( ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB )
			{
				// Algorithm:
				// 1. Classify v1 and v2
				// 2. Classify polygon centroid as front or back
				// 3. Flip normal if necessary
				// 4. Initialize normal range to [-pi, pi] about face normal
				// 5. Adjust normal range according to adjacent edges
				// 6. Visit each separating axes, only accept axes within the range
				// 7. Return if _any_ axis indicates separation
				// 8. Clip

				_xf = MathUtils.mulT( xfA, xfB );

				_centroidB = MathUtils.mul( ref _xf, polygonB.massData.centroid );

				_v0 = edgeA.vertex0;
				_v1 = edgeA._vertex1;
				_v2 = edgeA._vertex2;
				_v3 = edgeA.vertex3;

				var hasVertex0 = edgeA.hasVertex0;
				var hasVertex3 = edgeA.hasVertex3;

				var edge1 = _v2 - _v1;
                edge1.Normalize();
				//Nez.Vector2Ext.normalize( ref edge1 );
				_normal1 = new vec2( edge1.y, -edge1.x );
				var offset1 = vec2.Dot( _normal1, _centroidB - _v1 );
				float offset0 = 0.0f, offset2 = 0.0f;
				bool convex1 = false, convex2 = false;

				// Is there a preceding edge?
				if( hasVertex0 )
				{
					var edge0 = _v1 - _v0;
                    edge0.Normalize();
					//Nez.Vector2Ext.normalize( ref edge0 );
					_normal0 = new vec2( edge0.y, -edge0.x );
					convex1 = MathUtils.cross( edge0, edge1 ) >= 0.0f;
					offset0 = vec2.Dot( _normal0, _centroidB - _v0 );
				}

				// Is there a following edge?
				if( hasVertex3 )
				{
					var edge2 = _v3 - _v2;
                    edge2.Normalize();
					//Nez.Vector2Ext.normalize( ref edge2 );
					_normal2 = new vec2( edge2.y, -edge2.x );
					convex2 = MathUtils.cross( edge1, edge2 ) > 0.0f;
					offset2 = vec2.Dot( _normal2, _centroidB - _v2 );
				}

				// Determine front or back collision. Determine collision normal limits.
				if( hasVertex0 && hasVertex3 )
				{
					if( convex1 && convex2 )
					{
						_front = offset0 >= 0.0f || offset1 >= 0.0f || offset2 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal0;
							_upperLimit = _normal2;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal1;
							_upperLimit = -_normal1;
						}
					}
					else if( convex1 )
					{
						_front = offset0 >= 0.0f || ( offset1 >= 0.0f && offset2 >= 0.0f );
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal0;
							_upperLimit = _normal1;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal2;
							_upperLimit = -_normal1;
						}
					}
					else if( convex2 )
					{
						_front = offset2 >= 0.0f || ( offset0 >= 0.0f && offset1 >= 0.0f );
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal1;
							_upperLimit = _normal2;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal1;
							_upperLimit = -_normal0;
						}
					}
					else
					{
						_front = offset0 >= 0.0f && offset1 >= 0.0f && offset2 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal1;
							_upperLimit = _normal1;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal2;
							_upperLimit = -_normal0;
						}
					}
				}
				else if( hasVertex0 )
				{
					if( convex1 )
					{
						_front = offset0 >= 0.0f || offset1 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal0;
							_upperLimit = -_normal1;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = _normal1;
							_upperLimit = -_normal1;
						}
					}
					else
					{
						_front = offset0 >= 0.0f && offset1 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = _normal1;
							_upperLimit = -_normal1;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = _normal1;
							_upperLimit = -_normal0;
						}
					}
				}
				else if( hasVertex3 )
				{
					if( convex2 )
					{
						_front = offset1 >= 0.0f || offset2 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = -_normal1;
							_upperLimit = _normal2;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal1;
							_upperLimit = _normal1;
						}
					}
					else
					{
						_front = offset1 >= 0.0f && offset2 >= 0.0f;
						if( _front )
						{
							_normal = _normal1;
							_lowerLimit = -_normal1;
							_upperLimit = _normal1;
						}
						else
						{
							_normal = -_normal1;
							_lowerLimit = -_normal2;
							_upperLimit = _normal1;
						}
					}
				}
				else
				{
					_front = offset1 >= 0.0f;
					if( _front )
					{
						_normal = _normal1;
						_lowerLimit = -_normal1;
						_upperLimit = -_normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = _normal1;
						_upperLimit = _normal1;
					}
				}

				// Get polygonB in frameA
				_polygonB.count = polygonB.vertices.Count;
				for( int i = 0; i < polygonB.vertices.Count; ++i )
				{
					_polygonB.vertices[i] = MathUtils.mul( ref _xf, polygonB.vertices[i] );
					_polygonB.normals[i] = MathUtils.mul( _xf.q, polygonB.normals[i] );
				}

				_radius = 2.0f * Settings.polygonRadius;

				manifold.pointCount = 0;

				EPAxis edgeAxis = ComputeEdgeSeparation();

				// If no valid normal can be found than this edge should not collide.
				if( edgeAxis.type == EPAxisType.Unknown )
				{
					return;
				}

				if( edgeAxis.separation > _radius )
				{
					return;
				}

				EPAxis polygonAxis = ComputePolygonSeparation();
				if( polygonAxis.type != EPAxisType.Unknown && polygonAxis.separation > _radius )
				{
					return;
				}

				// Use hysteresis for jitter reduction.
				const float k_relativeTol = 0.98f;
				const float k_absoluteTol = 0.001f;

				EPAxis primaryAxis;
				if( polygonAxis.type == EPAxisType.Unknown )
				{
					primaryAxis = edgeAxis;
				}
				else if( polygonAxis.separation > k_relativeTol * edgeAxis.separation + k_absoluteTol )
				{
					primaryAxis = polygonAxis;
				}
				else
				{
					primaryAxis = edgeAxis;
				}

				FixedArray2<ClipVertex> ie = new FixedArray2<ClipVertex>();
				ReferenceFace rf;
				if( primaryAxis.type == EPAxisType.EdgeA )
				{
					manifold.type = ManifoldType.FaceA;

					// Search for the polygon normal that is most anti-parallel to the edge normal.
					int bestIndex = 0;
					var bestValue = vec2.Dot( _normal, _polygonB.normals[0] );
					for( int i = 1; i < _polygonB.count; ++i )
					{
						var value = vec2.Dot( _normal, _polygonB.normals[i] );
						if( value < bestValue )
						{
							bestValue = value;
							bestIndex = i;
						}
					}

					int i1 = bestIndex;
					int i2 = i1 + 1 < _polygonB.count ? i1 + 1 : 0;

					ClipVertex c0 = ie[0];
					c0.V = _polygonB.vertices[i1];
					c0.ID.features.indexA = 0;
					c0.ID.features.indexB = (byte)i1;
					c0.ID.features.typeA = (byte)ContactFeatureType.Face;
					c0.ID.features.typeB = (byte)ContactFeatureType.Vertex;
					ie[0] = c0;

					ClipVertex c1 = ie[1];
					c1.V = _polygonB.vertices[i2];
					c1.ID.features.indexA = 0;
					c1.ID.features.indexB = (byte)i2;
					c1.ID.features.typeA = (byte)ContactFeatureType.Face;
					c1.ID.features.typeB = (byte)ContactFeatureType.Vertex;
					ie[1] = c1;

					if( _front )
					{
						rf.i1 = 0;
						rf.i2 = 1;
						rf.v1 = _v1;
						rf.v2 = _v2;
						rf.normal = _normal1;
					}
					else
					{
						rf.i1 = 1;
						rf.i2 = 0;
						rf.v1 = _v2;
						rf.v2 = _v1;
						rf.normal = -_normal1;
					}
				}
				else
				{
					manifold.type = ManifoldType.FaceB;
					ClipVertex c0 = ie[0];
					c0.V = _v1;
					c0.ID.features.indexA = 0;
					c0.ID.features.indexB = (byte)primaryAxis.index;
					c0.ID.features.typeA = (byte)ContactFeatureType.Vertex;
					c0.ID.features.typeB = (byte)ContactFeatureType.Face;
					ie[0] = c0;

					ClipVertex c1 = ie[1];
					c1.V = _v2;
					c1.ID.features.indexA = 0;
					c1.ID.features.indexB = (byte)primaryAxis.index;
					c1.ID.features.typeA = (byte)ContactFeatureType.Vertex;
					c1.ID.features.typeB = (byte)ContactFeatureType.Face;
					ie[1] = c1;

					rf.i1 = primaryAxis.index;
					rf.i2 = rf.i1 + 1 < _polygonB.count ? rf.i1 + 1 : 0;
					rf.v1 = _polygonB.vertices[rf.i1];
					rf.v2 = _polygonB.vertices[rf.i2];
					rf.normal = _polygonB.normals[rf.i1];
				}

				rf.sideNormal1 = new vec2( rf.normal.y, -rf.normal.x );
				rf.sideNormal2 = -rf.sideNormal1;
				rf.sideOffset1 = vec2.Dot( rf.sideNormal1, rf.v1 );
				rf.sideOffset2 = vec2.Dot( rf.sideNormal2, rf.v2 );

				// Clip incident edge against extruded edge1 side edges.
				FixedArray2<ClipVertex> clipPoints1;
				FixedArray2<ClipVertex> clipPoints2;
				int np;

				// Clip to box side 1
				np = clipSegmentToLine( out clipPoints1, ref ie, rf.sideNormal1, rf.sideOffset1, rf.i1 );

				if( np < Settings.maxManifoldPoints )
				{
					return;
				}

				// Clip to negative box side 1
				np = clipSegmentToLine( out clipPoints2, ref clipPoints1, rf.sideNormal2, rf.sideOffset2, rf.i2 );

				if( np < Settings.maxManifoldPoints )
				{
					return;
				}

				// Now clipPoints2 contains the clipped points.
				if( primaryAxis.type == EPAxisType.EdgeA )
				{
					manifold.localNormal = rf.normal;
					manifold.localPoint = rf.v1;
				}
				else
				{
					manifold.localNormal = polygonB.normals[rf.i1];
					manifold.localPoint = polygonB.vertices[rf.i1];
				}

				int pointCount = 0;
				for( int i = 0; i < Settings.maxManifoldPoints; ++i )
				{
					float separation = vec2.Dot( rf.normal, clipPoints2[i].V - rf.v1 );

					if( separation <= _radius )
					{
						ManifoldPoint cp = manifold.points[pointCount];

						if( primaryAxis.type == EPAxisType.EdgeA )
						{
							cp.localPoint = MathUtils.mulT( ref _xf, clipPoints2[i].V );
							cp.id = clipPoints2[i].ID;
						}
						else
						{
							cp.localPoint = clipPoints2[i].V;
							cp.id.features.typeA = clipPoints2[i].ID.features.typeB;
							cp.id.features.typeB = clipPoints2[i].ID.features.typeA;
							cp.id.features.indexA = clipPoints2[i].ID.features.indexB;
							cp.id.features.indexB = clipPoints2[i].ID.features.indexA;
						}

						manifold.points[pointCount] = cp;
						++pointCount;
					}
				}

				manifold.pointCount = pointCount;
			}

			EPAxis ComputeEdgeSeparation()
			{
				EPAxis axis;
				axis.type = EPAxisType.EdgeA;
				axis.index = _front ? 0 : 1;
				axis.separation = Settings.maxFloat;

				for( int i = 0; i < _polygonB.count; ++i )
				{
					float s = vec2.Dot( _normal, _polygonB.vertices[i] - _v1 );
					if( s < axis.separation )
					{
						axis.separation = s;
					}
				}

				return axis;
			}

			EPAxis ComputePolygonSeparation()
			{
				EPAxis axis;
				axis.type = EPAxisType.Unknown;
				axis.index = -1;
				axis.separation = -Settings.maxFloat;

				vec2 perp = new vec2( -_normal.y, _normal.x );

				for( int i = 0; i < _polygonB.count; ++i )
				{
					vec2 n = -_polygonB.normals[i];

					float s1 = vec2.Dot( n, _polygonB.vertices[i] - _v1 );
					float s2 = vec2.Dot( n, _polygonB.vertices[i] - _v2 );
					float s = Math.Min( s1, s2 );

					if( s > _radius )
					{
						// No collision
						axis.type = EPAxisType.EdgeB;
						axis.index = i;
						axis.separation = s;
						return axis;
					}

					// Adjacency
					if( vec2.Dot( n, perp ) >= 0.0f )
					{
						if( vec2.Dot( n - _upperLimit, _normal ) < -Settings.angularSlop )
						{
							continue;
						}
					}
					else
					{
						if( vec2.Dot( n - _lowerLimit, _normal ) < -Settings.angularSlop )
						{
							continue;
						}
					}

					if( s > axis.separation )
					{
						axis.type = EPAxisType.EdgeB;
						axis.index = i;
						axis.separation = s;
					}
				}

				return axis;
			}
		}


		/// <summary>
		/// Clipping for contact manifolds.
		/// </summary>
		/// <param name="vOut">The v out.</param>
		/// <param name="vIn">The v in.</param>
		/// <param name="normal">The normal.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="vertexIndexA">The vertex index A.</param>
		/// <returns></returns>
		static int clipSegmentToLine( out FixedArray2<ClipVertex> vOut, ref FixedArray2<ClipVertex> vIn, vec2 normal, float offset, int vertexIndexA )
		{
			vOut = new FixedArray2<ClipVertex>();

			ClipVertex v0 = vIn[0];
			ClipVertex v1 = vIn[1];

			// Start with no output points
			int numOut = 0;

			// Calculate the distance of end points to the line
			float distance0 = normal.x * v0.V.x + normal.y * v0.V.y - offset;
			float distance1 = normal.x * v1.V.x + normal.y * v1.V.y - offset;

			// If the points are behind the plane
			if( distance0 <= 0.0f ) vOut[numOut++] = v0;
			if( distance1 <= 0.0f ) vOut[numOut++] = v1;

			// If the points are on different sides of the plane
			if( distance0 * distance1 < 0.0f )
			{
				// Find intersection point of edge and plane
				float interp = distance0 / ( distance0 - distance1 );

				ClipVertex cv = vOut[numOut];

				cv.V.x = v0.V.x + interp * ( v1.V.x - v0.V.x );
				cv.V.y = v0.V.y + interp * ( v1.V.y - v0.V.y );

				// VertexA is hitting edgeB.
				cv.ID.features.indexA = (byte)vertexIndexA;
				cv.ID.features.indexB = v0.ID.features.indexB;
				cv.ID.features.typeA = (byte)ContactFeatureType.Vertex;
				cv.ID.features.typeB = (byte)ContactFeatureType.Face;

				vOut[numOut] = cv;

				++numOut;
			}

			return numOut;
		}

		/// <summary>
		/// Find the separation between poly1 and poly2 for a give edge normal on poly1.
		/// </summary>
		/// <param name="poly1">The poly1.</param>
		/// <param name="xf1">The XF1.</param>
		/// <param name="edge1">The edge1.</param>
		/// <param name="poly2">The poly2.</param>
		/// <param name="xf2">The XF2.</param>
		/// <returns></returns>
		static float edgeSeparation( PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2 )
		{
			List<vec2> vertices1 = poly1.vertices;
			List<vec2> normals1 = poly1.normals;

			int count2 = poly2.vertices.Count;
			List<vec2> vertices2 = poly2.vertices;

			Debug.Assert( 0 <= edge1 && edge1 < poly1.vertices.Count );

			// Convert normal from poly1's frame into poly2's frame.
			vec2 normal1World = MathUtils.mul( xf1.q, normals1[edge1] );
			vec2 normal1 = MathUtils.mulT( xf2.q, normal1World );

			// Find support vertex on poly2 for -normal.
			int index = 0;
			float minDot = Settings.maxFloat;

			for( int i = 0; i < count2; ++i )
			{
				float dot = vec2.Dot( vertices2[i], normal1 );
				if( dot < minDot )
				{
					minDot = dot;
					index = i;
				}
			}

			vec2 v1 = MathUtils.mul( ref xf1, vertices1[edge1] );
			vec2 v2 = MathUtils.mul( ref xf2, vertices2[index] );
			float separation = vec2.Dot( v2 - v1, normal1World );
			return separation;
		}

		/// <summary>
		/// Find the max separation between poly1 and poly2 using edge normals from poly1.
		/// </summary>
		/// <param name="edgeIndex">Index of the edge.</param>
		/// <param name="poly1">The poly1.</param>
		/// <param name="xf1">The XF1.</param>
		/// <param name="poly2">The poly2.</param>
		/// <param name="xf2">The XF2.</param>
		/// <returns></returns>
		static float findMaxSeparation( out int edgeIndex, PolygonShape poly1, ref Transform xf1, PolygonShape poly2, ref Transform xf2 )
		{
			int count1 = poly1.vertices.Count;
			List<vec2> normals1 = poly1.normals;

			// Vector pointing from the centroid of poly1 to the centroid of poly2.
			vec2 d = MathUtils.mul( ref xf2, poly2.massData.centroid ) - MathUtils.mul( ref xf1, poly1.massData.centroid );
			vec2 dLocal1 = MathUtils.mulT( xf1.q, d );

			// Find edge normal on poly1 that has the largest projection onto d.
			int edge = 0;
			float maxDot = -Settings.maxFloat;
			for( int i = 0; i < count1; ++i )
			{
				float dot = vec2.Dot( normals1[i], dLocal1 );
				if( dot > maxDot )
				{
					maxDot = dot;
					edge = i;
				}
			}

			// Get the separation for the edge normal.
			float s = edgeSeparation( poly1, ref xf1, edge, poly2, ref xf2 );

			// Check the separation for the previous edge normal.
			int prevEdge = edge - 1 >= 0 ? edge - 1 : count1 - 1;
			float sPrev = edgeSeparation( poly1, ref xf1, prevEdge, poly2, ref xf2 );

			// Check the separation for the next edge normal.
			int nextEdge = edge + 1 < count1 ? edge + 1 : 0;
			float sNext = edgeSeparation( poly1, ref xf1, nextEdge, poly2, ref xf2 );

			// Find the best edge and the search direction.
			int bestEdge;
			float bestSeparation;
			int increment;
			if( sPrev > s && sPrev > sNext )
			{
				increment = -1;
				bestEdge = prevEdge;
				bestSeparation = sPrev;
			}
			else if( sNext > s )
			{
				increment = 1;
				bestEdge = nextEdge;
				bestSeparation = sNext;
			}
			else
			{
				edgeIndex = edge;
				return s;
			}

			// Perform a local search for the best edge normal.
			for( ;;)
			{
				if( increment == -1 )
					edge = bestEdge - 1 >= 0 ? bestEdge - 1 : count1 - 1;
				else
					edge = bestEdge + 1 < count1 ? bestEdge + 1 : 0;

				s = edgeSeparation( poly1, ref xf1, edge, poly2, ref xf2 );

				if( s > bestSeparation )
				{
					bestEdge = edge;
					bestSeparation = s;
				}
				else
				{
					break;
				}
			}

			edgeIndex = bestEdge;
			return bestSeparation;
		}

		static void findIncidentEdge( out FixedArray2<ClipVertex> c, PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2 )
		{
			c = new FixedArray2<ClipVertex>();
			Vertices normals1 = poly1.normals;

			int count2 = poly2.vertices.Count;
			Vertices vertices2 = poly2.vertices;
			Vertices normals2 = poly2.normals;

			Debug.Assert( 0 <= edge1 && edge1 < poly1.vertices.Count );

			// Get the normal of the reference edge in poly2's frame.
			vec2 normal1 = MathUtils.mulT( xf2.q, MathUtils.mul( xf1.q, normals1[edge1] ) );


			// Find the incident edge on poly2.
			int index = 0;
			float minDot = Settings.maxFloat;
			for( int i = 0; i < count2; ++i )
			{
				float dot = vec2.Dot( normal1, normals2[i] );
				if( dot < minDot )
				{
					minDot = dot;
					index = i;
				}
			}

			// Build the clip vertices for the incident edge.
			int i1 = index;
			int i2 = i1 + 1 < count2 ? i1 + 1 : 0;

			ClipVertex cv0 = c[0];

			cv0.V = MathUtils.mul( ref xf2, vertices2[i1] );
			cv0.ID.features.indexA = (byte)edge1;
			cv0.ID.features.indexB = (byte)i1;
			cv0.ID.features.typeA = (byte)ContactFeatureType.Face;
			cv0.ID.features.typeB = (byte)ContactFeatureType.Vertex;

			c[0] = cv0;

			ClipVertex cv1 = c[1];
			cv1.V = MathUtils.mul( ref xf2, vertices2[i2] );
			cv1.ID.features.indexA = (byte)edge1;
			cv1.ID.features.indexB = (byte)i2;
			cv1.ID.features.typeA = (byte)ContactFeatureType.Face;
			cv1.ID.features.typeB = (byte)ContactFeatureType.Vertex;

			c[1] = cv1;
		}

	}

}