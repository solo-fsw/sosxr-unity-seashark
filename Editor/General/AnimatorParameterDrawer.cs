#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Animatorウィンドウにあるパラメータをスクリプトでタイプセーフに扱うためのPropertyDrawer
    /// </summary>
    /// <exception cref='InvalidCastException'>
    ///     Is thrown when an explicit conversion (casting operation) fails because the source type cannot be converted to the
    ///     destination type.
    /// </exception>
    /// <exception cref='MissingComponentException'>
    ///     Is thrown when the missing component exception.
    /// </exception>
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterDrawer : PropertyDrawer
    {
        private AnimatorParameterAttribute animatorParameterAttribute => (AnimatorParameterAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                DefaultInspector(position, property, label);
                return;
            }

            var animatorController = GetAnimatorController(property);

            if (animatorController == null)
            {
                DefaultInspector(position, property, label);

                return;
            }

            var eventNames = animatorController.parameters
                                               .Where(t => CanAddEventName(t.type))
                                               .Select(t => t.name).ToList();

            if (eventNames.Count == 0)
            {
                // Don't wipe the stored value just because the controller currently has no matching parameter.
                DefaultInspector(position, property, label);

                return;
            }

            // Selection is derived from the property on every call: the attribute instance is shared by all
            // elements/objects using this drawer, so it must not hold per-property state.
            var currentIndex = eventNames.IndexOf(property.stringValue);

            if (currentIndex == -1 && !string.IsNullOrEmpty(property.stringValue))
            {
                eventNames.Insert(0, property.stringValue + " (missing)");
                currentIndex = 0;
            }

            var eventNamesArray = eventNames.ToArray();

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var selected = EditorGUI.Popup(position, label.text, Mathf.Max(currentIndex, 0), eventNamesArray);

            if (EditorGUI.EndChangeCheck() && !eventNamesArray[selected].EndsWith(" (missing)"))
            {
                property.stringValue = eventNamesArray[selected];
            }

            EditorGUI.EndProperty();
        }


        /// <summary>
        ///     Gets the animator controller.
        /// </summary>
        /// <returns>
        ///     The animator controller.
        /// </returns>
        /// <param name='property'>
        ///     Property.
        /// </param>
        private AnimatorController GetAnimatorController(SerializedProperty property)
        {
            // Non-Component targets (e.g. ScriptableObjects) have no Animator: fall back to the default field.
            if (property.serializedObject.targetObject is not Component component)
            {
                return null;
            }

            if (!component.TryGetComponent<Animator>(out var anim))
            {
                return null;
            }

            var controller = anim.runtimeAnimatorController;

            // Unwrap override controllers to reach the underlying AnimatorController
            while (controller is AnimatorOverrideController overrideController)
            {
                controller = overrideController.runtimeAnimatorController;
            }

            return controller as AnimatorController;
        }


        /// <summary>
        ///     Determines whether this instance can add event name the specified animatorController index.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this instance can add event name the specified animatorController i; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='animatorController'>
        ///     If set to <c>true</c> animator controller.
        /// </param>
        /// <param name='index'>
        ///     If set to <c>true</c> index.
        /// </param>
        private bool CanAddEventName(AnimatorControllerParameterType animatorControllerParameterType)
        {
            return !(animatorParameterAttribute.parameterType != AnimatorParameterAttribute.ParameterType.None
                     && (int) animatorControllerParameterType != (int) animatorParameterAttribute.parameterType);
        }


        /// <summary>
        ///     Sets the option values.
        /// </summary>
        /// <returns>
        ///     The option values.
        /// </returns>
        /// <param name='eventNames'>
        ///     Event names.
        /// </param>
        private int[] SetOptionValues(string[] eventNames)
        {
            var optionValues = new int[eventNames.Length];

            for (var i = 0; i < eventNames.Length; i++)
            {
                optionValues[i] = i;
            }

            return optionValues;
        }


        private void DefaultInspector(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}


#endif