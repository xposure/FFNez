using System;



namespace Nez.UI
{
	public enum Scaling
	{
		/// <summary>
		/// Scales the source to fit the target while keeping the same aspect ratio. This may cause the source to be smaller than the
		/// target in one direction
		/// </summary>
		Fit,

		/// <summary>
		/// Scales the source to fill the target while keeping the same aspect ratio. This may cause the source to be larger than the
		/// target in one direction.
		/// </summary>
		Fill,

		/// <summary>
		/// Scales the source to fill the target in the x direction while keeping the same aspect ratio. This may cause the source to be
		/// smaller or larger than the target in the y direction.
		/// </summary>
		FillX,

		/// <summary>
		/// Scales the source to fill the target in the y direction while keeping the same aspect ratio. This may cause the source to be
		/// smaller or larger than the target in the x direction.
		/// </summary>
		FillY,

		/// <summary>
		/// Scales the source to fill the target. This may cause the source to not keep the same aspect ratio.
		/// </summary>
		Stretch,

		/// <summary>
		/// Scales the source to fill the target in the x direction, without changing the y direction. This may cause the source to not
		/// keep the same aspect ratio.
		/// </summary>
		StretchX,

		/// <summary>
		/// Scales the source to fill the target in the y direction, without changing the x direction. This may cause the source to not
		/// keep the same aspect ratio.
		/// </summary>
		StretchY,

		/// <summary>
		/// The source is not scaled.
		/// </summary>
		None
	}


	public static class ScalingExt
	{
		/// <summary>
		/// Returns the size of the source scaled to the target
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="sourceWidth">Source width.</param>
		/// <param name="sourceHeight">Source height.</param>
		/// <param name="targetWidth">Target width.</param>
		/// <param name="targetHeight">Target height.</param>
		public static vec2 apply( this Scaling self, float sourceWidth, float sourceHeight, float targetWidth, float targetHeight )
		{
			var temp = new vec2();
			switch ( self )
			{
				case Scaling.Fit:
				{
					var targetRatio = targetHeight / targetWidth;
					var sourceRatio = sourceHeight / sourceWidth;
					var scale = targetRatio > sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
					temp.x = sourceWidth * scale;
					temp.y = sourceHeight * scale;
					break;
				}
				case Scaling.Fill:
				{
					var targetRatio = targetHeight / targetWidth;
					var sourceRatio = sourceHeight / sourceWidth;
					var scale = targetRatio < sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
					temp.x = sourceWidth * scale;
					temp.y = sourceHeight * scale;
					break;
				}
				case Scaling.FillX:
				{
					var scale = targetWidth / sourceWidth;
					temp.x = sourceWidth * scale;
					temp.y = sourceHeight * scale;
					break;
				}
				case Scaling.FillY:
				{
					var scale = targetHeight / sourceHeight;
					temp.x = sourceWidth * scale;
					temp.y = sourceHeight * scale;
					break;
				}
				case Scaling.Stretch:
					temp.x = targetWidth;
					temp.y = targetHeight;
				break;
				case Scaling.StretchX:
					temp.x = targetWidth;
					temp.y = sourceHeight;
				break;
				case Scaling.StretchY:
					temp.x = sourceWidth;
					temp.y = targetHeight;
				break;
				case Scaling.None:
					temp.x = sourceWidth;
					temp.y = sourceHeight;
				break;
			}

			return temp;
		}
	}
}

