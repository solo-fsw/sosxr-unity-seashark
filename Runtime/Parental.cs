using UnityEngine;


namespace SOSXR.SeaShark
{
    public class Parental : MonoBehaviour
    {
        [SerializeField] private GameObject m_adoptiveParent;
        [SerializeField] private bool m_doOnEnable = true;
        [SerializeField] private bool m_zeroPosition;
        [SerializeField] private bool m_zeroRotation;


        private void OnEnable()
        {
            if (m_doOnEnable)
            {
                ParentTo();
            }
        }


        [Button]
        private void ParentTo()
        {
            if (m_adoptiveParent != null && enabled)
            {
                transform.SetParent(m_adoptiveParent.transform);

                if (m_zeroPosition)
                {
                    transform.localPosition = Vector3.zero;
                }

                if (m_zeroRotation)
                {
                    transform.localRotation = Quaternion.identity;
                }

                //this.Verbose($"This is now parented to {m_adoptiveParent.name} (zeroPosition: {m_zeroPosition} / zeroRotation: {m_zeroRotation})");
            }
        }
    }
}