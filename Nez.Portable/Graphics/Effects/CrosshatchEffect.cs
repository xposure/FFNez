#if FEATURE_GRAPHICS
using System;
using Microsoft.Xna.Framework.Graphics;


namespace Atma
{
	public class CrosshatchEffect : Effect
	{
		/// <summary>
		/// size in pixels of the crosshatch. Should be an even number because the half size is also required. Defaults to 16.
		/// </summary>
		/// <value>The size of the cross hatch.</value>
		public int crosshatchSize
		{
			get { return _crosshatchSize; }
			set
			{
				// ensure we have an even number
				if( !Mathf.isEven( value ) )
					value += 1;
				
				if( _crosshatchSize != value )
				{
					_crosshatchSize = value;
					_crosshatchSizeParam.SetValue( _crosshatchSize );
				}
			}
		}

		int _crosshatchSize = 16;
		EffectParameter _crosshatchSizeParam;

		
		public CrosshatchEffect() : base( Core.graphicsDevice, EffectResource.crosshatchBytes )
		{
			_crosshatchSizeParam = Parameters["crossHatchSize"];
			_crosshatchSizeParam.SetValue( _crosshatchSize );
		}
	}
}

#endif
