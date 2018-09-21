#if FEATURE_ESC
using Microsoft.Xna.Framework;


namespace Atma
{
	public class Cube3D : GeometricPrimitive3D
	{
		public Cube3D()
		{
			vec3[] normals =
			{
				new vec3( 0, 0, 1 ),
				new vec3( 0, 0, -1 ),
				new vec3( 1, 0, 0 ),
				new vec3( -1, 0, 0 ),
				new vec3( 0, 1, 0 ),
				new vec3( 0, -1, 0 ),
			};

			Color[] colors =
			{
				Color.Red,
				Color.Yellow,
				Color.Blue,
				Color.Violet,
				Color.Green,
				Color.Orange
			};

			for( var i = 0; i < normals.Length; i++ )
			{
				var vertColor = colors[i];
				var normal = normals[i];
				var side1 = new vec3( normal.Y, normal.Z, normal.X );
				var side2 = vec3.Cross( normal, side1 );

				addIndex( _vertices.Count + 0 );
				addIndex( _vertices.Count + 1 );
				addIndex( _vertices.Count + 2 );

				addIndex( _vertices.Count + 0 );
				addIndex( _vertices.Count + 2 );
               	addIndex( _vertices.Count + 3 );

				addVertex( ( normal - side1 - side2 ) / 2, vertColor, normal );
				addVertex( ( normal - side1 + side2 ) / 2, vertColor, normal );
				addVertex( ( normal + side1 + side2 ) / 2, vertColor, normal );
				addVertex( ( normal + side1 - side2 ) / 2, vertColor, normal );
			}

			initializePrimitive();
		}
	}
}
#endif
