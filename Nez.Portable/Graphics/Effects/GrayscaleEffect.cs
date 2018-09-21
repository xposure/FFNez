#if FEATURE_GRAPHICS
using System;
using Microsoft.Xna.Framework.Graphics;


namespace Atma
{
	public class GrayscaleEffect : Effect
	{
		public GrayscaleEffect() : base( Core.graphicsDevice, EffectResource.grayscaleBytes )
		{
		}
	}
}

#endif
