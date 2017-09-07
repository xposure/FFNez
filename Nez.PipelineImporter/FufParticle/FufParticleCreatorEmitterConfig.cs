using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Nez.PipelineImporter.FufParticle
{
    public class FufParticleCreatorEmitterConfig
    {
        [JsonProperty("texture")]
        public string TextureName { get; }
        
        [JsonProperty("launchAngle")]
        public ValueBounds<float> LaunchAngle { get; } = new ValueBounds<float>(270);

        [JsonProperty("particleAngle")]
        public ValueBounds<float> ParticleAngle { get; } = new ValueBounds<float>(0);

        [JsonProperty("color")]
        public ValueBounds<Color> Color { get; } = new ValueBounds<Color>(Microsoft.Xna.Framework.Color.White);

        [JsonProperty("alpha")]
        public ValueBounds<float> Alpha { get; } = new ValueBounds<float>(1.0f);
        
        [JsonProperty("life")]
        public ValueBounds<float> Life { get; } = new ValueBounds<float>(1.0f);
        
        [JsonProperty("scale")]
        public ValueBounds<Vector2> Scale { get; } = new ValueBounds<Vector2>(new Vector2(1.0f));
        
        [JsonProperty("speed")]
        public ValueBounds<float> Speed { get; } = new ValueBounds<float>(100f);
        
        [JsonProperty("keepScaleRatio")]
        public bool KeepScaleRatio { get; }
    }
}