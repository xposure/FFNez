using Nez.Collision.Shapes;
using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public class FSCollisionEdge : FSCollisionShape
	{
		vec2 _vertex1 = new vec2( -0.01f, 0 );
		vec2 _vertex2 = new vec2( 0.01f, 0 );

		
		public FSCollisionEdge()
		{
			_fixtureDef.shape = new EdgeShape();
		}


		#region Configuration

		public FSCollisionEdge setVertices( vec2 vertex1, vec2 vertex2 )
		{
			_vertex1 = vertex1;
			_vertex2 = vertex2;
			recreateFixture();
			return this;
		}

		#endregion


		void recreateFixture()
		{
			destroyFixture();

			var edgeShape = _fixtureDef.shape as EdgeShape;
			edgeShape.vertex1 = (vec2)_vertex1 * transform.scale * FSConvert.displayToSim;
			edgeShape.vertex2 = (vec2)_vertex2 * transform.scale * FSConvert.displayToSim;

			createFixture();
		}

	}
}
