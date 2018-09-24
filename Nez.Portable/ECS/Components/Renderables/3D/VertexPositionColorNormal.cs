using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public struct VertexPositionColorNormal : IVertexType
	{
		public vec3 position;
		public Color color;
		public vec3 normal;


		static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration
		(
			new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
			new VertexElement( 12, VertexElementFormat.Color, VertexElementUsage.Color, 0 ),
			new VertexElement( 16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 )
		);

		VertexDeclaration IVertexType.VertexDeclaration { get { return _vertexDeclaration; } }


		public VertexPositionColorNormal( vec3 position, Color color, vec3 normal )
		{
			this.position = position;
			this.color = color;
			this.normal = normal;
		}




	}
}
