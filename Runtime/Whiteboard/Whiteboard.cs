using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Based on Justin P. Barnett's tutorial on: https://www.youtube.com/watch?v=sHE5ubsP-E8&t=1803s
    /// </summary>
    public class Whiteboard : MonoBehaviour
    {
        [SerializeField] private Vector2 m_textureSize = new(2048, 2048); // Create so that it updates to scale

        [SerializeField] private Renderer m_renderer;

        [Tooltip("If we want the eventual canvas to be throwable by XR, the MeshCollider needs to be on another object, which is not a child of the XR Grab Interactable (more precisely, not a child of the Rigidbody dealing with those components)")] [SerializeField]
        private Collider m_collider;

        public Vector2 TextureSize => m_textureSize;

        public Texture2D Texture { get; private set; }

        private Material _materialInstance;

        public Collider Collider
        {
            get => m_collider;
            private set => m_collider = value;
        }


        public void OnValidate()
        {
            if (m_renderer == null)
            {
                m_renderer = GetComponentInChildren<Renderer>();
            }

            if (m_collider == null)
            {
                m_collider = GetComponentInChildren<Collider>();
            }
        }


        private void Start()
        {
            SetTextureToRenderer();
        }


        [ContextMenu(nameof(SetTextureToRenderer))]
        public void SetTextureToRenderer()
        {
            if (m_renderer == null)
            {
                Debug.LogError("No renderer found! Cannot set texture.");

                return;
            }

            ReleaseTexture(); // Calling this again must not leak the previous texture

            Texture = new Texture2D((int) TextureSize.x, (int) TextureSize.y);

            // .material creates a per-renderer instance once; we own it and destroy it in OnDestroy.
            _materialInstance ??= m_renderer.material;
            _materialInstance.mainTexture = Texture; // Is the same as: SetTexture("_MainTex", Texture);

            if (Math.Abs(transform.localScale.x - transform.localScale.y) > 0.01f ||
                Math.Abs(transform.localScale.x - transform.localScale.z) > 0.01f)
            {
                Debug.LogWarning("Whiteboard scale is not uniform. This may cause issues with the texture.");
            }
        }


        private void ReleaseTexture()
        {
            if (Texture == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(Texture);
            }
            else
            {
                DestroyImmediate(Texture);
            }

            Texture = null;
        }


        private void OnDestroy()
        {
            ReleaseTexture();

            if (_materialInstance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_materialInstance);
                }
                else
                {
                    DestroyImmediate(_materialInstance);
                }

                _materialInstance = null;
            }
        }
    }
}