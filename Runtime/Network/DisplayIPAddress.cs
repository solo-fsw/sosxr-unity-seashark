using TMPro;
using UnityEngine;

namespace SOSXR.SeaShark
{
    public class DisplayIPAddress : MonoBehaviour
    {
        public TextMeshProUGUI m_text;

        private void Start()
        {
            ShowIP();
        }

        [Button]
        private void ShowIP()
        {
            var ip = Network.GetLocalIPAddress();
            var ipText = "IP = " + ip;
            if (m_text != null)
            {
                m_text.text = ipText;
            }
            Debug.Log(ipText);
        }
    }
}
