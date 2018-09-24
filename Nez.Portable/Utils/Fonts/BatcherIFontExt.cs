using System;
using System.Text;

using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	/// <summary>
	/// provides the full SpriteFont assortment of drawString methods
	/// </summary>
	public static class BatcherIFontExt
	{
		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		public static void drawString( this Batcher batcher, IFont font, StringBuilder text, vec2 position, Color color )
		{
			batcher.drawString( font, text, position, color, 0.0f, vec2.Zero, new vec2( 1.0f ), SpriteEffects.None, 0.0f );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="origin">Origin.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="effects">Effects.</param>
		/// <param name="layerDepth">Layer depth.</param>
		public static void drawString( this Batcher batcher, IFont font, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			batcher.drawString( font, text, position, color, rotation, origin, new vec2( scale ), effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		public static void drawString( this Batcher batcher, IFont font, string text, vec2 position, Color color )
		{
			batcher.drawString( font, text, position, color, 0.0f, vec2.Zero, new vec2( 1.0f ), SpriteEffects.None, 0.0f );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="origin">Origin.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="effects">Effects.</param>
		/// <param name="layerDepth">Layer depth.</param>
		public static void drawString( this Batcher batcher, IFont font, string text, vec2 position, Color color, float rotation,
			vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			batcher.drawString( font, text, position, color, rotation, origin, new vec2( scale ), effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="origin">Origin.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="effects">Effects.</param>
		/// <param name="layerDepth">Layer depth.</param>
		public static void drawString( this Batcher batcher, IFont font, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			Assert.isFalse( text == null );

			if( text.Length == 0 )
				return;

			font.drawInto( batcher, text, position, color, rotation, origin, scale, effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="batcher">Batcher.</param>
		/// <param name="font">Font.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="origin">Origin.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="effects">Effects.</param>
		/// <param name="layerDepth">Layer depth.</param>
		public static void drawString( this Batcher batcher, IFont font, string text, vec2 position, Color color, float rotation,
			vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			Assert.isFalse( text == null );

			if( text.Length == 0 )
				return;

			font.drawInto( batcher, text, position, color, rotation, origin, scale, effects, layerDepth );
		}

	}
}

