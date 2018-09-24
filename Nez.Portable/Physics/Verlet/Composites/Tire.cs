


namespace Nez.Verlet
{
	public class Tire : Composite
	{
		public Tire( vec2 origin, float radius, int segments, float spokeStiffness = 1, float treadStiffness = 1 )
		{
			var stride = 2 * glm.PI / segments;

			// particles
			for( var i = 0; i < segments; i++ )
			{
				var theta = i * stride;
				addParticle( new Particle( new vec2( origin.x + Mathf.cos( theta ) * radius, origin.y + Mathf.sin( theta ) * radius ) ) );
			}

			var centerParticle = addParticle( new Particle( origin ) );

			// constraints
			for( var i = 0; i < segments; i++ )
			{
				addConstraint( new DistanceConstraint( particles[i], particles[( i + 1 ) % segments], treadStiffness ) );
				addConstraint( new DistanceConstraint( particles[i], centerParticle, spokeStiffness ) )
					.setCollidesWithColliders( false );
				addConstraint( new DistanceConstraint( particles[i], particles[( i + 5 ) % segments], treadStiffness ) );
			}
		}
	}
}
