using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.IEnumerableExtensions;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class EffectInspector : Inspector
	{
		List<Inspector> _inspectors = new List<Inspector>();
        private string _effectName;

		public override void initialize( )
		{
			// we either have a getter that gets a Material or an Effedt
			var effect = _valueType == typeof( Material ) ? getValue<Material>().effect : getValue<Effect>();
			if( effect == null )
				return;

            _effectName = effect.GetType().Name;

			// figure out which properties are useful to add to the inspector
			var effectProps = ReflectionUtils.getProperties( effect.GetType() );
			foreach( var prop in effectProps )
			{
				if( prop.DeclaringType == typeof( Effect ) )
					continue;
				
				if( !prop.CanRead || !prop.CanWrite || prop.Name == "Name" )
					continue;

				if( ( !prop.GetMethod.IsPublic || !prop.SetMethod.IsPublic ) && IEnumerableExt.count( prop.GetCustomAttributes<InspectableAttribute>() ) == 0 )
					continue;

				var inspector = getInspectorForType( prop.PropertyType, effect, prop );
				if( inspector != null )
				{
					inspector.setTarget( effect, prop );
					inspector.initialize( );
					_inspectors.Add( inspector );
				}
			}
		}

		public override void update()
		{
			foreach( var i in _inspectors )
				i.update();
		}

        public override void render()
        {
            ImGui.LabelText(null, _effectName);
            foreach (var it in _inspectors)
                it.render();
        }
    }
}
#endif