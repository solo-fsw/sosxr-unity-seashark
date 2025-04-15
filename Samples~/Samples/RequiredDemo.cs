using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    public class RequiredDemo : MonoBehaviour
    {
        [Required]
        public GameObject m_gameObject;

        [Optional]
        public string m_string;

        [Optional(OptionalType.WillFind)]
        public int m_int;

        [Optional(OptionalType.WillGet)]
        public Transform m_transform;
    }
}