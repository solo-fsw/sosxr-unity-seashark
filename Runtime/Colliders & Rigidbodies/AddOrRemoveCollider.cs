using System;
using UnityEngine;


namespace SOSXR.SeaShark.Colliders___Rigidbodies
{
    public class AddOrRemoveCollider : MonoBehaviour
    {
        [SerializeField] private TypeOfCollider m_colliderType;
        private Collider _currentCollider;


        private void CheckForExistingCollider()
        {
            _currentCollider = GetComponent<Collider>();
        }


        [ContextMenu(nameof(AddColliderOfType))]
        public void AddColliderOfType()
        {
            CheckForExistingCollider();

            if (_currentCollider != null)
            {
                return; // Collider already exists
            }

            _currentCollider = AddCollider(m_colliderType);

            if (_currentCollider != null)
            {
                Debug.Log($"Added {m_colliderType} to the GameObject.");
            }
            else
            {
                Debug.LogError($"Failed to add collider: {m_colliderType}");
            }
        }


        [ContextMenu(nameof(RemoveAnyCollider))]
        public void RemoveAnyCollider()
        {
            CheckForExistingCollider();

            if (_currentCollider != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_currentCollider);
                }
                else
                {
                    DestroyImmediate(_currentCollider);
                }

                _currentCollider = null;
                Debug.Log($"Removed {m_colliderType} from the GameObject.");
            }
        }


        private Collider AddCollider(TypeOfCollider colliderType)
        {
            // Convert the enum to a string and find the corresponding Unity collider type
            var colliderTypeNameStr = colliderType.ToString();
            var fullTypeName = "UnityEngine." + colliderTypeNameStr + ", UnityEngine";

            var type = Type.GetType(fullTypeName);

            if (type != null && typeof(Collider).IsAssignableFrom(type))
            {
                return gameObject.AddComponent(type) as Collider;
            }

            Debug.LogError($"Invalid collider type: {colliderTypeNameStr}");

            return null;
        }
    }
}