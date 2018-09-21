#if FEATURE_GRAPHICS
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	/// <summary>
	/// helper class used to parse paths and also fetch the drawing points from a series of SvgPathSegments. This is an insanely slow way
	/// to build paths since it is stuck using reflection for everything. It is highly recommended that you use the SvgPathBuilder class
	/// instead, which must be manually placed in your project.
	/// </summary>
	public class SvgReflectionPathBuilder : ISvgPathBuilder
	{
		/// <summary>
		/// helper to convert a vec2 into a Point
		/// </summary>
		/// <returns>The draw point.</returns>
		/// <param name="vec">Vec.</param>
		static object toDrawPoint( vec2 vec )
		{
			var args = new object[] { (int)vec.X, (int)vec.Y };
			return System.Activator.CreateInstance( System.Type.GetType( "System.Drawing.Point, System.Drawing" ), args );
		}


		/// <summary>
		/// takes in a parsed path and returns a list of points that can be used to draw the path
		/// </summary>
		/// <returns>The drawing points.</returns>
		/// <param name="segments">Segments.</param>
		public vec2[] getDrawingPoints( List<SvgPathSegment> segments, float flatness = 3 )
		{
			var path = new FauxGraphicsPath();
			for( var j = 0; j < segments.Count; j++ )
			{
				var segment = segments[j];
				if( segment is SvgMoveToSegment )
				{
					path.StartFigure();
				}
				else if( segment is SvgCubicCurveSegment )
				{
					var cubicSegment = segment as SvgCubicCurveSegment;
					path.AddBezier( toDrawPoint( segment.start ), toDrawPoint( cubicSegment.firstCtrlPoint ), toDrawPoint( cubicSegment.secondCtrlPoint ), toDrawPoint( segment.end ) );
				}
				else if( segment is SvgClosePathSegment )
				{
					// important for custom line caps. Force the path the close with an explicit line, not just an implicit close of the figure.
					if( path.PointCount > 0 && !path.PathPoints.GetValue( 0 ).Equals( path.PathPoints.GetValue( path.PathPoints.Length - 1 ) ) )
					{
						var i = path.PathTypes.Length - 1;
						while( i >= 0 && path.PathTypes[i] > 0 )
							i--;
						if( i < 0 )
							i = 0;
						path.AddLine( path.PathPoints.GetValue( path.PathPoints.Length - 1 ), path.PathPoints.GetValue( i ) );
					}
					path.CloseFigure();
				}
				else if( segment is SvgLineSegment )
				{
					path.AddLine( toDrawPoint( segment.start ), toDrawPoint( segment.end ) );
				}
				else if( segment is SvgQuadraticCurveSegment )
				{
					var quadSegment = segment as SvgQuadraticCurveSegment;
					path.AddBezier( toDrawPoint( segment.start ), toDrawPoint( quadSegment.firstCtrlPoint ), toDrawPoint( quadSegment.secondCtrlPoint ), toDrawPoint( segment.end ) );
				}
				else
				{
					Debug.warn( "unknown type in getDrawingPoints" );
				}
			}

			var matrix = System.Activator.CreateInstance( System.Type.GetType( "System.Drawing.Drawing2D.mat4, System.Drawing" ) );
			path.Flatten( matrix, flatness );

			return path.pathPointsAsVectors();
		}

	}

}
#endif
