using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     MonoBehaviour that stores query data (base URL and query-string variables) for IHaveQueryData.
    /// </summary>
    public class QueryConfigData : MonoBehaviour, IHaveQueryData
    {
        /// <summary>
        ///     Test flag. Not used by this class.
        /// </summary>
        public bool Testes;
        /// <summary>
        ///     Test number. Not used by this class.
        /// </summary>
        public float Number;
        /// <summary>
        ///     Test text. Not used by this class.
        /// </summary>
        public string Text;
        [SerializeField] private string m_queryStringURL;

        /// <summary>
        ///     Test flag, backed by a serialized auto-property. Not used by this class.
        /// </summary>
        [field: SerializeField] public bool PropTest { get; set; }
        /// <summary>
        ///     Test number, backed by a serialized auto-property. Not used by this class.
        /// </summary>
        [field: SerializeField] public float PropNumber { get; set; }

        /// <summary>
        ///     Base URL for the query. Always https://www.youtube.com.
        /// </summary>
        public string BaseURL => "https://www.youtube.com";

        /// <summary>
        ///     Query-string variable names stored with this object.
        /// </summary>
        [field: SerializeField] public List<string> QueryStringVariables { get; set; } = new();

        /// <summary>
        ///     Query-string URL value, backed by the serialized field m_queryStringURL.
        /// </summary>
        public string QueryStringURL
        {
            get => m_queryStringURL;
            set => m_queryStringURL = value;
        }
    }
}