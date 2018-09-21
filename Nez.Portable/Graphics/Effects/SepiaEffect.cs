#if FEATURE_GRAPHICS
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Atma
{
	public class SepiaEffect : Effect
	{
		/// <summary>
		/// multiplied by the grayscale value for the final output. Defaults to 1.2f, 1.0f, 0.8f
		/// </summary>
		/// <value>The sepia tone.</value>
		public vec3 sepiaTone
		{
			get { return _sepiaTone; }
			set
			{
				_sepiaTone = value;
				_sepiaToneParam.SetValue( _sepiaTone );
			}
		}


		vec3 _sepiaTone = new vec3( 1.2f, 1.0f, 0.8f );
		EffectParameter _sepiaToneParam;

		
		public SepiaEffect() : base( Core.graphicsDevice, EffectResource.sepiaBytes )
		{
			_sepiaToneParam = Parameters["_sepiaTone"];
			_sepiaToneParam.SetValue( _sepiaTone );
		}
	}
}

#endif
