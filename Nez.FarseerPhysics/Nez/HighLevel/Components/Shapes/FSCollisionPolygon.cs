using System.Collections.Generic;
using Nez.Collision.Shapes;
using Nez.Common;
using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public class FSCollisionPolygon : FSCollisionShape
	{
		/// <summary>
		/// verts are stored in sim units
		/// </summary>
		protected Vertices _verts;
		vec2 _center;
		protected bool _areVertsDirty = true;


		public FSCollisionPolygon()
		{
			_fixtureDef.shape = new PolygonShape();
		}


		public FSCollisionPolygon( List<vec2> vertices ) : this()
		{
			_verts = new Vertices( vertices );
			_verts.scale( new vec2( FSConvert.displayToSim ) );
		}


		public FSCollisionPolygon( vec2[] vertices ) : this()
		{
			_verts = new Vertices( vertices );
			_verts.scale( new vec2( FSConvert.displayToSim ) );
		}


		#region Configuration

		public FSCollisionPolygon setVertices( Vertices vertices )
		{
			_verts = new Vertices( vertices );
			_areVertsDirty = true;
			recreateFixture();
			return this;
		}


		public FSCollisionPolygon setVertices( List<vec2> vertices )
		{
			_verts = new Vertices( vertices );
			_areVertsDirty = true;
			recreateFixture();
			return this;
		}


		public FSCollisionPolygon setCenter( vec2 center )
		{
			_center = center;
			_areVertsDirty = true;
			recreateFixture();
			return this;
		}

		#endregion


		public override void onAddedToEntity()
		{
			updateVerts();
			createFixture();
		}


		public override void onEntityTransformChanged( Transform.Component comp )
		{
			if( comp == Transform.Component.Scale )
				recreateFixture();
		}


		internal override void createFixture()
		{
			updateVerts();
			base.createFixture();
		}


		protected void recreateFixture()
		{
			destroyFixture();
			updateVerts();
			createFixture();
		}


		protected void updateVerts()
		{
			Assert.isNotNull( _verts, "verts cannot be null!" );

			if( !_areVertsDirty )
				return;
			_areVertsDirty = false;

			var shapeVerts = ( _fixtureDef.shape as PolygonShape ).vertices;
			shapeVerts.attachedToBody = false;

			shapeVerts.Clear();
			shapeVerts.AddRange( _verts );
			shapeVerts.scale( transform.scale );
			shapeVerts.translate( ref _center );

			( _fixtureDef.shape as PolygonShape ).setVerticesNoCopy( shapeVerts );
		}

	}
}
