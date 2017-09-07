using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Nez.PipelineImporter.FufParticle
{
    public class FufParticleCreatorEmitterConfig
    {
        [JsonProperty("texture")]
        public string Texture { get; set; }
        
        [JsonProperty("launchAngle")]
        public ValueBounds<float> LaunchAngle { get; set; } = new ValueBounds<float>(270);

        [JsonProperty("particleAngle")]
        public ValueBounds<float> ParticleAngle { get; set; } = new ValueBounds<float>(0);

        [JsonProperty("color")]
        public ValueBounds<Color> Color { get; set; } = new ValueBounds<Color>(Microsoft.Xna.Framework.Color.White);

        [JsonProperty("alpha")]
        public ValueBounds<float> Alpha { get; set; } = new ValueBounds<float>(1.0f);
        
        [JsonProperty("life")]
        public ValueBounds<float> Life { get; set; } = new ValueBounds<float>(1.0f);
        
        [JsonProperty("scale")]
        public ValueBounds<Vector2> Scale { get; set; } = new ValueBounds<Vector2>(new Vector2(1.0f));
        
        [JsonProperty("speed")]
        public ValueBounds<float> Speed { get; set; } = new ValueBounds<float>(100f);
        
        [JsonProperty("keepScaleRatio")]
        public bool KeepScaleRatio { get; set; }
    }
}