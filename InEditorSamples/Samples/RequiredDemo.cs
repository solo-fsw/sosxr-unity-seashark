using UnityEngine;


namespace SOSXR.SeaShark.Attributes.InEditorSamples.Samples
{
    public class RequiredDemo : MonoBehaviour
    {
        [Required]
        public GameObject m_gameObject;

        [Required]
        public string m_string;

        [Required]
        public int m_int;

        [Required]
        public Transform m_transform;
    }
}