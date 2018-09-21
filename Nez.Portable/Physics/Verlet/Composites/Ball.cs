#if FEATURE_PHYSICS
using Microsoft.Xna.Framework;


namespace Atma.Verlet
{
	/// <summary>
	/// single Particle composite
	/// </summary>
	public class Ball : Composite
	{
		public Ball( vec2 position, float radius = 10 )
		{
			addParticle( new Particle( position ) ).radius = radius;
		}
	}
}
#endif
