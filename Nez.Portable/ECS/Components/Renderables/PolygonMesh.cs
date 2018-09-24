using Microsoft.Xna.Framework;


namespace Nez
{
	/// <summary>
	/// renders a basic, CCW, convex polygon
	/// </summary>
	public class PolygonMesh : Mesh
	{
		public PolygonMesh( vec2[] points, bool arePointsCCW = true )
		{
			var triangulator = new Triangulator();
			triangulator.triangulate( points, arePointsCCW );

			setVertPositions( points );
			setTriangles( triangulator.triangleIndices.ToArray() );
			recalculateBounds( true );
		}
	}
}
