#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(PasswordAttribute))]
    public class PasswordDrawer : PropertyDrawer
    {
        private PasswordAttribute passwordAttribute => (PasswordAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsSupported(property))
            {
                EditorGUI.PropertyField(position, property, label);

                return;
            }

            var password = property.stringValue;
            var maxLength = passwordAttribute.maxLength;
            position.height = 16;

            if (property.stringValue.Length > maxLength)
            {
                password = password.Substring(0, maxLength);
            }

            // Only write on a real edit: writing every frame would copy one object's value to all selected objects.
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();

            var value = passwordAttribute.useMask
                ? EditorGUI.PasswordField(position, label, password)
                : EditorGUI.TextField(position, label, password);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = value.Length > maxLength ? value.Substring(0, maxLength) : value;
            }

            EditorGUI.showMixedValue = false;

            if (IsTooShort(property))
            {
                DrawHelpBox(position);
            }
        }


        private void DrawHelpBox(Rect position)
        {
            position.x += 10;
            position.y += 20;
            position.width -= 10;
            position.height += 8;
            EditorGUI.HelpBox(position, string.Format("Password must contain at least {0} characters!", passwordAttribute.minLength), MessageType.Error);
        }


        private bool IsSupported(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        private bool IsTooShort(SerializedProperty property)
        {
            return property.stringValue.Length < passwordAttribute.minLength;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsSupported(property))
            {
                if (property.stringValue.Length < passwordAttribute.minLength)
                {
                    return base.GetPropertyHeight(property, label) + 30;
                }
            }

            return base.GetPropertyHeight(property, label);
        }
    }
}
#endif