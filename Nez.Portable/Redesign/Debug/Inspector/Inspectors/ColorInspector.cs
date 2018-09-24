using ImGuiNET;

using Nez.UI;


#if DEBUG
namespace Nez
{
	public class ColorInspector : Inspector
	{
		public override void initialize()
		{
		}

		public override void update()
		{
		}

        public override void render()
        {
            base.render();

            var c = getValue<Color>();
            var v = (vec4)c;
            ImGui.ColorEdit4(_name, ref v, ColorEditFlags.Default);
            setValue(new Color(v));
        }

    }
}
#endif