using System;
using System.Collections;
using System.Collections.Generic;
using SOSXR.SeaShark.Statics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Plays the video clips in a folder one at a time, in random order, with a pause before the first clip and between clips.
    ///     Repeats the shuffled list while m_repeat is set. Requires a VideoPlayer on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerManager : MonoBehaviour
    {
        [SerializeField]
        private Material m_renderMaterial;

        [Header("Clip Settings")]
        [SerializeField]
        private bool m_startAutomatically = true;

        [SerializeField]
        [Range(0, 60)]
        private float m_beforeFirstClipPauseDuration = 5f;

        /// <summary>
        ///     The clips to play. Start adds one entry for each file found in the clip directory, in addition to any entries already set.
        /// </summary>
        [SerializeField]
        public List<VideoSettingsCustom> Clips;

        [SerializeField]
        private List<VideoSettingsCustom> m_randomizedClipList;

        [SerializeField]
        [Range(0, 60)]
        private float m_betweenEachClipPauseDuration = 2.5f;

        [SerializeField]
        private UnityEvent<string> VideoClipStarted;

        [Header("Info")]
        /// <summary>
        ///     The name of the current clip. Set when a clip starts.
        /// </summary>
        [SerializeField]
        public string CurrentClipName;

        /// <summary>
        ///     The length of the current clip in seconds, rounded to a whole second. Set when playback of the clip begins.
        /// </summary>
        [SerializeField]
        public float CurrentClipDuration;

        /// <summary>
        ///     The playback position of the current clip in seconds, rounded to a whole second. Updated once per second while playing.
        /// </summary>
        [SerializeField]
        public float CurrentClipTime;
        /// <summary>
        ///     The width and height of the current clip in pixels. Set when the render texture is created for the clip.
        /// </summary>
        public Vector2Int Dimensions;

        [SerializeField]
        private string m_clipDirectory;

        [SerializeField]
        private string[] m_extensions;

        [SerializeField]
        private bool m_repeat;

        private RenderTexture _renderTexture;

        private Coroutine _playerCR;
        private Coroutine _clipTimeCR;
        private bool _prepareFailed;

        private const float PrepareTimeout = 30f;

        /// <summary>
        ///     The VideoPlayer that plays the clips. Found on this GameObject or its children in Awake and OnValidate if not already set.
        /// </summary>
        public VideoPlayer VideoPlayer { get; private set; }
        /// <summary>
        ///     The AudioSource that plays the clip audio. Found on this GameObject or its children in Awake and OnValidate if not already set.
        /// </summary>
        public AudioSource AudioSource { get; private set; }

        private void OnValidate()
        {
            ResolveReferences();
        }

        private void Awake()
        {
            ResolveReferences();
        }

        private void ResolveReferences()
        {
            if (VideoPlayer == null)
            {
                VideoPlayer = GetComponentInChildren<VideoPlayer>();
            }

            if (VideoPlayer != null)
            {
                VideoPlayer.source = VideoSource.Url;
            }

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

            var clipNames = FileHelpers.GetFileNamesFromDirectory(
                m_extensions,
                false,
                true,
                m_clipDirectory
            );

            foreach (var clipName in clipNames)
            {
                Clips.Add(new VideoSettingsCustom { ClipName = clipName });
            }

            if (m_startAutomatically)
            {
                StartPlayer(null);
            }
        }

        private void OnEnable()
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.errorReceived += ReceivedAnError;
            }
        }

        private void ReceivedAnError(VideoPlayer source, string message)
        {
            Debug.LogError($"The VideoPlayer has received an error {source} {message}");

            _prepareFailed = true;
        }

        /// <summary>
        ///     Stops any running playback and starts playing the clips. Logs an error and does nothing if there are no clips.
        /// </summary>
        /// <param name="unused">Not used by the method.</param>
        public void StartPlayer(string unused)
        {
            if (Clips.Count == 0)
            {
                Debug.LogError($"Could not find any available clips in {m_clipDirectory}");

                return;
            }

            StopPlayerCoroutines();

            _playerCR = StartCoroutine(PlayerCR());
        }

        private void StopPlayerCoroutines()
        {
            if (_playerCR != null)
            {
                StopCoroutine(_playerCR);
                _playerCR = null;
            }

            if (_clipTimeCR != null)
            {
                StopCoroutine(_clipTimeCR);
                _clipTimeCR = null;
            }
        }

        private IEnumerator PlayerCR()
        {
            yield return new WaitForSeconds(m_beforeFirstClipPauseDuration);

            if (_clipTimeCR != null)
            {
                StopCoroutine(_clipTimeCR);
            }

            _clipTimeCR = StartCoroutine(UpdateCurrentClipTimeCR());

            do
            {
                m_randomizedClipList = new List<VideoSettingsCustom>(Clips);
                ListExtensions.Shuffle(m_randomizedClipList);

                if (m_randomizedClipList.Count == 0)
                {
                    Debug.LogWarning("No clips to play.");

                    yield break;
                }

                foreach (var clip in m_randomizedClipList)
                {
                    Debug.Log(
                        $"Playing clip {clip.ClipName} from {m_randomizedClipList.Count} clips."
                    );

                    VideoClipStarted?.Invoke(clip.ClipName);

                    CurrentClipName = clip.ClipName;

                    _prepareFailed = false;

                    GetURLAndPrepare(clip);

                    var prepareStartTime = Time.time;

                    while (!VideoPlayer.isPrepared && !_prepareFailed)
                    {
                        if (Time.time - prepareStartTime > PrepareTimeout)
                        {
                            Debug.LogError($"Timed out preparing clip {clip.ClipName}");

                            _prepareFailed = true;

                            break;
                        }

                        Debug.LogFormat("Preparing clip");

                        yield return new WaitForSeconds(0.1f);
                    }

                    if (_prepareFailed)
                    {
                        Debug.LogWarning($"Skipping clip {clip.ClipName} because it could not be prepared.");

                        StopPlaying();

                        // Always yield here, so a list of only broken clips with m_repeat on cannot spin without yielding.
                        yield return new WaitForSeconds(Mathf.Max(m_betweenEachClipPauseDuration, 0.1f));

                        continue;
                    }

                    CreateNewRenderTexture();

                    SetAudioSourceSettings(clip);

                    CurrentClipDuration = (float)Math.Round(VideoPlayer.length, 0);

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
            for (; ; )
            {
                CurrentClipTime = (float)Math.Round(VideoPlayer.clockTime, 0);

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
            ReleaseRenderTexture();

            Dimensions.x = (int)VideoPlayer.width;
            Dimensions.y = (int)VideoPlayer.height;
            _renderTexture = new RenderTexture(
                Dimensions.x,
                Dimensions.y,
                24,
                RenderTextureFormat.Default
            );
            _renderTexture.name = "RenderTexture: " + Dimensions;

            if (m_renderMaterial != null)
            {
                m_renderMaterial.mainTexture = _renderTexture;
            }

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
            if (m_renderMaterial != null)
            {
                m_renderMaterial.mainTexture = null;
            }

            ReleaseRenderTexture();
            AudioSource.Stop();
            AudioSource.enabled = false;

            Debug.LogFormat("Stopping playing");
        }

        private void ReleaseRenderTexture()
        {
            if (_renderTexture == null)
            {
                return;
            }

            if (VideoPlayer != null && VideoPlayer.targetTexture == _renderTexture)
            {
                VideoPlayer.targetTexture = null;
            }

            _renderTexture.Release();
            Destroy(_renderTexture);
            _renderTexture = null;
        }

        /// <summary>
        ///     Stops the current playback and starts the clips again in a new random order. Also available from the component's context menu.
        /// </summary>
        [ContextMenu(nameof(ReshuffleVideos))]
        public void ReshuffleVideos()
        {
            StartPlaying();
        }

        private void StartPlaying()
        {
            StopPlaying();

            StopPlayerCoroutines();

            _playerCR = StartCoroutine(PlayerCR());
        }

        private void OnDisable()
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.errorReceived -= ReceivedAnError;
            }

            StopAllCoroutines();
            _playerCR = null;
            _clipTimeCR = null;
        }

        private void OnDestroy()
        {
            ReleaseRenderTexture();
        }
    }
}
