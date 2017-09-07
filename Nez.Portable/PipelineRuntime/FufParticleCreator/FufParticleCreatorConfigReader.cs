using Microsoft.Xna.Framework.Content;
using Nez.ECS.Components.Renderables.Particles;

namespace Nez.PipelineRuntime.FufParticleCreator
{
    public class FufParticleCreatorConfigReader : ContentTypeReader<FufParticleCreatorConfig>
    {
        protected override FufParticleCreatorConfig Read(ContentReader reader,
            FufParticleCreatorConfig existingInstance)
        {
            var emitterConfig = new FufParticleCreatorConfig();

            emitterConfig.LaunchAngle.min = reader.ReadSingle();
            emitterConfig.LaunchAngle.max = reader.ReadSingle();

            emitterConfig.Speed.min = reader.ReadSingle();
            emitterConfig.Speed.max = reader.ReadSingle();

            emitterConfig.Life.min = reader.ReadSingle();
            emitterConfig.Life.max = reader.ReadSingle();

            emitterConfig.Color.min = reader.ReadColor();
            emitterConfig.Color.max = reader.ReadColor();

            emitterConfig.Alpha.min = reader.ReadSingle();
            emitterConfig.Alpha.max = reader.ReadSingle();

            emitterConfig.Scale.min = reader.ReadVector2();
            emitterConfig.Scale.max = reader.ReadVector2();
            emitterConfig.KeepScaleRatio = reader.ReadBoolean();

            emitterConfig.ParticleAngle.min = reader.ReadSingle();
            emitterConfig.ParticleAngle.max = reader.ReadSingle();

            return emitterConfig;
        }
    }
}