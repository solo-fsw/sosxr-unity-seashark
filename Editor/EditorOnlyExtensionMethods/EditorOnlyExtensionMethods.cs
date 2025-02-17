using UnityEditor;
using UnityEngine;


namespace SOSXR.Extensions
{
    public static class EditorOnlyExtensionMethods
    {
        /// <summary>
        ///     Allows you to remove any bindings from an AnimationClip, except when that binding contains a word.
        ///     Created to remove all extranaous bindings from Unity Recorder Skinned Mesh Renderer recordings that don't deal with
        ///     the BlendShapes
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="bindingContains"></param>
        public static void RemoveCurveBindingsExcept(this AnimationClip animationClip, string bindingContains = "blendShape")
        {
            var curveBindings = AnimationUtility.GetCurveBindings(animationClip);

            for (var i = curveBindings.Length - 1; i >= 0; i--)
            {
                if (!curveBindings[i].propertyName.Contains(bindingContains))
                {
                    AnimationUtility.SetEditorCurve(animationClip, curveBindings[i], null);

                    Debug.Log("Deleted CurveBinding " + curveBindings[i].propertyName);
                }
                else
                {
                    Debug.Log("Kept CurveBinding " + curveBindings[i].propertyName);
                }
            }
        }


        /// <summary>
        ///     Allows you to remove specific ObjectReference bindings from Animation clip.
        ///     Created to remove all extranaous bindings from Unity Recorder Skinned Mesh Renderer recordings that don't deal with
        ///     the BlendShapes
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="bindingContains"></param>
        public static void RemoveObjectReferenceBindingsContaining(this AnimationClip animationClip, string bindingContains = "Material")
        {
            var objectReferenceBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);

            for (var i = objectReferenceBindings.Length - 1; i >= 0; i--)
            {
                if (objectReferenceBindings[i].propertyName.Contains(bindingContains))
                {
                    AnimationUtility.SetObjectReferenceCurve(animationClip, objectReferenceBindings[i], null);

                    Debug.Log("Deleted CurveBinding " + objectReferenceBindings[i].propertyName);
                }
                else
                {
                    Debug.Log("Kept CurveBinding " + objectReferenceBindings[i].propertyName);
                }
            }
        }
    }
}