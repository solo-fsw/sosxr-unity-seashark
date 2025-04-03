using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark.QueryData
{
    [CreateAssetMenu(fileName = "QueryTest", menuName = "SOSXR/WebHelpers/QueryTest")]
    public class QueryTestSO : ScriptableObject, IHaveQueryData
    {
        public string PPNString { get; set; }

        public bool UpdateJsonOnValueChange { get; set; }

        public string ClipDirectory { get; set; }

        public string DebugUpdateInterval { get; set; }

        public string TestString { get; set; }

        public int TestInt { get; set; }

        public float TestFloat { get; set; }

        public bool TestBool { get; set; }

        public Vector3 TestVector3 { get; set; }

        public Vector2 TestVector2 { get; set; }

        public Color TestColor { get; set; }

        public AnimationCurve TestAnimationCurve { get; set; }

        public string Maarten { get; set; }

        public float Maarten2 { get; set; }

        public string Maarten3 { get; set; }

        public string BaseURL => "https://www.google.com";

        public List<string> QueryStringVariables { get; set; }

        public string QueryStringURL { get; set; }
    }
}