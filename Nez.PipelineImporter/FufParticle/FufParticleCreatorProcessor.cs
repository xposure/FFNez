using Microsoft.Xna.Framework.Content.Pipeline;

namespace Nez.PipelineImporter.FufParticle
{
    [ContentProcessor(DisplayName = "Fuf Particle Importer")]
    public class FufParticleCreatorProcessor : ContentProcessor<FufParticleCreatorContent, FufParticleEmitterProcessorResult>
    {
        public static ContentBuildLogger logger;

        public override FufParticleEmitterProcessorResult Process(FufParticleCreatorContent input,
            ContentProcessorContext context)
        {
            logger = context.Logger;
            var result = new FufParticleEmitterProcessorResult();
            
            // ...Process additional data

            result.emitterConfig = input.emitterConfig;
            
            return result;
        }
    }
}