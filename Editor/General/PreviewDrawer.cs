using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewDrawer : PropertyDrawer
    {
        private float imageHeight;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue is Texture or Material or Sprite or GameObject)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + imageHeight + 10;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (attribute is PreviewAttribute previewAttribute)
            {
                imageHeight = previewAttribute.Height;
            }

            Texture textureToDraw = null;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return;
            }

            switch (property.objectReferenceValue)
            {
                case Texture texture:
                    textureToDraw = texture;

                    break;
                case Material material:
                    textureToDraw = material.mainTexture;

                    break;
                case Sprite sprite:
                    textureToDraw = sprite.texture;

                    break;
                case GameObject gameObject:
                {
                    var renderer = gameObject.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        textureToDraw = renderer.sharedMaterial.mainTexture;
                    }

                    break;
                }
            }

            if (textureToDraw != null)
            {
                position.y += EditorGUI.GetPropertyHeight(property, label, true) + 5;
                position.height = imageHeight;
                DrawTexturePreview(position, textureToDraw);
            }
        }


        private static void DrawTexturePreview(Rect position, Texture texture)
        {
            GUI.DrawTexture(position, texture, ScaleMode.ScaleToFit);
        }
    }
}