#if !FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;


namespace Atma
{
	public static class GestureSampleExt
	{
		public static vec2 scaledPosition( this GestureSample gestureSample )
		{
			return Input.scaledPosition( gestureSample.Position );
		}

		public static vec2 scaledPosition2( this GestureSample gestureSample )
		{
			return Input.scaledPosition( gestureSample.Position2 );
		}
	}
}
#endif
