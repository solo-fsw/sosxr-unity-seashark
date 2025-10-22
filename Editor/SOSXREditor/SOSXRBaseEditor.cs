using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.EditorScripts
{
    public class SOSXRBaseEditor<T> : Editor where T : Object
    {
        private readonly string _textureName = "SOSXR_editor_icon";
        private readonly Dictionary<MethodInfo, object[]> _methodArgs = new();
        private static Texture2D _buttonIcon;


        public override void OnInspectorGUI()
        {
            LoadParameterValues();
            base.OnInspectorGUI();
            DrawButtonsForMethods();
            SaveParameterValues();
        }


        private void DrawButtonsForMethods()
        {
            if (_buttonIcon == null)
            {
                _buttonIcon = Resources.Load<Texture2D>(_textureName);
            }

            var monoFont = Resources.Load<Font>("Fonts/Maple Mono/Maple Mono");

            var style = new GUIStyle(GUI.skin.button)
            {
                imagePosition = ImagePosition.ImageLeft,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(6, 6, 2, 2),
                contentOffset = new Vector2(4, 0),
                fontSize = 12,
                fixedHeight = 24,
                font = monoFont ?? EditorStyles.label.font
            };

            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // find max parameters for alignment
            var maxParams = 0;

            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<ButtonAttribute>() != null || method.GetCustomAttribute<ContextMenu>() != null)
                {
                    maxParams = Mathf.Max(maxParams, method.GetParameters().Length);
                }
            }

            var totalWidth = EditorGUIUtility.currentViewWidth;
            var buttonWidth = totalWidth * 0.67f;
            var remainingWidth = totalWidth - buttonWidth;

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                var contextAttr = method.GetCustomAttribute<ContextMenu>();

                if (buttonAttr == null && contextAttr == null)
                {
                    continue;
                }

                // Add optional space above
                if (buttonAttr is {Space: > 0})
                {
                    GUILayout.Space(buttonAttr.Space);
                }

                // Optional horizontal line
                if (buttonAttr is {HorizontalLine: true})
                {
                    var rect = EditorGUILayout.GetControlRect(false, 1);
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
                    GUILayout.Space(3);
                }

                var parameters = method.GetParameters();

                if (!_methodArgs.ContainsKey(method))
                {
                    _methodArgs[method] = new object[parameters.Length];
                }

                EditorGUILayout.BeginHorizontal();

                var buttonRect = GUILayoutUtility.GetRect(buttonWidth, 24, GUILayout.ExpandWidth(true));
                var label = buttonAttr?.ItemName ?? contextAttr?.menuItem ?? method.Name;
                var tooltip = buttonAttr?.Tooltip ?? "";
                var content = new GUIContent(label, _buttonIcon, tooltip);

                if (GUI.Button(buttonRect, content, style))
                {
                    method.Invoke(target, _methodArgs[method]);
                }

                // Parameter fields
                if (maxParams > 0)
                {
                    var fieldSectionWidth = remainingWidth / maxParams;

                    for (var i = 0; i < maxParams; i++)
                    {
                        var fieldArea = GUILayoutUtility.GetRect(fieldSectionWidth, 24, GUILayout.ExpandWidth(true));

                        if (i < parameters.Length)
                        {
                            var param = parameters[i];
                            var fieldWidth = fieldArea.width * 0.8f;

                            var fieldRect = new Rect(
                                fieldArea.x + (fieldArea.width - fieldWidth) / 2,
                                fieldArea.y,
                                fieldWidth,
                                fieldArea.height
                            );

                            _methodArgs[method][i] = DrawFieldForType(param, _methodArgs[method][i], fieldRect);
                        }
                        else
                        {
                            GUILayout.Space(fieldSectionWidth);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }


        private object DrawFieldForType(ParameterInfo param, object currentValue, Rect rect)
        {
            var type = param.ParameterType;

            if (type == typeof(bool))
            {
                return EditorGUI.Toggle(rect, currentValue is bool b ? b : default);
            }

            if (type == typeof(int))
            {
                return EditorGUI.IntField(rect, currentValue is int i ? i : default);
            }

            if (type == typeof(float))
            {
                return EditorGUI.FloatField(rect, currentValue is float f ? f : default);
            }

            if (type == typeof(string))
            {
                return EditorGUI.TextField(rect, currentValue as string ?? "");
            }

            if (type == typeof(Vector3))
            {
                return EditorGUI.Vector3Field(rect, GUIContent.none, currentValue is Vector3 v ? v : default);
            }

            if (type.IsEnum)
            {
                return EditorGUI.EnumPopup(rect, (Enum) (currentValue ?? Activator.CreateInstance(type)));
            }

            return currentValue;
        }


        #region Persistence

        private void LoadParameterValues()
        {
            if (target == null)
            {
                return;
            }

            var targetId = target.GetInstanceID();

            foreach (var method in target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    continue;
                }

                var keyBase = $"{targetId}_{method.Name}";

                if (!_methodArgs.ContainsKey(method))
                {
                    _methodArgs[method] = new object[parameters.Length];
                }

                for (var i = 0; i < parameters.Length; i++)
                {
                    var key = keyBase + "_" + i;

                    if (EditorPrefs.HasKey(key))
                    {
                        _methodArgs[method][i] = DeserializeValue(EditorPrefs.GetString(key), parameters[i].ParameterType);
                    }
                }
            }
        }


        private void SaveParameterValues()
        {
            if (target == null)
            {
                return;
            }

            var targetId = target.GetInstanceID();

            foreach (var kvp in _methodArgs)
            {
                var method = kvp.Key;
                var parameters = method.GetParameters();

                for (var i = 0; i < parameters.Length; i++)
                {
                    var key = $"{targetId}_{method.Name}_{i}";
                    EditorPrefs.SetString(key, SerializeValue(kvp.Value[i]));
                }
            }
        }


        private string SerializeValue(object value)
        {
            if (value == null)
            {
                return "";
            }

            if (value is Vector3 v)
            {
                return $"{v.x},{v.y},{v.z}";
            }

            return value.ToString();
        }


        private object DeserializeValue(string str, Type type)
        {
            if (string.IsNullOrEmpty(str))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            try
            {
                if (type == typeof(bool))
                {
                    return bool.Parse(str);
                }

                if (type == typeof(int))
                {
                    return int.Parse(str);
                }

                if (type == typeof(float))
                {
                    return float.Parse(str);
                }

                if (type == typeof(string))
                {
                    return str;
                }

                if (type.IsEnum)
                {
                    return Enum.Parse(type, str);
                }

                if (type == typeof(Vector3))
                {
                    var parts = str.Split(',');

                    return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                }
            }
            catch
            {
            }

            return null;
        }

        #endregion
    }
}