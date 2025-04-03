using UnityEngine;


namespace SOSXR.SeaShark.Patterns.Command
{
    public class Player : MonoBehaviour
    {
        public void Move(Vector3 direction)
        {
            transform.position += direction;
        }


        public void PlayAudio(AudioClip audioClip)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }
}