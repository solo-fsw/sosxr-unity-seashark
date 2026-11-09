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
            if (property?.serializedObject?.targetObject == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                // Apply first so the callback sees the new value. MonoBehaviour.Invoke only runs in Play mode,
                // so call the (parameterless) method directly via reflection on every selected object.
                property.serializedObject.ApplyModifiedProperties();

                foreach (var t in property.serializedObject.targetObjects)
                {
                    foreach (var callbackName in observeAttribute.callbackNames)
                    {
                        var method = FindMethod(t.GetType(), callbackName);

                        if (method == null)
                        {
                            Debug.LogWarning($"[Observe] No parameterless method '{callbackName}' on {t.GetType().Name}", t);

                            continue;
                        }

                        try
                        {
                            method.Invoke(t, null);
                        }
                        catch (System.Reflection.TargetInvocationException exception)
                        {
                            Debug.LogException(exception.InnerException ?? exception, t);
                        }
                    }
                }

                property.serializedObject.Update();
            }
        }


        private static System.Reflection.MethodInfo FindMethod(System.Type type, string name)
        {
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                                                         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;

            for (var current = type; current != null; current = current.BaseType)
            {
                var method = current.GetMethod(name, flags, null, System.Type.EmptyTypes, null);

                if (method != null)
                {
                    return method;
                }
            }

            return null;
        }


        private bool IsMonoBehaviour(SerializedProperty property)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                return false;
            }

            return property.serializedObject.targetObject.GetType().IsSubclassOf(typeof(MonoBehaviour));
        }
    }
}
#endif