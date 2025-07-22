using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     New settings need to be derived from either ProjectSettingsBase or PreferencesBase, depending on whether they are project settings or user preferences.
    /// </summary>
    [InitializeOnLoad]
    public class ProjectSettingsSample
    {
        static ProjectSettingsSample()
        {
            ProjectSettingsProvider.OnGUIEvent += OnGUI;
        }


        /// <summary>
        ///     You get/set the value of the preference using a property like this.
        ///     This is the thing you can use in your code to check the value of the preference.
        /// </summary>
        public static bool TestOption
        {
            get => EditorPrefs.GetBool(_testOption, true);
            set => EditorPrefs.SetBool(_testOption, value);
        }

        /// <summary>
        ///     This stores the value of the preference to disk, so that it persists between Unity sessions.
        /// </summary>
        private const string _testOption = "TestOption";


        /// <summary>
        ///     This is just a GUI element to display the test option in the Unity Editor, and then setting that value.
        /// </summary>
        private static void OnGUI()
        {
            var isEnabled = TestOption;

            var value = EditorGUILayout.Toggle(_testOption.ConvertToSpaces(), isEnabled, GUILayout.Width(200));

            if (isEnabled != value)
            {
                TestOption = value;
            }
        }
    }


    public enum TestEnum
    {
        OptionOne,
        OptionTwo,
        OptionThree
    }
}