using System;



namespace Nez
{
	public static class RectangleExt
	{
		/// <summary>
		/// gets the position of the specified side
		/// </summary>
		/// <returns>The side.</returns>
		/// <param name="edge">Side.</param>
		public static int getSide( this Rectangle rect, Edge edge )
		{
			switch( edge )
			{
				case Edge.Top:
					return rect.Top;
				case Edge.Bottom:
					return rect.Bottom;
				case Edge.Left:
					return rect.Left;
				case Edge.Right:
					return rect.Right;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		public static Rectangle getHalfRect( this Rectangle rect, Edge edge )
		{
			switch( edge )
			{
				case Edge.Top:
					return new Rectangle( rect.X, rect.Y, rect.Width, rect.Height / 2 );
				case Edge.Bottom:
					return new Rectangle( rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2 );
				case Edge.Left:
					return new Rectangle( rect.X, rect.Y, rect.Width / 2, rect.Height );
				case Edge.Right:
					return new Rectangle( rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height );
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		/// <summary>
		/// gets a portion of the Rectangle with a width/height of size that is on the Edge of the Rectangle but still contained within it.
		/// </summary>
		/// <returns>The rect edge portion.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="edge">Edge.</param>
		/// <param name="size">Size.</param>
		public static Rectangle getRectEdgePortion( this Rectangle rect, Edge edge, int size = 1 )
		{
			switch( edge )
			{
				case Edge.Top:
					return new Rectangle( rect.X, rect.Y, rect.Width, size );
				case Edge.Bottom:
					return new Rectangle( rect.X, rect.Y + rect.Height - size, rect.Width, size );
				case Edge.Left:
					return new Rectangle( rect.X, rect.Y, size, rect.Height );
				case Edge.Right:
					return new Rectangle( rect.X + rect.Width - size, rect.Y, size, rect.Height );
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		public static void expandSide( ref Rectangle rect, Edge edge, int amount )
		{
			// ensure we have a positive value
			amount = Math.Abs( amount );

			switch( edge )
			{
				case Edge.Top:
					rect.Y -= amount;
					rect.Height += amount;
					break;
				case Edge.Bottom:
					rect.Height += amount;
					break;
				case Edge.Left:
					rect.X -= amount;
					rect.Width += amount;
					break;
				case Edge.Right:
					rect.Width += amount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		public static void contract( ref Rectangle rect, int horizontalAmount, int verticalAmount )
		{
			rect.X += horizontalAmount;
			rect.Y += verticalAmount;
			rect.Width -= horizontalAmount * 2;
			rect.Height -= verticalAmount * 2;
		}


		/// <summary>
		/// returns a rectangle from the passed in floats
		/// </summary>
		/// <returns>The floats.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static Rectangle fromFloats( float x, float y, float width, float height )
		{
			return new Rectangle( (int)x, (int)y, (int)width, (int)height );
		}


		/// <summary>
		/// creates a Rectangle given min/max points (top-left, bottom-right points)
		/// </summary>
		/// <returns>The minimum max points.</returns>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static Rectangle fromMinMaxPoints( Point min, Point max )
		{
			return new Rectangle( min.X, min.Y, max.X - min.X, max.Y - min.Y );
		}


		/// <summary>
		/// calculates the union of the two Rectangles. The result will be a rectangle that encompasses the other two.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		/// <param name="result">Result.</param>
		public static void union( ref Rectangle value1, ref Rectangle value2, out Rectangle result )
		{
			result.X = Math.Min( value1.X, value2.X );
			result.Y = Math.Min( value1.Y, value2.Y );
			result.Width = Math.Max( value1.Right, value2.Right ) - result.X;
			result.Height = Math.Max( value1.Bottom, value2.Bottom ) - result.Y;
		}


		/// <summary>
		/// Update first to be the union of first and point
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="point">Point.</param>
		/// <param name="result">Result.</param>
		public static void union( ref Rectangle first, ref Point point, out Rectangle result )
		{
			var rect = new Rectangle( point.X, point.Y, 0, 0 );
			union( ref first, ref rect, out result );
		}


		/// <summary>
		/// given the points of a polygon calculates the bounds
		/// </summary>
		/// <returns>The from polygon points.</returns>
		/// <param name="points">Points.</param>
		public static Rectangle boundsFromPolygonPoints( vec2[] points )
		{
			// we need to find the min/max x/y values
			var minX = float.PositiveInfinity;
			var minY = float.PositiveInfinity;
			var maxX = float.NegativeInfinity;
			var maxY = float.NegativeInfinity;

			for( var i = 0; i < points.Length; i++ )
			{
				var pt = points[i];

				if( pt.x < minX )
					minX = pt.x;
				if( pt.x > maxX )
					maxX = pt.x;

				if( pt.y < minY )
					minY = pt.y;
				if( pt.y > maxY )
					maxY = pt.y;
			}

			return RectangleExt.fromMinMaxPoints( new Point( (int)minX, (int)minY ), new Point( (int)maxX, (int)maxY ) );
		}


		public static void calculateBounds( ref Rectangle rect, vec2 parentPosition, vec2 position, vec2 origin, vec2 scale, float rotation, float width, float height )
		{
			if( rotation == 0f )
			{
				rect.X = (int)( parentPosition.x + position.x - origin.x * scale.x );
				rect.Y = (int)( parentPosition.y + position.y - origin.y * scale.y );
				rect.Width = (int)( width * scale.x );
				rect.Height = (int)( height * scale.y );
			}
			else
			{
				// special care for rotated bounds. we need to find our absolute min/max values and create the bounds from that
				var worldPosX = parentPosition.x + position.x;
				var worldPosY = parentPosition.y + position.y;

				Matrix2D tempMat;
				// set the reference point to world reference taking origin into account
				var transformMatrix = Matrix2D.createTranslation( -worldPosX - origin.x, -worldPosY - origin.y );
				Matrix2D.createScale( scale.x, scale.y, out tempMat ); // scale ->
				Matrix2D.multiply( ref transformMatrix, ref tempMat, out transformMatrix );
				Matrix2D.createRotation( rotation, out tempMat ); // rotate ->
				Matrix2D.multiply( ref transformMatrix, ref tempMat, out transformMatrix );
				Matrix2D.createTranslation( worldPosX, worldPosY, out tempMat ); // translate back
				Matrix2D.multiply( ref transformMatrix, ref tempMat, out transformMatrix );

				// TODO: this is a bit silly. we can just leave the worldPos translation in the mat4 and avoid this
				// get all four corners in world space
				var topLeft = new vec2( worldPosX, worldPosY );
				var topRight = new vec2( worldPosX + width, worldPosY );
				var bottomLeft = new vec2( worldPosX, worldPosY + height );
				var bottomRight = new vec2( worldPosX + width, worldPosY + height );

				// transform the corners into our work space
				Vector2Ext.transform( ref topLeft, ref transformMatrix, out topLeft );
				Vector2Ext.transform( ref topRight, ref transformMatrix, out topRight );
				Vector2Ext.transform( ref bottomLeft, ref transformMatrix, out bottomLeft );
				Vector2Ext.transform( ref bottomRight, ref transformMatrix, out bottomRight );

				// find the min and max values so we can concoct our bounding box
				var minX = (int)Mathf.minOf( topLeft.x, bottomRight.x, topRight.x, bottomLeft.x );
				var maxX = (int)Mathf.maxOf( topLeft.x, bottomRight.x, topRight.x, bottomLeft.x );
				var minY = (int)Mathf.minOf( topLeft.y, bottomRight.y, topRight.y, bottomLeft.y );
				var maxY = (int)Mathf.maxOf( topLeft.y, bottomRight.y, topRight.y, bottomLeft.y );

				rect.Location = new Point( minX, minY );
				rect.Width = (int)( maxX - minX );
				rect.Height = (int)( maxY - minY );
			}
		}


		/// <summary>
		/// clones and returns a new Rectangle with the same data as the current rectangle
		/// </summary>
		/// <param name="rect">Rect.</param>
		public static Rectangle clone( this Rectangle rect )
		{
			return new Rectangle( rect.X, rect.Y, rect.Width, rect.Height );
		}


		/// <summary>
		/// scales the rect
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="scale">Scale.</param>
		public static void scale( ref Rectangle rect, vec2 scale )
		{
			rect.X = (int)( rect.X * scale.x );
			rect.Y = (int)( rect.Y * scale.y );
			rect.Width = (int)( rect.Width * scale.x );
			rect.Height = (int)( rect.Height * scale.y );
		}


		public static void translate( ref Rectangle rect, vec2 vec )
		{
            rect.Location += (Point)vec;
		}

		
		public static bool rayIntersects( ref Rectangle rect, ref Ray2D ray, out float distance )
		{
			distance = 0f;
			var maxValue = float.MaxValue;

			if( Math.Abs( ray.direction.x ) < 1E-06f )
			{
				if( ( ray.start.x < rect.X ) || ( ray.start.x > rect.X + rect.Width ) )
					return false;
			}
			else
			{
				var num11 = 1f / ray.direction.x;
				var num8 = ( rect.X - ray.start.x ) * num11;
				var num7 = ( rect.X + rect.Width - ray.start.x ) * num11;
				if( num8 > num7 )
				{
					var num14 = num8;
					num8 = num7;
					num7 = num14;
				}

				distance = glm.Max( num8, distance );
				maxValue = glm.Min( num7, maxValue );
				if( distance > maxValue )
					return false;
			}

			if( Math.Abs( ray.direction.y ) < 1E-06f )
			{
				if( ( ray.start.y < rect.Y ) || ( ray.start.y > rect.Y + rect.Height ) )
				{
					return false;
				}
			}
			else
			{
				var num10 = 1f / ray.direction.y;
				var num6 = ( rect.Y - ray.start.y ) * num10;
				var num5 = ( rect.Y + rect.Height - ray.start.y ) * num10;
				if( num6 > num5 )
				{
					var num13 = num6;
					num6 = num5;
					num5 = num13;
				}

				distance = glm.Max( num6, distance );
				maxValue = glm.Min( num5, maxValue );
				if( distance > maxValue )
					return false;
			}

			return true;
		}


		public static float? rayIntersects( this Rectangle rectangle, Ray ray )
		{
			var num = 0f;
			var maxValue = float.MaxValue;

			if( Math.Abs( ray.Direction.x ) < 1E-06f )
			{
				if( ( ray.Origin.x < rectangle.Left ) || ( ray.Origin.x > rectangle.Right ) )
					return null;
			}
			else
			{
				float num11 = 1f / ray.Direction.x;
				float num8 = ( rectangle.Left - ray.Origin.x ) * num11;
				float num7 = ( rectangle.Right - ray.Origin.x ) * num11;
				if( num8 > num7 )
				{
					float num14 = num8;
					num8 = num7;
					num7 = num14;
				}

				num = glm.Max( num8, num );
				maxValue = glm.Min( num7, maxValue );
				if( num > maxValue )
				{
					return null;  
				}  
			}

			if( Math.Abs( ray.Direction.y ) < 1E-06f )
			{
				if( ( ray.Origin.y< rectangle.Top ) || ( ray.Origin.y> rectangle.Bottom ) )
				{
					return null;
				}
			}
			else
			{
				float num10 = 1f / ray.Direction.y;
				float num6 = ( rectangle.Top - ray.Origin.y) * num10;
				float num5 = ( rectangle.Bottom - ray.Origin.y) * num10;
				if( num6 > num5 )
				{
					float num13 = num6;
					num6 = num5;
					num5 = num13;
				}

				num = glm.Max( num6, num );
				maxValue = glm.Min( num5, maxValue );
				if( num > maxValue )
					return null;
			}

			return new float?( num );
		}


		/// <summary>
		/// returns a Bounds the spans the current bounds and the provided delta positions
		/// </summary>
		/// <returns>The swept broadphase box.</returns>
		/// <param name="velocityX">Velocity x.</param>
		/// <param name="velocityY">Velocity y.</param>
		public static Rectangle getSweptBroadphaseBounds( ref Rectangle rect, float deltaX, float deltaY )
		{
			return getSweptBroadphaseBounds( ref rect, (int)deltaX, (int)deltaY );
		}


		/// <summary>
		/// returns a Bounds the spans the current bounds and the provided delta positions
		/// </summary>
		/// <returns>The swept broadphase box.</returns>
		/// <param name="velocityX">Velocity x.</param>
		/// <param name="velocityY">Velocity y.</param>
		public static Rectangle getSweptBroadphaseBounds( ref Rectangle rect, int deltaX, int deltaY )
		{
			var broadphasebox = Rectangle.Empty;

			broadphasebox.X = deltaX > 0 ? rect.X : rect.X + deltaX;
			broadphasebox.Y = deltaY > 0 ? rect.Y : rect.Y + deltaY;
			broadphasebox.Width = deltaX > 0 ? deltaX + rect.Width : rect.Width - deltaX;
			broadphasebox.Height = deltaY > 0 ? deltaY + rect.Height : rect.Height - deltaY;

			return broadphasebox;
		}


		/// <summary>
		/// returns true if rect1 intersects rect2
		/// </summary>
		/// <param name="value1">Value1.</param>
		/// <param name="value2">Value2.</param>
		public static bool intersect( ref Rectangle rect1, ref Rectangle rect2 )
		{
			bool result;
			rect1.Intersects( ref rect2, out result );
			return result;
		}


		/// <summary>
		/// returns true if the boxes are colliding
		/// moveX and moveY will return the movement that b1 must move to avoid the collision
		/// </summary>
		/// <param name="other">Other.</param>
		/// <param name="moveX">Move x.</param>
		/// <param name="moveY">Move y.</param>
		public static bool collisionCheck( ref Rectangle rect, ref Rectangle other, out float moveX, out float moveY )
		{
			moveX = moveY = 0.0f;

			var l = other.X - ( rect.X + rect.Width );
			var r = ( other.X + other.Width ) - rect.X;
			var t = other.Y - ( rect.Y + rect.Height );
			var b = ( other.Y + other.Height ) - rect.Y;

			// check that there was a collision
			if( l > 0 || r < 0 || t > 0 || b < 0 )
				return false;

			// find the offset of both sides
			moveX = Math.Abs( l ) < r ? l : r;
			moveY = Math.Abs( t ) < b ? t : b;

			// only use whichever offset is the smallest
			if( Math.Abs( moveX ) < Math.Abs( moveY ) )
				moveY = 0.0f;
			else
				moveX = 0.0f;

			return true;
		}


		/// <summary>
		/// Calculates the signed depth of intersection between two rectangles.
		/// </summary>
		/// <returns>
		/// The amount of overlap between two intersecting rectangles. These depth values can be negative depending on which sides the rectangles
		/// intersect. This allows callers to determine the correct direction to push objects in order to resolve collisions.
		/// If the rectangles are not intersecting, vec2.Zero is returned.
		/// </returns>
		public static vec2 getIntersectionDepth( ref Rectangle rectA, ref Rectangle rectB )
		{
			// calculate half sizes
			var halfWidthA = rectA.Width / 2.0f;
			var halfHeightA = rectA.Height / 2.0f;
			var halfWidthB = rectB.Width / 2.0f;
			var halfHeightB = rectB.Height / 2.0f;

			// calculate centers
			var centerA = new vec2( rectA.Left + halfWidthA, rectA.Top + halfHeightA );
			var centerB = new vec2( rectB.Left + halfWidthB, rectB.Top + halfHeightB );

			// calculate current and minimum-non-intersecting distances between centers
			var distanceX = centerA.x - centerB.x;
			var distanceY = centerA.y - centerB.y;
			var minDistanceX = halfWidthA + halfWidthB;
			var minDistanceY = halfHeightA + halfHeightB;

			// if we are not intersecting at all, return (0, 0)
			if( Math.Abs( distanceX ) >= minDistanceX || Math.Abs( distanceY ) >= minDistanceY )
				return vec2.Zero;

			// calculate and return intersection depths
			var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
			var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

			return new vec2( depthX, depthY );
		}


		public static vec2 getClosestPointOnBoundsToOrigin( ref Rectangle rect )
		{
			var max = RectangleExt.getMax( ref rect );
			var minDist = Math.Abs( rect.Location.X );
			var boundsPoint = new vec2( rect.Location.X, 0 );

			if( Math.Abs( max.X ) < minDist )
			{
				minDist = Math.Abs( max.X );
				boundsPoint.x = max.X;
				boundsPoint.y = 0f;
			}

			if( Math.Abs( max.Y ) < minDist )
			{
				minDist = Math.Abs( max.Y );
				boundsPoint.x = 0f;
				boundsPoint.y = max.Y;
			}

			if( Math.Abs( rect.Location.Y ) < minDist )
			{
				minDist = Math.Abs( rect.Location.Y );
				boundsPoint.x = 0;
				boundsPoint.y = rect.Location.Y;
			}

			return boundsPoint;
		}


		/// <summary>
		/// returns the closest point that is in or on the Rectangle to the given point
		/// </summary>
		/// <returns>The closest point on rectangle to point.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="point">Point.</param>
		public static vec2 getClosestPointOnRectangleToPoint( ref Rectangle rect, vec2 point )
		{
			// for each axis, if the point is outside the box clamp it to the box else leave it alone
			var res = new vec2();
			res.x = glm.Clamp( point.x, rect.Left, rect.Right );
			res.y = glm.Clamp( point.y, rect.Top, rect.Bottom );

			return res;
		}


		/// <summary>
		/// gets the closest point that is on the rectangle border to the given point
		/// </summary>
		/// <returns>The closest point on rectangle border to point.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="point">Point.</param>
		public static Point getClosestPointOnRectangleBorderToPoint( ref Rectangle rect, vec2 point )
		{
			// for each axis, if the point is outside the box clamp it to the box else leave it alone
			var res = new Point();
			res.X = Mathf.clamp( (int)point.x, rect.Left, rect.Right );
			res.Y = Mathf.clamp( (int)point.y, rect.Top, rect.Bottom );

			// if point is inside the rectangle we need to push res to the border since it will be inside the rect
			if( rect.Contains( res ) )
			{
				var dl = res.X - rect.Left;
				var dr = rect.Right - res.X;
				var dt = res.Y - rect.Top;
				var db = rect.Bottom - res.Y;

				var min = Mathf.minOf( dl, dr, dt, db );
				if( min == dt )
					res.Y = rect.Top;
				else if( min == db )
					res.Y = rect.Bottom;
				else if( min == dl )
					res.X = rect.Left;
				else
					res.X = rect.Right;
			}

			return res;
		}


		/// <summary>
		/// gets the center point of the rectangle as a vec2
		/// </summary>
		/// <returns>The center.</returns>
		/// <param name="rect">Rect.</param>
		public static vec2 getCenter( ref Rectangle rect )
		{
			return new vec2( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 );
		}


		/// <summary>
		/// gets the center point of the rectangle as a vec2
		/// </summary>
		/// <returns>The center.</returns>
		/// <param name="rect">Rect.</param>
		public static vec2 getCenter( this Rectangle rect )
		{
			return new vec2( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 );
		}


		/// <summary>
		/// gets the half size of the rect
		/// </summary>
		/// <returns>The half size.</returns>
		/// <param name="rect">Rect.</param>
		public static vec2 getHalfSize( this Rectangle rect )
		{
			return new vec2( rect.Width * 0.5f, rect.Height * 0.5f );
		}

		/// <summary>
		/// gets the max point of the rectangle, the bottom-right corner
		/// </summary>
		/// <returns>The max.</returns>
		/// <param name="rect">Rect.</param>
		public static Point getMax( ref Rectangle rect )
		{
			return new Point( rect.Right, rect.Bottom );
		}


		/// <summary>
		/// gets the position of the rectangle as a vec2
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="rect">Rect.</param>
		public static vec2 getPosition( ref Rectangle rect )
		{
			return new vec2( rect.X, rect.Y );
		}

	}
}

