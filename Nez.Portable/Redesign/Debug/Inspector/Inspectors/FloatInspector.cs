using ImGuiNET;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class FloatInspector : Inspector
	{
        private float min = float.MinValue, max = float.MaxValue;

		public override void initialize( )
		{
			// if we have a RangeAttribute we need to make a slider
			var rangeAttr = getFieldOrPropertyAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                min = rangeAttr.minValue;
                max = rangeAttr.maxValue;
            }
		}


		public override void update()
		{
		}

        public override void render()
        {
            var value = getValue<float>();
            ImGui.DragFloat(_name, ref value, min, max);
            setValue(value);
        }
    }
}
#endif