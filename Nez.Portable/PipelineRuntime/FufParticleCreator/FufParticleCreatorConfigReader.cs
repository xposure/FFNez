#if FEATURE_PIPELINE
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Atma.ECS.Components.Renderables.Particles;
using Atma.Textures;

namespace Atma.PipelineRuntime.FufParticleCreator
{
    public class FufParticleCreatorConfigReader : ContentTypeReader<FufParticleCreatorConfig>
    {
        protected override FufParticleCreatorConfig Read(ContentReader reader,
            FufParticleCreatorConfig existingInstance)
        {
            var emitterConfig = new FufParticleCreatorConfig();

            emitterConfig.launchAngle.min = reader.ReadSingle();
            emitterConfig.launchAngle.max = reader.ReadSingle();

            emitterConfig.speed.min = reader.ReadSingle();
            emitterConfig.speed.max = reader.ReadSingle();

            emitterConfig.life.min = reader.ReadSingle();
            emitterConfig.life.max = reader.ReadSingle();

            emitterConfig.color.start.min = new Color(reader.ReadColor().PackedValue);
            emitterConfig.color.start.max = new Color(reader.ReadColor().PackedValue);
            emitterConfig.color.end.min = new Color(reader.ReadColor().PackedValue);
            emitterConfig.color.end.max = new Color(reader.ReadColor().PackedValue);

            emitterConfig.alpha.start.min = reader.ReadSingle();
            emitterConfig.alpha.start.max = reader.ReadSingle();
            emitterConfig.alpha.end.min = reader.ReadSingle();
            emitterConfig.alpha.end.max = reader.ReadSingle();

            emitterConfig.scale.start.min = reader.ReadSingle();
            emitterConfig.scale.start.max = reader.ReadSingle();
            emitterConfig.scale.end.min = reader.ReadSingle();
            emitterConfig.scale.end.max = reader.ReadSingle();

            emitterConfig.offset.min = reader.ReadVector2();
            emitterConfig.offset.max = reader.ReadVector2();

            emitterConfig.particleRotation.min = reader.ReadSingle();
            emitterConfig.particleRotation.max = reader.ReadSingle();

            var texture = reader.ReadObject<Texture2D>();
            emitterConfig.subtexture = new Subtexture(texture);

            return emitterConfig;
        }
    }
}
#endif
