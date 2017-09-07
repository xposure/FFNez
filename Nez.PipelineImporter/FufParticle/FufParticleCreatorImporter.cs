using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace Nez.PipelineImporter.FufParticle
{
    [ContentImporter(".fpx", DefaultProcessor = nameof(FufParticleCreatorProcessor),
        DisplayName = "Fuf Particle Importer")]
    public class FufParticleCreatorImporter : ContentImporter<FufParticleCreatorContent>
    {
        public override FufParticleCreatorContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"\nImporting FufParticle config file: {filename}");

            using (var streamReader = new StreamReader(filename))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var deserializer = new JsonSerializer();
                var emitterConfig = deserializer.Deserialize<FufParticleCreatorEmitterConfig>(jsonTextReader);
                
                return new FufParticleCreatorContent(context, emitterConfig, filename);
            }
        }
    }
}