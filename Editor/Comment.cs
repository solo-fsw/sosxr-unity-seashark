using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace SOSXR.SeaShark.EditorScripts
{
    /// From Unity Experiment Framework
    [CustomEditor(typeof(Comment), true)]
    [CanEditMultipleObjects]
    public class CommentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            string note = serializedObject.FindProperty("Note").stringValue;
            EditorGUILayout.HelpBox(note, MessageType.Info);
        }
    }
}
