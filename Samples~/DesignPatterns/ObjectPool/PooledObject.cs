using System.Collections;
using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Provides convenience behavior for pooled objects so they can return themselves to their owning <see cref="GameObjectPool"/>.
    ///     This supports reusable stimulus displays, feedback indicators, and temporary visual elements that should self-despawn after a delay.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        [SerializeField] private float m_autoReturnTime = 0f;

        private GameObjectPool _pool;
        private Coroutine _autoReturnRoutine;


        /// <summary>
        ///     Assigns the parent pool that owns this instance.
        /// </summary>
        /// <param name="pool">The pool responsible for this object's lifecycle.</param>
        public void Initialize(GameObjectPool pool)
        {
            _pool = pool;
        }


        /// <summary>
        ///     Returns this object to its assigned pool.
        /// </summary>
        public void ReturnToPool()
        {
            if (_pool == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _pool.Return(gameObject);
        }


        private void OnEnable()
        {
            if (m_autoReturnTime <= 0f)
            {
                return;
            }

            _autoReturnRoutine = StartCoroutine(AutoReturnAfterDelay());
        }


        private void OnDisable()
        {
            if (_autoReturnRoutine == null)
            {
                return;
            }

            StopCoroutine(_autoReturnRoutine);
            _autoReturnRoutine = null;
        }


        private IEnumerator AutoReturnAfterDelay()
        {
            yield return new WaitForSeconds(m_autoReturnTime);

            _autoReturnRoutine = null;
            ReturnToPool();
        }
    }
}
