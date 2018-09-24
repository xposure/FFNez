using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class Vector2Inspector : Inspector
	{

		public override void initialize( )
		{
		}


        public override void update()
        {
        }
        public override void render()
        {
            var value = getValue<vec2>();
            ImGui.DragVector2(_name, ref value, float.MinValue, float.MaxValue);
            setValue(value);
        }
    }
}
#endif