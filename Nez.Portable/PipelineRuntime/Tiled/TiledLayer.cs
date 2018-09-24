using System.Collections.Generic;



namespace Nez.Tiled
{
	public abstract class TiledLayer
	{
		public vec2 offset;
		public string name;
		public Dictionary<string,string> properties;
		public bool visible = true;
		public float opacity;


		protected TiledLayer( string name )
		{
			this.name = name;
			properties = new Dictionary<string,string>();
		}


		public abstract void draw( Batcher batcher, vec2 position, float layerDepth, RectangleF cameraClipBounds );

	}
}