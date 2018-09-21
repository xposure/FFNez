#if FEATURE_AI
using System;


namespace Atma.AI.BehaviorTrees
{
	public abstract class Decorator<T> : Behavior<T>
	{
		public Behavior<T> child;


		public override void invalidate()
		{
			base.invalidate();
			child.invalidate();
		}
	}
}

#endif
