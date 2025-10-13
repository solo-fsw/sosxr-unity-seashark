using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     By using this as a base, you can keep the Button functionality in one place, and use it in multiple Editors.
    ///     Those editors should inherit from this class,  with the type of the object they are editing.
    ///     A texture with `_textureName` should be in one of the Resources folders
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SOSXRBaseEditor<T> : Editor where T : Object
    {
        private readonly string _textureName = "SOSXR_editor_icon"; // Needs to be in a Resources folder
        private static Texture2D _buttonIcon;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawButtonsForMethods();
        }


        private void DrawButtonsForMethods()
        {
            if (_buttonIcon == null)
            {
                _buttonIcon = Resources.Load<Texture2D>(_textureName);
            }

            var iconSize = new Vector2(24, 24); // or 24x24 etc.

            var monoFont = Resources.Load<Font>("Fonts/Maple Mono/Maple Mono");

            var style = new GUIStyle(GUI.skin.button)
            {
                imagePosition = ImagePosition.ImageLeft,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(6, 6, 2, 2),
                contentOffset = new Vector2(4, 0),
                fontSize = 12,
                fixedHeight = 24,
                font = monoFont != null ? monoFont : EditorStyles.label.font
            };

            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                var contextMenuAttribute = method.GetCustomAttribute<ContextMenu>();

                if (buttonAttribute == null && contextMenuAttribute == null)
                {
                    continue;
                }

                var args = GetParameters(method);

                var label = buttonAttribute?.ItemName ?? contextMenuAttribute?.menuItem ?? method.Name;
                var tooltip = buttonAttribute?.Tooltip ?? "";

                string argsList = null;

                if (args.Length > 0)
                {
                    argsList = string.Join(", ", args);

                    if (!string.IsNullOrEmpty(tooltip))
                    {
                        tooltip += "\n";
                    }

                    tooltip = tooltip + "Called with default args: " + argsList;
                }

                var spacingAndTooltipIndicator = string.IsNullOrEmpty(tooltip) && string.IsNullOrEmpty(argsList) ? "     " : "  ᵀ  "; // Spacing in between icon and button name. Will get a small indicator mark if a tooltip is present
                var content = new GUIContent(string.Concat(spacingAndTooltipIndicator, label), _buttonIcon, tooltip); // Will draw icon if it has it, otherwise none

                if (GUILayout.Button(content, style, GUILayout.Height(iconSize.y)))
                {
                    method.Invoke(target, args);
                }
            }
        }


        /// <summary>
        ///     If any parameters are found on the method, it will first try to get the already present default value of the parameter.
        ///     If no defaults are specified in the method, it will grab a pre-defined default value, depending on the type of the arg.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private object[] GetParameters(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var args = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramType = param.ParameterType;

                // First try to get the preset default arg value of the parameter:
                if (param.HasDefaultValue)
                {
                    args[i] = param.DefaultValue;
                }
                else // Get general defaults to use
                {
                    var getDefault = typeof(SOSXRDefaultValues)
                        .GetMethod(nameof(SOSXRDefaultValues.Get))
                        ?.MakeGenericMethod(paramType);

                    args[i] = getDefault?.Invoke(null, null) ?? GetFallbackValue(param);

                    //var allArgsInARow = string.Join(", ", args.Select(a => a?.ToString() ?? "null"));
                    //this.Verbose($"{method.Name} was invoked with default values: {allArgsInARow}"); // This does get logged since it's basically making up values to send...
                }
            }

            return args;
        }


        private static object GetFallbackValue(ParameterInfo param)
        {
            if (param.HasDefaultValue)
            {
                return param.DefaultValue;
            }

            return param.ParameterType.IsValueType
                ? Activator.CreateInstance(param.ParameterType)
                : null;
        }
    }
}