using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class QueryConfigData : MonoBehaviour, IHaveQueryData
    {
        public bool Testes;
        public float Number;
        public string Text;
        [SerializeField] private string m_queryStringURL;

        public bool PropTest { get; set; }
        public float PropNumber { get; set; }

        public string BaseURL => "https://www.youtube.com";

        public List<string> QueryStringVariables { get; set; }

        public string QueryStringURL
        {
            get => m_queryStringURL;
            set => m_queryStringURL = value;
        }
    }
}