﻿
using System;
using System.Collections.Generic;
using Nez.PhysicsShapes;
using System.Runtime.CompilerServices;

namespace Nez.Shadows
{
	/// <summary>
	/// Class which computes a mesh that represents which regions are visibile from the origin point given a set of occluders. Usage is as
	/// follows:
	/// 
	/// - call begin
	/// - add any occluders
	/// - call end to get the visibility polygon. When end is called all internal storage is cleared.
	/// 
	/// based on: http://www.redblobgames.com/articles/visibility/ and http://roy-t.nl/index.php/2014/02/27/2d-lighting-and-shadows-preview/
	/// </summary>
	public class VisibilityComputer
	{
		/// <summary>
		/// total number of lines that will be used when approximating a circle. Only a 180 degree hemisphere is needed so this will be the number
		/// of segments to approximate that hemisphere.
		/// </summary>
		public int lineCountForCircleApproximation = 10;

		float _radius;
		vec2 _origin;
		bool _isSpotLight;
		float _spotStartAngle, _spotEndAngle;

		// TODO: use FastList and convert EndPoint and Segment to structs
		List<EndPoint> _endpoints = new List<EndPoint>();
		List<Segment> _segments = new List<Segment>();
		EndPointComparer _radialComparer;

		static vec2[] _cornerCache = new vec2[4];
		static LinkedList<Segment> _openSegments = new LinkedList<Segment>();


		public VisibilityComputer()
		{
			_radialComparer = new EndPointComparer();
		}


		public VisibilityComputer( vec2 origin, float radius ) : this()
		{
			_origin = origin;
			_radius = radius;
		}


		/// <summary>
		/// adds a Collider as an occluder for the PolyLight
		/// </summary>
		/// <param name="collider">Collider.</param>
		public void addColliderOccluder( Collider collider )
		{
			// special case for BoxColliders with no rotation
			if( collider is BoxCollider && collider.rotation == 0 )
			{
				addSquareOccluder( collider.bounds );
				return;
			}

			if( collider is PolygonCollider )
			{
				var poly = collider.shape as Polygon;
				for( var i = 0; i < poly.points.Length; i++ )
				{
					var firstIndex = i - 1;
					if( i == 0 )
						firstIndex += poly.points.Length;
					addLineOccluder( poly.points[firstIndex] + poly.position, poly.points[i] + poly.position );
				}
			}
			else if( collider is CircleCollider )
			{
				addCircleOccluder( collider.absolutePosition, ( collider as CircleCollider ).radius );
			}
		}


		/// <summary>
		/// Add a square shaped occluder
		/// </summary>        
		public void addSquareOccluder( vec2 position, float width, float rotation )
		{
			var x = position.x;
			var y = position.y;

			// The distance to each corner is half of the width times sqrt(2)
			var radius = width * 0.5f * 1.41f;

            // Add Pi/4 to get the corners
            rotation += glm.PIOverFour;

			for( var i = 0; i < 4; i++ )
			{
				_cornerCache[i] = new vec2(
					(float)Math.Cos( rotation + i * Math.PI * 0.5 ) * radius + x,
					(float)Math.Sin( rotation + i * Math.PI * 0.5 ) * radius + y
				);
			}

			addSegment( _cornerCache[0], _cornerCache[1] );
			addSegment( _cornerCache[1], _cornerCache[2] );
			addSegment( _cornerCache[2], _cornerCache[3] );
			addSegment( _cornerCache[3], _cornerCache[0] );
		}


		/// <summary>
		/// Add a square shaped occluder
		/// </summary>        
		public void addSquareOccluder( RectangleF bounds )
		{
			var tr = new vec2( bounds.right, bounds.top );
			var bl = new vec2( bounds.left, bounds.bottom );
			var br = new vec2( bounds.right, bounds.bottom );

			addSegment( bounds.location, tr );
			addSegment( tr, br );
			addSegment( br, bl );
			addSegment( bl, bounds.location );
		}


		/// <summary>
		/// adds a circle shaped occluder
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="radius">Radius.</param>
		public void addCircleOccluder( vec2 position, float radius )
		{
			var dirToCircle = position - _origin;
			var angle = Mathf.atan2( dirToCircle.y, dirToCircle.x );

			var stepSize = glm.PI / lineCountForCircleApproximation;
			var startAngle = angle + glm.PIOverTwo;
			var lastPt = Mathf.angleToVector( startAngle, radius ) + position;
			for( var i = 1; i < lineCountForCircleApproximation; i++ )
			{
				var nextPt = Mathf.angleToVector( startAngle + i * stepSize, radius ) + position;
				addLineOccluder( lastPt, nextPt );
				lastPt = nextPt;
			}
		}


		/// <summary>
		/// Add a line shaped occluder
		/// </summary>        
		public void addLineOccluder( vec2 p1, vec2 p2 )
		{
			addSegment( p1, p2 );
		}


		// Add a segment, where the first point shows up in the
		// visualization but the second one does not. (Every endpoint is
		// part of two segments, but we want to only show them once.)
		void addSegment( vec2 p1, vec2 p2 )
		{
			var segment = new Segment();
			var endPoint1 = new EndPoint();
			var endPoint2 = new EndPoint();

			endPoint1.position = p1;
			endPoint1.segment = segment;

			endPoint2.position = p2;
			endPoint2.segment = segment;

			segment.p1 = endPoint1;
			segment.p2 = endPoint2;

			_segments.Add( segment );
			_endpoints.Add( endPoint1 );
			_endpoints.Add( endPoint2 );
		}


		/// <summary>
		/// Remove all occluders
		/// </summary>
		public void clearOccluders()
		{
			_segments.Clear();
			_endpoints.Clear();
		}


		/// <summary>
		/// prepares the computer for calculating the current poly light
		/// </summary>
		/// <param name="origin">Origin.</param>
		/// <param name="radius">Radius.</param>
		public void begin( vec2 origin, float radius )
		{
			_origin = origin;
			_radius = radius;
			_isSpotLight = false;
		}


		/// <summary>
		/// Computes the visibility polygon and returns the vertices of the triangle fan (minus the center vertex). Returned List is from the
		/// ListPool.
		/// </summary>        
		public List<vec2> end()
		{
			var output = ListPool<vec2>.obtain();
			updateSegments();
			_endpoints.Sort( _radialComparer );

			var currentAngle = 0f;

			// At the beginning of the sweep we want to know which segments are active. The simplest way to do this is to make
			// a pass collecting the segments, and make another pass to both collect and process them. However it would be more
			// efficient to go through all the segments, figure out which ones intersect the initial sweep line, and then sort them.
			for( var pass = 0; pass < 2; pass++ )
			{
				foreach( var p in _endpoints )
				{
					var currentOld = _openSegments.Count == 0 ? null : _openSegments.First.Value;

					if( p.begin )
					{
						// Insert into the right place in the list
						var node = _openSegments.First;
						while( node != null && isSegmentInFrontOf( p.segment, node.Value, _origin ) )
							node = node.Next;

						if( node == null )
							_openSegments.AddLast( p.segment );
						else
							_openSegments.AddBefore( node, p.segment );
					}
					else
					{
						_openSegments.Remove( p.segment );
					}


					Segment currentNew = null;
					if( _openSegments.Count != 0 )
						currentNew = _openSegments.First.Value;

					if( currentOld != currentNew )
					{
						if( pass == 1 )
						{
							if( !_isSpotLight || ( between( currentAngle, _spotStartAngle, _spotEndAngle ) && between( p.angle, _spotStartAngle, _spotEndAngle ) ) )
								addTriangle( output, currentAngle, p.angle, currentOld );
						}
						currentAngle = p.angle;
					}
				}
			}

			_openSegments.Clear();
			clearOccluders();

			// uncomment to draw squares at all the encounter points
			//for( var i = 0; i < output.Count; i++ )
			//	Debug.drawPixel( output[i], 10, Color.Orange );

			return output;
		}


		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static bool between( float value, float min, float max )
		{
			//const float maxDistance = (float)Math.PI * 0.5f; // 90 degrees

			//return Math.Abs( glm.WrapAngle( min - value ) ) < maxDistance
			//	       && Math.Abs( glm.WrapAngle( max - value ) ) < maxDistance;
			
			
			//var normalisedMin = min > 0 ? min : 2 * Math.PI + min;
			//var normalisedMax = max > 0 ? max : 2 * Math.PI + max;
			//var normalisedTarget = value > 0 ? value : 2 * Math.PI + value;

			//return normalisedMin <= normalisedTarget && normalisedTarget <= normalisedMax;


			value = ( 360 + ( value % 360 ) ) % 360;
			min = ( 3600000 + min ) % 360;
			max = ( 3600000 + max ) % 360;

			if( min < max )
				return min <= value && value <= max;
			return min <= value || value <= max;


			//return value >= min && value <= max;
		}


		/// <summary>
		/// Helper function to construct segments along the outside perimiter in order to limit the radius of the light
		/// </summary>        
		public void loadRectangleBoundaries()
		{
			//Top
			addSegment( new vec2( _origin.x - _radius, _origin.y - _radius ),
				new vec2( _origin.x + _radius, _origin.y - _radius ) );

			//Bottom
			addSegment( new vec2( _origin.x - _radius, _origin.y + _radius ),
				new vec2( _origin.x + _radius, _origin.y + _radius ) );

			//Left
			addSegment( new vec2( _origin.x - _radius, _origin.y - _radius ),
				new vec2( _origin.x - _radius, _origin.y + _radius ) );

			//Right
			addSegment( new vec2( _origin.x + _radius, _origin.y - _radius ),
				new vec2( _origin.x + _radius, _origin.y + _radius ) );
		}


		public void loadSpotLightBoundaries( vec2[] points )
		{
			_isSpotLight = true;

			// add the two outer edges of the polygon but lerp them a bit so they dont start at the origin
			var first = vec2.Lerp( _origin, _origin + points[1], 0.1f );
			var second = vec2.Lerp( _origin, _origin + points[points.Length - 1], 0.1f );
			addSegment( first, _origin + points[1] );
			addSegment( second, _origin + points[points.Length - 1] );

			loadRectangleBoundaries();
		}


		/// <summary>
		/// Processes segments so that we can sort them later
		/// </summary>
		void updateSegments()
		{
			foreach( var segment in _segments )
			{
				// NOTE: future optimization: we could record the quadrant and the y/x or x/y ratio, and sort by (quadrant,
				// ratio), instead of calling atan2. See <https://github.com/mikolalysenko/compare-slope> for a
				// library that does this.

				segment.p1.angle = Mathf.atan2( segment.p1.position.y - _origin.y, segment.p1.position.x - _origin.x );
				segment.p2.angle = Mathf.atan2( segment.p2.position.y - _origin.y, segment.p2.position.x - _origin.x );

				// Map angle between -Pi and Pi
				var dAngle = segment.p2.angle - segment.p1.angle;
				if( dAngle <= -glm.PI )
					dAngle += glm.TwoPI;

				if( dAngle > glm.PI )
					dAngle -= glm.TwoPI;

				segment.p1.begin = ( dAngle > 0.0f );
				segment.p2.begin = !segment.p1.begin;
			}

			// if we have a spot light we need to store the first two segments angles. These are the spot boundaries and we will use them to filter
			// any verts outside of them.
			if( _isSpotLight )
			{
				_spotStartAngle = _segments[0].p2.angle;
				_spotEndAngle = _segments[1].p2.angle;
			}
		}


		/// <summary>
		/// Helper: do we know that segment a is in front of b? Implementation not anti-symmetric (that is to say,
		/// isSegmentInFrontOf(a, b) != (!isSegmentInFrontOf(b, a)). Also note that it only has to work in a restricted set of cases
		/// in the visibility algorithm; I don't think it handles all cases. See http://www.redblobgames.com/articles/visibility/segment-sorting.html
		/// </summary>
		/// <returns><c>true</c>, if in front of was segmented, <c>false</c> otherwise.</returns>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="relativeTo">Relative to.</param>
		bool isSegmentInFrontOf( Segment a, Segment b, vec2 relativeTo )
		{
			// NOTE: we slightly shorten the segments so that intersections of the endpoints (common) don't count as intersections in this algorithm
			var a1 = isLeftOf( a.p2.position, a.p1.position, interpolate( b.p1.position, b.p2.position, 0.01f ) );
			var a2 = isLeftOf( a.p2.position, a.p1.position, interpolate( b.p2.position, b.p1.position, 0.01f ) );
			var a3 = isLeftOf( a.p2.position, a.p1.position, relativeTo );

			var b1 = isLeftOf( b.p2.position, b.p1.position, interpolate( a.p1.position, a.p2.position, 0.01f ) );
			var b2 = isLeftOf( b.p2.position, b.p1.position, interpolate( a.p2.position, a.p1.position, 0.01f ) );
			var b3 = isLeftOf( b.p2.position, b.p1.position, relativeTo );

			// NOTE: this algorithm is probably worthy of a short article but for now, draw it on paper to see how it works. Consider
			// the line A1-A2. If both B1 and B2 are on one side and relativeTo is on the other side, then A is in between the
			// viewer and B. We can do the same with B1-B2: if A1 and A2 are on one side, and relativeTo is on the other side, then
			// B is in between the viewer and A.
			if( b1 == b2 && b2 != b3 )
				return true;
			if( a1 == a2 && a2 == a3 )
				return true;
			if( a1 == a2 && a2 != a3 )
				return false;
			if( b1 == b2 && b2 == b3 )
				return false;

			// If A1 != A2 and B1 != B2 then we have an intersection. A more robust implementation would split segments at intersections so that
			// part of the segment is in front and part is behind but we shouldnt have overlapping colliders anyway so it isnt too important.

			return false;

			// NOTE: previous implementation was a.d < b.d. That's simpler but trouble when the segments are of dissimilar sizes. If
			// you're on a grid and the segments are similarly sized, then using distance will be a simpler and faster implementation.
		}


		void addTriangle( List<vec2> triangles, float angle1, float angle2, Segment segment )
		{
			var p1 = _origin;
			var p2 = new vec2( _origin.x + Mathf.cos( angle1 ), _origin.y + Mathf.sin( angle1 ) );
			var p3 = vec2.Zero;
			var p4 = vec2.Zero;

			if( segment != null )
			{
				// Stop the triangle at the intersecting segment
				p3.x = segment.p1.position.x;
				p3.y = segment.p1.position.y;
				p4.x = segment.p2.position.x;
				p4.y = segment.p2.position.y;
			}
			else
			{
				// Stop the triangle at a fixed distance
				p3.x = _origin.x + Mathf.cos( angle1 ) * _radius * 2;
				p3.y = _origin.y + Mathf.sin( angle1 ) * _radius * 2;
				p4.x = _origin.x + Mathf.cos( angle2 ) * _radius * 2;
				p4.y = _origin.y + Mathf.sin( angle2 ) * _radius * 2;
			}

			var pBegin = lineLineIntersection( p3, p4, p1, p2 );

			p2.x = _origin.x + Mathf.cos( angle2 );
			p2.y = _origin.y + Mathf.sin( angle2 );

			var pEnd = lineLineIntersection( p3, p4, p1, p2 );

			triangles.Add( pBegin );
			triangles.Add( pEnd );
		}


		/// <summary>
		/// Computes the intersection point of the line p1-p2 with p3-p4
		/// </summary>        
		static vec2 lineLineIntersection( vec2 p1, vec2 p2, vec2 p3, vec2 p4 )
		{
			// From http://paulbourke.net/geometry/lineline2d/
			var s = ( ( p4.x - p3.x ) * ( p1.y - p3.y ) - ( p4.y - p3.y ) * ( p1.x - p3.x ) )
				/ ( ( p4.y - p3.y ) * ( p2.x - p1.x ) - ( p4.x - p3.x ) * ( p2.y - p1.y ) );
			return new vec2( p1.x + s * ( p2.x - p1.x ), p1.y + s * ( p2.y - p1.y ) );
		}


		/// <summary>
		/// Returns if the point is 'left' of the line p1-p2
		/// </summary>        
		static bool isLeftOf( vec2 p1, vec2 p2, vec2 point )
		{
			float cross = ( p2.x - p1.x ) * ( point.y - p1.y )
				- ( p2.y - p1.y ) * ( point.x - p1.x );

			return cross < 0;
		}


		/// <summary>
		/// Returns a slightly shortened version of the vector:
		/// p * (1 - f) + q * f
		/// </summary>        
		static vec2 interpolate( vec2 p, vec2 q, float f )
		{
			return new vec2( p.x * ( 1.0f - f ) + q.x * f, p.y * ( 1.0f - f ) + q.y * f );
		}

	}
}
