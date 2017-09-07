using Microsoft.Xna.Framework;
using Nez.Textures;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticleCreatorConfig
    {
        public Subtexture Subtexture { get; }
        
        public ValueBounds<float> LaunchAngle { get; }

        public ValueBounds<float> ParticleAngle { get; }

        public ValueBounds<Color> Color { get; }

        public ValueBounds<float> Alpha { get; }

        public ValueBounds<float> Life { get; }

        public ValueBounds<Vector2> Scale { get; }

        public ValueBounds<float> Speed { get; }

        public bool KeepScaleRatio { get; set; }
    }
}