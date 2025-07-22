using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     Because this derives from PreferencesBase, it will be shown in the Preferences window.
    /// </summary>
    [InitializeOnLoad]
    public class PreferencesSample
    {
        static PreferencesSample()
        {
            PreferencesProvider.OnGUIEvent += OnGUI;
        }


        /// <summary>
        ///     You get/set the value of the preference using a property like this.
        ///     This is the thing you can use in your code to check the value of the preference.
        /// </summary>
        public static int TestOption
        {
            get => EditorPrefs.GetInt(_testOption, 64);
            set => EditorPrefs.SetInt(_testOption, value);
        }

        /// <summary>
        ///     This stores the value of the preference to disk, so that it persists between Unity sessions.
        /// </summary>
        private const string _testOption = "SomePreferencesI_Like";

        private static Vector2Int _range = new(0, 100);


        /// <summary>
        ///     This is just a GUI element to display the test option in the Unity Editor, and then setting that value.
        /// </summary>
        private static void OnGUI()
        {
            var currentValue = TestOption;

            // Show range slider 0-100
            var value = EditorGUILayout.IntSlider(_testOption.ConvertToSpaces(), currentValue, _range.x, _range.y, GUILayout.Width(400));

            if (currentValue != value)
            {
                TestOption = value;
            }
        }
    }
}