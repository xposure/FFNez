


namespace Nez
{
	public static class Vector3Ext
	{
		/// <summary>
		/// returns a vec2 ignoring the z component
		/// </summary>
		/// <returns>The vector2.</returns>
		/// <param name="vec">Vec.</param>
		public static vec2 toVector2( this vec3 vec )
		{
			return new vec2( vec.X, vec.Y );
		}

	}
}

