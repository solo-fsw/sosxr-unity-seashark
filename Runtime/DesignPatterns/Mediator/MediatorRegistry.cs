using System;
using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [CreateAssetMenu(fileName = "MediatorRegistry", menuName = "SOSXR/Patterns/MediatorRegistry")]
    /// <summary>
    /// Maintains a registry of mediums, listeners, and callers for the mediator pattern.
    /// It tracks metadata about which listeners are subscribed to which mediums
    /// and which objects publish to them.
    /// </summary>
    public class MediatorRegistry : ScriptableObject
    {
        [SerializeField] private List<MediumInfo> m_registry = new() {new MediumInfo {Medium = new Medium(EmptyName)}};

        public static string EmptyName => "NONE";

        /// <summary>
        /// Gets or sets the registry entries that track all registered mediums.
        /// </summary>
        public List<MediumInfo> Registry
        {
            get => m_registry;
            set => m_registry = value;
        }


        /// <summary>
        /// Inspects the mediator's registered mediums and logs warnings for mediums that
        /// have no listeners or callers.
        /// </summary>
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


        /// <summary>
        /// Clears the registry metadata lists without unsubscribing listeners or stopping callers.
        /// </summary>
        [ContextMenu(nameof(ClearRegistryButNotMediums))]
        public void ClearRegistryButNotMediums()
        {
            foreach (var listed in Registry)
            {
                listed.Listeners.Clear();
                listed.Callers.Clear();
            }
        }


        /// <summary>
        /// Registers a listener name for the specified medium.
        /// </summary>
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


        /// <summary>
        /// Registers a caller name for the specified medium.
        /// </summary>
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


        /// <summary>
        /// Removes a listener name from the specified medium's registry.
        /// </summary>
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


        /// <summary>
        /// Removes a caller name from the specified medium's registry.
        /// </summary>
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


        /// <summary>
        /// Updates the medium's type name and data string in the registry to reflect its current state.
        /// </summary>
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


        /// <summary>
        /// Clears all data lists for every registered medium.
        /// </summary>
        public void ClearDataLists()
        {
            foreach (var registered in Registry)
            {
                registered.DataList.Clear();
            }

            Debug.LogFormat(this, "Data lists cleared");
        }


        /// <summary>
        /// Clears the data list for the specified medium.
        /// </summary>
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


    /// <summary>
    /// Represents a communication channel (medium) with a channel name and optional data payload.
    /// Provides helpers to obtain the data type name and a string representation of the data.
    /// </summary>
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


    /// <summary>
    /// Holds metadata about a medium: the medium itself, a list of data values,
    /// and lists of listener and caller names associated with the medium.
    /// </summary>
    [Serializable]
    public class MediumInfo
    {
        [Mediator(true)] public Medium Medium;
        public List<string> DataList = new();
        public List<string> Listeners = new();
        public List<string> Callers = new();
    }
}
