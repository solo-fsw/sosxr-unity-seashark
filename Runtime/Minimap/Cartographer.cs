using System;
using System.Collections.Generic;
using SOSXR.EnhancedLogger;
using UnityEngine;


namespace SOSXR.SeaShark
{
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
                this.Warning("We're missing our camera in the minimap, that can't be!");
            }

            // Get mask of camera, to be able to set the layer of our objects
            var mask = _camera.cullingMask;

            if (mask != 0 && (mask & (mask - 1)) == 0) // only one bit set
            {
                _layer = Mathf.RoundToInt(Mathf.Log(mask, 2));
            }
            else
            {
                this.Warning("Our camera either has nothing selected on the culling mask, or multiple layers. This cannot be. Select only one layer on the minimap camera");
            }
        }


        private void Start()
        {
            GenerateMap();
        }


        [Button]
        public void GenerateMap()
        {
            GetCamera();

            foreach (var mapMaker in m_map)
            {
                var go = GameObject.FindWithTag(mapMaker.Tag);

                if (go == null)
                {
                    this.Warning("Could not find a GameObject with the tag '" + mapMaker.Tag + "'!");

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

            this.Info("Regenerated map");
        }


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


    [Serializable]
    public struct MapMaker
    {
        [TagSelector] public string Tag;
        public Sprite Sprite;
        [Tooltip("X probably needs to be 90 to make it face up to the camera.")]
        public Vector3 Rotation;
        [Range(0f, 5f)] public float Scale;
    }
}