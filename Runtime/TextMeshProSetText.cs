using TMPro;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProSetText : MonoBehaviour
    {
        [SerializeField] [Optional(OptionalType.WillGet)] private TextMeshProUGUI m_textMeshProUGUI;

        [SerializeField] [Range(-1, 5000)] [Postfix("ms")] private int m_clearTextDelay;


        private void Awake()
        {
            if (m_textMeshProUGUI == null)
            {
                m_textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            }
        }


        private void Start()
        {
            m_textMeshProUGUI.enabled = false;
        }


        public void SetText(int input)
        {
            m_textMeshProUGUI.enabled = true;

            //this.Verbose($"New input is {input}");

            m_textMeshProUGUI.text = input.ToString();

            if (m_clearTextDelay > 0)
            {
                var delaySec = m_clearTextDelay / (float) 1000;

                //this.Verbose($"Will clear text in {delaySec} seconds");

                Invoke(nameof(ClearText), delaySec);
            }
        }


        public void ClearText()
        {
            m_textMeshProUGUI.enabled = false;
        }
    }
}