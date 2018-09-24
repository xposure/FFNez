using System.Collections.Generic;
using ImGuiNET;
using Nez.UI;

#if DEBUG
namespace Nez
{
    /// <summary>
    /// container for a Component/PostProcessor/Transform and all of its inspectable properties
    /// </summary>
    public class InspectorList
    {
        private bool _expanded = true;

        public object target;
        public string name;

        List<Inspector> _inspectors;

        public InspectorList(object target)
        {
            this.target = target;
            name = target.GetType().Name;
            _inspectors = Inspector.getInspectableProperties(target);
        }

        public InspectorList(Transform transform)
        {
            name = "Transform";
            _inspectors = Inspector.getTransformProperties(transform);
        }

        public void initialize()
        {
            foreach (var i in _inspectors)
                i.initialize();
        }

        public void update()
        {
            foreach (var i in _inspectors)
                i.update();
        }

        public void render()
        {
            var comp = target as Component;

            ImGui.PushID(name);
            if (comp != null)
            {
                ImGui.PushID("active");
                var active = comp.active;
                ImGui.Checkbox("", ref active);
                if (ImGui.IsItemHovered(HoveredFlags.Default))
                    ImGui.SetTooltip("Toggles if a component will have its update called.");
                ImGui.SameLine();
                comp.active = active;
                ImGui.PopID();

                ImGui.PushID("enabled");
                var enabled = comp.enabled;
                ImGui.Checkbox("", ref enabled);
                if (ImGui.IsItemHovered(HoveredFlags.Default))
                    ImGui.SetTooltip("Toggles if a component will have its render called.");
                ImGui.SameLine();
                comp.enabled = enabled;
                ImGui.PopID();
            }

            if (ImGui.Selectable(name, true))
            {
                _expanded = !_expanded;
            }

            var selectSize = ImGui.GetLastItemRectSize();
            ImGui.SameLine(selectSize.x - 10);
            ImGui.LabelText(string.Empty, _expanded ? "-" : "+");

            if (_expanded)
            {
                foreach (var it in _inspectors)
                    it.render();
            }
            ImGui.PopID();
        }
    }
}
#endif