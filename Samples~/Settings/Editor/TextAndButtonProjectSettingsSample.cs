using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Samples
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     This derives from ProjectSettingsBase, so it will be shown in the Project Settings window.
    /// </summary>
    public class TextAndButtonProjectSettingsSample : ProjectSettingsBase
    {
        public static string[] SomeStrings
        {
            get
            {
                var stored = EditorPrefs.GetString(_stringListKey, "Default");

                return string.IsNullOrEmpty(stored) ? new[] {"Default", "Lala"} : stored.Split(',');
            }
            set => EditorPrefs.SetString(_stringListKey, string.Join(",", value));
        }

        private const string _stringListKey = "SelectAnOptionStrings";


        public override void OnGUI()
        {
            // Space
            GUILayout.Space(10);
            // Draw a horizontal line
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            // Show header
            GUILayout.Label("Manage String List", EditorStyles.boldLabel);
            // Space
            GUILayout.Space(10);

            var currentValue = SomeStrings;
            var modified = false;

            for (var i = 0; i < currentValue.Length; i++)
            {
                GUILayout.BeginHorizontal();
                var newValue = EditorGUILayout.TextField($"String {i + 1}", currentValue[i], GUILayout.Width(400));

                if (newValue != currentValue[i])
                {
                    currentValue[i] = newValue;
                    modified = true;
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    var newList = new string[currentValue.Length - 1];

                    for (int j = 0, k = 0; j < currentValue.Length; j++)
                    {
                        if (j != i)
                        {
                            newList[k++] = currentValue[j];
                        }
                    }

                    currentValue = newList;
                    modified = true;
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add String", GUILayout.Width(200)))
            {
                var newList = new string[currentValue.Length + 1];
                currentValue.CopyTo(newList, 0);
                newList[^1] = "New String";
                currentValue = newList;
                modified = true;
            }

            if (modified)
            {
                SomeStrings = currentValue;
            }

            // Space
            GUILayout.Space(10);
            // Draw a horizontal line
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            // Space
            GUILayout.Space(10);
        }
    }
}