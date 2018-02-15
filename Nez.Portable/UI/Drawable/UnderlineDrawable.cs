using System;
using Microsoft.Xna.Framework;


namespace Nez.UI
{
	public class UnderlineDrawable : IDrawable
	{
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

		public Color? color;


		public UnderlineDrawable( Color? color = null )
		{
			this.color = color;
		}

		public virtual void draw( Graphics graphics, float x, float y, float width, float height, Color color )
		{
			var col = this.color.HasValue ? this.color.Value : color;
			if( color.A != 255 )
				col *= ( color.A / 255f );

				graphics.batcher.drawRect( x, y + height, width, 1, col );
		}
	}
}

