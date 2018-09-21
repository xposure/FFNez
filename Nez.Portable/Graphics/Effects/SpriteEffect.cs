#if FEATURE_GRAPHICS
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Atma
{
	public class SpriteEffect : Effect
	{
		EffectParameter _matrixTransformParam;


		public SpriteEffect() : base( Core.graphicsDevice, EffectResource.spriteEffectBytes )
		{
			_matrixTransformParam = Parameters["MatrixTransform"];
		}


		public void setMatrixTransform( Matrix matrixTransform)
        {
            _matrixTransformParam.SetValue(matrixTransform);
        }
        public void setMatrixTransform( ref mat4 matrixTransform )
		{
			_matrixTransformParam.SetValue( matrixTransform );
		}

	}
}

#endif
