using System;
using UnityEngine;


namespace SOSXR.SeaShark.VideoPlayer
{
    [Serializable]
    public class VideoSettingsCustom
    {
        [Tooltip("Full name, without path, but with extension. E.g. 'myVideo.mp4'")]
        public string ClipName;
        public Vector3 AudioLocation;
    }
}