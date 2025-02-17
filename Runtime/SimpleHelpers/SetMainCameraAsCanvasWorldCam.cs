using UnityEngine;


namespace SOSXR.SimpleHelpers
{
    /// <summary>
    ///     Automatically sets the main camera as the world camera for a canvas.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class SetMainCameraAsCanvasWorldCam : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private Canvas canvas;


        private void OnValidate()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
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
            if (canvas.worldCamera != null)
            {
                enabled = false; // Disable component if no action is needed

                return;
            }

            if (Camera.main == null)
            {
                return;
            }

            canvas.worldCamera = Camera.main;
        }
    }
}