using System;
using System.Collections;
using UnityEngine;


namespace SOSXR.AdditionalUnityEvents
{
    /// <summary>
    ///     Class to fire a Unity Event every x interval.
    ///     With options to change the interval on every time it fires.
    ///     With options to set a min/max allowed interval
    /// </summary>
    public class UnityEventVariableIntervalLoop : AdditionalUnityEvent
    {
        [Header("Settings")]
        [SerializeField] private bool m_autoStart;
        [SerializeField] private float m_initialFireInterval = 1.3f;
        [SerializeField] private float m_perIntervalChange = 0.02f;
        [SerializeField] private Vector2 m_minMax = new(0.5f, 2f);


        [Header("Not for editing")]
        [SerializeField] private float m_currentFireInterval;

        private Coroutine _coroutine;


        public void OnValidate()
        {
            m_currentFireInterval = m_initialFireInterval;
        }


        private void Start()
        {
            if (m_autoStart)
            {
                StartEventFiringLoop();
            }
        }


        [ContextMenu(nameof(StartEventFiringLoop))]
        public void StartEventFiringLoop()
        {
            if (_coroutine == null)
            {
            }
            else
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(FireEventOnLoopCR());
        }


        private IEnumerator FireEventOnLoopCR()
        {
            m_currentFireInterval = m_initialFireInterval;

            for (;;)
            {
                FireEvent();

                yield return new WaitForSeconds(m_currentFireInterval);

                if (m_currentFireInterval + m_perIntervalChange >= m_minMax.x && m_currentFireInterval + m_perIntervalChange <= m_minMax.y)
                {
                    m_currentFireInterval += m_perIntervalChange;
                    m_currentFireInterval = (float) Math.Round(m_currentFireInterval, 2, MidpointRounding.AwayFromZero);
                }
            }
        }


        [ContextMenu(nameof(StopFiringLoop))]
        public void StopFiringLoop()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}