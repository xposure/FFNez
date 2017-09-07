using Microsoft.Xna.Framework;
using Nez.Textures;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticleCreatorConfig
    {
        public Subtexture Subtexture { get; }
        
        public ValueBounds<float> LaunchAngle { get; }

        public ValueBounds<float> ParticleAngle { get; }

        public StartEndValueBounds<Color> Color { get; }

        public StartEndValueBounds<float> Alpha { get; }

        public ValueBounds<float> Life { get; }

        public StartEndValueBounds<float> Scale { get; }

        public ValueBounds<float> Speed { get; }
    }
}