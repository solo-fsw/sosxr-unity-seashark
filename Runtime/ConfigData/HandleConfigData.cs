using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData
{
    [Serializable]
    public static class HandleConfigData
    {
        public static string ConfigPath
        {
            get
            {
                if (string.IsNullOrEmpty(_configPath))
                {
                    var path = Path.Combine(Application.persistentDataPath, CONFIG_NAME);
                    var directory = Path.GetDirectoryName(path);

                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(directory);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to create directory: {ex.Message}");
                        }
                    }

                    _configPath = path;
                }

                return _configPath;
            }
        }

        private const string CONFIG_NAME = "config.json";
        private static string _configPath;


        public static void WriteConfigToJson(BaseConfigData configData, bool isUpdate = false)
        {
            if (configData == null)
            {
                Debug.LogWarning("ConfigData is null. Cannot write config.");

                return;
            }

            try
            {
                var jsonData = JsonUtility.ToJson(configData, true);
                jsonData = CleanJsonData(jsonData);

                var directory = Path.GetDirectoryName(ConfigPath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(jsonData);

                    if (!isUpdate)
                    {
                        Debug.Log($"Writing config to: {ConfigPath}");
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"Permission denied while writing config: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write config: {ex.Message}");
            }
        }


        public static void LoadConfigFromJson(BaseConfigData configData)
        {
            if (configData == null)
            {
                Debug.LogWarning("ConfigData is null. Cannot load config.");

                return;
            }

            if (!File.Exists(ConfigPath))
            {
                Debug.Log($"No config found at: {ConfigPath}. Creating new config.");

                WriteConfigToJson(configData);

                return;
            }

            try
            {
                string jsonData;

                using (var reader = new StreamReader(ConfigPath))
                {
                    jsonData = reader.ReadToEnd();
                }

                jsonData = CleanJsonData(jsonData);
                JsonUtility.FromJsonOverwrite(jsonData, configData);

                Debug.Log($"Loaded config from: {ConfigPath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"Permission denied while loading config: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load config: {ex.Message}");
            }
        }


        public static string ReadConfigJson(BaseConfigData configData)
        {
            var errorMsg = string.Empty;

            if (configData == null)
            {
                errorMsg = "ConfigData is null. Cannot load config.";
                Debug.LogWarning(errorMsg);

                return errorMsg;
            }

            if (!File.Exists(ConfigPath))
            {
                errorMsg = $"No config to read found at: {ConfigPath}.";
                Debug.Log(errorMsg);

                return errorMsg;
            }

            try
            {
                string jsonData;

                using (var reader = new StreamReader(ConfigPath))
                {
                    jsonData = reader.ReadToEnd();
                }

                jsonData = CleanJsonData(jsonData);

                Debug.Log($"Read config from: {ConfigPath}");

                return jsonData;
            }
            catch (UnauthorizedAccessException ex)
            {
                errorMsg = $"Permission denied while reading config: {ex.Message}";
                Debug.LogError(errorMsg);

                return errorMsg;
            }
            catch (Exception ex)
            {
                errorMsg = $"Failed to read config: {ex.Message}";
                Debug.LogError(errorMsg);

                return errorMsg;
            }
        }


        private static string CleanJsonData(string jsonData)
        {
            jsonData = Regex.Replace(jsonData, @"""<(.+?)>k__BackingField""", @"""$1""");

            return jsonData;
        }


        public static void DeleteConfigJson(bool isUpdate = false)
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    File.Delete(ConfigPath);

                    if (!isUpdate)
                    {
                        Debug.Log($"Deleted config at: {ConfigPath}");
                    }
                }
                else
                {
                    Debug.Log($"No config exists at: {ConfigPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete config: {ex.Message}");
            }
        }


        public static void UpdateConfigJson(BaseConfigData configData)
        {
            DeleteConfigJson(true);
            WriteConfigToJson(configData, true);
            Debug.LogFormat("Updated config at: {0}. (update = delete + write)", ConfigPath);
        }
    }
}