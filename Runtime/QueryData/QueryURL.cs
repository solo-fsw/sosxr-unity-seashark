using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [Serializable]
    /// <summary>
    /// Utility for building a query string URL from an object's fields and properties using reflection.
    /// </summary>
    public static class QueryURL
    {
        /// <summary>
        /// Builds a query string by inspecting the source object's fields and properties and
        /// appending matching values to the provided base URL.
        /// </summary>
        /// <param name="source">The object whose members are inspected to obtain values for the query parameters.</param>
        /// <param name="baseURL">The base URL to which query parameters will be appended. If null or whitespace, an error is logged and an empty string is returned.</param>
        /// <param name="paramNames">Names of the query parameters to include. Matching is performed against member names (fields or properties), with Unity-style private fields normalized as needed.</param>
        /// <returns>The base URL with an encoded query string consisting of the requested parameters, or the base URL if no parameters are found.</returns>
        public static string BuildQueryURL(this object source, string baseURL, params string[] paramNames)
        {
            // Validate base URL
            if (string.IsNullOrWhiteSpace(baseURL))
            {
                Debug.LogError("BaseURL is null or empty.");

                return string.Empty;
            }

            // Return early if no parameters requested
            if (paramNames.Length == 0)
            {
                Debug.LogWarning("ParamNames are null or empty.");

                return baseURL;
            }

            var queryParams = new List<string>();
            
            // Use reflection to inspect all fields and properties of the source object
            var fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Build query parameters by looking up each requested parameter name
            foreach (var paramName in paramNames)
            {
                // Try to find a matching field (with Unity-style name normalization: m_propertyName -> propertyName)
                var field = fields.FirstOrDefault(f => NormalizeName(f.Name) == paramName);
                
                // Try to find a matching property
                var property = properties.FirstOrDefault(p => p.Name == paramName);

                // Get the value from either the field or property (field takes precedence)
                var value = field?.GetValue(source) ?? property?.GetValue(source);

                // Add to query string if value exists, URI-encoding the string representation
                if (value != null)
                {
                    queryParams.Add($"{paramName}={Uri.EscapeDataString(value.ToString())}");
                }
                else
                {
                    Debug.LogWarning($"Query parameter '{paramName}' not found in fields or properties of {source.GetType().Name}.");
                }
            }

            // Construct the final query string, appending parameters only if any were found
            var queryString = queryParams.Count > 0 ? $"{baseURL}?{string.Join("&", queryParams)}" : baseURL;

            return queryString;
        }


        /// <summary>
        /// Private helper that converts Unity-style field names (e.g. m_propertyName) to
        /// their corresponding property names (e.g. PropertyName) for lookup.
        /// </summary>
        private static string NormalizeName(string name)
        {
            return name.TrimStart('m', '_'); // Normalizing Unity-style private field names
        }
    }
}
