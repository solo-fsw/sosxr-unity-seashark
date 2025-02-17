using UnityEditor;
using UnityEngine;


namespace SOSXR.SimpleHelpers.Editor
{
    [CustomEditor(typeof(AdditionalUnityEvent))]
    public class AdditionalUnityEventInspector : UnityEditor.Editor
    {
        private SerializedProperty m_triggerEventOn;
        private SerializedProperty m_autoStart;
        private SerializedProperty m_secondsToWaitAfterCalling;
        private SerializedProperty m_tagsToCheckAgainst;
        private SerializedProperty m_inputAction;
        private SerializedProperty m_initialFireInterval;
        private SerializedProperty m_perIntervalChange;
        private SerializedProperty m_minMax;
        private SerializedProperty m_eventToFire;


        private void OnEnable()
        {
            m_triggerEventOn = serializedObject.FindProperty("m_triggerEventOn");
            m_autoStart = serializedObject.FindProperty("m_autoStart");
            m_secondsToWaitAfterCalling = serializedObject.FindProperty("m_secondsToWaitAfterCalling");
            m_tagsToCheckAgainst = serializedObject.FindProperty("m_tagsToCheckAgainst");
            m_inputAction = serializedObject.FindProperty("m_inputAction");
            m_initialFireInterval = serializedObject.FindProperty("m_initialFireInterval");
            m_perIntervalChange = serializedObject.FindProperty("m_perIntervalChange");
            m_minMax = serializedObject.FindProperty("m_minMax");
            m_eventToFire = serializedObject.FindProperty("m_eventToFire");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawCustomInspector();
            serializedObject.ApplyModifiedProperties();
        }


        private void DrawCustomInspector()
        {
            // Determine the current value of the trigger event setting
            var triggerEventOnValue = (UnityEventTrigger) m_triggerEventOn.intValue;

            // Display trigger event options
            EditorGUILayout.PropertyField(m_triggerEventOn);

            if (triggerEventOnValue.HasFlag(UnityEventTrigger.VariableIntervalLoop))
            {
                // Show VariableIntervalLoop settings
                EditorGUILayout.PropertyField(m_autoStart);
                EditorGUILayout.PropertyField(m_initialFireInterval);
                EditorGUILayout.PropertyField(m_perIntervalChange);
                EditorGUILayout.PropertyField(m_minMax);
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(m_eventToFire);
                DrawButtons();

                return;
            }

            if (triggerEventOnValue.HasFlag(UnityEventTrigger.Update) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.FixedUpdate) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.LateUpdate) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnCollisionStay) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnTriggerStay))
            {
                // Disable seconds to wait for the above triggers
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.PropertyField(m_secondsToWaitAfterCalling);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(m_secondsToWaitAfterCalling);
            }

            // Only show tag check settings for relevant trigger types
            if (triggerEventOnValue.HasFlag(UnityEventTrigger.OnTriggerEnter) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnTriggerStay) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnTriggerExit) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnCollisionEnter) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnCollisionStay) ||
                triggerEventOnValue.HasFlag(UnityEventTrigger.OnCollisionExit))
            {
                EditorGUILayout.PropertyField(m_tagsToCheckAgainst);
            }

            // Show input action setting if the event uses an input action
            if (triggerEventOnValue.HasFlag(UnityEventTrigger.OnInputAction))
            {
                EditorGUILayout.PropertyField(m_inputAction);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(m_eventToFire);
            DrawButtons();
        }


        private void DrawButtons()
        {
            if (GUILayout.Button(nameof(AdditionalUnityEvent.FireEvent)))
            {
                ((AdditionalUnityEvent) target).FireEvent();
            }

            if (GUILayout.Button(nameof(AdditionalUnityEvent.StopFiring)))
            {
                ((AdditionalUnityEvent) target).StopFiring();
            }
        }
    }
}