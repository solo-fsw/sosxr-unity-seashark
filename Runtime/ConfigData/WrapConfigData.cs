using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     This is a wrapper for the ConfigData to ensure that the ConfigData is initialized and destroyed correctly.
    ///     The init and destroy are there to ensure the Json file is updated when the relevant values change.
    /// </summary>
    public class WrapConfigData : MonoBehaviour
    {
        [HideInInspector] public BaseConfigData ConfigData;


        private void Awake()
        {
            ConfigData.Initialize();
        }


        private void OnDestroy()
        {
            ConfigData.DeInitialize();
        }
    }
}