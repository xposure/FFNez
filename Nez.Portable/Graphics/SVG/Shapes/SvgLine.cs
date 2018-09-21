#if FEATURE_GRAPHICS
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	public class SvgLine : SvgElement
	{
		[XmlAttribute( "x1" )]
		public float x1;

		[XmlAttribute( "y1" )]
		public float y1;

		[XmlAttribute( "x2" )]
		public float x2;

		[XmlAttribute( "y2" )]
		public float y2;

		public vec2 start { get { return new vec2( x1, y1 ); } }
		public vec2 end { get { return new vec2( x2, y2 ); } }


		public vec2[] getTransformedPoints()
		{
			var pts = new vec2[] { start, end };
			var mat = getCombinedMatrix();
			Vector2Ext.transform( pts, ref mat, pts );

			return pts;
		}

	}
}
#endif
