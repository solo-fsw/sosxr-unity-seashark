using System.Collections.Generic;
using SOSXR.SeaShark;
using UnityEngine;

public class InfoBoxExample : MonoBehaviour
{
    [Info("This is some info")]
    public string SomeText;

    [Info("It can also work with SerializeField, and we can display an icon", MessageType = SOSXRMessageType.Info)]
    [SerializeField]
    private string m_anotherText;

    [Info("Private fields get their box at the bottom of the Inspector", MessageType = SOSXRMessageType.Warning)]
    private int _someInt;

    [Info("And with Tooltips", MessageType = SOSXRMessageType.Info)]
    [Tooltip("Information overload")]
    [SerializeField]
    private bool m_yes;

    [Info("It also works with Lists")]
    public List<int> Numbers;
}
