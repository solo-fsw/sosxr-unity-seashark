using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace SOSXR.SeaShark
{
    public static class QueryURL
    {
        /// <summary>
        ///     You can pass any number of values to this method, and it will build a query string URL using the values that match
        ///     those fields
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string BuildQueryStringURL(this object source, string baseURL, params object[] values)
        {
            if (values.Length == 0 || source == null)
            {
                return baseURL;
            }

            var queryParams = new List<string>();
            var fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var value in values)
            {
                // Try finding the field first
                var field = fields.FirstOrDefault(f => f.GetValue(source)?.Equals(value) == true);

                if (field != null)
                {
                    queryParams.Add($"{field.Name.TrimStart('m', '_')}={Uri.EscapeDataString(value.ToString())}");

                    continue;
                }

                // Try finding a matching property
                var property = properties.FirstOrDefault(p => p.GetValue(source)?.Equals(value) == true);

                if (property != null)
                {
                    queryParams.Add($"{property.Name}={Uri.EscapeDataString(value.ToString())}");

                    continue;
                }

                Debug.LogWarning($"Value '{value}' not found in fields or properties of {source.GetType().Name}.");
            }

            return queryParams.Count > 0 ? $"{baseURL}?{string.Join("&", queryParams)}" : baseURL;
        }
    }
}