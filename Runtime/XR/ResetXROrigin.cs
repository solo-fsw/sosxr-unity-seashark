using Unity.XR.CoreUtils;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Based on: Justin P. Barnett's https://www.youtube.com/watch?v=EmjBonbATS0
    ///     This needs to live directly on the XROrigin object.
    ///     If you don't add a m_resetTransform, it uses the world origin as reset transform.
    ///     if setHeight is false, then it will not change the height difference of headset. If true, it will change the Y value.
    /// </summary>
    [RequireComponent(typeof(XROrigin))]
    public class ResetXROrigin : MonoBehaviour
    {
        [SerializeField] private bool m_setHeight = true;
        [SerializeField] [Optional] private Transform m_resetTransform;
        [SerializeField] [HideInInspector] private XROrigin _xrOrigin;
        [SerializeField] [HideInInspector] private Camera _xrCamera;


        private GameObject _resetGameObject;

        private Transform _resetTransform
        {
            get
            {
                if (m_resetTransform != null)
                {
                    return m_resetTransform;
                }

                if (_resetGameObject == null)
                {
                    _resetGameObject = new GameObject("Reset GameObject");
                    _resetGameObject.transform.position = Vector3.zero;
                    _resetGameObject.transform.rotation = Quaternion.identity;
                }

                return _resetGameObject.transform;
            }
        }


        private void OnValidate()
        {
            if (_xrOrigin == null)
            {
                _xrOrigin = GetComponent<XROrigin>();
            }

            if (_xrCamera == null)
            {
                _xrCamera = GetComponentInChildren<Camera>();
            }
        }


        [Button]
        public void ResetOrigin()
        {
            ResetRotation(); // Always set Rotation before Position.

            ResetPosition();
        }


        [Button]
        public void ResetRotation()
        {
            var rotationAngleY = _resetTransform.eulerAngles.y - _xrCamera.transform.eulerAngles.y;

            _xrOrigin.transform.Rotate(0, rotationAngleY, 0);
        }


        [Button]
        public void ResetPosition()
        {
            var distanceDiff = _resetTransform.position - _xrCamera.transform.position;

            if (!m_setHeight)
            {
                distanceDiff.y = 0;
            }

            _xrOrigin.transform.position += distanceDiff;
        }
    }
}