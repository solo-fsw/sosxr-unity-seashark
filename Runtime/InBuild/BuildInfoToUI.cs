using System;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public class BuildInfoToUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_buildInfoText;

        private string _currentTime;
        private float _elapsedTime;
        private TextAsset _buildInfoTextAsset;
        private const float _updateInterval = 10f;


        private void Awake()
        {
            _buildInfoTextAsset = Resources.Load<TextAsset>("build_info");
        }


        private void Start()
        {
            InvokeRepeating(nameof(SetBuildText), 0, _updateInterval);
        }


        private void SetBuildText()
        {
            if (_buildInfoTextAsset == null)
            {
                m_buildInfoText.text = "No build information available.";

                return;
            }

            var lastLine = ReadLastLineOfFile(_buildInfoTextAsset.text);

            if (lastLine != null && !IsHeaderLine(lastLine))
            {
                var now = DateTime.Now;
                _currentTime = now.ToString("HH:mm", CultureInfo.InvariantCulture);
                m_buildInfoText.text = FormatBuildInfo(lastLine, now);
            }
            else
            {
                m_buildInfoText.text = "No build information available.";
            }
        }


        /// <summary>
        ///     Returns the last line that is not empty or whitespace, or null if there is none.
        /// </summary>
        public static string ReadLastLineOfFile(string fileContent)
        {
            string lastLine = null;

            if (fileContent == null)
            {
                return null;
            }

            using var sr = new StringReader(fileContent);

            while (sr.ReadLine() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                lastLine = line;
            }

            return lastLine;
        }


        private static bool IsHeaderLine(string line)
        {
            return line.TrimStart().StartsWith("PackageName", StringComparison.Ordinal);
        }


        /// <summary>
        ///     Formats one CSV line written by the EditorSpice BuildInfoManager.
        ///     Columns: PackageName, UnityVersion, TotalAttemptedBuilds, BuildTarget, ScriptingBackend, APICompatibility,
        ///     ProductionBuild, SemVer, AndroidBundleCode, BuildDate, BuildTime, then the optional post-build columns.
        /// </summary>
        public static string FormatBuildInfo(string line, DateTime now)
        {
            const string invalid = "Invalid build information format.";

            if (string.IsNullOrWhiteSpace(line))
            {
                return invalid;
            }

            var parts = line.Split(',');

            if (parts.Length < 11)
            {
                return invalid;
            }

            var packageName = parts[0].Trim();
            var unityVersion = parts[1].Trim();
            var productionBuild = parts[6].Trim();
            var semVer = parts[7].Trim();
            var bundleCode = parts[8].Trim();
            var buildDate = parts[9].Trim();
            var buildTime = parts[10].Trim();

            if (!DateTime.TryParseExact($"{buildDate} {buildTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var buildDateTime))
            {
                return invalid;
            }

            var daysAgo = GetRelativeDateDescription(buildDateTime.Date, now.Date);

            return $"Name: {packageName}\n" +
                   $"Unity: {unityVersion}\n" +
                   $"ProductionBuild: {productionBuild}\n" +
                   $"SemVer: {semVer}\n" +
                   $"BundleCode: {bundleCode}\n" +
                   $"Build Date: {daysAgo} at {buildTime}\n" +
                   $"Current Time: {now.ToString("HH:mm", CultureInfo.InvariantCulture)}";
        }


        private static string GetRelativeDateDescription(DateTime buildDate, DateTime currentDate)
        {
            var daysDifference = (currentDate - buildDate).Days;

            return daysDifference switch
            {
                0 => "today",
                1 => "yesterday",
                _ => $"{daysDifference} days ago"
            };
        }
    }
}
