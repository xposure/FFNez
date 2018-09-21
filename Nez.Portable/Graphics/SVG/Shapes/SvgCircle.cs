#if FEATURE_GRAPHICS
using System.Xml.Serialization;


namespace Atma.Svg
{
	public class SvgCircle : SvgElement
	{
		[XmlAttribute( "r" )]
		public float radius;

		[XmlAttribute( "cy" )]
		public float centerY;

		[XmlAttribute( "cx" )]
		public float centerX;
	}
}
#endif
