#if FEATURE_GRAPHICS
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	public class SvgPolygon : SvgElement
	{
		[XmlAttribute( "cx" )]
		public float centerX;

		[XmlAttribute( "cy" )]
		public float centerY;

		[XmlAttribute( "sides" )]
		public int sides;

		[XmlAttribute( "points" )]
		public string pointsAttribute
		{
			get { return null; }
			set { parsePoints( value ); }
		}

		public vec2[] points;


		void parsePoints( string str )
		{
			var pairs = str.Split( new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries );
			points = new vec2[pairs.Length];

			for( var i = 0; i < pairs.Length; i++ )
			{
				var parts = pairs[i].Split( new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries );
				points[i] = new vec2( float.Parse( parts[0] ), float.Parse( parts[1] ) );
			}
		}


		public vec2[] getTransformedPoints()
		{
			var pts = new vec2[points.Length];
			var mat = getCombinedMatrix();
			Vector2Ext.transform( points, ref mat, pts );

			return pts;
		}


		/// <summary>
		/// gets the points relative to the center. SVG by default uses absolute positions for points.
		/// </summary>
		/// <returns>The relative points.</returns>
		public vec2[] getRelativePoints()
		{
			var pts = new vec2[points.Length];

			var center = new vec2( centerX, centerY );
			for( var i = 0; i < points.Length; i++ )
				pts[i] = points[i] - center;

			return pts;
		}

	}
}
#endif
