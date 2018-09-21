#if FEATURE_PIPELINE
using System;


namespace Atma.Tiled
{
	public class TiledImageTile : TiledTile
	{
		public new TiledTilesetTile tilesetTile;
		public string imageSource;


		public TiledImageTile( int id, TiledTilesetTile tilesetTile, string imageSource ) : base( id )
		{
			this.tilesetTile = tilesetTile;
			this.imageSource = imageSource;
		}
	}
}

#endif
