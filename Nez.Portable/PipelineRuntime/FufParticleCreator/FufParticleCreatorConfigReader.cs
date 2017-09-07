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

            emitterConfig.Color.startMin = reader.ReadColor();
            emitterConfig.Color.startMax = reader.ReadColor();
            emitterConfig.Color.endMin = reader.ReadColor();
            emitterConfig.Color.endMax = reader.ReadColor();

            emitterConfig.Alpha.startMin = reader.ReadSingle();
            emitterConfig.Alpha.startMax = reader.ReadSingle();
            emitterConfig.Alpha.endMin = reader.ReadSingle();
            emitterConfig.Alpha.endMax = reader.ReadSingle();

            emitterConfig.Scale.startMin = reader.ReadSingle();
            emitterConfig.Scale.startMax = reader.ReadSingle();
            emitterConfig.Scale.endMin = reader.ReadSingle();
            emitterConfig.Scale.endMax = reader.ReadSingle();

            emitterConfig.ParticleAngle.min = reader.ReadSingle();
            emitterConfig.ParticleAngle.max = reader.ReadSingle();

            return emitterConfig;
        }
    }
}