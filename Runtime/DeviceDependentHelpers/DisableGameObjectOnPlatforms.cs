using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Allows you to specify a list of platforms in which a GameObject should be specifically enabled or disabled
    ///     Can only be run from active GameObjects
    /// </summary>
    public class ToggleGameObjectOnPlatforms : MonoBehaviour
    {
        [SerializeField] private bool m_forceEnableOn;
        [SerializeField] private bool m_forceDisableOn;
        [SerializeField] private List<RuntimePlatform> m_platforms;


        private void Awake()
        {
            if (m_forceEnableOn && m_forceDisableOn)
            {
                Debug.LogError("Both forceEnable and forceDisable cannot be true. Use multiple instances of this class in that case.");

                return;
            }

            if (m_forceDisableOn)
            {
                ToggleOnPlatforms(true);
            }
            else if (m_forceDisableOn)
            {
                ToggleOnPlatforms(false);
            }
        }


        private void ToggleOnPlatforms(bool enable)
        {
            if (m_platforms.Contains(Application.platform))
            {
                gameObject.SetActive(enable);
            }
        }
    }
}