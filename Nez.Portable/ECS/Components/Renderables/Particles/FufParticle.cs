using Microsoft.Xna.Framework;
using Nez.PhysicsShapes;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticle
    {
        private static Circle _circleCollisionShape = new Circle( 0 );
        
        public Vector2 position;
        public Color color;
        public float rotation;
        public float scale;
    }
}