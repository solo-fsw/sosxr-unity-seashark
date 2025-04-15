﻿#if ENABLED_UNITY_URP
using UnityEngine.Rendering;
#endif


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://github.com/adammyhre/Unity-Utils
    /// </summary>
    public static class ResourcesUtils
    {
        #if ENABLED_UNITY_URP
        /// <summary>
        ///     Load volume profile from given path.
        /// </summary>
        /// <param name="path">Path from where volume profile should be loaded.</param>
        public static void LoadVolumeProfile(this Volume volume, string path)
        {
            var profile = Resources.Load<VolumeProfile>(path);
            volume.profile = profile;
        }
        #endif
    }
}