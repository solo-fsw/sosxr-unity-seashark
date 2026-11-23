using UnityEngine;


namespace SOSXR.SeaShark
{
    public class HeaderTest : MonoBehaviour
    {
        [Header("Test Header")] public string TestString;

        [Header("Another Header")] public int TestInt;

        [Header("Yet Another Header")] public float TestFloat;
    }
}