
using Nez.Sprites;


namespace Nez
{
	public class Text : Sprite
	{
		public override RectangleF bounds
		{
			get
			{
				if( _areBoundsDirty )
				{
					_bounds.calculateBounds( entity.transform.position, _localOffset, _origin, entity.transform.scale, entity.transform.rotation, _size.x, _size.y );
					_areBoundsDirty = false;
				}

				return _bounds;
			}
		}

		/// <summary>
		/// text to draw
		/// </summary>
		/// <value>The text.</value>
		public string text
		{
			get { return _text; }
			set { setText( value ); }
		}

		/// <summary>
		/// horizontal alignment of the text
		/// </summary>
		/// <value>The horizontal origin.</value>
		public HorizontalAlign horizontalOrigin
		{
			get { return _horizontalAlign; }
			set { setHorizontalAlign( value ); }
		}

		/// <summary>
		/// vertical alignment of the text
		/// </summary>
		/// <value>The vertical origin.</value>
		public VerticalAlign verticalOrigin
		{
			get { return _verticalAlign; }
			set { setVerticalAlign( value ); }
		}


		protected HorizontalAlign _horizontalAlign;
		protected VerticalAlign _verticalAlign;
		protected IFont _font;
		protected string _text;
		vec2 _size;


		public Text( IFont font, string text, vec2 localOffset, Color color )
		{
			_font = font;
			_text = text;
			_localOffset = localOffset;
			this.color = color;
			_horizontalAlign = HorizontalAlign.Left;
			_verticalAlign = VerticalAlign.Top;

			updateSize();
		}


		#region Fluent setters

		public Text setFont( IFont font )
		{
			_font = font;
			updateSize();

			return this;
		}


		public Text setText( string text )
		{
			_text = text;
			updateSize();
			updateCentering();

			return this;
		}


		public Text setHorizontalAlign( HorizontalAlign hAlign )
		{
			_horizontalAlign = hAlign;
			updateCentering();

			return this;
		}


		public Text setVerticalAlign( VerticalAlign vAlign )
		{
			_verticalAlign = vAlign;
			updateCentering();

			return this;
		}

		#endregion


		void updateSize()
		{
			_size = _font.measureString( _text );
			updateCentering();
		}


		void updateCentering()
		{
			var oldOrigin = _origin;

			if( _horizontalAlign == HorizontalAlign.Left )
				oldOrigin.x = 0;
			else if( _horizontalAlign == HorizontalAlign.Center )
				oldOrigin.x = _size.x / 2;
			else
				oldOrigin.x = _size.x;

			if( _verticalAlign == VerticalAlign.Top )
				oldOrigin.y = 0;
			else if( _verticalAlign == VerticalAlign.Center )
				oldOrigin.y = _size.y / 2;
			else
				oldOrigin.y = _size.y;

			origin = new vec2( (int)oldOrigin.x, (int)oldOrigin.y );
		}


		public override void render( Graphics graphics, Camera camera )
		{
			graphics.batcher.drawString( _font, _text, entity.transform.position + _localOffset, color, entity.transform.rotation, origin, entity.transform.scale, spriteEffects, layerDepth );
		}

	}
}

