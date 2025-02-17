using System;
using UnityEngine;


[CreateAssetMenu(menuName = "SOSXR/CurrentDevice", fileName = "CurrentDevice")]
public class CurrentDevice : ScriptableObject
{
    public Device Current;

    public string DeviceName { get; set; }
}


[Serializable]
public enum Device
{
    None,
    HMD,
    Tablet,
    Editor
}