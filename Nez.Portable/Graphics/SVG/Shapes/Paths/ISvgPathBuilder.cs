#if FEATURE_GRAPHICS
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Atma.Svg
{
	/// <summary>
	/// dummy interface used by SvgPath.getTransformedDrawingPoints to workaround PCL not having System.Drawing
	/// </summary>
	public interface ISvgPathBuilder
	{
		vec2[] getDrawingPoints( List<SvgPathSegment> segments, float flatness = 3 );
	}
}
#endif
