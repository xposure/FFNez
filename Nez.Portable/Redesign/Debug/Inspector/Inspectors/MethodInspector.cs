using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using Nez.UI;


#if DEBUG
namespace Nez
{
    public class MethodInspector : Inspector
    {
        // the TextField for our parameter if we have one
        Type _parameterType;
        object[] _paramValues;
        static byte[] _textInputBuffer = new byte[1024];

        private List<Inspector> _inspectors = new List<Inspector>();


        public static bool areParametersValid(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
                return true;

            if (parameters.Length > 1)
            {
                Debug.warn($"method {parameters[0].Member.Name} has InspectorCallableAttribute but it has more than 1 parameter");
                return false;
            }

            var paramType = parameters[0].ParameterType;
            if (paramType == typeof(int) || paramType == typeof(float) || paramType == typeof(string) || paramType == typeof(bool))
                return true;

            Debug.warn($"method {parameters[0].Member.Name} has InspectorCallableAttribute but it has an invalid paraemter type {paramType}");

            return false;
        }


        public override void initialize()
        {
            // we could have zero or 1 param
            var parameters = (_memberInfo as MethodInfo).GetParameters();
            if (parameters.Length == 0)
            {
                _paramValues = new object[0];
                return;
            }

            var parameter = parameters[0];
            _parameterType = parameter.ParameterType;

            _paramValues = new object[1];
            // add a filter for float/int
            if (_parameterType == typeof(float))
                _paramValues[0] = 0f;
            else if (_parameterType == typeof(int))
                _paramValues[0] = 0;
            else if (_parameterType == typeof(bool))
                _paramValues[0] = false;
            else
                _paramValues[0] = string.Empty;
        }


        public override void update()
        { }

        public override void render()
        {
            var clicked = ImGui.Button(_name);
            ImGui.SameLine();
            if (_parameterType != null)
            {
                if (_parameterType == typeof(float))
                {
                    var f = (float)_paramValues[0];
                    ImGui.DragFloat(string.Empty, ref f, float.MinValue, float.MaxValue);
                    _paramValues[0] = f;
                }
                else if (_parameterType == typeof(int))
                {
                    var f = (int)_paramValues[0];
                    ImGui.DragInt(string.Empty, ref f, 1, int.MinValue, int.MaxValue, null);
                    _paramValues[0] = f;
                }
                else if (_parameterType == typeof(bool))
                {
                    var f = (bool)_paramValues[0];
                    ImGui.Checkbox(null, ref f);
                    _paramValues[0] = f;
                }
                else
                {
                    var text = _paramValues[0].ToString();
                    var len = System.Text.Encoding.UTF8.GetBytes(text, 0, text.Length, _textInputBuffer, 0);

                    ImGui.InputText(_name, _textInputBuffer, (uint)_textInputBuffer.Length, InputTextFlags.Default, null);

                    _paramValues[0] = System.Text.Encoding.UTF8.GetString(_textInputBuffer, 0, len);
                }
            }

            if (clicked)
                (_memberInfo as MethodInfo).Invoke(_target, _paramValues);
        }

    }
}
#endif
