using System;
using System.Collections.Generic;
using System.Linq;
using SOSXR.EnhancedLogger;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    /// </summary>
    public class PreferencesProvider : SettingsProvider
    {
        static PreferencesProvider()
        {
            DiscoverSections();
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        public PreferencesProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
            // Needs nothing here
        }


        private static readonly List<PreferencesBase> _sections = new();


        private static void DiscoverSections()
        {
            _sections.Clear();

            foreach (var type in TypeCache.GetTypesDerivedFrom<PreferencesBase>().Where(type => !type.IsAbstract && !type.IsInterface))
            {
                try
                {
                    if (Activator.CreateInstance(type) is PreferencesBase instance)
                    {
                        AddSection(instance);
                    }
                }
                catch (Exception e)
                {
                    Log.Static($"Failed to create {type.Name}: {e.Message}");
                }
            }
        }


        public static void AddSection(PreferencesBase section)
        {
            if (!_sections.Contains(section))
            {
                _sections.Add(section);
            }
        }


        public override void OnGUI(string searchContext)
        {
            GUILayout.Space(20);

            foreach (var section in _sections)
            {
                section.OnGUI();
            }
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new PreferencesProvider("Preferences/SOSXR");
        }
    }
}