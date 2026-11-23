using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [CreateAssetMenu(menuName = "SOSXR/CurrentDevice", fileName = "CurrentDevice")]
    public class CurrentDevice : ScriptableObject
    {
        public Device Current;

        public string DeviceName { get; set; }

        /// <summary>Raised by <see cref="Set" /> after the current device changed.</summary>
        public event Action<CurrentDevice> Changed;


        /// <summary>Sets <see cref="Current" /> and raises <see cref="Changed" />.</summary>
        public void Set(Device device)
        {
            Current = device;
            Changed?.Invoke(this);
        }
    }


    [Serializable]
    public enum Device
    {
        None,
        HMD,
        Tablet,
        Editor
    }
}