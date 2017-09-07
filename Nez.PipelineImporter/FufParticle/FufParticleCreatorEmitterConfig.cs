using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nez.PipelineImporter.FufParticle
{
    public class FufParticleCreatorEmitterConfig
    {
        [JsonProperty("angle")]
        public float Angle { get; } = 0;
    }
}