


namespace Nez.Verlet
{
	/// <summary>
	/// constrains 3 particles to an angle
	/// </summary>
	public class AngleConstraint : Constraint
	{
		/// <summary>
		/// [0-1]. the stiffness of the Constraint. Lower values are more springy and higher are more rigid.
		/// </summary>
		public float stiffness;

		/// <summary>
		/// the angle in radians that the Constraint will attempt to maintain
		/// </summary>
		public float angleInRadians;

		Particle _particleA;
		Particle _centerParticle;
		Particle _particleC;


		public AngleConstraint( Particle a, Particle center, Particle c, float stiffness )
		{
			_particleA = a;
			_centerParticle = center;
			_particleC = c;
			this.stiffness = stiffness;

			// not need for this Constraint to collide. There will be DistanceConstraints to do that if necessary
			collidesWithColliders = false;

			angleInRadians = angleBetweenParticles();
		}


		float angleBetweenParticles()
		{
			var first = _particleA.position - _centerParticle.position;
			var second = _particleC.position - _centerParticle.position;

			return Mathf.atan2( first.x * second.y - first.y * second.x, first.x * second.x + first.y * second.y );
		}


		public override void solve()
		{
			var angleBetween = angleBetweenParticles();
			var diff = angleBetween - angleInRadians;

			if( diff <= -glm.PI )
				diff += 2 * glm.PI;
			else if( diff >= glm.PI )
				diff -= 2 * glm.PI;

			diff *= stiffness;

			_particleA.position = Mathf.rotateAround( _particleA.position, _centerParticle.position, diff );
			_particleC.position = Mathf.rotateAround( _particleC.position, _centerParticle.position, -diff );
			_centerParticle.position = Mathf.rotateAround( _centerParticle.position, _particleA.position, diff );
			_centerParticle.position = Mathf.rotateAround( _centerParticle.position, _particleC.position, -diff );
		}

	}
}
