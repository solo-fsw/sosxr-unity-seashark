using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    public class MediatorUIExample : MonoBehaviour
    {
        [Mediator] [SerializeField] private Medium m_medium;

        public Stack<string> Stacking;


        private void OnEnable()
        {
            Mediator.Subscribe(m_medium, UpdateText);
        }


        private void UpdateText(Medium medium)
        {
            if (m_medium.Data == null)
            {
                Debug.LogWarningFormat(this, "This channel's medium.Data seems to be null... is that correct?");
            }

            var text = (string) medium.Data;

            if (text == null)
            {
                Debug.LogWarningFormat(this, "No text found");

                return;
            }

            m_medium = medium;

            Debug.LogFormat(this, text);
        }


        private void OnDisable()
        {
            Mediator.Unsubscribe(m_medium, UpdateText);
        }
    }
}