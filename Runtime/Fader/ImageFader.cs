using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Fades the alpha of a child UI Image. FadeIn makes it transparent and FadeOut makes it opaque. If m_fadeOnAwake is set, it fades in on Awake.
    /// </summary>
    public class ImageFader : MonoBehaviour
    {
        [SerializeField] private bool m_fadeOnAwake = false;
        [SerializeField][Range(0f, 10f)] private float m_preFadeInDuration = 0f;
        [SerializeField][Range(0, 10)] private float m_defaultFadeDuration = 4f;
        [SerializeField][Range(0f, 10f)] private float m_preFadeOutDuration = 0f;
        [SerializeField] private AnimationCurve m_fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Image _fadeImage;
        private Coroutine _fadeInCoroutine;
        private Coroutine _fadeOutCoroutine;


        private void OnValidate()
        {
            if (_fadeImage != null)
            {
                return;
            }

            _fadeImage = GetComponentInChildren<Image>();
        }


        private void Awake()
        {
            if (_fadeImage == null)
            {
                _fadeImage = GetComponentInChildren<Image>();
            }

            if (!m_fadeOnAwake)
            {
                return;
            }

            if (_fadeImage == null)
            {
                Debug.LogWarning("We don't have an image to fade. Did you add one? (UI (Canvas) - Image)");

                return;
            }

            FadeIn();
        }


        /// <summary>
        ///     Fades in using the default fade duration. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(FadeIn))]
        public void FadeIn()
        {
            FadeIn(m_defaultFadeDuration);
        }


        /// <summary>
        ///     Fades in over the given duration. Does nothing if a fade-in is already running.
        /// </summary>
        /// <param name="duration">Fade duration in seconds.</param>
        public void FadeIn(float duration)
        {
            if (_fadeInCoroutine != null)
            {
                return;
            }

            _fadeInCoroutine = StartCoroutine(FadeInCR(duration));
        }


        /// <summary>
        ///     Fades out using the default fade duration. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(FadeOut))]
        public void FadeOut()
        {
            FadeOut(m_defaultFadeDuration);
        }


        /// <summary>
        ///     Fades out over the given duration. Enables the image first. Does nothing if a fade-out is already running.
        /// </summary>
        /// <param name="duration">Fade duration in seconds.</param>
        public void FadeOut(float duration)
        {
            if (_fadeOutCoroutine != null)
            {
                return;
            }

            _fadeImage.enabled = true;
            _fadeOutCoroutine = StartCoroutine(FadeOutCR(duration));
        }


        /// <summary>
        ///     Fades out, then fades back in, over twice the default fade duration. Available from the component context menu.
        /// </summary>
        [ContextMenu(nameof(FadeRound))]
        public void FadeRound()
        {
            FadeRound(m_defaultFadeDuration * 2);
        }


        /// <summary>
        ///     Fades out first, then fades back in
        /// </summary>
        public void FadeRound(float totalFadeRoundDuration)
        {
            FadeOut(totalFadeRoundDuration / 2);
            FadeIn(totalFadeRoundDuration / 2);
        }


        private IEnumerator FadeInCR(float duration)
        {
            _fadeImage.gameObject.SetActive(true);
            _fadeImage.enabled = true;

            if (_fadeOutCoroutine != null)
            {
                yield return new WaitUntil(() => _fadeOutCoroutine == null);
            }

            var elapsedTime = 0f;
            var color = _fadeImage.color;

            yield return new WaitForSeconds(m_preFadeInDuration);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = m_fadeCurve.Evaluate(elapsedTime / duration);
                color.a = Mathf.Lerp(1f, 0f, t);
                _fadeImage.color = color;

                yield return null;
            }

            color.a = 0f;
            _fadeImage.color = color;
            _fadeInCoroutine = null;
            _fadeImage.enabled = false;
        }


        private IEnumerator FadeOutCR(float duration)
        {
            if (_fadeInCoroutine != null)
            {
                yield return new WaitUntil(() => _fadeInCoroutine == null);
            }

            var elapsedTime = 0f;
            var color = _fadeImage.color;

            yield return new WaitForSeconds(m_preFadeOutDuration);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = m_fadeCurve.Evaluate(elapsedTime / duration);
                color.a = Mathf.Lerp(0f, 1f, t);
                _fadeImage.color = color;

                yield return null;
            }

            color.a = 1f;
            _fadeImage.color = color;
            _fadeOutCoroutine = null;
        }


        private void OnDisable()
        {
            StopAllCoroutines();
            _fadeInCoroutine = null;
            _fadeOutCoroutine = null;
        }
    }
}
