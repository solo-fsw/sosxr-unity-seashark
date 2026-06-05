using UnityEngine;
using System.Collections;
using System;

namespace SOSXR.SeaShark
{
    /// From Unity Experiment Framework
    public class Comment : MonoBehaviour
    {
        public string Note;

        void Start()
        {
            if (!Application.isEditor)
            {
                Destroy(this);
            }
        }
    }
}
