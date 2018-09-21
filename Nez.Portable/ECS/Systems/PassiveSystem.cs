#if FEATURE_ESC
using System;
using System.Collections.Generic;


namespace Atma
{
	public abstract class PassiveSystem : EntitySystem
	{
		public override void onChange( Entity entity )
		{
			// We do not manage any notification of entities changing state  and avoid polluting our list of entities as we want to keep it empty
		}


		protected override void process( List<Entity> entities )
		{
			// We replace the basic entity system with our own that doesn't take into account entities
			begin();
			end();
		}

	}
}

#endif
