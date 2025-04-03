using System;
using UnityEngine;


namespace SOSXR.SeaShark.Colliders___Rigidbodies
{
    public class FindCollidersInScene : MonoBehaviour
    {
        [SerializeField] private TypeOfCollider m_colliderType;
        [SerializeField] private Collider[] m_filteredColliders;

        public Collider[] FilteredColliders
        {
            get => m_filteredColliders;
            set => m_filteredColliders = value;
        }


        [ContextMenu(nameof(FindAllColliders))]
        protected virtual void FindAllColliders()
        {
            var allColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

            m_filteredColliders = FilterCollidersByType(allColliders, m_colliderType);
        }


        private Collider[] FilterCollidersByType(Collider[] colliders, TypeOfCollider colliderType)
        {
            if (colliderType == TypeOfCollider.Collider)
            {
                return colliders; // Return all colliders if the selected type is the base Collider type
            }

            var fullTypeName = "UnityEngine." + colliderType + ", UnityEngine";
            var type = Type.GetType(fullTypeName);

            if (type == null || !typeof(Collider).IsAssignableFrom(type))
            {
                Debug.LogError($"Invalid collider type: {colliderType}");

                return Array.Empty<Collider>();
            }

            return Array.FindAll(colliders, coll => coll.GetType() == type);
        }
    }
}