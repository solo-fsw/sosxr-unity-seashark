using System;
using System.Collections.Generic;
using UnityEngine;


#pragma warning disable 0169, 0414 // Fields only exist to be found by reflection.

namespace SOSXR.SeaShark.Tests
{
    /// <summary>Fields covering every value type RequiredAttributeDrawer checks.</summary>
    public class RequiredTarget : ScriptableObject
    {
        public List<GameObject> Items = new();
        public GameObject[] Array = System.Array.Empty<GameObject>();
        public string Text = "";
        public GameObject Reference;
        public Color Tint = Color.clear;
        public int Number;
    }


    [Serializable]
    public class ShowIfNested
    {
        public bool Show;
        public int Value;
    }


    public class ShowIfBaseTarget : ScriptableObject
    {
        [SerializeField] private bool _baseFlag;


        public void SetBaseFlag(bool value)
        {
            _baseFlag = value;
        }
    }


    public class ShowIfTarget : ShowIfBaseTarget
    {
        public bool Flag;
        public int Value;
        public ShowIfNested Nested = new();
        public List<ShowIfNested> Items = new() {new ShowIfNested {Show = false}, new ShowIfNested {Show = true}};
    }


    public class InfoBaseTarget : ScriptableObject
    {
        [Info("base private")] private int _basePrivate;
    }


    public class InfoTarget : InfoBaseTarget
    {
        [Info("serialized")] public int Serialized;
        [Info("serialize field")] [SerializeField] private int _serializeField;
        [Info("private")] private int _private;
        [Info("static")] private static int StaticField;
        [Info("one")] [Info("two")] private int _twoInfos;
    }
}

#pragma warning restore 0169, 0414
