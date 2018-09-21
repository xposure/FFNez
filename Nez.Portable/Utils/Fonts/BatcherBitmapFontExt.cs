#if FEATURE_UTILS
using System;
using Microsoft.Xna.Framework;
using Atma.TextureAtlases;
using System.Text;
using Atma.BitmapFonts;
using Atma;
using Microsoft.Xna.Framework.Graphics;


namespace Atma
{
	/// <summary>
	/// provides the full SpriteFont assortment of drawString methods
	/// </summary>
	public static class BatcherBitmapFontExt
	{
		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		public static void drawString( this Batcher batcher, BitmapFont bitmapFont, string text, vec2 position, Color color )
		{
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, 0, vec2.Zero, vec2.One, SpriteEffects.None, 0f );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public static void drawString( this Batcher batcher, BitmapFont bitmapFont, string text, vec2 position, Color color,
			float rotation, vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			var scaleVec = new vec2( scale, scale );
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, rotation, origin, scaleVec, effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public static void drawString( this Batcher batcher, BitmapFont bitmapFont, string text, vec2 position, Color color,
			float rotation, vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, rotation, origin, scale, effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		public static void drawString( this Batcher batcher, BitmapFont bitmapFont, StringBuilder text, vec2 position, Color color )
		{
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, 0, vec2.Zero, vec2.One, SpriteEffects.None, 0f );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public static void drawString(
			this Batcher batcher, BitmapFont bitmapFont, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, float scale, SpriteEffects effects, float layerDepth )
		{
			var scaleVec = new vec2( scale, scale );
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, rotation, origin, scaleVec, effects, layerDepth );
		}


		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public static void drawString(
			this Batcher batcher, BitmapFont bitmapFont, StringBuilder text, vec2 position, Color color,
			float rotation, vec2 origin, vec2 scale, SpriteEffects effects, float layerDepth )
		{
			var source = new FontCharacterSource( text );
			bitmapFont.drawInto( batcher, ref source, position, color, rotation, origin, scale, effects, layerDepth );
		}

	}
}
#endif
