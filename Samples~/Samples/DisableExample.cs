using SOSXR.SeaShark;
using UnityEngine;

public class DisableExample : MonoBehaviour
{
    [DisableEditing] public string hoge = "hoge";

    [DisableEditing] public int fuga = 1;

    [DisableEditing] public AudioType audioType = AudioType.ACC;
}