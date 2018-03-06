using ImGuiNET;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class BoolInspector : Inspector
	{
		public override void initialize( )
		{
		}


		public override void update()
		{
		}

        public override void render()
        {
            var b = getValue<bool>();
            ImGui.Checkbox(_name, ref b);
            setValue(b);
        }
    }
}
#endif