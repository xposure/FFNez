using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Nez.Tiled
{
	public class TiledObject
	{
		public enum TiledObjectType
		{
			None,
			Ellipse,
			Image,
			Polygon,
			Polyline
		}

		public int gid;
		public string name;
		public string type;
		public int x;
		public int y;
		public int width;
		public int height;
		public int rotation;
		public bool visible;
		public TiledObjectType tiledObjectType;
		public string objectType;
		public vec2[] polyPoints;
		public Dictionary<string,string> properties = new Dictionary<string,string>();
		
		/// <summary>
		/// wraps the x/y fields in a Vector
		/// </summary>
	        public vec2 position
	        {
	            get { return new vec2( x, y ); }
	            set { x = (int)value.x; y = (int)value.y; }
	        }
	}
}

