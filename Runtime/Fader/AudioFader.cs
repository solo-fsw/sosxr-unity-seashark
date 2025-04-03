using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Supports setting the desired AudioSource(s) in the inspector, or to search for all available AudioSources in the
    ///     scene.
    /// </summary>
    public class AudioFader : MonoBehaviour
    {
        [SerializeField] private List<AudioSource> m_sources;
        [SerializeField] private float m_duration = 2.5f;
        [SerializeField] private float m_targetVolume = 0.35f;
        [SerializeField] private AnimationCurve m_fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);


        [ContextMenu(nameof(FindAllAudioSourcesInScene))]
        public void FindAllAudioSourcesInScene()
        {
            m_sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).ToList();
        }


        [ContextMenu(nameof(StartAudioFadeOut))]
        public void StartAudioFadeOut()
        {
            AudioFade(m_duration, 0f);
        }


        [ContextMenu(nameof(StartAudioFadeIn))]
        public void StartAudioFadeIn()
        {
            AudioFade(m_duration, 1f);
        }


        [ContextMenu(nameof(StartAudioFadeTo))]
        public void StartAudioFadeTo()
        {
            AudioFade(m_duration, m_targetVolume);
        }


        public void AudioFade(float fadeDuration, float targetVolume)
        {
            StopAllCoroutines();

            foreach (var source in m_sources)
            {
                StartCoroutine(AudioFadeCR(source, fadeDuration, targetVolume));
            }
        }


        private IEnumerator AudioFadeCR(AudioSource audioSource, float fadeDuration, float targetVolume)
        {
            if (audioSource == null || !audioSource.gameObject.activeInHierarchy)
            {
                yield break;
            }

            float currentTime = 0;
            var startVolume = audioSource.volume;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;

                if (audioSource == null || !audioSource.gameObject.activeInHierarchy)
                {
                    yield break;
                }

                var t = currentTime / fadeDuration;

                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, m_fadeCurve.Evaluate(1 - t));

                yield return null;
            }

            audioSource.volume = targetVolume;
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}