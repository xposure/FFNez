using System;
using Microsoft.Xna.Framework;


namespace Nez
{
	/// <summary>
	/// while technically not a ray (rays are just start and direction) it does double duty as both a line and a ray.
	/// </summary>
	public struct Ray2D
	{
		public vec2 start;
		public vec2 end;
		public vec2 direction;

		
		public Ray2D( vec2 position, vec2 end )
		{
			this.start = position;
			this.end = end;
			direction = end - start;
		}
	}
}

