using UnityEngine;


namespace SOSXR.SeaShark.Attributes.InEditorSamples.Samples
{
    public class SuffixDemo : MonoBehaviour
    {
        [Suffix("SOSXR CAN MAKE LONG SUFFIX")]
        public float Weight;

        [Suffix("m")]
        public float Length;

        [Suffix("s")]
        public int Duration;

        [Suffix("First name")]
        public string Name;

        [Suffix("Bush placement")]
        public Vector2 Trees;
    }
}