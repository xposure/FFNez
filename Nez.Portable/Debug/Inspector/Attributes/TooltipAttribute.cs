#if FEATURE_UI
using System;


namespace Atma
{
	/// <summary>
	/// displays a tooltip when hovering over the label of any inspectable elements
	/// </summary>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public class TooltipAttribute : InspectableAttribute
	{
		public string tooltip;

		public TooltipAttribute( string tooltip )
		{
			this.tooltip = tooltip;
		}
	}
}

#endif
