using SOSXR.SeaShark;
using UnityEngine;

public class ObserveExample : MonoBehaviour
{
    public enum Test
    {
        Hoge,
        Fuga
    }


    [Observe("Callback")] public string
        hoge;

    [Observe("Callback", "Callback2")] public Test
        test;


    public void Callback()
    {
        Debug.Log("call");
    }


    private void Callback2()
    {
        Debug.Log("call2");
    }
}