using System.Collections.Generic;
using Nez.Collision.Shapes;
using Nez.Common;
using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public class FSCollisionChain : FSCollisionShape
	{
		List<vec2> _verts;
		bool _loop;


		public FSCollisionChain()
		{
			_fixtureDef.shape = new ChainShape();
		}


		public FSCollisionChain( List<vec2> verts ) : this()
		{
			_verts = verts;
		}


		public FSCollisionChain( vec2[] verts ) : this()
		{
			_verts = new List<vec2>( verts );
		}


		#region Configuration

		public FSCollisionChain setLoop( bool loop )
		{
			_loop = loop;
			recreateFixture();
			return this;
		}


		public FSCollisionChain setVertices( Vertices verts )
		{
			_verts = verts;
			recreateFixture();
			return this;
		}


		public FSCollisionChain setVertices( List<vec2> verts )
		{
			_verts = verts;
			recreateFixture();
			return this;
		}


		public FSCollisionChain setVertices( vec2[] verts )
		{
			if( _verts == null )
				_verts = new List<vec2>();

			_verts.Clear();
			_verts.AddRange( verts );
			recreateFixture();
			return this;
		}

		#endregion


		public override void onEntityTransformChanged( Transform.Component comp )
		{
			if( comp == Transform.Component.Scale )
				recreateFixture();
		}


		void recreateFixture()
		{
			Assert.isNotNull( _verts, "verts cannot be null!" );

			destroyFixture();

			// scale our verts and convert them to sim units
			var verts = new Vertices( _verts );
			verts.scale( transform.scale * FSConvert.displayToSim );

			var chainShape = _fixtureDef.shape as ChainShape;
			chainShape.setVertices( verts, _loop );

			createFixture();
		}

	}
}
