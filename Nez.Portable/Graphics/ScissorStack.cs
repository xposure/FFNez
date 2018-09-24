using System;
using System.Collections.Generic;



namespace Nez
{
	/// <summary>
	/// A stack of Rectangle objects to be used for clipping via GraphicsDevice.ScissorRectangle. When a new
	/// Rectangle is pushed onto the stack, it will be merged with the current top of stack.The minimum area of overlap is then set as
	/// the real top of the stack.
	/// </summary>
	public static class ScissorStack
	{
		static Stack<Rectangle> _scissors = new Stack<Rectangle>();


		public static bool pushScissors( Rectangle scissor )
		{
			if( _scissors.Count > 0 )
			{
				// merge scissors
				var parent = _scissors.Peek();
				var minX = (int)Math.Max( parent.X, scissor.X );
				var maxX = (int)Math.Min( parent.X + parent.Width, scissor.X + scissor.Width );
				if( maxX - minX < 1 )
					return false;

				var minY = (int)Math.Max( parent.Y, scissor.Y );
				var maxY = (int)Math.Min( parent.Y + parent.Height, scissor.Y + scissor.Height );
				if( maxY - minY < 1 )
					return false;

				scissor.X = minX;
				scissor.Y = minY;
				scissor.Width = maxX - minX;
				scissor.Height =(int) Math.Max( 1, maxY - minY );
			}

			_scissors.Push( scissor );
			Core.graphicsDevice.ScissorRectangle = scissor;

			return true;
		}


		/// <summary>
		/// Pops the current scissor rectangle from the stack and sets the new scissor area to the new top of stack rectangle.
		/// Any drawing should be flushed before popping scissors.
		/// </summary>
		/// <returns>The scissors.</returns>
		public static Rectangle popScissors()
		{
			var scissors = _scissors.Pop();

			// reset the ScissorRectangle to the viewport bounds
			if( _scissors.Count == 0 )
				Core.graphicsDevice.ScissorRectangle = Core.graphicsDevice.Viewport.Bounds;
			else
				Core.graphicsDevice.ScissorRectangle = _scissors.Peek();
			
			return scissors;
		}


		/// <summary>
		/// Calculates a screen space scissor rectangle using the given Camera. If the Camera is null than the scissor will
		/// be calculated only with the batchTransform
		/// </summary>
		/// <returns>The scissors.</returns>
		/// <param name="camera">Camera.</param>
		/// <param name="batchTransform">Batch transform.</param>
		/// <param name="scissor">Area.</param>
		public static Rectangle calculateScissors( Camera camera, mat4 batchTransform, Rectangle scissor )
		{
			// convert the top-left point to screen space
			var tmp = new vec2( scissor.X, scissor.Y );
			tmp = vec2.Transform( tmp, batchTransform );

			if( camera != null )
				tmp = camera.worldToScreenPoint( tmp );

			var newScissor = new Rectangle();
			newScissor.X = (int)tmp.x;
			newScissor.Y = (int)tmp.y;

			// convert the bottom-right point to screen space
			tmp.x = scissor.X + scissor.Width;
			tmp.y = scissor.Y + scissor.Height;
			tmp = vec2.Transform( tmp, batchTransform );

			if( camera != null )
				tmp = camera.worldToScreenPoint( tmp );
			newScissor.Width = (int)tmp.x - newScissor.X;
			newScissor.Height = (int)tmp.y - newScissor.Y;

			return newScissor;
		}


		/// <summary>
		/// Calculates a screen space scissor rectangle using the given Camera. If the Camera is null than the scissor will
		/// be calculated only with the batchTransform
		/// </summary>
		/// <returns>The scissors.</returns>
		/// <param name="camera">Camera.</param>
		/// <param name="batchTransform">Batch transform.</param>
		/// <param name="scissor">Area.</param>
		public static Rectangle calculateScissors( Camera camera, Matrix2D batchTransform, Rectangle scissor )
		{
			// convert the top-left point to screen space
			var tmp = new vec2( scissor.X, scissor.Y );
			tmp = vec2.Transform( tmp, batchTransform );

			if( camera != null )
				tmp = camera.worldToScreenPoint( tmp );

			var newScissor = new Rectangle();
			newScissor.X = (int)tmp.x;
			newScissor.Y = (int)tmp.y;

			// convert the bottom-right point to screen space
			tmp.x = scissor.X + scissor.Width;
			tmp.y = scissor.Y + scissor.Height;
			tmp = vec2.Transform( tmp, batchTransform );

			if( camera != null )
				tmp = camera.worldToScreenPoint( tmp );
			newScissor.Width = (int)tmp.x - newScissor.X;
			newScissor.Height = (int)tmp.y - newScissor.Y;

			return newScissor;
		}

	}
}

