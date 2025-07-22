using System;
using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [CreateAssetMenu(fileName = "MediatorRegistry", menuName = "SOSXR/Patterns/MediatorRegistry")]
    public class MediatorRegistry : ScriptableObject
    {
        [SerializeField] private List<MediumInfo> m_registry = new() {new MediumInfo {Medium = new Medium(EmptyName)}};

        public static string EmptyName => "NONE";

        public List<MediumInfo> Registry
        {
            get => m_registry;
            set => m_registry = value;
        }


        [ContextMenu(nameof(CheckMediatorForOrphanMediums))]
        public void CheckMediatorForOrphanMediums()
        {
            foreach (var listed in Registry)
            {
                if (listed.Medium.Channel == EmptyName)
                {
                    continue;
                }

                if (!Mediator.Mediations.ContainsKey(listed.Medium.Channel))
                {
                    Debug.LogWarning($"No listeners for Medium: {listed.Medium.Channel}");
                }
            }

            if (Mediator.Mediations.ContainsKey(EmptyName))
            {
                Debug.LogWarningFormat("There are Listeners for {0} medium, this should not be the case", EmptyName);
            }
        }


        [ContextMenu(nameof(ClearRegistryButNotMediums))]
        public void ClearRegistryButNotMediums()
        {
            foreach (var listed in Registry)
            {
                listed.Listeners.Clear();
                listed.Callers.Clear();
            }
        }


        public void RegisterListener(Medium medium, string listenerString)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            if (mediumInfo == null)
            {
                mediumInfo = new MediumInfo {Medium = medium};
                Registry.Add(mediumInfo);
            }

            mediumInfo.Listeners.AddIfUnique(listenerString);
        }


        public void RegisterCaller(Medium medium, string callerName)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            if (mediumInfo == null)
            {
                mediumInfo = new MediumInfo {Medium = medium};
                Registry.Add(mediumInfo);
            }

            mediumInfo.Callers.AddIfUnique(callerName);
        }


        public void UnregisterListener(Medium medium, string listenerString)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            mediumInfo?.Listeners.Remove(listenerString);
        }


        public void UnregisterCaller(Medium medium, string callerName)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            mediumInfo?.Callers.Remove(callerName);
        }


        public void UpdateData(Medium medium)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            if (mediumInfo == null)
            {
                mediumInfo = new MediumInfo {Medium = medium};
                Registry.Add(mediumInfo);
            }

            var currentTypeName = medium.GetTypeName();
            medium.TypeName = currentTypeName;

            var currentDataString = medium.GetDataString();
            medium.DataString = currentDataString;

            //mediumInfo.DataList.Insert(0, currentDataString); // This makes a tonnnnn of data
        }


        public void ClearDataLists()
        {
            foreach (var registered in Registry)
            {
                registered.DataList.Clear();
            }

            Debug.LogFormat(this, "Data lists cleared");
        }


        public void ClearDataList(Medium medium)
        {
            if (Registry == null)
            {
                Debug.LogError("No registry found");

                return;
            }

            var mediumInfo = Registry.Find(m => m.Medium.Channel == medium.Channel);

            mediumInfo?.DataList.Clear();

            Debug.LogFormat(this, "Data lists cleared");
        }
    }


    [Serializable]
    public class Medium
    {
        public string Channel;
        public string TypeName;
        public string DataString;
        private object _data;


        public Medium(string channel, object data = null)
        {
            Channel = channel;
            Data = data;
        }


        public object Data
        {
            get => _data;
            set
            {
                if (_data == value)
                {
                    return;
                }

                _data = value;

                TypeName = GetTypeName();

                DataString = GetDataString();
            }
        }


        public string GetTypeName()
        {
            return Data?.GetType().FullName ?? "Type Unknown";
        }


        public string GetDataString()
        {
            return Data?.ToString() ?? "Data Unknown";
        }
    }


    [Serializable]
    public class MediumInfo
    {
        [Mediator(true)] public Medium Medium;
        public List<string> DataList = new();
        public List<string> Listeners = new();
        public List<string> Callers = new();
    }
}