#if FEATURE_GRAPHICS
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	public class SvgRectangle : SvgElement
	{
		[XmlAttribute( "x" )]
		public float x;

		[XmlAttribute( "y" )]
		public float y;

		[XmlAttribute( "width" )]
		public float width;

		[XmlAttribute( "height" )]
		public float height;

		public vec2 center { get { return new vec2( x + width / 2, y + height / 2 ); } }


		/// <summary>
		/// gets the points for the rectangle with all transforms applied
		/// </summary>
		/// <returns>The transformed points.</returns>
		public vec2[] getTransformedPoints()
		{
			var pts = new vec2[] { new vec2( x, y ), new vec2( x + width, y ), new vec2( x + width, y + height ), new vec2( x, y + height ) };
			var mat = getCombinedMatrix();
			Vector2Ext.transform( pts, ref mat, pts );

			return pts;
		}

	}
}
#endif
