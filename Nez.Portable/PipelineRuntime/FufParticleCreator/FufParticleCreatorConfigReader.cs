using Microsoft.Xna.Framework.Content;
using Nez.ECS.Components.Renderables.Particles;

namespace Nez.PipelineRuntime.FufParticleCreator
{
    public class FufParticleCreatorConfigReader : ContentTypeReader<FufParticleCreatorConfig>
    {
        protected override FufParticleCreatorConfig Read(ContentReader reader, FufParticleCreatorConfig existingInstance)
        {
            var emitterConfig = new FufParticleCreatorConfig();

            emitterConfig.angle = reader.ReadSingle();

            return emitterConfig;
        }
    }
}