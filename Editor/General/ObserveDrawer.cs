#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(ObserveAttribute))]
    public class ObserveDrawer : PropertyDrawer
    {
        private ObserveAttribute observeAttribute => (ObserveAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                if (IsMonoBehaviour(property))
                {
                    var mono = (MonoBehaviour) property.serializedObject.targetObject;

                    foreach (var callbackName in observeAttribute.callbackNames)
                    {
                        mono.Invoke(callbackName, 0);
                    }
                }
            }
        }


        private bool IsMonoBehaviour(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetType().IsSubclassOf(typeof(MonoBehaviour));
        }
    }
}
#endif