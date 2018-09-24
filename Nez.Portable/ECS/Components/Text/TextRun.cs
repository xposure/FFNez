using System;

using Microsoft.Xna.Framework.Graphics;
using Nez.BitmapFonts;


namespace Nez
{
	/// <summary>
	/// provides a cached run of text for super fast text drawing. Note that this is only appropriate for text that doesnt change often
	/// and doesnt move.
	/// </summary>
	public class TextRun
	{
        struct CharDetails
        {
            public Texture2D texture;
            public vec3[] verts;
            public vec2[] texCoords;
            public Color color;

            public void initialize()
            {
                verts = new vec3[4];
                texCoords = new vec2[4];
            }
		}

		public float width { get { return _size.x; } }
		public float height { get { return _size.y; } }
		public vec2 origin { get { return _origin; } }
		public float rotation;
		public vec2 position;

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


		HorizontalAlign _horizontalAlign;
		VerticalAlign _verticalAlign;
		BitmapFont _font;
		string _text;
		vec2 _size;
		Color _color = Color.White;
		vec2 _origin;
		vec2 _scale = vec2.One;
		CharDetails[] _charDetails;

		static readonly float[] _cornerOffsetX = { 0.0f, 1.0f, 0.0f, 1.0f };
		static readonly float[] _cornerOffsetY = { 0.0f, 0.0f, 1.0f, 1.0f };


		public TextRun( BitmapFont font )
		{
			_font = font;
			_horizontalAlign = HorizontalAlign.Left;
			_verticalAlign = VerticalAlign.Top;
		}


		#region Fluent setters

		public TextRun setFont( BitmapFont font )
		{
			_font = font;
			updateSize();
			return this;
		}


		public TextRun setText( string text )
		{
			_text = text;
			updateSize();
			updateCentering();
			return this;
		}


		public TextRun setHorizontalAlign( HorizontalAlign hAlign )
		{
			_horizontalAlign = hAlign;
			updateCentering();
			return this;
		}


		public TextRun setVerticalAlign( VerticalAlign vAlign )
		{
			_verticalAlign = vAlign;
			updateCentering();
			return this;
		}

		#endregion


		void updateSize()
		{
			_size = _font.measureString( _text ) * _scale;
			updateCentering();
		}


		void updateCentering()
		{
			var newOrigin = vec2.Zero;

			if( _horizontalAlign == HorizontalAlign.Left )
				newOrigin.x = 0;
			else if( _horizontalAlign == HorizontalAlign.Center )
				newOrigin.x = _size.x / 2;
			else
				newOrigin.x = _size.x;

			if( _verticalAlign == VerticalAlign.Top )
				newOrigin.y = 0;
			else if( _verticalAlign == VerticalAlign.Center )
				newOrigin.y = _size.y / 2;
			else
				newOrigin.y = _size.y;

			_origin = new vec2( (int)( newOrigin.x * _scale.x ), (int)( newOrigin.y * _scale.y ) );
		}


		/// <summary>
		/// compiles the text into raw verts/texture coordinates. This method must be called anytime text or any other properties are
		/// changed.
		/// </summary>
		public void compile()
		{
			_charDetails = new CharDetails[_text.Length];
			BitmapFontRegion currentFontRegion = null;
			var effects = (byte)SpriteEffects.None;

			var _transformationMatrix = Matrix2D.identity;
			var requiresTransformation = rotation != 0f || _scale != vec2.One;
			if( requiresTransformation )
			{
				Matrix2D temp;
				Matrix2D.createTranslation( -_origin.x, -_origin.y, out _transformationMatrix );
				Matrix2D.createScale( _scale.x, _scale.y, out temp );
				Matrix2D.multiply( ref _transformationMatrix, ref temp, out _transformationMatrix );
				Matrix2D.createRotation( rotation, out temp );
				Matrix2D.multiply( ref _transformationMatrix, ref temp, out _transformationMatrix );
				Matrix2D.createTranslation( position.x, position.y, out temp );
				Matrix2D.multiply( ref _transformationMatrix, ref temp, out _transformationMatrix );
			}

			var offset = requiresTransformation ? vec2.Zero : position - _origin;

			for( var i = 0; i < _text.Length; ++i )
			{
				_charDetails[i].initialize();
				_charDetails[i].color = _color;

				var c = _text[i];
				if( c == '\n' )
				{
					offset.x = requiresTransformation ? 0f : position.x - _origin.x;
					offset.y += _font.lineHeight;
					currentFontRegion = null;
					continue;
				}

				if( currentFontRegion != null )
					offset.x += _font.spacing + currentFontRegion.xAdvance;

				currentFontRegion = _font.fontRegionForChar( c, true );
				var p = offset;
				p.x += currentFontRegion.xOffset;
				p.y += currentFontRegion.yOffset;

				// transform our point if we need to
				if( requiresTransformation )
					Vector2Ext.transform( ref p, ref _transformationMatrix, out p );

				var destination = new vec4( p.x, p.y, currentFontRegion.width * _scale.x, currentFontRegion.height * _scale.y );
				_charDetails[i].texture = currentFontRegion.subtexture.texture2D;


				// Batcher calculations
				var sourceRectangle = currentFontRegion.subtexture.sourceRect;
				float sourceX, sourceY, sourceW, sourceH;
				var destW = destination.z;
				var destH = destination.w;

				// calculate uvs
				var inverseTexW = 1.0f / (float)currentFontRegion.subtexture.texture2D.Width;
				var inverseTexH = 1.0f / (float)currentFontRegion.subtexture.texture2D.Height;

				sourceX = sourceRectangle.X * inverseTexW;
				sourceY = sourceRectangle.Y * inverseTexH;
				sourceW = Math.Max( sourceRectangle.Width, float.Epsilon ) * inverseTexW;
				sourceH = Math.Max( sourceRectangle.Height, float.Epsilon ) * inverseTexH;

				// Rotation Calculations
				float rotationMatrix1X;
				float rotationMatrix1Y;
				float rotationMatrix2X;
				float rotationMatrix2Y;
				if( !Mathf.withinEpsilon( rotation, 0.0f ) )
				{
					var sin = Mathf.sin( rotation );
					var cos = Mathf.cos( rotation );
					rotationMatrix1X = cos;
					rotationMatrix1Y = sin;
					rotationMatrix2X = -sin;
					rotationMatrix2Y = cos;
				}
				else
				{
					rotationMatrix1X = 1.0f;
					rotationMatrix1Y = 0.0f;
					rotationMatrix2X = 0.0f;
					rotationMatrix2Y = 1.0f;
				}

				// Calculate vertices, finally.
				// top-left
				_charDetails[i].verts[0].X = rotationMatrix2X + rotationMatrix1X + destination.x - 1;
				_charDetails[i].verts[0].Y = rotationMatrix2Y + rotationMatrix1Y + destination.y - 1;

				// top-right
				var cornerX = _cornerOffsetX[1] * destW;
				var cornerY = _cornerOffsetY[1] * destH;
				_charDetails[i].verts[1].X = (
					( rotationMatrix2X * cornerY ) +
					( rotationMatrix1X * cornerX ) +
					destination.x
				);
				_charDetails[i].verts[1].Y = (
					( rotationMatrix2Y * cornerY ) +
					( rotationMatrix1Y * cornerX ) +
					destination.y
				);

				// bottom-left
				cornerX = _cornerOffsetX[2] * destW;
				cornerY = _cornerOffsetY[2] * destH;
				_charDetails[i].verts[2].X = (
					( rotationMatrix2X * cornerY ) +
					( rotationMatrix1X * cornerX ) +
					destination.x
				);
				_charDetails[i].verts[2].Y = (
					( rotationMatrix2Y * cornerY ) +
					( rotationMatrix1Y * cornerX ) +
					destination.y
				);

				// bottom-right
				cornerX = _cornerOffsetX[3] * destW;
				cornerY = _cornerOffsetY[3] * destH;
				_charDetails[i].verts[3].X = (
					( rotationMatrix2X * cornerY ) +
					( rotationMatrix1X * cornerX ) +
					destination.x
				);
				_charDetails[i].verts[3].Y = (
					( rotationMatrix2Y * cornerY ) +
					( rotationMatrix1Y * cornerX ) +
					destination.y
				);


				// texture coordintes
				_charDetails[i].texCoords[0].x = ( _cornerOffsetX[0 ^ effects] * sourceW ) + sourceX;
				_charDetails[i].texCoords[0].y = ( _cornerOffsetY[0 ^ effects] * sourceH ) + sourceY;
				_charDetails[i].texCoords[1].x = ( _cornerOffsetX[1 ^ effects] * sourceW ) + sourceX;
				_charDetails[i].texCoords[1].y = ( _cornerOffsetY[1 ^ effects] * sourceH ) + sourceY;
				_charDetails[i].texCoords[2].x = ( _cornerOffsetX[2 ^ effects] * sourceW ) + sourceX;
				_charDetails[i].texCoords[2].y = ( _cornerOffsetY[2 ^ effects] * sourceH ) + sourceY;
				_charDetails[i].texCoords[3].x = ( _cornerOffsetX[3 ^ effects] * sourceW ) + sourceX;
				_charDetails[i].texCoords[3].y = ( _cornerOffsetY[3 ^ effects] * sourceH ) + sourceY;
			}
		}


		public void render( Graphics graphics )
		{
			for( var i = 0; i < _charDetails.Length; i++ )
				graphics.batcher.drawRaw( _charDetails[i].texture, _charDetails[i].verts.ToVec3(), _charDetails[i].texCoords.ToVec2(), _charDetails[i].color );
		}

	}
}

