#if FEATURE_GRAPHICS
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	/// <summary>
	/// base class for all of the different SVG path types. Note that arcs are not supported at this time.
	/// </summary>
	public abstract class SvgPathSegment
	{
		public vec2 start, end;


		protected SvgPathSegment()
		{}


		protected SvgPathSegment( vec2 start, vec2 end )
		{
			this.start = start;
			this.end = end;
		}


		protected string toSvgString( vec2 point )
		{
			return string.Format( "{0} {1}", point.X, point.Y );
		}
	}


	public sealed class SvgMoveToSegment : SvgPathSegment
	{
		public SvgMoveToSegment( vec2 position )
		{
			start = position;
			end = position;
		}


		public override string ToString()
		{
			return "M" + toSvgString( start );
		}
	}


	public sealed class SvgLineSegment : SvgPathSegment
	{
		public SvgLineSegment( vec2 start, vec2 end )
		{
			this.start = start;
			this.end = end;
		}


		public override string ToString()
		{
			return "L" + toSvgString( end );
		}

	}


	public sealed class SvgClosePathSegment : SvgPathSegment
	{
		public override string ToString()
		{
			return "z";
		}
	}


	public sealed class SvgQuadraticCurveSegment : SvgPathSegment
	{
		public vec2 controlPoint;

		public vec2 firstCtrlPoint
		{
			get
			{
				var x1 = start.X + ( controlPoint.X - start.X ) * 2 / 3;
				var y1 = start.Y + ( controlPoint.Y - start.Y ) * 2 / 3;

				return new vec2( x1, y1 );
			}
		}

		public vec2 secondCtrlPoint
		{
			get
			{
				var x2 = controlPoint.X + ( end.X - controlPoint.X ) / 3;
				var y2 = controlPoint.Y + ( end.Y - controlPoint.Y ) / 3;

				return new vec2( x2, y2 );
			}
		}

		public SvgQuadraticCurveSegment( vec2 start, vec2 controlPoint, vec2 end )
		{
			this.start = start;
			this.controlPoint = controlPoint;
			this.end = end;
		}


		public override string ToString()
		{
			return "Q" + toSvgString( controlPoint ) + " " + toSvgString( end );
		}

	}


	public sealed class SvgCubicCurveSegment : SvgPathSegment
	{
		public vec2 firstCtrlPoint;
		public vec2 secondCtrlPoint;


		public SvgCubicCurveSegment( vec2 start, vec2 firstCtrlPoint, vec2 secondCtrlPoint, vec2 end )
		{
			this.start = start;
			this.end = end;
			this.firstCtrlPoint = firstCtrlPoint;
			this.secondCtrlPoint = secondCtrlPoint;
		}


		public override string ToString()
		{
			return "C" + toSvgString( firstCtrlPoint ) + " " + toSvgString( secondCtrlPoint ) + " " + toSvgString( end );
		}
	}

}
#endif
