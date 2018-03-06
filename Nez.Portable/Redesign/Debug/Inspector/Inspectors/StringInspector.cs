using ImGuiNET;
using Nez.UI;
using System;
using System.Runtime.InteropServices;


#if DEBUG
namespace Nez
{
    public class StringInspector : Inspector
    {
        static byte[] _textInputBuffer = new byte[1024];

        public override void initialize()
        {
        }


        public override void update()
        {
        }

        public override void render()
        {
            var text = getValue<string>();
            Assert.isTrue(text.Length < 1024);

            var len = System.Text.Encoding.UTF8.GetBytes(text, 0, text.Length, _textInputBuffer, 0);

            ImGui.InputText(_name, _textInputBuffer, (uint)_textInputBuffer.Length, InputTextFlags.Default, null);

            text = System.Text.Encoding.UTF8.GetString(_textInputBuffer, 0, len);
            setValue(text);
        }
    }
}
#endif