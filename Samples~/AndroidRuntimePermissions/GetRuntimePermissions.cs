using System;
using System.Threading.Tasks;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class GetRuntimePermission : MonoBehaviour
    {
        [SerializeField] private string[] m_additionalPermissions = { }; // e.g. "android.permission.READ_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE"


        private void Awake()
        {
            #if !UNITY_ANDROID || UNITY_EDITOR
            Debug.Log("This script is only for Android platform. Skipping permission checks.");
            enabled = false;

            return;
            #endif

            _ = RequestAllFilesPermission();
            _ = RequestAdditionalPermissionsAsync();
        }


        private async Task RequestAllFilesPermission()
        {
            if (GetAndroidAPILevel() >= 30 && !AndroidHasManageAllFilesPermission())
            {
                OpenManageAllFilesAccessSettings();
            }
        }


        private async Task RequestAdditionalPermissionsAsync()
        {
            foreach (var additionalPermission in m_additionalPermissions) // Skips if empty
            {
                await RequestPermission(additionalPermission);
            }
        }


        private int GetAndroidAPILevel()
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                var sdkInt = version.GetStatic<int>("SDK_INT");
                Debug.Log($"Android API Level: {sdkInt}");

                return sdkInt;
            }
        }


        private bool AndroidHasManageAllFilesPermission()
        {
            using (var env = new AndroidJavaClass("android.os.Environment"))
            {
                var isExternalStorageManager = env.CallStatic<bool>("isExternalStorageManager");
                Debug.Log($"Android has Manage All Files Access Permission: {isExternalStorageManager}");

                return isExternalStorageManager;
            }
        }


        private void OpenManageAllFilesAccessSettings()
        {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            using (var intent = new AndroidJavaObject("android.content.Intent", "android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION"))
            using (var uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("fromParts", "package", Application.identifier, null))
            {
                intent.Call<AndroidJavaObject>("setData", uri);
                activity.Call("startActivity", intent);
            }

            Debug.Log("Opened Manage All Files Access Settings. Please grant permission manually.");
        }


        private async Task RequestPermission(string permission)
        {
            try
            {
                Debug.Log("Requesting permission for: " + permission);
                var result = await AndroidRuntimePermissions.RequestPermissionAsync(permission);

                if (result == AndroidRuntimePermissions.Permission.Granted)
                {
                    Debug.Log("Granted: " + permission);
                }
                else
                {
                    Debug.LogWarning($"Permission {result} for {permission} was not granted.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error requesting permission: " + e.Message);
            }
        }
    }
}