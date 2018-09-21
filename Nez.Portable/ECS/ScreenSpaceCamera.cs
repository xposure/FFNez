#if FEATURE_ESC
using System;


namespace Atma
{
	public class ScreenSpaceCamera : Camera
	{
		/// <summary>
		/// we are screen space, so our matrixes should always be identity
		/// </summary>
		protected override void updateMatrixes()
		{}
	}
}

#endif
