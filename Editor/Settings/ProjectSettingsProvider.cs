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
    public class ProjectSettingsProvider : SettingsProvider
    {
        static ProjectSettingsProvider()
        {
            DiscoverSections();
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        public ProjectSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
        }


        private static readonly List<ProjectSettingsBase> _sections = new();


        private static void DiscoverSections()
        {
            _sections.Clear();

            foreach (var type in TypeCache.GetTypesDerivedFrom<ProjectSettingsBase>().Where(type => !type.IsAbstract && !type.IsInterface))
            {
                try
                {
                    if (Activator.CreateInstance(type) is ProjectSettingsBase instance)
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


        public static void AddSection(ProjectSettingsBase section)
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
            return new ProjectSettingsProvider("Project/SOSXR");
        }
    }
}