using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace SOSXR.SeaShark.DeviceDependentHelpers
{
    public class CheckDeviceType : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("operatingSystem: " + SystemInfo.operatingSystem);
            Debug.Log("deviceModel: " + SystemInfo.deviceModel);
            Debug.Log("deviceName: " + SystemInfo.deviceName);
            Debug.Log("deviceType: " + SystemInfo.deviceType);
            Debug.Log("platform: " + Application.platform);

            var devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            foreach (var device in devices)
            {
                Debug.Log("Device connected: " + device.name);
            }
        }
    }
}