using UnityEngine;


namespace SOSXR.SeaShark
{
    public class ToggleMultipleTogglers : MonoBehaviour
    {
        [SerializeField] private ToggleMultipleRenderers[] m_togglers;


        [Button]
        public void Toggle(bool setEnabled)
        {
            foreach (var toggler in m_togglers)
            {
                toggler.Toggle(setEnabled);
            }
        }
    }
}