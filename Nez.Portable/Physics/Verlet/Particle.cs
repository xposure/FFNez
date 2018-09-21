#if FEATURE_PHYSICS
using Microsoft.Xna.Framework;


namespace Atma.Verlet
{
	public class Particle
	{
		/// <summary>
		/// the current position of the Particle
		/// </summary>
		public vec2 position;

		/// <summary>
		/// the position of the Particle prior to its latest move
		/// </summary>
		public vec2 lastPosition;

		/// <summary>
		/// the mass of the Particle. Taken into account for all forces and constraints
		/// </summary>
		public float mass = 1;

		/// <summary>
		/// the radius of the Particle
		/// </summary>
		public float radius;

		/// <summary>
		/// if true, the Particle will collide with standard Nez Colliders
		/// </summary>
		public bool collidesWithColliders = true;

		internal bool isPinned;
		internal vec2 acceleration;
		internal vec2 pinnedPosition;


		public Particle( vec2 position )
		{
			this.position = position;
			lastPosition = position;
		}


		public Particle( float x, float y ) : this( new vec2( x, y ) )
		{}


		/// <summary>
		/// applies a force taking mass into account to the Particle
		/// </summary>
		/// <param name="force">Force.</param>
		public void applyForce( vec2 force )
		{
			// acceleration = (1 / mass) * force
			acceleration += force / mass;
		}


		/// <summary>
		/// pins the Particle to its current position
		/// </summary>
		public Particle pin()
		{
			isPinned = true;
			pinnedPosition = position;
			return this;
		}


		/// <summary>
		/// pins the particle to the specified position
		/// </summary>
		/// <param name="position">Position.</param>
		public Particle pinTo( vec2 position )
		{
			isPinned = true;
			pinnedPosition = position;
			this.position = pinnedPosition;
			return this;
		}


		/// <summary>
		/// unpins the particle setting it free like the wind
		/// </summary>
		public Particle unpin()
		{
			isPinned = false;
			return this;
		}

	}
}
#endif
