using UnityEngine;


namespace SOSXR.AdditionalUnityEvents
{
    /// <summary>
    ///     This uses non-rotating bounds.Intersects.
    ///     This is faster, and does not rely on the Physics engine.
    ///     However, it is less precise, since the bounding box doesn't strictly rotate along with the object.
    ///     Instead, it shrinks and expands to keep the entire object within the bounds.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class UnityEventIfTaggedBoundsIntersect : AdditionalUnityEvent
    {
        [SerializeField] private string m_tagToCheckAgainst = "MainCamera";
        private GameObject _taggedGameObject;
        private Collider _taggedCollider;
        private Collider _thisCollider;


        private void OnValidate()
        {
            if (_thisCollider != null)
            {
                return;
            }

            _thisCollider = GetComponent<Collider>();
        }


        private void Awake()
        {
            FindTaggedCollider();
        }


        private void FindTaggedCollider()
        {
            if (_taggedCollider != null)
            {
                return;
            }

            _taggedGameObject = GameObject.FindGameObjectWithTag(m_tagToCheckAgainst);

            if (_taggedGameObject != null)
            {
                _taggedCollider = _taggedGameObject.GetComponent<Collider>();
            }
        }


        /// <summary>
        ///     No need to run in FixedUpdate, since this is not a Physics calculation.
        /// </summary>
        private void Update()
        {
            if (_taggedCollider == null)
            {
                FindTaggedCollider();
            }
            else if (_thisCollider.bounds.Intersects(_taggedCollider.bounds))
            {
                FireEvent();
            }
        }
    }
}