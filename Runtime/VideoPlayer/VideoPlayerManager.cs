using System;
using System.Collections;
using System.Collections.Generic;
using SOSXR.SeaShark.Statics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayerManager : MonoBehaviour
    {
        [SerializeField] private Material m_renderMaterial;

        [Header("Clip Settings")]
        [SerializeField] private bool m_startAutomatically = true;
        [SerializeField] [Range(0, 60)] private float m_beforeFirstClipPauseDuration = 5f;
        [SerializeField] public List<VideoSettingsCustom> Clips;
        [SerializeField] private List<VideoSettingsCustom> m_randomizedClipList;
        [SerializeField] [Range(0, 60)] private float m_betweenEachClipPauseDuration = 2.5f;
        [SerializeField] private UnityEvent<string> VideoClipStarted;

        [Header("Info")]
        [SerializeField] public string CurrentClipName;
        [SerializeField] public float CurrentClipDuration;
        [SerializeField] public float CurrentClipTime;
        public Vector2Int Dimensions;


        [SerializeField] private string m_clipDirectory;
        [SerializeField] private string[] m_extensions;
        [SerializeField] private bool m_repeat;


        private RenderTexture _renderTexture;

        private Coroutine _playerCR;

        public UnityEngine.Video.VideoPlayer VideoPlayer { get; private set; }
        public AudioSource AudioSource { get; private set; }


        private void OnValidate()
        {
            if (VideoPlayer == null)
            {
                VideoPlayer = GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
            }

            VideoPlayer.source = VideoSource.Url;

            if (AudioSource == null)
            {
                AudioSource = GetComponentInChildren<AudioSource>();
            }
        }


        private void Start()
        {
            if (VideoPlayer == null || AudioSource == null)
            {
                Debug.LogErrorFormat("VideoPlayer or AudioSource not assigned.");

                enabled = false;

                return;
            }

            var clipNames = FileHelpers.GetFileNamesFromDirectory(m_extensions, false, true, m_clipDirectory);

            foreach (var clipName in clipNames)
            {
                Clips.Add(new VideoSettingsCustom
                {
                    ClipName = clipName
                });
            }

            if (m_startAutomatically)
            {
                StartPlayer(null);
            }
        }


        private void OnEnable()
        {
            VideoPlayer.errorReceived += ReceivedAnError;
        }


        private void ReceivedAnError(UnityEngine.Video.VideoPlayer source, string message)
        {
            Debug.LogErrorFormat($"The VideoPlayer has received an error {source} {message}");
        }


        public void StartPlayer(string unused)
        {
            if (Clips.Count == 0)
            {
                Debug.LogErrorFormat($"Could not find any available clips in {m_clipDirectory}");

                return;
            }

            _playerCR = StartCoroutine(PlayerCR());
        }


        private IEnumerator PlayerCR()
        {
            yield return new WaitForSeconds(m_beforeFirstClipPauseDuration);

            StartCoroutine(UpdateCurrentClipTimeCR());

            do
            {
                //m_randomizedClipList

                foreach (var clip in m_randomizedClipList)
                {
                    Debug.LogFormat($"Playing clip {clip.ClipName} from {m_randomizedClipList.Count} clips.");

                    VideoClipStarted?.Invoke(clip.ClipName);

                    CurrentClipName = clip.ClipName;

                    GetURLAndPrepare(clip);

                    while (!VideoPlayer.isPrepared)
                    {
                        Debug.LogFormat("Preparing clip");

                        yield return new WaitForSeconds(0.1f);
                    }

                    CreateNewRenderTexture();

                    SetAudioSourceSettings(clip);

                    CurrentClipDuration = (float) Math.Round(VideoPlayer.length, 0);

                    VideoPlayer.Play();

                    AudioSource.enabled = true;

                    Debug.LogFormat("Playing clip");

                    yield return new WaitForSeconds(CurrentClipDuration);

                    StopPlaying();

                    yield return new WaitForSeconds(m_betweenEachClipPauseDuration);
                }
            } while (m_repeat);

            Debug.LogFormat("Done playing all clips");
        }


        private IEnumerator UpdateCurrentClipTimeCR()
        {
            for (;;)
            {
                CurrentClipTime = (float) Math.Round(VideoPlayer.clockTime, 0);

                yield return new WaitForSeconds(1);
            }
        }


        private void GetURLAndPrepare(VideoSettingsCustom clip)
        {
            VideoPlayer.url = m_clipDirectory + "/" + clip.ClipName;

            VideoPlayer.Prepare();
        }


        private void CreateNewRenderTexture()
        {
            Dimensions.x = (int) VideoPlayer.width;
            Dimensions.y = (int) VideoPlayer.height;
            _renderTexture = new RenderTexture(Dimensions.x, Dimensions.y, 24, RenderTextureFormat.Default);
            _renderTexture.name = "RenderTexture: " + Dimensions;

            m_renderMaterial.mainTexture = _renderTexture;
            VideoPlayer.targetTexture = _renderTexture;
        }


        private void SetAudioSourceSettings(VideoSettingsCustom clip)
        {
            VideoPlayer.SetTargetAudioSource(0, AudioSource);

            AudioSource.spatialBlend = clip.AudioLocation == Vector3.zero ? 0 : 1;

            AudioSource.transform.position = clip.AudioLocation;
        }


        private void StopPlaying()
        {
            VideoPlayer.Stop();
            VideoPlayer.clip = null;
            m_renderMaterial.mainTexture = new RenderTexture(0, 0, 0);
            AudioSource.Stop();
            AudioSource.enabled = false;

            Debug.LogFormat("Stopping playing");
        }


        [ContextMenu(nameof(ReshuffleVideos))]
        public void ReshuffleVideos()
        {
            StartPlaying();
        }


        private void StartPlaying()
        {
            StopPlaying();

            if (_playerCR != null)
            {
                StopCoroutine(_playerCR);

                _playerCR = null;
            }

            _playerCR = StartCoroutine(PlayerCR());
        }


        private void OnDisable()
        {
            VideoPlayer.errorReceived -= ReceivedAnError;

            StopAllCoroutines();
        }
    }
}