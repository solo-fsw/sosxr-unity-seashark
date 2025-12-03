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
        private List<MethodInfo> _cachedMethods;
        private Dictionary<MethodInfo, ParameterInfo[]> _cachedParams;
        private Dictionary<MethodInfo, GUIContent> _cachedContent;
        private Dictionary<MethodInfo, ButtonAttribute> _cachedButtonAttrs; // Cache attributes
        private Dictionary<MethodInfo, InfoAttribute[]> _cachedInfoAttrs; // Cache info attributes
        private Font _monoFont;
        private GUIStyle _style;
        private GUIStyle _infoStyle; // Cache info style
        private int _maxParams;
        private static Texture2D _buttonIcon;

        // Cache layout calculations
        private float _buttonWidth;
        private float _remainingWidth;
        private float _sectionWidth;
        private const float ButtonHeight = 24f;
        private const float Spacing = 2f;


        private void OnEnable()
        {
            if (_buttonIcon == null)
            {
                _buttonIcon = Resources.Load<Texture2D>(_textureName);
            }

            if (_monoFont == null)
            {
                _monoFont = Resources.Load<Font>("Fonts/Maple Mono/Maple Mono");
            }

            _cachedMethods = new List<MethodInfo>();
            _cachedParams = new Dictionary<MethodInfo, ParameterInfo[]>();
            _cachedContent = new Dictionary<MethodInfo, GUIContent>();
            _cachedButtonAttrs = new Dictionary<MethodInfo, ButtonAttribute>();
            _cachedInfoAttrs = new Dictionary<MethodInfo, InfoAttribute[]>();
            _maxParams = 0;

            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var m in methods)
            {
                var buttonAttr = m.GetCustomAttribute<ButtonAttribute>();
                var contextAttr = m.GetCustomAttribute<ContextMenu>();

                if (buttonAttr != null || contextAttr != null)
                {
                    _cachedMethods.Add(m);
                    var parameters = m.GetParameters();
                    _cachedParams[m] = parameters;
                    _maxParams = Math.Max(_maxParams, parameters.Length);

                    var label = buttonAttr?.ItemName ?? contextAttr?.menuItem ?? m.Name;
                    var tooltip = buttonAttr?.Tooltip ?? "";
                    _cachedContent[m] = new GUIContent(label, _buttonIcon, tooltip);

                    // Cache attributes
                    _cachedButtonAttrs[m] = buttonAttr;
                    _cachedInfoAttrs[m] = (InfoAttribute[]) m.GetCustomAttributes(typeof(InfoAttribute), true);

                    if (!_methodArgs.ContainsKey(m))
                    {
                        _methodArgs[m] = new object[parameters.Length];
                    }
                }
            }

            LoadParameterValues();
        }


        private void CreateStyleIfNone()
        {
            _style ??= new GUIStyle(GUI.skin.button)
            {
                imagePosition = ImagePosition.ImageLeft,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(6, 6, 2, 2),
                contentOffset = new Vector2(4, 0),
                fontSize = 12,
                fixedHeight = ButtonHeight,
                font = _monoFont ?? EditorStyles.label.font
            };

            _infoStyle ??= new GUIStyle(EditorStyles.helpBox)
            {
                wordWrap = true,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 6, 6)
            };
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Pre-create styles
            CreateStyleIfNone();

            // Cache width calculations once per frame
            var width = EditorGUIUtility.currentViewWidth;
            _buttonWidth = width * 0.67f;
            _remainingWidth = width - _buttonWidth;
            _sectionWidth = _maxParams > 0 ? _remainingWidth / _maxParams : 0;

            var y = EditorGUILayout.GetControlRect(false, 0).y;

            foreach (var method in _cachedMethods)
            {
                var buttonAttr = _cachedButtonAttrs[method];

                if (buttonAttr is {Space: > 0})
                {
                    y += buttonAttr.Space;
                }

                if (buttonAttr is {HorizontalLine: true})
                {
                    EditorGUI.DrawRect(new Rect(0, y, width, 1), new Color(0.3f, 0.3f, 0.3f));
                    y += 4;
                }

                y = DrawMethodInfo(method, y, width);

                var parameters = _cachedParams[method];
                var buttonRect = new Rect(0, y, _buttonWidth, ButtonHeight);

                if (GUI.Button(buttonRect, _cachedContent[method], _style))
                {
                    method.Invoke(target, _methodArgs[method]);
                }

                // Draw parameter fields
                if (_maxParams > 0)
                {
                    for (var i = 0; i < _maxParams; i++)
                    {
                        if (i < parameters.Length)
                        {
                            var x = _buttonWidth + i * _sectionWidth;
                            var rect = new Rect(x + _sectionWidth * 0.1f, y, _sectionWidth * 0.8f, ButtonHeight);
                            _methodArgs[method][i] = DrawField(parameters[i], _methodArgs[method][i], rect);
                        }
                    }
                }

                y += ButtonHeight + Spacing;
            }

            GUILayout.Space(y);
        }


        private object DrawField(ParameterInfo param, object value, Rect rect)
        {
            var type = param.ParameterType;

            if (type == typeof(bool))
            {
                return EditorGUI.Toggle(rect, value is bool b && b);
            }

            if (type == typeof(int))
            {
                return EditorGUI.IntField(rect, value is int i ? i : 0);
            }

            if (type == typeof(float))
            {
                return EditorGUI.FloatField(rect, value is float f ? f : 0f);
            }

            if (type == typeof(string))
            {
                return EditorGUI.TextField(rect, value as string ?? "");
            }

            if (type == typeof(Vector3))
            {
                return EditorGUI.Vector3Field(rect, GUIContent.none, value is Vector3 v ? v : Vector3.zero);
            }

            if (type.IsEnum)
            {
                return EditorGUI.EnumPopup(rect, (Enum) (value ?? Activator.CreateInstance(type)));
            }

            return value;
        }


        private float DrawMethodInfo(MethodInfo method, float startY, float width)
        {
            var y = startY;
            var infoAttrs = _cachedInfoAttrs[method];

            if (infoAttrs.Length == 0)
            {
                return y;
            }

            var availableWidth = width - 20f;

            foreach (var info in infoAttrs)
            {
                var content = new GUIContent(info.InfoText);
                var height = _infoStyle.CalcHeight(content, availableWidth);
                EditorGUI.HelpBox(new Rect(0, y, availableWidth, height), info.InfoText, (MessageType) info.MessageType);
                y += height + 8;
            }

            return y;
        }


        private void OnDisable()
        {
            SaveParameterValues();
        }


        private void LoadParameterValues()
        {
            if (target == null)
            {
                return;
            }

            var id = target.GetInstanceID();

            foreach (var method in _cachedMethods)
            {
                var parameters = _cachedParams[method];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var key = $"{id}_{method.Name}_{i}";

                    if (EditorPrefs.HasKey(key))
                    {
                        _methodArgs[method][i] = Deserialize(EditorPrefs.GetString(key), parameters[i].ParameterType);
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

            var id = target.GetInstanceID();

            foreach (var kvp in _methodArgs)
            {
                var method = kvp.Key;
                var parameters = _cachedParams[method];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var key = $"{id}_{method.Name}_{i}";
                    EditorPrefs.SetString(key, Serialize(kvp.Value[i]));
                }
            }
        }


        private string Serialize(object value)
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


        private object Deserialize(string str, Type type)
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
                    var p = str.Split(',');

                    return new Vector3(float.Parse(p[0]), float.Parse(p[1]), float.Parse(p[2]));
                }
            }
            catch
            {
                // Silent fail, return default
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}