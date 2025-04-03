using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Runtime.ConfigData
{
    public abstract class BaseConfigData : ScriptableObject
    {
        [SerializeField] [HideInInspector] private List<string> m_updateJsonOnSpecificValueChanged;

        private readonly Dictionary<string, Delegate> _valueChangedEvents = new();
        private readonly Dictionary<string, object> _previousValues = new();

        public List<string> UpdateJsonOnValueChange
        {
            get => m_updateJsonOnSpecificValueChanged;
            set
            {
                if (value == m_updateJsonOnSpecificValueChanged)
                {
                    return;
                }

                m_updateJsonOnSpecificValueChanged = value;
                NotifyChange(nameof(UpdateJsonOnValueChange), value);
            }
        }


        private event Action<string, object> _onAnyValueChanged;


        protected void SetValue<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            NotifyChange(propertyName, value);
        }


        public void Initialize()
        {
            Debug.Log($"Initializing with values which trigger JSON update: {string.Join(", ", UpdateJsonOnValueChange)}");

            foreach (var propertyName in UpdateJsonOnValueChange)
            {
                Subscribe(propertyName, _ => UpdateConfigJson());
            }
        }


        public void Subscribe(string propertyName, Action<object> callback)
        {
            if (_valueChangedEvents.TryGetValue(propertyName, out var existing))
            {
                _valueChangedEvents[propertyName] = Delegate.Combine(existing, callback);
            }
            else
            {
                _valueChangedEvents[propertyName] = callback;
            }
        }


        public void Unsubscribe(string propertyName, Action<object> callback)
        {
            if (_valueChangedEvents.TryGetValue(propertyName, out var existing))
            {
                var updated = Delegate.Remove(existing, callback);

                if (updated == null)
                {
                    _valueChangedEvents.Remove(propertyName);
                }
                else
                {
                    _valueChangedEvents[propertyName] = updated;
                }
            }
        }


        public void SubscribeToAny(Action<string, object> callback)
        {
            _onAnyValueChanged += callback;
        }


        public void UnsubscribeFromAny(Action<string, object> callback)
        {
            _onAnyValueChanged -= callback;
        }


        protected void NotifyChange(string propertyName, object value)
        {
            if (_valueChangedEvents.TryGetValue(propertyName, out var callback))
            {
                try
                {
                    (callback as Action<object>)?.Invoke(value);
                    Debug.LogFormat(this, $"Notified specific listeners for {propertyName}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error notifying specific listeners for {propertyName}: {ex}");
                }
            }

            try
            {
                _onAnyValueChanged?.Invoke(propertyName, value);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error notifying general listeners for {propertyName}: {ex}");
            }
        }


        private void OnValidate()
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var fieldToPropertyMap = new Dictionary<string, string>(); // Build a mapping of field names to property names

            foreach (var property in properties)
            {
                var backingFieldName = $"m_{char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}";
                fieldToPropertyMap[backingFieldName] = property.Name;
            }

            foreach (var field in fields)
            {
                var currentValue = field.GetValue(this);

                if (field.Name == nameof(m_updateJsonOnSpecificValueChanged))
                {
                    continue;
                }

                // Get the previous value, or set it if this is the first time
                if (!_previousValues.TryGetValue(field.Name, out var previousValue))
                {
                    _previousValues[field.Name] = currentValue;

                    continue;
                }

                // Field names and property names differ in that the field starts with "m_" and the property does not, and the property is capitalized.
                var guessedPropertyName = field.Name.StartsWith("m_") ? field.Name.Substring(2) : field.Name;
                guessedPropertyName = char.ToUpper(guessedPropertyName[0]) + guessedPropertyName.Substring(1);

                if (!Equals(previousValue, currentValue))
                {
                    _previousValues[field.Name] = currentValue; // Update the stored previous value

                    if (fieldToPropertyMap.TryGetValue(field.Name, out var propertyName)) // Find the corresponding property name
                    {
                        NotifyChange(propertyName, currentValue);
                    }
                    else if (fieldToPropertyMap.ContainsValue(guessedPropertyName)) // If no property found, use the guessed property name
                    {
                        NotifyChange(guessedPropertyName, currentValue);
                    }
                    else // If no property found, use the field name directly
                    {
                        NotifyChange(field.Name, currentValue);

                        Debug.LogWarningFormat("Notified using Field name {0}, is this correct?", field.Name);
                    }

                    _onAnyValueChanged?.Invoke(propertyName ?? field.Name, currentValue); // Always notify general listeners with the same name we used aboveeioc
                }
            }
        }


        [ContextMenu(nameof(LoadConfigJson))]
        public void LoadConfigJson()
        {
            HandleConfigData.LoadConfigFromJson(this);
        }


        [ContextMenu(nameof(ReadJson))]
        public string ReadJson()
        {
            return HandleConfigData.ReadConfigJson(this);
        }


        [ContextMenu(nameof(UpdateConfigJson))]
        public void UpdateConfigJson()
        {
            HandleConfigData.UpdateConfigJson(this);
        }


        public void DeInitialize()
        {
            foreach (var change in UpdateJsonOnValueChange)
            {
                Unsubscribe(change, _ => UpdateConfigJson());
            }
        }


        [ContextMenu(nameof(ForceUnsubscribeAll))]
        public void ForceUnsubscribeAll()
        {
            _valueChangedEvents.Clear();
            _onAnyValueChanged = null;
            Debug.LogWarning("All listeners have been forcibly evicted.");
        }


        private void OnDestroy()
        {
            HandleConfigData.DeleteConfigJson();
        }
    }
}