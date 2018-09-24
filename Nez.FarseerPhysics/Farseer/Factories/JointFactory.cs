﻿using Nez.Dynamics;
using Nez.Dynamics.Joints;
using Microsoft.Xna.Framework;


namespace Nez.Factories
{
	/// <summary>
	/// An easy to use factory for using joints.
	/// </summary>
	public static class JointFactory
	{
		#region Motor Joint

		public static MotorJoint createMotorJoint( World world, Body bodyA, Body bodyB, bool useWorldCoordinates = false )
		{
			var joint = new MotorJoint( bodyA, bodyB, useWorldCoordinates );
			world.addJoint( joint );
			return joint;
		}

		#endregion

		#region Revolute Joint

		public static RevoluteJoint createRevoluteJoint( World world, Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
		{
			var joint = new RevoluteJoint( bodyA, bodyB, anchorA, anchorB, useWorldCoordinates );
			world.addJoint( joint );
			return joint;
		}

		public static RevoluteJoint createRevoluteJoint( World world, Body bodyA, Body bodyB, vec2 anchor )
		{
			var localanchorA = bodyA.getLocalPoint( bodyB.getWorldPoint( anchor ) );
			var joint = new RevoluteJoint( bodyA, bodyB, localanchorA, anchor );
			world.addJoint( joint );
			return joint;
		}


		#endregion

		#region Rope Joint

		public static RopeJoint createRopeJoint( World world, Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
		{
			var ropeJoint = new RopeJoint( bodyA, bodyB, anchorA, anchorB, useWorldCoordinates );
			world.addJoint( ropeJoint );
			return ropeJoint;
		}

		#endregion

		#region Weld Joint

		public static WeldJoint createWeldJoint( World world, Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
		{
			var weldJoint = new WeldJoint( bodyA, bodyB, anchorA, anchorB, useWorldCoordinates );
			world.addJoint( weldJoint );
			return weldJoint;
		}

		#endregion

		#region Prismatic Joint

		public static PrismaticJoint createPrismaticJoint( World world, Body bodyA, Body bodyB, vec2 anchor, vec2 axis, bool useWorldCoordinates = false )
		{
			PrismaticJoint joint = new PrismaticJoint( bodyA, bodyB, anchor, axis, useWorldCoordinates );
			world.addJoint( joint );
			return joint;
		}

		#endregion

		#region Wheel Joint

		public static WheelJoint createWheelJoint( World world, Body bodyA, Body bodyB, vec2 anchor, vec2 axis, bool useWorldCoordinates = false )
		{
			WheelJoint joint = new WheelJoint( bodyA, bodyB, anchor, axis, useWorldCoordinates );
			world.addJoint( joint );
			return joint;
		}

		public static WheelJoint createWheelJoint( World world, Body bodyA, Body bodyB, vec2 axis )
		{
			return createWheelJoint( world, bodyA, bodyB, vec2.Zero, axis );
		}

		#endregion

		#region Angle Joint

		public static AngleJoint createAngleJoint( World world, Body bodyA, Body bodyB )
		{
			var angleJoint = new AngleJoint( bodyA, bodyB );
			world.addJoint( angleJoint );
			return angleJoint;
		}

		#endregion

		#region Distance Joint

		public static DistanceJoint createDistanceJoint( World world, Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, bool useWorldCoordinates = false )
		{
			var distanceJoint = new DistanceJoint( bodyA, bodyB, anchorA, anchorB, useWorldCoordinates );
			world.addJoint( distanceJoint );
			return distanceJoint;
		}

		public static DistanceJoint createDistanceJoint( World world, Body bodyA, Body bodyB )
		{
			return createDistanceJoint( world, bodyA, bodyB, vec2.Zero, vec2.Zero );
		}

		#endregion

		#region Friction Joint

		public static FrictionJoint createFrictionJoint( World world, Body bodyA, Body bodyB, vec2 anchor, bool useWorldCoordinates = false )
		{
			var frictionJoint = new FrictionJoint( bodyA, bodyB, anchor, useWorldCoordinates );
			world.addJoint( frictionJoint );
			return frictionJoint;
		}

		public static FrictionJoint createFrictionJoint( World world, Body bodyA, Body bodyB )
		{
			return createFrictionJoint( world, bodyA, bodyB, vec2.Zero );
		}

		#endregion

		#region Gear Joint

		public static GearJoint createGearJoint( World world, Body bodyA, Body bodyB, Joint jointA, Joint jointB, float ratio )
		{
			var gearJoint = new GearJoint( bodyA, bodyB, jointA, jointB, ratio );
			world.addJoint( gearJoint );
			return gearJoint;
		}

		#endregion

		#region Pulley Joint

		public static PulleyJoint createPulleyJoint( World world, Body bodyA, Body bodyB, vec2 anchorA, vec2 anchorB, vec2 worldAnchorA, vec2 worldAnchorB, float ratio, bool useWorldCoordinates = false )
		{
			var pulleyJoint = new PulleyJoint( bodyA, bodyB, anchorA, anchorB, worldAnchorA, worldAnchorB, ratio, useWorldCoordinates );
			world.addJoint( pulleyJoint );
			return pulleyJoint;
		}

		#endregion

		#region MouseJoint

		public static FixedMouseJoint createFixedMouseJoint( World world, Body body, vec2 worldAnchor )
		{
			var joint = new FixedMouseJoint( body, worldAnchor );
			world.addJoint( joint );
			return joint;
		}

		#endregion
	}
}