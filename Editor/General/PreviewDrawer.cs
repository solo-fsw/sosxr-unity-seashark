using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewDrawer : PropertyDrawer
    {
        private const float Spacing = 5f;


        // Read the height from the attribute here: GetPropertyHeight runs before OnGUI, so a value cached in OnGUI is stale.
        private float ImageHeight => ((PreviewAttribute) attribute).Height;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label, true);

            return GetPreviewTexture(property) != null ? height + ImageHeight + Spacing * 2 : height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, fieldHeight), property, label, true);

            var textureToDraw = GetPreviewTexture(property);

            if (textureToDraw == null)
            {
                return;
            }

            var previewRect = new Rect(position.x, position.y + fieldHeight + Spacing, position.width, ImageHeight);
            DrawTexturePreview(previewRect, textureToDraw);
        }


        private static Texture GetPreviewTexture(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || property.hasMultipleDifferentValues)
            {
                return null;
            }

            return GetPreviewTexture(property.objectReferenceValue);
        }


        /// <summary>
        ///     The texture to preview for an assigned object, or null if there is nothing to show.
        /// </summary>
        internal static Texture GetPreviewTexture(Object value)
        {
            switch (value)
            {
                case Texture texture:
                    return texture;
                case Material material:
                    return material.mainTexture;
                case Sprite sprite:
                    return sprite.texture;
                case GameObject gameObject:
                    // A Renderer without a material is valid (e.g. a freshly added MeshRenderer): don't throw.
                    return gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.sharedMaterial != null
                        ? renderer.sharedMaterial.mainTexture
                        : null;
                default:
                    return null;
            }
        }


        private static void DrawTexturePreview(Rect position, Texture texture)
        {
            GUI.DrawTexture(position, texture, ScaleMode.ScaleToFit);
        }
    }
}