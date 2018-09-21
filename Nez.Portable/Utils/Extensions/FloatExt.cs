#if FEATURE_UTILS
namespace Atma
{
	public static class FloatExt
	{
		public static bool approximately( this float self, float other )
		{
			return Mathf.approximately( self, other );
		}
	}
}

#endif
