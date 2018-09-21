#if FEATURE_UTILS
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atma.BitmapFonts;


namespace Atma
{
	/// <summary>
	/// helper methods for drawing text with NezSpriteFonts
	/// </summary>
	public static class BatcherSpriteFontExt
	{
		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, StringBuilder text, vec2 position, Color color )
		{
			batcher.drawString( spriteFont, text, position, color, 0.0f, vec2.Zero, new vec2( 1.0f ), SpriteEffects.None, 0.0f );
		}


		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			batcher.drawString( spriteFont, text, position, color, rotation, origin, new vec2( scale ), effects, layerDepth );
		}


		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, string text, vec2 position, Color color )
		{
			batcher.drawString( spriteFont, text, position, color, 0.0f, vec2.Zero, new vec2( 1.0f ), SpriteEffects.None, 0.0f );
		}


		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, string text, vec2 position, Color color, float rotation,
			vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			batcher.drawString( spriteFont, text, position, color, rotation, origin, new vec2( scale ), effects, layerDepth );
		}


		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			Assert.isFalse( text == null );

			if( text.Length == 0 )
				return;

			var source = new FontCharacterSource( text );
			spriteFont.drawInto( batcher, ref source, position, color, rotation, origin, scale, effects, layerDepth );
		}


		public static void drawString( this Batcher batcher, NezSpriteFont spriteFont, string text, vec2 position, Color color, float rotation,
			vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			Assert.isFalse( text == null );

			if( text.Length == 0 )
				return;

			var source = new FontCharacterSource( text );
			spriteFont.drawInto( batcher, ref source, position, color, rotation, origin, scale, effects, layerDepth );
		}
	
	}
}

#endif
