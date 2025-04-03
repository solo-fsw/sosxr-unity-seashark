using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    [CustomEditor(typeof(DrawGizmo))]
    public class DrawGizmoEditor : UnityEditor.Editor
    {
        private SerializedProperty m_gizmoType;
        private SerializedProperty m_endTransform;
        private SerializedProperty m_gizmoColor;
        private SerializedProperty m_gizmoSize;
        private SerializedProperty m_direction;


        private void OnEnable()
        {
            m_gizmoType = serializedObject.FindProperty("m_type");
            m_endTransform = serializedObject.FindProperty("m_end");
            m_gizmoColor = serializedObject.FindProperty("m_color");
            m_gizmoSize = serializedObject.FindProperty("m_size");
            m_direction = serializedObject.FindProperty("m_direction");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var gizmoType = (GizmoType) m_gizmoType.enumValueIndex;

            EditorGUILayout.PropertyField(m_gizmoType);
            EditorGUILayout.PropertyField(m_gizmoColor);

            if (gizmoType != GizmoType.LineBetween)
            {
                EditorGUILayout.PropertyField(m_gizmoSize);
            }

            switch (gizmoType)
            {
                case GizmoType.LineBetween:
                    if (m_endTransform.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("End Transform is not set", MessageType.Warning);
                    }

                    EditorGUILayout.PropertyField(m_endTransform, new GUIContent("End Transform"));

                    break;

                case GizmoType.LinePointing:
                    EditorGUILayout.PropertyField(m_direction, new GUIContent("Direction"));

                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}