using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;


namespace Atma.BitmapFontImporter
{
	public class BitmapFontProcessorResult
	{
		public List<Texture2DContent> textures = new List<Texture2DContent>();
		public List<string> textureNames = new List<string>();
		public List<vec2> textureOrigins = new List<vec2>();
		public BitmapFontFile fontFile;
		public bool packTexturesIntoXnb;


		public BitmapFontProcessorResult( BitmapFontFile fontFile )
		{
			this.fontFile = fontFile;
		}
	}
}