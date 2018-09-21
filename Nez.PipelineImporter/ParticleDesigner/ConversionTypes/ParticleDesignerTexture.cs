using System;
using System.Xml.Serialization;


namespace Atma.ParticleDesignerImporter
{
	public class ParticleDesignerTexture
	{
		[XmlAttribute]
		public string name;

		[XmlAttribute]
		public string data;
	}
}

