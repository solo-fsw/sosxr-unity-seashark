using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Implements the Object Pool design pattern for <see cref="GameObject"/> instances in Unity.
    ///     <para>
    ///         <b>What:</b> This component pre-creates and reuses objects instead of creating and destroying them every time they are needed.
    ///     </para>
    ///     <para>
    ///         <b>Why:</b> Frequent <see cref="Object.Instantiate(Object)"/> and <see cref="Object.Destroy(Object)"/> calls allocate memory and produce garbage collection spikes,
    ///         which can cause frame hitches in timing-critical experimental systems.
    ///     </para>
    ///     <para>
    ///         <b>How:</b> Inactive objects are stored in an internal queue, active objects are tracked in a list, and requests are served by reactivating queued objects.
    ///         If the pool is exhausted, it can either expand (up to a maximum) or recycle the oldest active instance.
    ///     </para>
    ///     <para>
    ///         Use this pattern for high-frequency presentation workflows such as RSVP streams, visual search arrays, and trial feedback indicators.
    ///         It is also useful for stimuli, response indicators, visual markers, and temporary feedback elements.
    ///         Unity also provides <c>UnityEngine.Pool.ObjectPool&lt;T&gt;</c>; this implementation intentionally avoids it so developers can learn the underlying mechanics directly.
    ///     </para>
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/GameObject Pool")]
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private int m_initialSize = 10;
        [SerializeField] private int m_maxSize = 50;
        [SerializeField] private bool m_expandable = true;

        private readonly Queue<GameObject> _available = new();
        private readonly List<GameObject> _active = new();


        /// <summary>
        ///     Gets the number of objects currently checked out of the pool and active in the scene.
        /// </summary>
        public int ActiveCount => _active.Count;


        /// <summary>
        ///     Gets the number of inactive objects currently ready to be reused.
        /// </summary>
        public int AvailableCount => _available.Count;


        private void Awake()
        {
            if (m_prefab == null)
            {
                Debug.LogError($"{nameof(GameObjectPool)} requires a prefab reference.", this);
                return;
            }

            m_initialSize = Mathf.Max(0, m_initialSize);
            m_maxSize = Mathf.Max(1, m_maxSize);

            if (m_initialSize > m_maxSize)
            {
                m_initialSize = m_maxSize;
            }

            for (var i = 0; i < m_initialSize; i++)
            {
                CreateInstance(addToAvailable: true);
            }
        }


        /// <summary>
        ///     Retrieves an instance from the pool, positions it, rotates it, and activates it.
        ///     If no inactive instance is available, the pool expands (when allowed) or recycles the oldest active object.
        /// </summary>
        /// <param name="position">World position to assign to the retrieved object.</param>
        /// <param name="rotation">World rotation to assign to the retrieved object.</param>
        /// <returns>A pooled and active <see cref="GameObject"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no object can be provided due to pool constraints.</exception>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            if (m_prefab == null)
            {
                throw new InvalidOperationException($"{nameof(GameObjectPool)} cannot provide objects because no prefab is assigned.");
            }

            CleanupNullEntries();

            if (_available.Count == 0)
            {
                var totalCount = _available.Count + _active.Count;

                if (m_expandable && totalCount < m_maxSize)
                {
                    CreateInstance(addToAvailable: true);
                }
                else if (_active.Count > 0)
                {
                    var oldestActive = _active[0];
                    Return(oldestActive);
                }
            }

            if (_available.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(GameObjectPool)} on '{name}' could not provide an object because the pool is exhausted.");
            }

            var instance = _available.Dequeue();
            _active.Add(instance);

            var instanceTransform = instance.transform;
            instanceTransform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);

            return instance;
        }


        /// <summary>
        ///     Returns an active object to the pool, deactivating it and enqueueing it for reuse.
        /// </summary>
        /// <param name="obj">The pooled object to return.</param>
        public void Return(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            var activeIndex = _active.IndexOf(obj);

            if (activeIndex < 0)
            {
                return;
            }

            _active.RemoveAt(activeIndex);
            obj.SetActive(false);
            _available.Enqueue(obj);
        }


        /// <summary>
        ///     Returns every currently active pooled object back to the available queue.
        /// </summary>
        public void ReturnAll()
        {
            for (var i = _active.Count - 1; i >= 0; i--)
            {
                var instance = _active[i];

                if (instance == null)
                {
                    _active.RemoveAt(i);
                    continue;
                }

                instance.SetActive(false);
                _available.Enqueue(instance);
                _active.RemoveAt(i);
            }
        }


        private void CreateInstance(bool addToAvailable)
        {
            var instance = Instantiate(m_prefab, transform);
            instance.SetActive(false);

            var pooledObject = instance.GetComponent<PooledObject>();

            if (pooledObject == null)
            {
                pooledObject = instance.AddComponent<PooledObject>();
            }

            pooledObject.Initialize(this);

            if (addToAvailable)
            {
                _available.Enqueue(instance);
            }
        }


        private void CleanupNullEntries()
        {
            _active.RemoveAll(activeObject => activeObject == null);

            if (_available.Count == 0)
            {
                return;
            }

            var retained = new Queue<GameObject>(_available.Count);

            while (_available.Count > 0)
            {
                var entry = _available.Dequeue();

                if (entry != null)
                {
                    retained.Enqueue(entry);
                }
            }

            while (retained.Count > 0)
            {
                _available.Enqueue(retained.Dequeue());
            }
        }
    }
}
