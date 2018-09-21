#if FEATURE_UI
using System;


namespace Atma
{
	/// <summary>
	/// adding this to a method will expose it to the inspector if it has 0 params or 1 param of a supported type: int, float, string
	/// and bool are currently supported.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public class InspectorCallableAttribute : InspectableAttribute
	{}
}

#endif
