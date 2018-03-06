using ImGuiNET;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class IntInspector : Inspector
	{
        private int min = int.MinValue, max = int.MaxValue;

        public override void initialize( )
		{
			// if we have a RangeAttribute we need to make a slider
			var rangeAttr = getFieldOrPropertyAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                min = (int)rangeAttr.minValue;
                max = (int)rangeAttr.maxValue;
            }
		}

		public override void update()
		{
		}

        public override void render()
        {
            var value = getValue<int>();
            ImGui.DragInt(_name, ref value,1, min, max, null);
            setValue(value);
        }
    }
}
#endif