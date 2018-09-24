using System;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;


namespace Nez.UI
{
	/// <summary>
	/// A table that can be dragged and resized. The top padding is used as the window's title height.
	/// 
	/// The preferred size of a window is the preferred size of the title text and the children as laid out by the table. After adding
	/// children to the window, it can be convenient to call {@link #pack()} to size the window to the size of the children.
	/// </summary>
	public class Window : Table, IInputListener
	{
		static private int MOVE = 1 << 5;

		private WindowStyle style;
		bool _isMovable = true, _isResizable;
		int resizeBorderSize = 8;
		bool _dragging;
		bool _keepWithinStage = true;
		Label titleLabel;
		Table titleTable;


		public Window( string title, WindowStyle style )
		{
			Assert.isNotNull( title, "title cannot be null" );

			touchable = Touchable.Enabled;
			clip = true;

			titleLabel = new Label( title, new LabelStyle( style.titleFont, style.titleFontColor ) );
			titleLabel.setEllipsis( true );

			titleTable = new Table();
			titleTable.add( titleLabel ).setExpandX().setFillX().setMinWidth( 0 );
			addElement( titleTable );

			setStyle( style );
			width = 150;
			height = 150;
		}


		public Window( string title, Skin skin, string styleName = null ) : this( title, skin.get<WindowStyle>( styleName ) )
		{}


		#region IInputListener

		int edge;
		float startX, startY, lastX, lastY;

		void IInputListener.onMouseEnter()
		{
		}


		void IInputListener.onMouseExit()
		{
		}


		bool IInputListener.onMousePressed( vec2 mousePos )
		{
			float width = getWidth(), height = getHeight();
			edge = 0;
			if( _isResizable && mousePos.x >= 0 && mousePos.x < width && mousePos.y >= 0 && mousePos.y < height )
			{
				if( mousePos.x < resizeBorderSize )
					edge |= (int)AlignInternal.left;
				if( mousePos.x > width - resizeBorderSize )
					edge |= (int)AlignInternal.right;
				if( mousePos.y < resizeBorderSize )
					edge |= (int)AlignInternal.top;
				if( mousePos.y > height - resizeBorderSize )
					edge |= (int)AlignInternal.bottom;

				int tempResizeBorderSize = resizeBorderSize;
				if( edge != 0 )
					tempResizeBorderSize += 25;
				if( mousePos.x < tempResizeBorderSize )
					edge |= (int)AlignInternal.left;
				if( mousePos.x > width - tempResizeBorderSize )
					edge |= (int)AlignInternal.right;
				if( mousePos.y < tempResizeBorderSize )
					edge |= (int)AlignInternal.top;
				if( mousePos.y > height - tempResizeBorderSize )
					edge |= (int)AlignInternal.bottom;
			}

			if( _isMovable && edge == 0 && mousePos.y >= 0 && mousePos.y <= getPadTop() && mousePos.x >= 0 && mousePos.x <= width )
				edge = MOVE;
			
			_dragging = edge != 0;

			startX = mousePos.x;
			startY = mousePos.y;
			lastX = mousePos.x;
			lastY = mousePos.y;

			return true;
		}


		void IInputListener.onMouseMoved( vec2 mousePos )
		{
			if( !_dragging )
				return;
			
			float width = getWidth(), height = getHeight();
			float windowX = getX(), windowY = getY();

			var stage = getStage();
			var parentWidth = stage.getWidth();
			var parentHeight = stage.getHeight();

			var clampPosition = _keepWithinStage && getParent() == stage.getRoot();

			if( ( edge & MOVE ) != 0 )
			{
				float amountX = mousePos.x - startX, amountY = mousePos.y - startY;

				if( clampPosition )
				{
					if( windowX + amountX < 0 )
						amountX = -windowX;
					if( windowY + amountY < 0 )
						amountY = -windowY;
					if( windowX + width + amountX > parentWidth )
						amountX = parentWidth - windowX - width;
					if( windowY + height + amountY > parentHeight )
						amountY = parentHeight - windowY - height;
				}

				windowX += amountX;
				windowY += amountY;
			}
			if( ( edge & (int)AlignInternal.left ) != 0 )
			{
				float amountX = mousePos.x - startX;
				if( width - amountX < minWidth )
					amountX = -( minWidth - width );
				if( clampPosition && windowX + amountX < 0 )
					amountX = -windowX;
				width -= amountX;
				windowX += amountX;
			}
			if( ( edge & (int)AlignInternal.top ) != 0 )
			{
				float amountY = mousePos.y - startY;
				if( height - amountY < minHeight )
					amountY = -( minHeight - height );
				if( clampPosition && windowY + amountY < 0 )
					amountY = -windowY;
				height -= amountY;
				windowY += amountY;
			}
			if( ( edge & (int)AlignInternal.right ) != 0 )
			{
				float amountX = mousePos.x - lastX;
				if( width + amountX < minWidth )
					amountX = minWidth - width;
				if( clampPosition && windowX + width + amountX > parentWidth )
					amountX = parentWidth - windowX - width;
				width += amountX;
			}
			if( ( edge & (int)AlignInternal.bottom ) != 0 )
			{
				float amountY = mousePos.y - lastY;
				if( height + amountY < minHeight )
					amountY = minHeight - height;
				if( clampPosition && windowY + height + amountY > parentHeight )
					amountY = parentHeight - windowY - height;
				height += amountY;
			}

			lastX = mousePos.x;
			lastY = mousePos.y;
			setBounds( Mathf.round( windowX ), Mathf.round( windowY ), Mathf.round( width ), Mathf.round( height ) );
		}


		void IInputListener.onMouseUp( vec2 mousePos )
		{
			_dragging = false;
		}


		bool IInputListener.onMouseScrolled( int mouseWheelDelta )
		{
			return false;
		}

		#endregion


		public Window setStyle( WindowStyle style )
		{
			this.style = style;
			setBackground( style.background );

			var labelStyle = titleLabel.getStyle();
			labelStyle.font = style.titleFont ?? labelStyle.font;
			labelStyle.fontColor = style.titleFontColor;
			titleLabel.setStyle( labelStyle );

			invalidateHierarchy();
			return this;
		}


		/// <summary>
		/// Returns the window's style. Modifying the returned style may not have an effect until {@link #setStyle(WindowStyle)} is called
		/// </summary>
		/// <returns>The style.</returns>
		public WindowStyle getStyle()
		{
			return style;
		}


		public void keepWithinStage()
		{
			if( !_keepWithinStage )
				return;

			var stage = getStage();
			var parentWidth = stage.getWidth();
			var parentHeight = stage.getHeight();

			if( x < 0 )
				x = 0;
			if( y < 0 )
				y = 0;
			if( getY( AlignInternal.bottom ) > parentHeight )
				y = parentHeight - height;
			if( getX( AlignInternal.right ) > parentWidth )
				x = parentWidth - width;
		}


		public override void draw( Graphics graphics, float parentAlpha )
		{
            keepWithinStage();

			if( style.stageBackground != null )
			{
				var stagePos = stageToLocalCoordinates( vec2.Zero );
				var stageSize = stageToLocalCoordinates( new vec2( stage.getWidth(), stage.getHeight() ) );
				drawStageBackground( graphics, parentAlpha, getX() + stagePos.x, getY() + stagePos.y, getX() + stageSize.x, getY() + stageSize.y );
			}

			base.draw( graphics, parentAlpha );
		}


		protected void drawStageBackground( Graphics graphics, float parentAlpha, float x, float y, float width, float height )
		{
			style.stageBackground.draw( graphics, x, y, width, height, new Color( color, (int)(color.A * parentAlpha) ) );
		}


		protected override void drawBackground( Graphics graphics, float parentAlpha, float x, float y )
		{
			base.drawBackground( graphics, parentAlpha, x, y );

			// Manually draw the title table before clipping is done.
			titleTable.color.A = color.A;
			float padTop = getPadTop(), padLeft = getPadLeft();
			titleTable.setSize( getWidth() - padLeft - getPadRight(), padTop );
			titleTable.setPosition( padLeft, 0 );
		}


		public override Element hit( vec2 point )
		{
			var hit = base.hit( point );
			if( hit == null || hit == this )
				return hit;

			if( point.y >= 0 && point.y <= getPadTop() && point.x >= 0 && point.x <= width )
			{
				// Hit the title bar, don't use the hit child if it is in the Window's table.
				Element current = hit;
				while( current.getParent() != this )
					current = current.getParent();

				if( getCell( current ) != null )
					return this;
			}
			return hit;
		}
        

		public bool isMovable()
		{
			return _isMovable;
		}


		public Window setMovable( bool isMovable )
		{
			_isMovable = isMovable;
			return this;
		}


		public Window setKeepWithinStage( bool keepWithinStage )
		{
			_keepWithinStage = keepWithinStage;
			return this;
		}


		public bool isResizable()
		{
			return _isResizable;
		}


		public Window setResizable( bool isResizable )
		{
			_isResizable = isResizable;
			return this;
		}


		public Window setResizeBorderSize( int resizeBorderSize )
		{
			this.resizeBorderSize = resizeBorderSize;
			return this;
		}


		public bool isDragging()
		{
			return _dragging;
		}


		public float getPrefWidth()
		{
			return Math.Max( base.preferredWidth, titleLabel.preferredWidth + getPadLeft() + getPadRight() );
		}


		public Table getTitleTable()
		{
			return titleTable;
		}


		public Label getTitleLabel()
		{
			return titleLabel;
		}

	}


	public class WindowStyle
	{
		public BitmapFont titleFont;
		/** Optional. */
		public IDrawable background;
		/** Optional. */
		public Color titleFontColor = Color.White;
		/** Optional. */
		public IDrawable stageBackground;


		public WindowStyle()
		{
			titleFont = Graphics.instance.bitmapFont;
		}


		public WindowStyle( BitmapFont titleFont, Color titleFontColor, IDrawable background )
		{
			this.titleFont = titleFont ?? Graphics.instance.bitmapFont;
			this.background = background;
			this.titleFontColor = titleFontColor;
		}


		public WindowStyle clone()
		{
			return new WindowStyle {
				background = background,
				titleFont = titleFont,
				titleFontColor = titleFontColor,
				stageBackground = stageBackground
			};
		}
	}
}

