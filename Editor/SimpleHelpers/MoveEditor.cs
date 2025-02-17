using UnityEditor;
using UnityEngine;


namespace SOSXR.SimpleHelpers.Editor
{
    [CustomEditor(typeof(Move))]
    public class MoveEditor : UnityEditor.Editor
    {
        // Cached SerializedProperties
        private SerializedProperty actionToTakeProp;
        private SerializedProperty targetProp;
        private SerializedProperty velocityProp;
        private SerializedProperty rotationSpeedProp;
        private SerializedProperty smoothTimeProp;
        private SerializedProperty offsetProp;
        private SerializedProperty bezierPointsProp;
        private SerializedProperty bezierLoopProp;
        private SerializedProperty orbitRadiusProp;
        private SerializedProperty orbitSpeedProp;
        private SerializedProperty amplitudeProp;
        private SerializedProperty frequencyProp;
        private SerializedProperty bounceStartPointProp;
        private SerializedProperty bounceEndPointProp;
        private SerializedProperty bounceSpeedProp;
        private SerializedProperty springForceProp;
        private SerializedProperty dampingProp;
        private SerializedProperty heightConstraintsProp;


        private bool _init;


        public SerializedProperty vector3OptionsProp;


        // Cache properties in OnEnable
        private void OnEnable()
        {
            if (target == null)
            {
                Debug.LogWarning("Target is null. Skipping property initialization.");

                return;
            }

            actionToTakeProp = serializedObject.FindProperty("m_actionToTake");
            targetProp = serializedObject.FindProperty("m_target");
            velocityProp = serializedObject.FindProperty("m_velocity");
            rotationSpeedProp = serializedObject.FindProperty("m_rotationSpeed");
            smoothTimeProp = serializedObject.FindProperty("m_smoothTime");
            offsetProp = serializedObject.FindProperty("m_offset");
            bezierPointsProp = serializedObject.FindProperty("m_bezierPoints");
            bezierLoopProp = serializedObject.FindProperty("m_bezierLoop");
            orbitRadiusProp = serializedObject.FindProperty("m_orbitRadius");
            orbitSpeedProp = serializedObject.FindProperty("m_orbitSpeed");
            amplitudeProp = serializedObject.FindProperty("m_amplitude");
            frequencyProp = serializedObject.FindProperty("m_frequency");
            bounceStartPointProp = serializedObject.FindProperty("m_bounceStartPoint");
            bounceEndPointProp = serializedObject.FindProperty("m_bounceEndPoint");
            bounceSpeedProp = serializedObject.FindProperty("m_bounceSpeed");
            springForceProp = serializedObject.FindProperty("m_springForce");
            dampingProp = serializedObject.FindProperty("m_damping");
            heightConstraintsProp = serializedObject.FindProperty("m_heightConstraints");

            _init = true;
        }


        private void OnSceneGUI()
        {
            if (!_init || target == null)
            {
                return;
            }

            var move = (Move) target;

            // Ensure ActionToTake is MoveAlongPath and there are enough points
            if (actionToTakeProp.enumValueIndex != (int) ActionToTake.MoveAlongPath || move.m_bezierPoints == null || move.m_bezierPoints.Length < 4)
            {
                return;
            }

            // Draw the full path by iterating through each segment
            Handles.color = Color.green;
            var segmentCount = (move.m_bezierPoints.Length - 1) / 3; // Number of cubic Bezier segments
            var curveResolution = 20; // Number of points per segment

            for (var segment = 0; segment < segmentCount; segment++)
            {
                var startPointIndex = segment * 3;

                // Get the 4 control points for this segment
                var segmentPoints = new Vector3[4];

                for (var i = 0; i < 4; i++)
                {
                    segmentPoints[i] = move.m_bezierPoints[startPointIndex + i];
                }

                // Draw the curve for this segment
                for (var i = 0; i < curveResolution; i++)
                {
                    var t1 = i / (float) curveResolution;
                    var t2 = (i + 1) / (float) curveResolution;

                    var point1 = Move.GetCubicBezierPoint(segmentPoints, t1);
                    var point2 = Move.GetCubicBezierPoint(segmentPoints, t2);

                    Handles.DrawLine(point1, point2);
                }
            }

            // Draw control points and the connecting lines
            Handles.color = Color.cyan;

            for (var i = 0; i < move.m_bezierPoints.Length; i++)
            {
                move.m_bezierPoints[i] = Handles.PositionHandle(move.m_bezierPoints[i], Quaternion.identity);

                if (i > 0)
                {
                    Handles.DrawLine(move.m_bezierPoints[i - 1], move.m_bezierPoints[i]);
                }
            }

            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(move);
            }
        }


        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                EditorGUILayout.HelpBox("SerializedObject or target is null. Check the Move component setup.", MessageType.Warning);

                return;
            }

            // Begin checking for changes
            EditorGUI.BeginChangeCheck();

            // Draw the ActionToTake enum first
            EditorGUILayout.PropertyField(actionToTakeProp);

            // Draw target field for most movement types (except MoveByOffset and MoveInLocalSpace)
            switch ((ActionToTake) actionToTakeProp.enumValueIndex)
            {
                case ActionToTake.ParentTo:
                case ActionToTake.SyncTransform:
                case ActionToTake.MoveTowards:
                case ActionToTake.LookAt:
                case ActionToTake.SmoothLookAtTarget:
                case ActionToTake.SmoothMoveTowards:
                case ActionToTake.MoveInCircle:
                case ActionToTake.SmoothFollow:
                case ActionToTake.MoveWithElasticity:
                    DrawTargetField();

                    break;
            }

            // Conditionally draw fields based on selected ActionToTake
            switch ((ActionToTake) actionToTakeProp.enumValueIndex)
            {
                case ActionToTake.MoveTowards:
                case ActionToTake.SmoothMoveTowards:
                case ActionToTake.SmoothLookAtTarget:
                    DrawVelocityField();

                    break;

                case ActionToTake.MoveByOffset:
                    DrawVelocityField();
                    DrawOffsetField();

                    break;

                case ActionToTake.MoveAlongPath:
                    DrawNotImplementedMessage();
                    // DrawVelocityField();
                    // DrawBezierPathFields();
                    // DrawBezierLoopField();

                    break;

                case ActionToTake.MoveInLocalSpace:
                    DrawNotImplementedMessage();
                    // DrawVector3OptionsField(); 
                    // DrawVelocityField();

                    break;

                case ActionToTake.MoveInCircle:
                    DrawOrbitFields();

                    break;

                case ActionToTake.MoveSinusoidally:
                    DrawNotImplementedMessage();
                    // DrawSinusoidalFields();

                    break;

                case ActionToTake.SmoothBounceBackAndForth:
                    DrawBounceFields();

                    break;

                case ActionToTake.MoveWithElasticity:
                    DrawNotImplementedMessage();
                    // DrawElasticityFields();

                    break;

                case ActionToTake.SmoothFollow:
                    DrawVelocityField();
                    DrawHeightConstraintsFields();

                    break;
            }

            // Apply changes
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }


        private void DrawVector3OptionsField()
        {
            // I want an enum dropdown here
            // I want to be able to select from a list of Vector3 options
        }


        private void DrawNotImplementedMessage()
        {
            EditorGUILayout.HelpBox("This feature is not yet implemented.", MessageType.Warning);
        }


        private void DrawBezierLoopField()
        {
            EditorGUILayout.PropertyField(bezierLoopProp, new GUIContent("Loop Path"));
        }


        // Existing drawing methods remain the same, but use cached properties
        private void DrawTargetField()
        {
            EditorGUILayout.PropertyField(targetProp, new GUIContent("Target"));
        }


        private void DrawVelocityField()
        {
            EditorGUILayout.PropertyField(velocityProp, new GUIContent("Velocity"));

            // Rotation speed for RotateTowards
            if (actionToTakeProp.enumValueIndex == (int) ActionToTake.SmoothLookAtTarget)
            {
                EditorGUILayout.PropertyField(rotationSpeedProp, new GUIContent("Rotation Speed"));
            }

            // Smooth time for SmoothFollow
            if (actionToTakeProp.enumValueIndex == (int) ActionToTake.SmoothFollow)
            {
                EditorGUILayout.PropertyField(smoothTimeProp, new GUIContent("Smooth Time"));
            }
        }


        // Other drawing methods follow the same pattern, replacing direct FindProperty calls with cached properties
        private void DrawOffsetField()
        {
            EditorGUILayout.PropertyField(offsetProp, new GUIContent("Movement Offset"));
        }


        private void DrawBezierPathFields()
        {
            EditorGUILayout.PropertyField(bezierPointsProp, new GUIContent("Bezier Points"));
        }


        private void DrawOrbitFields()
        {
            EditorGUILayout.PropertyField(orbitRadiusProp, new GUIContent("Orbit Radius"));
            EditorGUILayout.PropertyField(orbitSpeedProp, new GUIContent("Orbit Speed"));
        }


        private void DrawSinusoidalFields()
        {
            EditorGUILayout.PropertyField(amplitudeProp, new GUIContent("Amplitude"));
            EditorGUILayout.PropertyField(frequencyProp, new GUIContent("Frequency"));
        }


        private void DrawBounceFields()
        {
            EditorGUILayout.PropertyField(bounceStartPointProp, new GUIContent("Bounce Start Point"));
            EditorGUILayout.PropertyField(bounceEndPointProp, new GUIContent("Bounce End Point"));
            EditorGUILayout.PropertyField(bounceSpeedProp, new GUIContent("Bounce Speed"));
        }


        private void DrawElasticityFields()
        {
            EditorGUILayout.PropertyField(springForceProp, new GUIContent("Spring Force"));
            EditorGUILayout.PropertyField(dampingProp, new GUIContent("Damping"));
        }


        private void DrawHeightConstraintsFields()
        {
            EditorGUILayout.PropertyField(heightConstraintsProp, new GUIContent("Height Constraints"));
        }
    }
}