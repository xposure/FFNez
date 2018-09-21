#if FEATURE_AI
using System;


namespace Atma.AI.UtilityAI
{
	public interface IAction<T>
	{
		void execute( T context );
	}
}

#endif
