#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(AngleAttribute))]
    public class AngleDrawer : PropertyDrawer
    {
        private readonly MethodInfo knobMethodInfo = typeof(EditorGUI).GetMethod("Knob",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        private AngleAttribute angleAttribute => (AngleAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                using (new EditorGUI.PropertyScope(position, label, property))
                {
                    EditorGUI.LabelField(position, label);
                    var knobRect = new Rect(position);
                    knobRect.x += EditorGUIUtility.labelWidth;

                    property.floatValue = Knob(knobRect, Vector2.one * angleAttribute.knobSize, property.floatValue, angleAttribute.min,
                        angleAttribute.max, angleAttribute.unit, angleAttribute.backgroundColor, angleAttribute.activeColor, angleAttribute.showValue);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }


        private float Knob(Rect position, Vector2 knobSize, float currentValue, float start, float end, string unit, Color backgroundColor, Color activeColor, bool showValue)
        {
            var invoke = knobMethodInfo.Invoke(null, new object[] {position, knobSize, currentValue, start, end, unit, backgroundColor, activeColor, showValue, GUIUtility.GetControlID("Knob".GetHashCode(), FocusType.Passive, position)});

            return (float) (invoke ?? 0);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            return property.propertyType != SerializedPropertyType.Float ? height : angleAttribute.knobSize + 4;
        }
    }
}
#endif