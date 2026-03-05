using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates ServiceLocator for cross-cutting experiment services. Toggle m_muteAudio to register
    ///     NullAudioService for silent testing. The calling code never changes — it always requests
    ///     ServiceLocator.Get&lt;IAudioService&gt;(). This enables the same experiment protocol to run on
    ///     hardware-equipped lab machines and basic laptops in classrooms.
    /// </summary>
    [AddComponentMenu("SOSXR/Design Patterns/Research Services (Service Locator Example)")]
    public class ResearchServicesExample : MonoBehaviour
    {
        [SerializeField] private AudioClip m_instructionAudio;
        [SerializeField] private AudioClip m_feedbackTone;
        [SerializeField] private bool m_muteAudio = false;

        private const string SaveKey = "ResearchServicesExample.ResponseCount";

        private bool _isInstructionPlaying;
        private bool _registeredNullAudioService;
        private bool _registeredSaveService;


        private void Awake()
        {
            if (m_muteAudio)
            {
                ServiceLocator.Register<IAudioService>(new NullAudioService());
                _registeredNullAudioService = true;
            }

            if (!ServiceLocator.TryGet(out ISaveService _))
            {
                ServiceLocator.Register<ISaveService>(new PlayerPrefsSaveService());
                _registeredSaveService = true;
            }
        }


        private void OnDestroy()
        {
            if (_registeredNullAudioService)
            {
                if (ServiceLocator.TryGet(out IAudioService audioService) && audioService is NullAudioService)
                {
                    ServiceLocator.Unregister<IAudioService>();
                }
            }

            if (_registeredSaveService)
            {
                if (ServiceLocator.TryGet(out ISaveService _))
                {
                    ServiceLocator.Unregister<ISaveService>();
                }
            }
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (ServiceLocator.TryGet(out IAudioService audioService))
                {
                    audioService.PlaySFX(m_feedbackTone);
                }
                else
                {
                    Debug.LogWarning("No IAudioService registered. Assign a UnityAudioService or enable Mute Audio.", this);
                }

                ISaveService saveService = ServiceLocator.Get<ISaveService>();
                int responseCount = saveService.LoadInt(SaveKey);
                saveService.SaveInt(SaveKey, responseCount + 1);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleInstructionAudio();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                int responseCount = ServiceLocator.Get<ISaveService>().LoadInt(SaveKey);
                Debug.Log($"Loaded response count: {responseCount}.", this);
            }
        }


        private void ToggleInstructionAudio()
        {
            if (!ServiceLocator.TryGet(out IAudioService audioService))
            {
                Debug.LogWarning("No IAudioService registered.", this);
                return;
            }

            if (_isInstructionPlaying)
            {
                audioService.StopMusic();
            }
            else
            {
                audioService.PlayMusic(m_instructionAudio, true);
            }

            _isInstructionPlaying = !_isInstructionPlaying;
        }
    }
}
