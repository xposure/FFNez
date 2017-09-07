using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nez.ECS.Components.Renderables.Particles;
using Nez.Textures;

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

            emitterConfig.Color.start.min = reader.ReadColor();
            emitterConfig.Color.start.max = reader.ReadColor();
            emitterConfig.Color.end.min = reader.ReadColor();
            emitterConfig.Color.end.max = reader.ReadColor();

            emitterConfig.Alpha.start.min = reader.ReadSingle();
            emitterConfig.Alpha.start.max = reader.ReadSingle();
            emitterConfig.Alpha.end.min = reader.ReadSingle();
            emitterConfig.Alpha.end.max = reader.ReadSingle();

            emitterConfig.Scale.start.min = reader.ReadSingle();
            emitterConfig.Scale.start.max = reader.ReadSingle();
            emitterConfig.Scale.end.min = reader.ReadSingle();
            emitterConfig.Scale.end.max = reader.ReadSingle();

            emitterConfig.ParticleAngle.min = reader.ReadSingle();
            emitterConfig.ParticleAngle.max = reader.ReadSingle();

            var texture = reader.ReadObject<Texture2D>();
            emitterConfig.Subtexture = new Subtexture(texture);

            return emitterConfig;
        }
    }
}