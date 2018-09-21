#if FEATURE_ESC
using Microsoft.Xna.Framework;
using Atma.Textures;

namespace Atma.ECS.Components.Renderables.Particles
{
    public class FufParticleCreatorConfig
    {
        public Subtexture subtexture { get; set; }

        public FloatValueBounds launchAngle { get; } = new FloatValueBounds(270);

        public FloatValueBounds particleRotation { get; } = new FloatValueBounds(0);

        public StartEndColorValueBounds color { get; } = new StartEndColorValueBounds(Microsoft.Xna.Framework.Color.White);

        public StartEndFloatValueBounds alpha { get; } = new StartEndFloatValueBounds(1f);

        public FloatValueBounds life { get; } = new FloatValueBounds(1f);

        public StartEndFloatValueBounds scale { get; } = new StartEndFloatValueBounds(1f);

        public Vector2ValueBounds offset { get; } = new Vector2ValueBounds(vec2.Zero);

        public FloatValueBounds speed { get; } = new FloatValueBounds(1f);
    }
}
#endif
