﻿using Nez.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Nez.UI
{
	/// <summary>
	/// Drawable for a {@link Subtexture}
	/// </summary>
	public class SubtextureDrawable : IDrawable
	{
		public Color? tintColor;
        public float alpha = 1f;
        public float rotation = 0f;
        public float scale = 1f;
		public SpriteEffects spriteEffects = SpriteEffects.None;

		/// <summary>
		/// determines if the sprite should be rendered normally or flipped horizontally
		/// </summary>
		/// <value><c>true</c> if flip x; otherwise, <c>false</c>.</value>
		public bool flipX
		{
			get
			{
				return ( spriteEffects & SpriteEffects.FlipHorizontally ) == SpriteEffects.FlipHorizontally;
			}
			set
			{
				spriteEffects = value ? ( spriteEffects | SpriteEffects.FlipHorizontally ) : ( spriteEffects & ~SpriteEffects.FlipHorizontally );
			}
		}

		/// <summary>
		/// determines if the sprite should be rendered normally or flipped vertically
		/// </summary>
		/// <value><c>true</c> if flip y; otherwise, <c>false</c>.</value>
		public bool flipY
		{
			get
			{
				return ( spriteEffects & SpriteEffects.FlipVertically ) == SpriteEffects.FlipVertically;
			}
			set
			{
				spriteEffects = value ? ( spriteEffects | SpriteEffects.FlipVertically ) : ( spriteEffects & ~SpriteEffects.FlipVertically );
			}
		}

		public Subtexture subtexture
		{
			get { return _subtexture; }
			set
			{
				_subtexture = value;
				minWidth = _subtexture.sourceRect.Width;
				minHeight = _subtexture.sourceRect.Height;
			}
		}
		protected Subtexture _subtexture;


		#region IDrawable implementation

		public float leftWidth { get; set; }
		public float rightWidth { get; set; }
		public float topHeight { get; set; }
		public float bottomHeight { get; set; }
		public float minWidth { get; set; }
		public float minHeight { get; set; }


		public void setPadding( float top, float bottom, float left, float right )
		{
			topHeight = top;
			bottomHeight = bottom;
			leftWidth = left;
			rightWidth = right;
		}

		#endregion


		public SubtextureDrawable( Subtexture subtexture )
		{
			this.subtexture = subtexture;
		}


		public SubtextureDrawable( Texture2D texture ) : this( new Subtexture( texture ) )
		{ }


		public virtual void draw( Graphics graphics, float x, float y, float width, float height, Color color )
		{
			if( tintColor.HasValue )
				color = color.multiply( tintColor.Value );

            if(alpha != 1)
                color *= alpha;

            var newWidth = width * scale;
            var newHeight = height * scale;
            var newX = x + newWidth / 2 - (newWidth - width) / 2;
            var newY = y + newHeight / 2 - (newHeight - height) / 2;
            var destRect = new Rectangle((int)newX, (int)newY, (int)newWidth, (int)newHeight);
            graphics.batcher.draw(_subtexture.texture2D, destRect, _subtexture.sourceRect, color, rotation, new Vector2(subtexture.sourceRect.Width, subtexture.sourceRect.Height) * 0.5f, spriteEffects, 0f);
		}


		/// <summary>
		/// returns a new drawable with the tint color specified
		/// </summary>
		/// <returns>The tinted drawable.</returns>
		/// <param name="tint">Tint.</param>
		public SubtextureDrawable newTintedDrawable( Color tint )
		{
			return new SubtextureDrawable( _subtexture )
			{
				leftWidth = leftWidth,
				rightWidth = rightWidth,
				topHeight = topHeight,
				bottomHeight = bottomHeight,
				minWidth = minWidth,
				minHeight = minHeight,
				tintColor = tint
			};
		}
	}
}

