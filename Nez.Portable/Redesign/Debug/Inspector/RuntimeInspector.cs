using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez.UI;


#if DEBUG
namespace Nez
{
    public class RuntimeInspector : IDisposable
    {
        List<InspectorList> _inspectors = new List<InspectorList>();

        Entity _entity;

        /// <summary>
        /// creates a PostProcessor inspector
        /// </summary>
        public RuntimeInspector()
        {
        }


        /// <summary>
        /// creates an Entity inspector
        /// </summary>
        /// <param name="entity">Entity.</param>
        public RuntimeInspector(Entity entity)
        {
            _entity = entity;
            cacheTransformInspector();
        }


        void onSceneChanged()
        {
            Console.DebugConsole.instance._runtimeInspector = null;
            Dispose();
        }

        public void update()
        {
            // if we have an Entity this is an Entity inspector else it is a PostProcessor inspector
            if (_entity != null)
            {
                // update transform, which has a null Component
                getOrCreateInspectorList((Component)null).update();

                for (var i = 0; i < _entity.components.count; i++)
                    getOrCreateInspectorList(_entity.components[i]).update();
            }
            else
            {
                for (var i = 0; i < Core.scene._postProcessors.length; i++)
                    getOrCreateInspectorList(Core.scene._postProcessors.buffer[i]).update();
            }
        }


        public void render()
        {
            // manually start a fresh batch and call the UICanvas Component lifecycle methods since it isnt attached to the Scene
            if (ImGui.BeginWindow("inspector"))
            {
                foreach (var it in _inspectors)
                    it.render();

                ImGui.EndWindow();
            }
        }


        /// <summary>
        /// attempts to find a cached version of the InspectorList and if it cant find one it will create a new one
        /// </summary>
        /// <returns>The or create inspector list.</returns>
        /// <param name="comp">Comp.</param>
        InspectorList getOrCreateInspectorList(object comp)
        {
            var inspector = _inspectors.Where(i => i.target == comp).FirstOrDefault();
            if (inspector == null)
            {
                inspector = new InspectorList(comp);
                inspector.initialize();
                _inspectors.Add(inspector);
            }

            return inspector;
        }


        void cacheTransformInspector()
        {
            // add Transform separately
            var transformInspector = new InspectorList(_entity.transform);
            transformInspector.initialize();
            _inspectors.Add(transformInspector);
        }

        #region IDisposable Support

        bool _disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _entity = null;
                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
#endif