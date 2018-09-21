using System;
using Microsoft.Xna.Framework;

namespace Nez.UI
{
    public enum PrimitiveStyle : int
    {
        Hidden = 0,
        Top  = 1,
        Left = 2,
        Right = 4,
        Bottom = 8,
        Hollow = Top | Left | Right | Bottom,
        Fill = 16
    }

	public class PrimitiveDrawable : IDrawable
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
		public PrimitiveStyle primitiveStyle =  PrimitiveStyle.Fill;


		public PrimitiveDrawable( Color? color = null )
		{
			this.color = color;
		}


		public PrimitiveDrawable( Color color, float horizontalPadding ) : this( color )
		{
			leftWidth = rightWidth = horizontalPadding;
		}


		public PrimitiveDrawable( Color color, float horizontalPadding, float verticalPadding ) : this( color )
		{
			leftWidth = rightWidth = horizontalPadding;
			topHeight = bottomHeight = verticalPadding;
		}


		public PrimitiveDrawable( float minWidth, float minHeight, Color? color = null ) : this( color )
		{
			this.minWidth = minWidth;
			this.minHeight = minHeight;
		}


		public PrimitiveDrawable( float minSize ) : this( minSize, minSize )
		{}


		public PrimitiveDrawable( float minSize, Color color ) : this( minSize, minSize, color )
		{}


		public virtual void draw( Graphics graphics, float x, float y, float width, float height, Color color )
		{
            if (primitiveStyle == PrimitiveStyle.Hidden)
                return;

			var col = this.color.HasValue ? this.color.Value : color;
			if( color.A != 255 )
				col *= ( color.A / 255f );
			if (col.A != 255)
				col *= ( col.A / 255f );

			if(primitiveStyle == PrimitiveStyle.Fill)
				graphics.batcher.drawRect( x, y, width, height, col );
			else if (primitiveStyle == PrimitiveStyle.Hollow)
				graphics.batcher.drawHollowRect( x, y, width, height, col );
            else{
                if ((primitiveStyle & PrimitiveStyle.Left) == PrimitiveStyle.Left)
                    graphics.batcher.drawRect(x, y, 1, height, col);
                if ((primitiveStyle & PrimitiveStyle.Right) == PrimitiveStyle.Right)
                    graphics.batcher.drawRect(x + width - 1, y, 1, height, col);
                if ((primitiveStyle & PrimitiveStyle.Top) == PrimitiveStyle.Top)
                    graphics.batcher.drawRect(x, y, width, 1, col);
                if ((primitiveStyle & PrimitiveStyle.Bottom) == PrimitiveStyle.Bottom)
                    graphics.batcher.drawRect(x, y + height - 1, width, 1, col);
            }
        }
	}
}

