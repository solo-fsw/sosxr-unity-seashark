using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SimpleHelpers
{
    /// <summary>
    ///     Performs actions when a raycast hits a target within a specified distance.
    /// </summary>
    public class RaycastUnityEventTrigger : MonoBehaviour
    {
        public float RaycastDistance = 1f;

        public UnityEvent<RaycastHit> OnRaycastHit;
        public UnityEvent OnRaycastMiss;

        public RaycastHit Hit { get; private set; }


        private void FixedUpdate()
        {
            var ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out var hit, RaycastDistance))
            {
                Hit = hit;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                OnRaycastHit?.Invoke(hit);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * RaycastDistance, Color.red);
                Hit = new RaycastHit();
                OnRaycastMiss?.Invoke();
            }
        }
    }
}