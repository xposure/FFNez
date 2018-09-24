using System.Collections.Generic;
using Nez.Collision;
using Nez.Collision.Shapes;
using Nez.Dynamics;
using Microsoft.Xna.Framework;


namespace Nez.Controllers
{
	public sealed class BuoyancyController : Controller
	{
		#region Properties/Fields

		/// <summary>
		/// Controls the rotational drag that the fluid exerts on the bodies within it. Use higher values will simulate thick fluid, like honey, lower values to
		/// simulate water-like fluids. 
		/// </summary>
		public float angularDragCoefficient;

		/// <summary>
		/// Density of the fluid. Higher values will make things more buoyant, lower values will cause things to sink.
		/// </summary>
		public float density;

		/// <summary>
		/// Controls the linear drag that the fluid exerts on the bodies within it. Use higher values will simulate thick fluid, like honey, lower values to
		/// simulate water-like fluids.
		/// </summary>
		public float linearDragCoefficient;

		/// <summary>
		/// Acts like waterflow. Defaults to 0,0.
		/// </summary>
		public vec2 velocity;

		AABB _container;

		vec2 _gravity;
		vec2 _normal;
		float _offset;
		Dictionary<int, Body> _uniqueBodies = new Dictionary<int, Body>();

		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="BuoyancyController"/> class.
		/// </summary>
		/// <param name="container">Only bodies inside this AABB will be influenced by the controller</param>
		/// <param name="density">Density of the fluid</param>
		/// <param name="linearDragCoefficient">Linear drag coefficient of the fluid</param>
		/// <param name="rotationalDragCoefficient">Rotational drag coefficient of the fluid</param>
		/// <param name="gravity">The direction gravity acts. Buoyancy force will act in opposite direction of gravity.</param>
		public BuoyancyController( AABB container, float density, float linearDragCoefficient, float rotationalDragCoefficient, vec2 gravity )
			: base( ControllerType.BuoyancyController )
		{
			this.container = container;
			_normal = new vec2( 0, 1 );
			this.density = density;
			this.linearDragCoefficient = linearDragCoefficient;
			angularDragCoefficient = rotationalDragCoefficient;
			_gravity = gravity;
		}

		public AABB container
		{
			get { return _container; }
			set
			{
				_container = value;
				_offset = _container.upperBound.y;
			}
		}

		public override void update( float dt )
		{
			_uniqueBodies.Clear();
			world.queryAABB( fixture =>
								 {
									 if( fixture.body.isStatic || !fixture.body.isAwake )
										 return true;

									 if( !_uniqueBodies.ContainsKey( fixture.body.bodyId ) )
										 _uniqueBodies.Add( fixture.body.bodyId, fixture.body );

									 return true;
								 }, ref _container );

			foreach( KeyValuePair<int, Body> kv in _uniqueBodies )
			{
				Body body = kv.Value;

				vec2 areac = vec2.Zero;
				vec2 massc = vec2.Zero;
				float area = 0;
				float mass = 0;

				for( int j = 0; j < body.fixtureList.Count; j++ )
				{
					Fixture fixture = body.fixtureList[j];

					if( fixture.shape.shapeType != ShapeType.Polygon && fixture.shape.shapeType != ShapeType.Circle )
						continue;

					Shape shape = fixture.shape;

					vec2 sc;
					float sarea = shape.computeSubmergedArea( ref _normal, _offset, ref body._xf, out sc );
					area += sarea;
					areac.x += sarea * sc.x;
					areac.y += sarea * sc.y;

					mass += sarea * shape.density;
					massc.x += sarea * sc.x * shape.density;
					massc.y += sarea * sc.y * shape.density;
				}

				areac.x /= area;
				areac.y /= area;
				massc.x /= mass;
				massc.y /= mass;

				if( area < Settings.epsilon )
					continue;

				//Buoyancy
				var buoyancyForce = -density * area * _gravity;
				body.applyForce( buoyancyForce, massc );

				//Linear drag
				var dragForce = body.getLinearVelocityFromWorldPoint( areac ) - velocity;
				dragForce *= -linearDragCoefficient * area;
				body.applyForce( dragForce, areac );

				//Angular drag
				body.applyTorque( -body.inertia / body.mass * area * body.angularVelocity * angularDragCoefficient );
			}
		}
	
	}
}