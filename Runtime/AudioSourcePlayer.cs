using System.Collections;
using SOSXR.SeaShark;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Plays configured audio clips with optional delays and callback hooks.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioSourcePlayer : MonoBehaviour
{
    private const float ClearHapticDelaySeconds = 5f;
    private static readonly WaitForSeconds s_waitForClearHaptic = new(ClearHapticDelaySeconds);

    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private int m_index = 0;
    [SerializeField][DisableEditing] private string m_currentClip;
    [SerializeField][DisableEditing] private bool m_looping;

    [SerializeField][HideInInspector] private AudioSource _audioSource;

    [Tooltip("ms")]
    [SerializeField][Range(0, 1000)] private int m_delay;

    [SerializeField] private UnityEvent<int> m_beforeDelay;
    [SerializeField] private UnityEvent<int> m_onPlay;
    [SerializeField] private UnityEvent<int> m_afterDelay;

    private Coroutine _play;


    /// <summary>
    /// Synchronizes serialized inspector fields with the current <see cref="AudioSource"/>.
    /// </summary>
    private void OnValidate()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_audioSource != null)
        {
            m_looping = _audioSource.loop;
        }
    }


    /// <summary>
    /// Toggles looping on the backing <see cref="AudioSource"/>.
    /// </summary>
    [Button]
    private void SetLoop()
    {
        if (_audioSource.isPlaying)
        {
            Stop();
        }

        _audioSource.loop = !_audioSource.loop;

        m_looping = _audioSource.loop;
        Debug.Log(string.Concat("Looping is now ", m_looping.ToString()));
    }


    /// <summary>
    /// Stops the active clip immediately.
    /// </summary>
    [Button]
    private void Stop()
    {
        _audioSource.Stop();
    }


    /// <summary>
    /// Advances to the next configured clip.
    /// </summary>
    [Button]
    private void SetNextClip()
    {
        if (_audioSource.isPlaying)
        {
            Stop();
        }

        m_index++;

        if (m_index >= _clips.Length)
        {
            m_index = 0;
        }

        _audioSource.clip = _clips[m_index];
        m_currentClip = _clips[m_index].name;
    }


    /// <summary>
    /// Starts playback routine for the current clip.
    /// </summary>
    [Button]
    private void Play()
    {
        if (_play != null)
        {
            StopCoroutine(_play);
        }

        _play = StartCoroutine(PlayCR());
    }


    /// <summary>
    /// Handles delayed playback and callback timing for the current clip.
    /// </summary>
    private IEnumerator PlayCR()
    {
        if (_audioSource.isPlaying)
        {
            Stop();

            Debug.Log("Source was still playing, will wait 5 seconds to clear the haptic effect");

            yield return s_waitForClearHaptic;
        }

        _audioSource.clip = _clips[m_index];
        // Name read allocates, but this is inspector/debug state on a one-shot path, not a per-frame hot path.
        m_currentClip = _clips[m_index].name;

        var m_delaySec = m_delay / 1000f;
        var waitForDelay = m_delaySec > 0f ? new WaitForSeconds(m_delaySec) : null;
        Debug.Log(string.Concat("Before delay of ", m_delay.ToString(), " ms (which is ", m_delaySec.ToString(), " seconds)"));
        m_beforeDelay?.Invoke(m_delay);

        if (waitForDelay != null)
        {
            // Reuse the same wait instruction within this playback pass instead of allocating twice.
            yield return waitForDelay;
        }

        _audioSource.Play();
        var clipLength = _clips[m_index].length;
        var clipDurationMS = (int)(clipLength * 1000);
        Debug.Log(string.Concat("OnPlay, with clip duration: ", clipLength.ToString(), " sec, which is ", clipDurationMS.ToString(), " ms"));
        m_onPlay?.Invoke(clipDurationMS);

        yield return new WaitForSeconds(_audioSource.clip.length);

        if (waitForDelay != null)
        {
            yield return waitForDelay;
        }

        Debug.Log(string.Concat("After delay of ", m_delay.ToString(), " ms (which is ", m_delaySec.ToString(), " seconds)"));
        m_afterDelay?.Invoke(m_delay);

        // this.Verbose("Playing");
    }


    /// <summary>
    /// Ensures pending playback routines stop when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
