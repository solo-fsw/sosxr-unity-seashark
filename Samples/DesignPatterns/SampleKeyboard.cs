using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Keyboard checks for the Design Pattern samples. Uses the Input System package when it is enabled,
    ///     otherwise the legacy Input Manager, so the samples work with either Active Input Handling setting.
    ///     Supports letters, digits (Alpha0 to Alpha9), F1 to F12, Space, and the Shift and Control keys.
    /// </summary>
    internal static class SampleKeyboard
    {
        /// <summary>True while the key is held down.</summary>
        public static bool GetKey(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
            var control = GetControl(keyCode);

            return control != null && control.isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(keyCode);
#else
            return false;
#endif
        }


        /// <summary>True in the frame the key was pressed.</summary>
        public static bool GetKeyDown(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
            var control = GetControl(keyCode);

            return control != null && control.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(keyCode);
#else
            return false;
#endif
        }


#if ENABLE_INPUT_SYSTEM
        private static UnityEngine.InputSystem.Controls.KeyControl GetControl(KeyCode keyCode)
        {
            var keyboard = Keyboard.current;
            var key = ToKey(keyCode);

            if (keyboard == null || key == Key.None)
            {
                return null;
            }

            return keyboard[key];
        }


        /// <summary>Maps a legacy <see cref="KeyCode" /> to an Input System <see cref="Key" />. Returns <see cref="Key.None" /> for unsupported keys.</summary>
        public static Key ToKey(KeyCode keyCode)
        {
            if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
            {
                return Key.A + (keyCode - KeyCode.A);
            }

            if (keyCode >= KeyCode.Alpha1 && keyCode <= KeyCode.Alpha9)
            {
                return Key.Digit1 + (keyCode - KeyCode.Alpha1);
            }

            if (keyCode >= KeyCode.F1 && keyCode <= KeyCode.F12)
            {
                return Key.F1 + (keyCode - KeyCode.F1);
            }

            return keyCode switch
                   {
                       KeyCode.Alpha0 => Key.Digit0,
                       KeyCode.Space => Key.Space,
                       KeyCode.LeftShift => Key.LeftShift,
                       KeyCode.RightShift => Key.RightShift,
                       KeyCode.LeftControl => Key.LeftCtrl,
                       KeyCode.RightControl => Key.RightCtrl,
                       _ => Key.None
                   };
        }
#endif
    }
}
