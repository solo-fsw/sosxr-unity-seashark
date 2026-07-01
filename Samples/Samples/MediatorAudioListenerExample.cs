using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    public class MediatorAudioListenerExample : MonoBehaviour
    {
        private Medium _medium;


        private void Awake()
        {
            _medium ??= new Medium("Audio");
        }


        private void OnEnable()
        {
            Mediator.Subscribe(_medium, PlayAudio);
        }


        private void PlayAudio(Medium medium)
        {
            var audioClip = (AudioClip) medium.Data;

            if (audioClip == null)
            {
                Debug.LogError("No audio clip found");

                return;
            }

            _medium = medium;

            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }


        private void OnDisable()
        {
            Mediator.Unsubscribe(_medium, PlayAudio);
        }
    }
}