using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nez
{
    public class SceneHierarchyWindow
    {
        private Scene _scene;
        private Entity _selectedEntity;

        public event Action<Entity> onEntitySelect;

        public SceneHierarchyWindow(Scene scene)
        {
            _scene = scene;
        }


        public void update()
        {

        }

        public void render()
        {
            if (ImGui.BeginWindow("Scene"))
            {
                for (int i = 0; i < _scene.entities.count; i++)
                {
                    var e = _scene.entities[i];
                    var flags = (TreeNodeFlags)0;

                    flags |= TreeNodeFlags.Leaf;
                    if(_selectedEntity != null && _selectedEntity == e)
                    {
                        flags |= TreeNodeFlags.Selected;
                    }

                    if (ImGui.TreeNodeEx(e.name, flags))
                    {
                        if (ImGui.IsItemHovered(HoveredFlags.Default) && ImGui.IsMouseClicked(0))
                        {
                            if (_selectedEntity != null ||  _selectedEntity != e)
                            {
                                _selectedEntity = e;
                                onEntitySelect?.Invoke(_selectedEntity);
                            }
                        }
                        ImGui.TreePop();
                    }
                }
                //ImGui.TreeNode()
                ImGui.EndWindow();
            }
        }

    }
}
