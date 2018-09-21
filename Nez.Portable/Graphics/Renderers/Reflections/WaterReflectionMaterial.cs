#if FEATURE_GRAPHICS
using System;
using Atma;
using Microsoft.Xna.Framework.Graphics;
using Atma.Textures;


namespace Atma
{
	/// <summary>
	/// used by the WaterReflectionPlane
	/// </summary>
	public class WaterReflectionMaterial : Material<WaterReflectionEffect>
	{
		/// <summary>
		/// we store a reference to the RenderTarget so we can update the Effect when it changes
		/// </summary>
		RenderTarget2D _renderTarget;

		/// <summary>
		/// cache the array so we dont have to recreate it every frame
		/// </summary>
		RenderTargetBinding[] _renderTargetBinding = new RenderTargetBinding[1];


		public WaterReflectionMaterial() : base( new WaterReflectionEffect() )
		{}


		public override void onPreRender( Camera camera )
		{
			Core.graphicsDevice.GetRenderTargets( _renderTargetBinding );
			var boundRenderTarget = _renderTargetBinding[0].RenderTarget as RenderTarget2D;

			// only update the Shader when the renderTarget changes. it will be swapped out whenever the GraphicsDevice resets.
			if( _renderTarget == null || _renderTarget != boundRenderTarget )
			{
				_renderTarget = boundRenderTarget;
				effect.renderTexture = boundRenderTarget;
			}

			effect.matrixTransform = camera.viewProjectionMatrix;
		}
	}
}

#endif
