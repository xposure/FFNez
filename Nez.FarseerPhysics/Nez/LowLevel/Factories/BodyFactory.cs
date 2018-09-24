using System.Collections.Generic;
using Nez.Collision.Shapes;
using Nez.Common;
using Nez.Dynamics;
using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public static partial class Farseer
	{
		/// <summary>
		/// exactly the same as FarseerPhysics.Factories.BodyFactory except all units are in display/Nez space as opposed to simulation space
		/// </summary>
		public static class BodyFactory
		{
			public static Body createBody( World world, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return new Body( world, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createEdge( World world, vec2 start, vec2 end, object userData = null )
			{
				return Nez.Factories.BodyFactory.createEdge( world, FSConvert.toSimUnits( start ), FSConvert.toSimUnits( end ), userData );
			}


			public static Body createChainShape( World world, Vertices vertices, vec2 position = new vec2(), object userData = null )
			{
				for( var i = 0; i < vertices.Count; i++ )
					vertices[i] *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createChainShape( world, vertices, FSConvert.toSimUnits( position ), userData );
			}


			public static Body createLoopShape( World world, Vertices vertices, vec2 position = new vec2(), object userData = null )
			{
				for( var i = 0; i < vertices.Count; i++ )
					vertices[i] *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createLoopShape( world, vertices, FSConvert.toSimUnits( position ), userData );
			}


			public static Body createRectangle( World world, float width, float height, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createRectangle( world, FSConvert.toSimUnits( width ), FSConvert.toSimUnits( height ), density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createCircle( World world, float radius, float density, vec2 position = new vec2(), BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createCircle( world, FSConvert.toSimUnits( radius ), density, FSConvert.toSimUnits( position ), bodyType, userData );
			}


			public static Body createEllipse( World world, float xRadius, float yRadius, int edges, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createEllipse( world, FSConvert.toSimUnits( xRadius ), FSConvert.toSimUnits( yRadius ), edges, density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createPolygon( World world, Vertices vertices, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				for( var i = 0; i < vertices.Count; i++ )
					vertices[i] *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createPolygon( world, vertices, density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createCompoundPolygon( World world, List<Vertices> list, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createCompoundPolygon( world, list, density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createGear( World world, float radius, int numberOfTeeth, float tipPercentage, float toothHeight, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createGear( world, FSConvert.toSimUnits( radius ), numberOfTeeth, tipPercentage, FSConvert.toSimUnits( toothHeight ), density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createCapsule( World world, float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				height *= FSConvert.displayToSim;
				topRadius *= FSConvert.displayToSim;
				bottomRadius *= FSConvert.displayToSim;
				position *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createCapsule( world, height, topRadius, topEdges, bottomRadius, bottomEdges, density, position, rotation, bodyType, userData );
			}


			public static Body createCapsule( World world, float height, float endRadius, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createCapsule( world, FSConvert.toSimUnits( height ), FSConvert.toSimUnits( endRadius ), density, FSConvert.toSimUnits( position ), rotation, bodyType, userData );
			}


			public static Body createRoundedRectangle( World world, float width, float height, float xRadius, float yRadius, int segments, float density, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				width *= FSConvert.displayToSim;
				height *= FSConvert.displayToSim;
				xRadius *= FSConvert.displayToSim;
				yRadius *= FSConvert.displayToSim;
				position *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createRoundedRectangle( world, width, height, xRadius, yRadius, segments, density, position, rotation, bodyType, userData );
			}


			public static Body createLineArc( World world, float radians, int sides, float radius, bool closed = false, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				var body = Nez.Factories.BodyFactory.createLineArc( world, radians, sides, FSConvert.toSimUnits( radius ), closed, FSConvert.toSimUnits( position ), rotation, bodyType );
				body.userData = userData;
				return body;
			}


			public static Body createSolidArc( World world, float density, float radians, int sides, float radius, vec2 position = new vec2(), float rotation = 0, BodyType bodyType = BodyType.Static, object userData = null )
			{
				return Nez.Factories.BodyFactory.createSolidArc( world, density, radians, sides, FSConvert.toSimUnits( radius ), FSConvert.toSimUnits( position ), rotation, bodyType );
			}


			public static BreakableBody createBreakableBody( World world, Vertices vertices, float density, vec2 position = new vec2(), float rotation = 0 )
			{
				for( var i = 0; i < vertices.Count; i++ )
					vertices[i] *= FSConvert.displayToSim;

				return Nez.Factories.BodyFactory.createBreakableBody( world, vertices, density, FSConvert.toSimUnits( position ), rotation );
			}


			public static BreakableBody createBreakableBody( World world, IEnumerable<Shape> shapes, vec2 position = new vec2(), float rotation = 0 )
			{
				return Nez.Factories.BodyFactory.createBreakableBody( world, shapes, FSConvert.toSimUnits( position ), rotation );
			}
		
		}
	}
}