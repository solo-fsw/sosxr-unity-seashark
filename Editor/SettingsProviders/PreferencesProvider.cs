using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     Create a POCO with a [InitializeOnLoad] attribute. In a static constructor, subscribe a GUI method to the OnGUIEvent of this PreferencesProvider.
    ///     Properties in the POCO can be used to store user preferences, which can be accessed throughout the project.
    ///     See the samples for more information.
    /// </summary>
    public class PreferencesProvider : SettingsProvider
    {
        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        public PreferencesProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
            // Needs nothing here
        }


        private static readonly string _path = "Preferences/SOSXR/SeaShark";


        public static Action OnGUIEvent;


        public override void OnGUI(string searchContext)
        {
            GUILayout.Space(20);

            OnGUIEvent?.Invoke();
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new PreferencesProvider(_path);
        }
    }
}