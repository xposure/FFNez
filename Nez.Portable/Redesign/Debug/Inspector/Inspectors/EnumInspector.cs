using System;
using Nez.UI;
using System.Linq;
using System.Collections.Generic;
using ImGuiNET;


#if DEBUG
namespace Nez
{
    public class EnumInspector : Inspector
    {
        string[] _enumStringValues;
        private int index = 0;

        public override void initialize()
        {
            var enumValues = Enum.GetValues(_valueType);
            var enumStringValues = new List<string>();
            foreach (var e in enumValues)
                enumStringValues.Add(e.ToString());

            _enumStringValues = enumStringValues.ToArray();
            var selected = getValue<object>().ToString();
            index = enumStringValues.IndexOf(selected);
            if (index < 0)
                index = 0;
        }


        public override void update()
        {
        }

        public override void render()
        {
            ImGui.Combo(_name, ref index, _enumStringValues);
        }
    }
}
#endif