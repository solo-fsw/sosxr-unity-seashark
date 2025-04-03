using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    public class ButtonExampleClass : MonoBehaviour
    {
        [Button]
        private void SomeMethod()
        {
            Debug.LogWarning("This is a test on a method");
        }
    }
}