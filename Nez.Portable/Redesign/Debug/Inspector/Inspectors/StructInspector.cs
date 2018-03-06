using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez.IEnumerableExtensions;
using Nez.UI;


#if DEBUG
namespace Nez
{
	public class StructInspector : Inspector
	{
		List<Inspector> _inspectors = new List<Inspector>();


		public override void initialize( )
		{
			// add a header

			// figure out which fiedls and properties are useful to add to the inspector
			var fields = ReflectionUtils.getFields( _valueType );
			foreach( var field in fields )
			{
				if( !field.IsPublic && IEnumerableExt.count( field.GetCustomAttributes<InspectableAttribute>() ) == 0 )
					continue;

				var inspector = getInspectorForType( field.FieldType, _target, field );
				if( inspector != null )
				{
					inspector.setStructTarget( _target, this, field );
					inspector.initialize();
					_inspectors.Add( inspector );
				}
			}

			var properties = ReflectionUtils.getProperties( _valueType );
			foreach( var prop in properties )
			{
				if( !prop.CanRead || !prop.CanWrite )
					continue;

				if( ( !prop.GetMethod.IsPublic || !prop.SetMethod.IsPublic ) && IEnumerableExt.count( prop.GetCustomAttributes<InspectableAttribute>() ) == 0 )
					continue;

				var inspector = getInspectorForType( prop.PropertyType, _target, prop );
				if( inspector != null )
				{
					inspector.setStructTarget( _target, this, prop );
					inspector.initialize();
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
            ImGui.LabelText(null, _name);
            foreach (var it in _inspectors)
                it.render();
        }

    }
}
#endif