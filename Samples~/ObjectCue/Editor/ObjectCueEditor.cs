using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomEditor(typeof(ObjectCue))]
    [CanEditMultipleObjects]
    public class ObjectCueEditor : UnityEditor.Editor
    {
        private ObjectCue _objectCue;


        private void OnEnable()
        {
            _objectCue = (ObjectCue) target;
        }


        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                if (GUILayout.Button(nameof(ObjectCue.InitialiseRenderers)))
                {
                    _objectCue.InitialiseRenderers();
                }

                GUILayout.Space(10);
            }

            if (GUILayout.Button(nameof(ObjectCue.StartCue)))
            {
                _objectCue.StartCue();
            }

            if (GUILayout.Button(nameof(ObjectCue.StopCue)))
            {
                _objectCue.StopCue();
            }

            if (GUILayout.Button(nameof(ObjectCue.ToggleCue)))
            {
                _objectCue.ToggleCue();
            }

            GUILayout.Space(10);

            base.OnInspectorGUI();
        }
    }
}