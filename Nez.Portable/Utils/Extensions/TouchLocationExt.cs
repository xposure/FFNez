#if !FNA

using Microsoft.Xna.Framework.Input.Touch;


namespace Nez
{
	public static class TouchLocationExt
	{
		public static vec2 scaledPosition( this TouchLocation touchLocation )
		{
			return Input.scaledPosition( touchLocation.Position );
		}
	}
}
#endif
