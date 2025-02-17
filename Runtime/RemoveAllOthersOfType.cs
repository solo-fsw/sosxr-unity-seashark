using System;
using UnityEngine;



namespace SOSXR.SimpleHelpers
{
    public enum RemovalAction
    {
        Disable,
        Destroy
    }


    public class RemoveAllOthersOfType : MonoBehaviour
    {
        [SerializeField] private bool m_autoRemoveOnAwake = true;
        [Tooltip("Drag a component of the type you want to control.")]
      
        [SerializeField] private MonoBehaviour m_allowedComponent;

        [SerializeField] private RemovalAction m_removalAction = RemovalAction.Disable;


        private void Awake()
        {
            if (!m_autoRemoveOnAwake)
            {
                return;
            }

            RemoveOtherObjects();
        }


        [ContextMenu(nameof(RemoveAllOthersOfType))]
        public void RemoveOtherObjects()
        {
            if (m_allowedComponent == null)
            {
                Debug.LogWarning($"No component assigned to {nameof(RemoveAllOthersOfType)} on {gameObject.name}. This script will not function.");

                return;
            }

            var type = m_allowedComponent.GetType();
            ProcessOtherObjectsOfType(type);
        }


        private void ProcessOtherObjectsOfType(Type type)
        {
            var objects = FindObjectsByType(type, FindObjectsSortMode.None);

            foreach (var obj in objects)
            {
                if (obj == m_allowedComponent)
                {
                    continue;
                }

                if (obj is MonoBehaviour monoBehaviour)
                {
                    PerformActionOnObject(monoBehaviour, type);
                }
                else
                {
                    Debug.LogWarning($"{obj.name} is not a MonoBehaviour and cannot be processed.");
                }
            }
        }


        private void PerformActionOnObject(MonoBehaviour obj, Type type)
        {
            switch (m_removalAction)
            {
                case RemovalAction.Destroy:
                    if (Application.isPlaying)
                    {
                        Destroy(obj);
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }

                    Debug.Log($"{obj.name} of type {type.Name} has been destroyed.");

                    break;

                case RemovalAction.Disable:
                    obj.enabled = false;
                    Debug.Log($"{obj.name} of type {type.Name} has been disabled.");

                    break;

                default:
                    Debug.LogWarning($"Unknown action {m_removalAction}. No action taken on {obj.name}.");

                    break;
            }
        }
    }
}