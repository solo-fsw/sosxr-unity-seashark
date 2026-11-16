using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [CreateAssetMenu(fileName = "QueryTest", menuName = "SOSXR/WebHelpers/QueryTest")]
    /// <summary>
    ///     ScriptableObject that stores query data and test values for IHaveQueryData. Create it from the SOSXR/WebHelpers/QueryTest asset menu.
    /// </summary>
    public class QueryTestSO : ScriptableObject, IHaveQueryData
    {
        /// <summary>
        ///     String value. Not used by this class.
        /// </summary>
        [field: SerializeField] public string PPNString { get; set; }

        /// <summary>
        ///     Boolean flag. Not used by this class.
        /// </summary>
        [field: SerializeField] public bool UpdateJsonOnValueChange { get; set; }

        /// <summary>
        ///     String value for a clip directory. Not used by this class.
        /// </summary>
        [field: SerializeField] public string ClipDirectory { get; set; }

        /// <summary>
        ///     String value for a debug update interval. Not used by this class.
        /// </summary>
        [field: SerializeField] public string DebugUpdateInterval { get; set; }

        /// <summary>
        ///     Test string. Not used by this class.
        /// </summary>
        [field: SerializeField] public string TestString { get; set; }

        /// <summary>
        ///     Test integer. Not used by this class.
        /// </summary>
        [field: SerializeField] public int TestInt { get; set; }

        /// <summary>
        ///     Test float. Not used by this class.
        /// </summary>
        [field: SerializeField] public float TestFloat { get; set; }

        /// <summary>
        ///     Test boolean. Not used by this class.
        /// </summary>
        [field: SerializeField] public bool TestBool { get; set; }

        /// <summary>
        ///     Test Vector3. Not used by this class.
        /// </summary>
        [field: SerializeField] public Vector3 TestVector3 { get; set; }

        /// <summary>
        ///     Test Vector2. Not used by this class.
        /// </summary>
        [field: SerializeField] public Vector2 TestVector2 { get; set; }

        /// <summary>
        ///     Test color. Not used by this class.
        /// </summary>
        [field: SerializeField] public Color TestColor { get; set; }

        /// <summary>
        ///     Test animation curve. Not used by this class.
        /// </summary>
        [field: SerializeField] public AnimationCurve TestAnimationCurve { get; set; }

        /// <summary>
        ///     String value. Not used by this class.
        /// </summary>
        [field: SerializeField] public string Maarten { get; set; }

        /// <summary>
        ///     Float value. Not used by this class.
        /// </summary>
        [field: SerializeField] public float Maarten2 { get; set; }

        /// <summary>
        ///     String value. Not used by this class.
        /// </summary>
        [field: SerializeField] public string Maarten3 { get; set; }

        /// <summary>
        ///     Base URL for the query. Always https://www.google.com.
        /// </summary>
        public string BaseURL => "https://www.google.com";

        /// <summary>
        ///     Query-string variable names stored with this object.
        /// </summary>
        [field: SerializeField] public List<string> QueryStringVariables { get; set; } = new();

        /// <summary>
        ///     Query-string URL value, implemented as a serialized auto-property.
        /// </summary>
        [field: SerializeField] public string QueryStringURL { get; set; }
    }
}