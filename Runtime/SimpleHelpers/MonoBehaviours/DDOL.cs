using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Attach to GameObject to make sure it stays loaded between scenes
    /// </summary>
    public class DDOL : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log($"{gameObject.name} will not be destroyed on scene load");
        }
    }
}