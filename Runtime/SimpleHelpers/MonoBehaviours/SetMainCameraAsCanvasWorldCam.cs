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
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }

            AssignWorldCamera();
        }


        private void Awake()
        {
            AssignWorldCamera();
        }


        private void Update()
        {
            AssignWorldCamera();
        }


        private void AssignWorldCamera()
        {
            if (_canvas.worldCamera != null)
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