using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Automatically sets the main camera as the world camera for a canvas.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class SetMainCameraAsCanvasWorldCam : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private Canvas _canvas;


        private void OnValidate()
        {
            // Only resolve the reference here. Assigning the camera from OnValidate could disable (and save) this component in Edit mode.
            ResolveCanvas();
        }


        private void ResolveCanvas()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
        }


        private void Awake()
        {
            ResolveCanvas(); // OnValidate does not run in builds or after AddComponent at runtime
            AssignWorldCamera();
        }


        private void Update()
        {
            AssignWorldCamera();
        }


        private void AssignWorldCamera()
        {
            if (_canvas == null || _canvas.worldCamera != null)
            {
                enabled = false; // Disable component if no action is needed

                return;
            }

            if (Camera.main == null)
            {
                return;
            }

            _canvas.worldCamera = Camera.main;
        }
    }
}