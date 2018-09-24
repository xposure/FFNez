using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace Nez.Common
{
	//Contributed by Matthew Bettcher

	/// <summary>
	/// Path:
	/// Very similar to Vertices, but this
	/// class contains vectors describing
	/// control points on a Catmull-Rom
	/// curve.
	/// </summary>
	public class Path
	{
		/// <summary>
		/// All the points that makes up the curve
		/// </summary>
		public List<vec2> controlPoints;

		float _deltaT;


		/// <summary>
		/// Initializes a new instance of the <see cref="Path"/> class.
		/// </summary>
		public Path()
		{
			controlPoints = new List<vec2>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Path"/> class.
		/// </summary>
		/// <param name="vertices">The vertices to created the path from.</param>
		public Path( vec2[] vertices )
		{
			controlPoints = new List<vec2>( vertices.Length );

			for( int i = 0; i < vertices.Length; i++ )
			{
				add( vertices[i] );
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Path"/> class.
		/// </summary>
		/// <param name="vertices">The vertices to created the path from.</param>
		public Path( IList<vec2> vertices )
		{
			controlPoints = new List<vec2>( vertices.Count );
			for( int i = 0; i < vertices.Count; i++ )
			{
				add( vertices[i] );
			}
		}

		/// <summary>
		/// True if the curve is closed.
		/// </summary>
		/// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
		public bool isClosed { get; set; }

		/// <summary>
		/// Gets the next index of a controlpoint
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public int nextIndex( int index )
		{
			if( index == controlPoints.Count - 1 )
			{
				return 0;
			}
			return index + 1;
		}

		/// <summary>
		/// Gets the previous index of a controlpoint
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public int previousIndex( int index )
		{
			if( index == 0 )
			{
				return controlPoints.Count - 1;
			}
			return index - 1;
		}

		/// <summary>
		/// Translates the control points by the specified vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		public void translate( ref vec2 vector )
		{
			for( int i = 0; i < controlPoints.Count; i++ )
				controlPoints[i] = vec2.Add( controlPoints[i], vector );
		}

		/// <summary>
		/// Scales the control points by the specified vector.
		/// </summary>
		/// <param name="value">The Value.</param>
		public void scale( ref vec2 value )
		{
			for( int i = 0; i < controlPoints.Count; i++ )
				controlPoints[i] = vec2.Multiply( controlPoints[i], value );
		}

		/// <summary>
		/// Rotate the control points by the defined value in radians.
		/// </summary>
		/// <param name="value">The amount to rotate by in radians.</param>
		public void rotate( float value )
		{
			Matrix rotationMatrix;
			Matrix.CreateRotationZ( value, out rotationMatrix );

			for( int i = 0; i < controlPoints.Count; i++ )
				controlPoints[i] = vec2.Transform( controlPoints[i], rotationMatrix );
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for( int i = 0; i < controlPoints.Count; i++ )
			{
				builder.Append( controlPoints[i].ToString() );
				if( i < controlPoints.Count - 1 )
				{
					builder.Append( " " );
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// Returns a set of points defining the
		/// curve with the specifed number of divisions
		/// between each control point.
		/// </summary>
		/// <param name="divisions">Number of divisions between each control point.</param>
		/// <returns></returns>
		public Vertices getVertices( int divisions )
		{
			var verts = new Vertices();
			float timeStep = 1f / divisions;

			for( float i = 0; i < 1f; i += timeStep )
			{
				verts.Add( getPosition( i ) );
			}

			return verts;
		}

		public vec2 getPosition( float time )
		{
			vec2 temp;

			if( controlPoints.Count < 2 )
				throw new Exception( "You need at least 2 control points to calculate a position." );

			if( isClosed )
			{
				add( controlPoints[0] );

				_deltaT = 1f / ( controlPoints.Count - 1 );

				int p = (int)( time / _deltaT );

				// use a circular indexing system
				int p0 = p - 1;
				if( p0 < 0 ) p0 = p0 + ( controlPoints.Count - 1 );
				else if( p0 >= controlPoints.Count - 1 ) p0 = p0 - ( controlPoints.Count - 1 );
				int p1 = p;
				if( p1 < 0 ) p1 = p1 + ( controlPoints.Count - 1 );
				else if( p1 >= controlPoints.Count - 1 ) p1 = p1 - ( controlPoints.Count - 1 );
				int p2 = p + 1;
				if( p2 < 0 ) p2 = p2 + ( controlPoints.Count - 1 );
				else if( p2 >= controlPoints.Count - 1 ) p2 = p2 - ( controlPoints.Count - 1 );
				int p3 = p + 2;
				if( p3 < 0 ) p3 = p3 + ( controlPoints.Count - 1 );
				else if( p3 >= controlPoints.Count - 1 ) p3 = p3 - ( controlPoints.Count - 1 );

				// relative time
				float lt = ( time - _deltaT * p ) / _deltaT;

				temp = vec2.CatmullRom( controlPoints[p0], controlPoints[p1], controlPoints[p2], controlPoints[p3], lt );

				removeAt( controlPoints.Count - 1 );
			}
			else
			{
				int p = (int)( time / _deltaT );

				// 
				int p0 = p - 1;
				if( p0 < 0 ) p0 = 0;
				else if( p0 >= controlPoints.Count - 1 ) p0 = controlPoints.Count - 1;
				int p1 = p;
				if( p1 < 0 ) p1 = 0;
				else if( p1 >= controlPoints.Count - 1 ) p1 = controlPoints.Count - 1;
				int p2 = p + 1;
				if( p2 < 0 ) p2 = 0;
				else if( p2 >= controlPoints.Count - 1 ) p2 = controlPoints.Count - 1;
				int p3 = p + 2;
				if( p3 < 0 ) p3 = 0;
				else if( p3 >= controlPoints.Count - 1 ) p3 = controlPoints.Count - 1;

				// relative time
				float lt = ( time - _deltaT * p ) / _deltaT;

				temp = vec2.CatmullRom( controlPoints[p0], controlPoints[p1], controlPoints[p2], controlPoints[p3], lt );
			}

			return temp;
		}

		/// <summary>
		/// Gets the normal for the given time.
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns>The normal.</returns>
		public vec2 getPositionNormal( float time )
		{
			var offsetTime = time + 0.0001f;

			var a = getPosition( time );
			var b = getPosition( offsetTime );

			vec2 output, temp;
			vec2.Subtract( ref a, ref b, out temp );

			output.x = -temp.y;
			output.y = temp.x;

            output.Normalize();
			//Nez.Vector2Ext.normalize( ref output );

			return output;
		}

		public void add( vec2 point )
		{
			controlPoints.Add( point );
			_deltaT = 1f / ( controlPoints.Count - 1 );
		}

		public void remove( vec2 point )
		{
			controlPoints.Remove( point );
			_deltaT = 1f / ( controlPoints.Count - 1 );
		}

		public void removeAt( int index )
		{
			controlPoints.RemoveAt( index );
			_deltaT = 1f / ( controlPoints.Count - 1 );
		}

		public float getLength()
		{
			List<vec2> verts = getVertices( controlPoints.Count * 25 );
			float length = 0;

			for( int i = 1; i < verts.Count; i++ )
			{
				length += vec2.Distance( verts[i - 1], verts[i] );
			}

			if( isClosed )
				length += vec2.Distance( verts[controlPoints.Count - 1], verts[0] );

			return length;
		}

		public List<Vector3> subdivideEvenly( int divisions )
		{
			List<Vector3> verts = new List<Vector3>();

			float length = getLength();

			float deltaLength = length / divisions + 0.001f;
			float t = 0.000f;

			// we always start at the first control point
			vec2 start = controlPoints[0];
			vec2 end = getPosition( t );

			// increment t until we are at half the distance
			while( deltaLength * 0.5f >= vec2.Distance( start, end ) )
			{
				end = getPosition( t );
				t += 0.0001f;

				if( t >= 1f )
					break;
			}

			start = end;

			// for each box
			for( int i = 1; i < divisions; i++ )
			{
				vec2 normal = getPositionNormal( t );
				float angle = (float)Math.Atan2( normal.y, normal.x );

				verts.Add( new Vector3( end, angle ) );

				// until we reach the correct distance down the curve
				while( deltaLength >= vec2.Distance( start, end ) )
				{
					end = getPosition( t );
					t += 0.00001f;

					if( t >= 1f )
						break;
				}
				if( t >= 1f )
					break;

				start = end;
			}
			return verts;
		}

		public static string getFileNameWithoutExtension( string fontFile )
		{
			throw new NotImplementedException();
		}
	
	}
}