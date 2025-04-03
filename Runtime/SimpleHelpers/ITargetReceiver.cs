using UnityEngine;


namespace SOSXR.SeaShark
{
    public interface ITargetReceiver
    {
        public GameObject Target { get; set; }
    }


    public interface ITargetsReceiver
    {
        public GameObject[] Targets { get; set; }
    }
}