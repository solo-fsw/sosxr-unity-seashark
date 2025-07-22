using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     This derives from ProjectSettingsBase, so it will be shown in the Project Settings window.
    /// </summary>
    [InitializeOnLoad]
    public class AnotherProjectSettingsSample
    {
        static AnotherProjectSettingsSample()
        {
            ProjectSettingsProvider.OnGUIEvent += OnGUI;
        }


        /// <summary>
        ///     You get/set the value of the preference using a property like this.
        ///     This is the thing you can use in your code to check the value of the preference.
        /// </summary>
        public static TestEnum SomeEnum
        {
            get => (TestEnum) EditorPrefs.GetInt(_testOption, (int) TestEnum.OptionOne);
            set => EditorPrefs.SetInt(_testOption, (int) value);
        }

        /// <summary>
        ///     This stores the value of the preference to disk, so that it persists between Unity sessions.
        /// </summary>
        private const string _testOption = "SelectAnOption";


        /// <summary>
        ///     This is just a GUI element to display the test option in the Unity Editor, and then setting that value.
        /// </summary>
        private static void OnGUI()
        {
            var currentValue = SomeEnum;

            var value = (TestEnum) EditorGUILayout.EnumPopup(_testOption.ConvertToSpaces(), currentValue, GUILayout.Width(400));

            if (currentValue != value)
            {
                SomeEnum = value;
            }
        }
    }
}