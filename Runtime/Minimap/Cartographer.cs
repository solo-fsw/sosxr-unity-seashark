using System;
using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Generates and manages a minimap by creating sprite renderers for objects tagged for the map.
    /// Runs in both editor and play mode (ExecuteAlways).
    /// The minimap configuration is defined via MapMaker entries that specify which objects to map
    /// and how to display them.
    /// </summary>
    [ExecuteAlways]
    public class Cartographer : MonoBehaviour
    {
        [SerializeField] [TagSelector] private string m_minimapObjectsTag = "Minimap";
        [SerializeField] private List<MapMaker> m_map;
        [Tooltip("Toggle this if during Edit mode (not Play Mode) you want to regenerate the map, so that changes to the above values are displayed live. Untoggle for performance reasons.")]
        [SerializeField] private bool m_regenerate;
        private Camera _camera;

        private int _layer = -1;


        private void Awake()
        {
            GetCamera();
        }


        private void GetCamera()
        {
            _camera = GetComponentInChildren<Camera>();

            if (_camera == null)
            {
                Debug.LogWarning("We're missing our camera in the minimap, that can't be!");
            }

            // Get mask of camera, to be able to set the layer of our objects
            var mask = _camera.cullingMask;

            if (mask != 0 && (mask & (mask - 1)) == 0) // only one bit set
            {
                _layer = Mathf.RoundToInt(Mathf.Log(mask, 2));
            }
            else
            {
                Debug.LogWarning("Our camera either has nothing selected on the culling mask, or multiple layers. This cannot be. Select only one layer on the minimap camera");
            }
        }


        private void Start()
        {
            GenerateMap();
        }


        /// <summary>
        /// Creates minimap sprites for all configured tags. Sprites are instantiated as children
        /// of the tagged objects and positioned/rotated according to MapMaker settings.
        /// This method is available as a context menu item for manual triggering in the editor or play mode.
        /// </summary>
        [Button]
        public void GenerateMap()
        {
            GetCamera();

            foreach (var mapMaker in m_map)
            {
                var go = GameObject.FindWithTag(mapMaker.Tag);

                if (go == null)
                {
                    Debug.LogWarning("Could not find a GameObject with the tag '" + mapMaker.Tag + "'!");

                    continue;
                }

                var spriteGo = new GameObject(string.Concat("miniMap: ", mapMaker.Sprite.name));
                spriteGo.transform.SetParent(go.transform);
                spriteGo.ZeroOutLocalTransform();
                spriteGo.transform.localEulerAngles = mapMaker.Rotation;

                var scale = new Vector3(mapMaker.Scale, mapMaker.Scale, mapMaker.Scale);
                spriteGo.transform.localScale = scale;

                spriteGo.layer = _layer;
                spriteGo.tag = m_minimapObjectsTag;

                var sprite = spriteGo.AddComponent<SpriteRenderer>();
                sprite.sprite = mapMaker.Sprite;
            }
        }


        private void Update()
        {
            if (!m_regenerate || Application.isPlaying)
            {
                return;
            }

            DestroyMap();

            GenerateMap();

            Debug.Log("Regenerated map");
        }


        /// <summary>
        /// Destroys all minimap sprites that were previously generated.
        /// </summary>
        [Button]
        public void DestroyMap()
        {
            var gameObjects = GameObject.FindGameObjectsWithTag(m_minimapObjectsTag);

            foreach (var go in gameObjects)
            {
                if (Application.isPlaying)
                {
                    Destroy(go);
                }
                else
                {
                    DestroyImmediate(go);
                }
            }
        }
    }


    /// <summary>
    /// Defines the configuration for mapping a GameObject to the minimap.
    /// Specifies the tag to search for, the sprite to render, rotation offset, and the scale
    /// factor for the minimap representation.
    /// </summary>
    [Serializable]
    public struct MapMaker
    {
        /// <summary>Tag used to find GameObjects in the scene to include on the minimap.</summary>
        [TagSelector] public string Tag;
        /// <summary>Sprite to render for objects with the matching tag.</summary>
        public Sprite Sprite;
        [Tooltip("X probably needs to be 90 to make it face up to the camera.")]
        /// <summary>Rotation offset applied to the minimap sprite.</summary>
        public Vector3 Rotation;
        /// <summary>Scale factor applied to the minimap sprite.</summary>
        [Range(0f, 5f)] public float Scale;
    }
}
