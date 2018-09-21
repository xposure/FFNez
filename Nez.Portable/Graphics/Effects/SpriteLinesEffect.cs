#if FEATURE_GRAPHICS
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Atma
{
	/// <summary>
	/// draws the sprite with just vertical or horizonal lines of the specified color. The effect works in screen space.
	/// </summary>
	public class SpriteLinesEffect : Effect
	{
		/// <summary>
		/// color of the lines. Defaults to red.
		/// </summary>
		/// <value>The color of the line.</value>
		public Color lineColor
		{
			get { return new Color( _lineColor ); }
			set
			{
				var blinkVec = value;
				if( _lineColor != blinkVec )
				{
					_lineColor = blinkVec;
					_lineColorParam.SetValue( _lineColor );
				}
			}
		}

		/// <summary>
		/// size of the lines in pixels. Defaults to 5.
		/// </summary>
		/// <value>The size of the line.</value>
		public float lineSize
		{
			get { return _lineSize; }
			set
			{
				if( _lineSize != value )
				{
					_lineSize = value;
					_lineSizeParam.SetValue( _lineSize );
				}
			}
		}

		/// <summary>
		/// toggles vertical/horizontal lines
		/// </summary>
		/// <value><c>true</c> if is vertical; otherwise, <c>false</c>.</value>
		public bool isVertical
		{
			get { return _isVertical; }
			set
			{
				if( _isVertical != value )
				{
					_isVertical = value;
					CurrentTechnique = _isVertical ? Techniques["VerticalLines"] : Techniques["HorizontalLines"];
				}
			}
		}

		Vector4 _lineColor = new Vector4( 1, 0, 0, 1 );
		float _lineSize = 5f;
		bool _isVertical = true;

		EffectParameter _lineColorParam;
		EffectParameter _lineSizeParam;


		public SpriteLinesEffect() : base( Core.graphicsDevice, EffectResource.spriteLinesEffectBytes )
		{
			_lineColorParam = Parameters["_lineColor"];
			_lineSizeParam = Parameters["_lineSize"];

			_lineColorParam.SetValue( _lineColor );
			_lineSizeParam.SetValue( _lineSize );
		}
	}
}

#endif
