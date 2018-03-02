using ImGuiNET;
using Nez.UI;
using System;
using System.Runtime.InteropServices;


#if DEBUG
namespace Nez
{
    public class StringInspector : Inspector
    {
        TextField _textField;
        static byte[] _textInputBuffer = new byte[1024];

        public override void initialize(Table table, Skin skin)
        {
            var label = createNameLabel(table, skin);
            _textField = new TextField(getValue<string>(), skin);
            _textField.setTextFieldFilter(new FloatFilter());
            _textField.onTextChanged += (field, str) =>
            {
                setValue(str);
            };

            table.add(label);
            table.add(_textField).setMaxWidth(70);
        }


        public override void update()
        {
            _textField.setText(getValue<string>());
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