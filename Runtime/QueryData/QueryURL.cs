using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace SOSXR.SeaShark.QueryData
{
    [Serializable]
    public static class QueryURL
    {
        /// <summary>
        ///     You can pass any number of values to this method, and it will build a query string URL using the values that match
        ///     those fields
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string BuildQueryURL(this object source, string baseURL, params string[] paramNames)
        {
            if (string.IsNullOrWhiteSpace(baseURL))
            {
                Debug.LogError("BaseURL is null or empty.");

                return string.Empty;
            }

            if (paramNames.Length == 0)
            {
                Debug.LogWarning("ParamNames are null or empty.");

                return baseURL;
            }

            var queryParams = new List<string>();
            var fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var paramName in paramNames)
            {
                var field = fields.FirstOrDefault(f => NormalizeName(f.Name) == paramName);
                var property = properties.FirstOrDefault(p => p.Name == paramName);

                var value = field?.GetValue(source) ?? property?.GetValue(source);

                if (value != null)
                {
                    queryParams.Add($"{paramName}={Uri.EscapeDataString(value.ToString())}");
                }
                else
                {
                    Debug.LogWarning($"Query parameter '{paramName}' not found in fields or properties of {source.GetType().Name}.");
                }
            }

            var queryString = queryParams.Count > 0 ? $"{baseURL}?{string.Join("&", queryParams)}" : baseURL;

            return queryString;
        }


        private static string NormalizeName(string name)
        {
            return name.TrimStart('m', '_'); // Normalizing Unity-style private field names
        }
    }
}