using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;


namespace Atma.ConversionTypes
{
    public class ConversionTypeVector2
    {
        [XmlAttribute] [JsonProperty] public float x;

        [XmlAttribute] [JsonProperty] public float y;


        public static implicit operator vec2(ConversionTypeVector2 pdvec)
        {
            return new vec2(pdvec.x, pdvec.y);
        }
    }
}