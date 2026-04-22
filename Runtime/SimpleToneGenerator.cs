using System.IO;
using SOSXR.EnhancedLogger;
using SOSXR.SeaShark;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SimpleToneGenerator : MonoBehaviour
{
    [Range(1, 500)] [SerializeField] private int m_frequency = 120;
    [Range(0, 1)] [SerializeField] private float m_amplitude = 0.1f;
    [SerializeField] [Range(100, 5000)] private int m_durationMS = 500;

    [SerializeField] [DisableEditing] private AudioSource _audioSource;
    [SerializeField] [DisableEditing] private int _sampleRate;
    [SerializeField] [DisableEditing] private float _currentAmplitude;

    [SerializeField] private Vector3Int m_rangeFrequency = new(40, 100, 5);
    [SerializeField] private string m_saveDirectory = "Assets/_SOSXR/Audio/Generated/";

    private readonly float _fadeInDuration = 0.025f;
    private readonly float _fadeOutDuration = 0.025f;

    private bool _playing;
    private double _phase;
    private double _increment;
    private float _timeRemaining;


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


    public void PlayForDuration(float duration)
    {
        if (_playing)
        {
            return;
        }

        _timeRemaining = duration;
        StartTone();
    }


    [Button]
    public void PlayForDuration()
    {
        PlayForDuration(m_durationMS);
    }


    [Button]
    public void StopTone()
    {
        _playing = false;
    }


    [Button]
    public void GenerateRangeOfTones()
    {
        for (var freq = m_rangeFrequency.x; freq <= m_rangeFrequency.y; freq += m_rangeFrequency.z)
        {
            m_frequency = freq;
            GenerateAndSaveWav();
        }
    }


    [Button]
    public void GenerateAndSaveWav()
    {
        var fileName = $"Tone_{m_frequency}Hz_{m_amplitude * 100f:0}pct_{m_durationMS}ms.wav";

        if (!Directory.Exists(m_saveDirectory))
        {
            Directory.CreateDirectory(m_saveDirectory);
        }

        var fullPath = Path.Combine(m_saveDirectory, fileName);
        ToneExporter.GenerateAndSaveWav(m_frequency, m_amplitude, m_durationMS, fullPath);
        this.Info($"Tone saved to: {fullPath}");
    }


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


    private void OnAudioFilterRead(float[] data, int channels)
    {
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

            var sample = Mathf.Sin((float) _phase) * _currentAmplitude;

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