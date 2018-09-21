#if FEATURE_AI
using System;


namespace Atma.AI.UtilityAI
{
	/// <summary>
	/// scorer for use with a Consideration
	/// </summary>
	public interface IAppraisal<T>
	{
		float getScore( T context );
	}
}

#endif
