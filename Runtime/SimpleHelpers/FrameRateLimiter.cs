using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://github.com/adammyhre/Unity-Utils
    ///     This class is used to set the target frame rate of the application based on keyboard input,
    ///     useful for catching performance issues in the game.
    ///     Hold Left Shift and press F1 (10 fps), F2 (20), F3 (30), F4 (60) or F5 (900).
    ///     Works with the Input System package and with the legacy Input Manager.
    /// </summary>
    public class FrameRateLimiter : MonoBehaviour
    {
        private void Update()
        {
            if (!IsShiftHeld())
            {
                return;
            }

            var frameRate = GetRequestedFrameRate();

            if (frameRate > 0)
            {
                Application.targetFrameRate = frameRate;
            }
        }


        /// <summary>Maps a function key number (1 to 5) to its frame rate. Returns 0 for any other number.</summary>
        public static int FrameRateForFunctionKey(int functionKey)
        {
            return functionKey switch
                   {
                       1 => 10,
                       2 => 20,
                       3 => 30,
                       4 => 60,
                       5 => 900,
                       _ => 0
                   };
        }


        private static int GetRequestedFrameRate()
        {
            for (var functionKey = 1; functionKey <= 5; functionKey++)
            {
                if (WasFunctionKeyPressed(functionKey))
                {
                    return FrameRateForFunctionKey(functionKey);
                }
            }

            return 0;
        }


        private static bool IsShiftHeld()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;

            return keyboard != null && keyboard.leftShiftKey.isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.LeftShift);
#else
            return false;
#endif
        }


        private static bool WasFunctionKeyPressed(int functionKey)
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return false;
            }

            return functionKey switch
                   {
                       1 => keyboard.f1Key.wasPressedThisFrame,
                       2 => keyboard.f2Key.wasPressedThisFrame,
                       3 => keyboard.f3Key.wasPressedThisFrame,
                       4 => keyboard.f4Key.wasPressedThisFrame,
                       5 => keyboard.f5Key.wasPressedThisFrame,
                       _ => false
                   };
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(KeyCode.F1 + (functionKey - 1));
#else
            return false;
#endif
        }
    }
}
