using Nez.Textures;
using Microsoft.Xna.Framework.Graphics;



namespace Nez.Sprites
{
	/// <summary>
	/// the most basic and common Renderable. Renders a Subtexture/Texture.
	/// </summary>
	public class Sprite : RenderableComponent
	{
		public override RectangleF bounds
		{
			get
			{
				if( _areBoundsDirty )
				{
					_bounds.calculateBounds( entity.transform.position, _localOffset, _origin, entity.transform.scale, entity.transform.rotation, subtexture.sourceRect.Width, subtexture.sourceRect.Height );
					_areBoundsDirty = false;
				}

				return _bounds;
			}
		}


		/// <summary>
		/// the origin of the Sprite. This is set automatically when setting a Subtexture.
		/// </summary>
		/// <value>The origin.</value>
		public vec2 origin
		{
			get { return _origin; }
			set { setOrigin( value ); }
		}

		/// <summary>
		/// helper property for setting the origin in normalized fashion (0-1 for x and y)
		/// </summary>
		/// <value>The origin normalized.</value>
		public vec2 originNormalized
		{
			get { return new vec2( _origin.x / width, _origin.y / height ); }
			set { setOrigin( new vec2( value.x * width, value.y * height ) ); }
		}

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

		/// <summary>
		/// Batchers passed along to the Batcher when rendering. flipX/flipY are helpers for setting this.
		/// </summary>
		public SpriteEffects spriteEffects = SpriteEffects.None;

		/// <summary>
		/// the Subtexture that should be displayed by this Sprite. When set, the origin of the Sprite is also set to match Subtexture.origin.
		/// </summary>
		/// <value>The subtexture.</value>
		public Subtexture subtexture
		{
			get { return _subtexture; }
			set { setSubtexture( value ); }
		}

		protected vec2 _origin;
		protected Subtexture _subtexture;


		public Sprite()
		{}


		public Sprite( Subtexture subtexture )
		{
			_subtexture = subtexture;
			_origin = subtexture.center;
		}


		public Sprite( Texture2D texture ) : this( new Subtexture( texture ) )
		{ }


		#region fluent setters

		/// <summary>
		/// sets the Subtexture and updates the origin of the Sprite to match Subtexture.origin. If for whatever reason you need
		/// an origin different from the Subtexture either clone it or set the origin AFTER setting the Subtexture here.
		/// </summary>
		/// <returns>The subtexture.</returns>
		/// <param name="subtexture">Subtexture.</param>
		public Sprite setSubtexture( Subtexture subtexture )
		{
			_subtexture = subtexture;

			if( _subtexture != null )
				_origin = subtexture.origin;
			return this;
		}


		/// <summary>
		/// sets the origin for the Renderable
		/// </summary>
		/// <returns>The origin.</returns>
		/// <param name="origin">Origin.</param>
		public Sprite setOrigin( vec2 origin )
		{
			if( _origin != origin )
			{
				_origin = origin;
				_areBoundsDirty = true;
			}
			return this;
		}


		/// <summary>
		/// helper for setting the origin in normalized fashion (0-1 for x and y)
		/// </summary>
		/// <returns>The origin normalized.</returns>
		/// <param name="origin">Origin.</param>
		public Sprite setOriginNormalized( vec2 value )
		{
			setOrigin( new vec2( value.x * width, value.y * height ) );
			return this;
		}

		#endregion


		/// <summary>
		/// Draws the Renderable with an outline. Note that this should be called on disabled Renderables since they shouldnt take part in default
		/// rendering if they need an ouline.
		/// </summary>
		/// <param name="graphics">Graphics.</param>
		/// <param name="camera">Camera.</param>
		/// <param name="offset">Offset.</param>
		public void drawOutline( Graphics graphics, Camera camera, int offset = 1 )
		{
			drawOutline( graphics, camera, Color.Black, offset );
		}


		public void drawOutline( Graphics graphics, Camera camera, Color outlineColor, int offset = 1 )
		{
			// save the stuff we are going to modify so we can restore it later
			var originalPosition = _localOffset;
			var originalColor = color;
			var originalLayerDepth = _layerDepth;

			// set our new values
			color = outlineColor;
			_layerDepth += 0.01f;

			for( var i = -1; i < 2; i++ )
			{
				for( var j = -1; j < 2; j++ )
				{
					if( i != 0 || j != 0 )
					{
						_localOffset = originalPosition + new vec2( i * offset, j * offset );
						render( graphics, camera );
					}
				}
			}

			// restore changed state
			_localOffset = originalPosition;
			color = originalColor;
			_layerDepth = originalLayerDepth;
		}


		public override void render( Graphics graphics, Camera camera )
		{
			graphics.batcher.draw( _subtexture, entity.transform.position + localOffset, color, entity.transform.rotation, origin, entity.transform.scale, spriteEffects, _layerDepth );
		}

	}
}

