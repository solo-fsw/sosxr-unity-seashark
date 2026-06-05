using System.IO;
using SOSXR.SeaShark;
using UnityEngine;


/// <summary>
/// Generates simple tones for preview playback and WAV export.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SimpleToneGenerator : MonoBehaviour
{
    [Range(1, 500)][SerializeField] private int m_frequency = 120;
    [Range(0, 1)][SerializeField] private float m_amplitude = 0.1f;
    [SerializeField][Range(100, 5000)] private int m_durationMS = 500;

    [SerializeField][DisableEditing] private AudioSource _audioSource;
    [SerializeField][DisableEditing] private int _sampleRate;
    [SerializeField][DisableEditing] private float _currentAmplitude;

    [SerializeField] private Vector3Int m_rangeFrequency = new(40, 100, 5);
    [SerializeField] private string m_saveDirectory = "Assets/_SOSXR/Audio/Generated/";

    private readonly float _fadeInDuration = 0.025f;
    private readonly float _fadeOutDuration = 0.025f;

    private bool _playing;
    private double _phase;
    private double _increment;
    private float _timeRemaining;


    /// <summary>
    /// Refreshes cached audio settings and inspector references.
    /// </summary>
    private void OnValidate()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        _sampleRate = AudioSettings.outputSampleRate;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _currentAmplitude = 0f;
    }


    /// <summary>
    /// Starts continuous tone playback.
    /// </summary>
    [Button]
    public void StartTone()
    {
        if (_playing)
        {
            return;
        }

        _playing = true;
        _timeRemaining = m_durationMS;
        _audioSource.Play();
    }


    /// <summary>
    /// Plays the tone for a caller-provided duration.
    /// </summary>
    /// <param name="duration">Playback duration.</param>
    public void PlayForDuration(float duration)
    {
        if (_playing)
        {
            return;
        }

        _timeRemaining = duration;
        StartTone();
    }


    /// <summary>
    /// Plays the tone for the configured serialized duration.
    /// </summary>
    [Button]
    public void PlayForDuration()
    {
        PlayForDuration(m_durationMS);
    }


    /// <summary>
    /// Stops tone playback.
    /// </summary>
    [Button]
    public void StopTone()
    {
        _playing = false;
    }


    /// <summary>
    /// Exports a batch of tones across the configured frequency range.
    /// </summary>
    [Button]
    public void GenerateRangeOfTones()
    {
        for (var freq = m_rangeFrequency.x; freq <= m_rangeFrequency.y; freq += m_rangeFrequency.z)
        {
            m_frequency = freq;
            GenerateAndSaveWav();
        }
    }


    /// <summary>
    /// Exports the current tone settings to a WAV file.
    /// </summary>
    [Button]
    public void GenerateAndSaveWav()
    {
        var fileName = $"Tone_{m_frequency.ToString()}Hz_{(m_amplitude * 100f).ToString("0")}pct_{m_durationMS.ToString()}ms.wav";

        if (!Directory.Exists(m_saveDirectory))
        {
            Directory.CreateDirectory(m_saveDirectory);
        }

        var fullPath = Path.Combine(m_saveDirectory, fileName);
        ToneExporter.GenerateAndSaveWav(m_frequency, m_amplitude, m_durationMS, fullPath);
        Debug.Log(string.Concat("Tone saved to: ", fullPath));
    }


    /// <summary>
    /// Updates playback state for timed tones.
    /// </summary>
    private void Update()
    {
        if (_playing && _timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0f)
            {
                _playing = false;
            }
        }
    }


    /// <summary>
    /// Generates audio samples on Unity's audio thread.
    /// </summary>
    /// <param name="data">Output sample buffer.</param>
    /// <param name="channels">Channel count for the current output device.</param>
    private void OnAudioFilterRead(float[] data, int channels)
    {
        // Keep audio callback allocation-free; no managed allocations inside this loop.
        _increment = m_frequency * 2.0 * Mathf.PI / _sampleRate;

        var targetAmplitude = _playing ? m_amplitude : 0f;
        var fadeStep = 1f / _sampleRate; // per sample step, will scale below

        for (var i = 0; i < data.Length; i += channels)
        {
            // Linear fade in/out
            if (_currentAmplitude < targetAmplitude)
            {
                _currentAmplitude += fadeStep / Mathf.Max(_fadeInDuration, 0.001f);
            }
            else if (_currentAmplitude > targetAmplitude)
            {
                _currentAmplitude -= fadeStep / Mathf.Max(_fadeOutDuration, 0.001f);
            }

            _currentAmplitude = Mathf.Clamp01(_currentAmplitude);

            var sample = Mathf.Sin((float)_phase) * _currentAmplitude;

            for (var c = 0; c < channels; c++)
            {
                data[i + c] = sample;
            }

            _phase += _increment;

            if (_phase > 2.0 * Mathf.PI)
            {
                _phase -= 2.0 * Mathf.PI;
            }
        }
    }
}
