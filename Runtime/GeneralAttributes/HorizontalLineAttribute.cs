using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From Warped Imagination: https://youtu.be/533OH2m7fNg?si=hfyYb2p9s5WBl1vP
    /// </summary>
    public class HorizontalLineAttribute : PropertyAttribute
    {
        public float Thickness { get; set; } = 2.5f;

        public float Padding { get; set; } = 12.5f;
    }
}