using Microsoft.Xna.Framework;
using Nez.Textures;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticleCreatorConfig
    {
        public Subtexture Subtexture { get; set; }

        public FloatValueBounds LaunchAngle { get; } = new FloatValueBounds(270);

        public FloatValueBounds ParticleAngle { get; } = new FloatValueBounds(0);

        public StartEndColorValueBounds Color { get; } = new StartEndColorValueBounds(Microsoft.Xna.Framework.Color.White);

        public StartEndFloatValueBounds Alpha { get; } = new StartEndFloatValueBounds(1f);

        public FloatValueBounds Life { get; } = new FloatValueBounds(1f);

        public StartEndFloatValueBounds Scale { get; } = new StartEndFloatValueBounds(1f);

        public FloatValueBounds Speed { get; } = new FloatValueBounds(1f);
    }
}