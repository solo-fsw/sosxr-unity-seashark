using SOSXR.SeaShark.Patterns.Mediator;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.InEditorSamples.Samples
{
    public class MediatorTruthExample : MonoBehaviour
    {
        [Mediator] [SerializeField] private Medium m_medium;


        private void OnEnable()
        {
            Mediator.Subscribe(m_medium, IsThatSo);
        }


        private void IsThatSo(Medium medium)
        {
            var isBool = medium.Data is bool;

            if (!isBool)
            {
                Debug.LogError("This is super false because this aint a bool");

                return;
            }

            m_medium = medium;

            var truth = (bool) medium.Data;

            Debug.LogFormat("Do we have any idea what we're doing? {0}", truth);
        }


        private void OnDisable()
        {
            Mediator.Unsubscribe(m_medium, IsThatSo);
        }
    }
}