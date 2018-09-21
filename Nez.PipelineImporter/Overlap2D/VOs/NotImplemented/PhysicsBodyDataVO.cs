using System;
using Microsoft.Xna.Framework;


namespace Atma.Overlap2D.Runtime
{
	public class PhysicsBodyDataVO
	{
		public int bodyType = 0;

		public float mass;
		public vec2 centerOfMass;
		public float rotationalInertia;
		public float damping;
		public float gravityScale;
		public bool allowSleep;
		public bool awake;
		public bool bullet;
		public bool sensor;

		public float density;
		public float friction;
		public float restitution;
	}
}

