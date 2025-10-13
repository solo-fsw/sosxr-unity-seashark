using SOSXR.EnhancedLogger;
using UnityEngine;

namespace SOSXR.SeaShark.Samples
{
    public class ButtonExampleClass : MonoBehaviour
    {
        [Button]
        private void SomeMethod()
        {
            this.Error("This is a test on a method");
        }


        [Button(Tooltip = "You can add tooltips too!")]
        private void OtherMethod()
        {
            this.Warning("This is a test on an other method");
        }


        [ContextMenu(nameof(DoesSomething))]
        private void DoesSomething()
        {
            this.Debug("ContextMenu calls should also still create a button (Unity 6 and up)");
        }


        [Button(Tooltip = "Don't eat yellow snow")]
        public void Testes(int value, string payload = "No", float lala = 10)
        {
            this.Verbose($"{value}, {payload}, {lala}");
        }


        [Button]
        private void Something(double value, Vector2 payload)
        {
            this.Info($"{value}, {payload}");
        }
    }
}