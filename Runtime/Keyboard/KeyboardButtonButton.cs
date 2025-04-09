using UnityEngine;
using UnityEngine.UI;


/// <summary>
///     Usefull for testing the button in the editor.
///     A better version of this is in the [EditorSpice](https://github.com/solo-fsw/sosxr-unity-editorspice) package.
/// </summary>
[RequireComponent(typeof(Button))]
public class KeyboardButtonButton : MonoBehaviour
{
    [SerializeField] private Button _button;


    private void OnValidate()
    {
        if (_button != null)
        {
            return;
        }

        _button = GetComponent<Button>();
    }


    [ContextMenu(nameof(Click))]
    public void Click()
    {
        _button.onClick.Invoke();
    }
}