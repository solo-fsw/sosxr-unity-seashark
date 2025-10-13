using SOSXR.EnhancedLogger;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SOSXR.SeaShark.EditorScripts
{
    public static class TMPProjectSetup
    {
        private static readonly string _defaultFontAssetPath = "Fonts & Materials/Maple Mono/Maple Mono";
        private static TMP_FontAsset _defaultFontAsset;

        private static readonly string _fallBackFontPaths = "Fonts & Materials/Liberation Sans SDF";
        private static TMP_FontAsset _fallBackFontAsset;


        [MenuItem("SOSXR/Setup/Setup TMP Default")]
        public static void SetupTMPDefaults()
        {
            SetDefaultTMPFont();
            SetFallBackTMPFont();
        }


        private static void SetDefaultTMPFont()
        {
            _defaultFontAsset = GetFont(_defaultFontAssetPath);

            if (_defaultFontAsset == null)
            {
                Log.Static($"Default font asset not found at path: {_defaultFontAssetPath}");

                return;
            }

            if (TMP_Settings.defaultFontAsset == _defaultFontAsset)
            {
                Log.Static("TMP default font asset is already set to ", _defaultFontAssetPath);

                return;
            }

            Undo.RecordObject(TMP_Settings.instance, "Change TMP Default Font");
            TMP_Settings.defaultFontAsset = _defaultFontAsset;
            EditorUtility.SetDirty(TMP_Settings.instance);
            AssetDatabase.SaveAssets();

            Debug.Log("TMP default font asset set.");
        }


        private static void SetFallBackTMPFont()
        {
            _fallBackFontAsset = GetFont(_fallBackFontPaths);

            if (_fallBackFontAsset == null)
            {
                Log.Static($"Fallback font asset not found at path: {_fallBackFontPaths}");

                return;
            }

            if (TMP_Settings.fallbackFontAssets.Contains(_fallBackFontAsset))
            {
                Log.Static("TMP fallback font asset is already set to ", _fallBackFontPaths);

                return;
            }

            Undo.RecordObject(TMP_Settings.instance, "Change TMP Fallback Font");
            TMP_Settings.fallbackFontAssets.Add(_fallBackFontAsset);
            EditorUtility.SetDirty(TMP_Settings.instance);
            AssetDatabase.SaveAssets();

            Debug.Log("TMP fallback font asset set.");
        }


        private static TMP_FontAsset GetFont(string fontAssetPath)
        {
            var fontAsset = Resources.Load(fontAssetPath) as TMP_FontAsset;

            if (fontAsset == null)
            {
                Log.Static($"Font asset {fontAssetPath} not found at specified path.");

                return null;
            }

            return fontAsset;
        }
    }
}