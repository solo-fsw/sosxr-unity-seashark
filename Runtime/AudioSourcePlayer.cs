using System.Collections;
using SOSXR.EnhancedLogger;
using SOSXR.SeaShark;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(AudioSource))]
public class AudioSourcePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private int m_index = 0;
    [SerializeField] [DisableEditing] private string m_currentClip;
    [SerializeField] [DisableEditing] private bool m_looping;

    [SerializeField] [HideInInspector] private AudioSource _audioSource;

    [Tooltip("ms")]
    [SerializeField] [Range(0, 1000)] private int m_delay;

    [SerializeField] private UnityEvent<int> m_beforeDelay;
    [SerializeField] private UnityEvent<int> m_onPlay;
    [SerializeField] private UnityEvent<int> m_afterDelay;

    private Coroutine _play;


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


    [Button]
    private void SetLoop()
    {
        if (_audioSource.isPlaying)
        {
            Stop();
        }

        _audioSource.loop = !_audioSource.loop;

        m_looping = _audioSource.loop;
        this.Verbose($"Looping is now {m_looping}");
    }


    [Button]
    private void Stop()
    {
        _audioSource.Stop();
    }


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


    [Button]
    private void Play()
    {
        if (_play != null)
        {
            StopCoroutine(_play);
        }

        _play = StartCoroutine(PlayCR());
    }


    private IEnumerator PlayCR()
    {
        if (_audioSource.isPlaying)
        {
            Stop();

            var waitTime = 5f;
            this.Warning($"Source was still playing, will wait {waitTime} seconds to clear the haptic effect");

            yield return new WaitForSeconds(waitTime);
        }

        _audioSource.clip = _clips[m_index];
        m_currentClip = _clips[m_index].name;

        var m_delaySec = m_delay / (float) 1000;
        this.Verbose($"Before delay of {m_delay} ms (which is {m_delaySec} seconds)");
        m_beforeDelay?.Invoke(m_delay);

        yield return new WaitForSeconds(m_delaySec);

        _audioSource.Play();
        var clipLength = _clips[m_index].length;
        var clipDurationMS = (int) (clipLength * 1000);
        this.Verbose($"OnPlay, with clip duration: {clipLength} sec, which is {clipDurationMS} ms");
        m_onPlay?.Invoke(clipDurationMS);

        yield return new WaitForSeconds(_audioSource.clip.length);

        yield return new WaitForSeconds(m_delaySec);
        this.Verbose($"After delay of {m_delay} ms (which is {m_delaySec} seconds)");
        m_afterDelay?.Invoke(m_delay);

        // this.Verbose("Playing");
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}