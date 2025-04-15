using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.SeaShark
{
    public abstract class ConfigValueToUnityEvent<T> : MonoBehaviour
    {
        public BaseConfigData ConfigData;
        public string ValueName;
        public bool RunOnStart = true;
        public UnityEvent<T> EventToFire;
        public bool SubscribeToChanges = true;
        private Action<object> _onValueChanged;

        private Func<T> GetValue { get; set; }


        private void OnValidate()
        {
            CacheValueGetter();
        }


        private void Awake()
        {
            CacheValueGetter();

            _onValueChanged = HandleValueChanged; // Create a persistent delegate for the change handler
        }


        private void Start()
        {
            if (RunOnStart)
            {
                FireCurrentValue();
            }
        }


        private void OnEnable()
        {
            if (SubscribeToChanges && ConfigData != null)
            {
                ConfigData.Subscribe(ValueName, _onValueChanged);
            }
        }


        private void HandleValueChanged(object newValue)
        {
            if (newValue is T typedValue)
            {
                FireEvent(typedValue);
            }
            else
            {
                Debug.LogErrorFormat(this, $"Received value of incorrect type. Expected {typeof(T).Name}, got {newValue?.GetType().Name ?? "null"}");
            }
        }


        public void FireCurrentValue()
        {
            if (GetValue == null)
            {
                LogValueError();

                return;
            }

            FireEvent(GetValue());
        }


        private void LogValueError()
        {
            var typeName = typeof(T).Name;

            if (typeName == "Single")
            {
                typeName = "Float";
            }

            Debug.LogErrorFormat(this,
                $"Property or Field of type {typeName} '{ValueName}' not found in {ConfigData?.GetType().Name}.");
        }


        protected abstract void FireEvent(T value);


        private void CacheValueGetter()
        {
            if (ConfigData == null || string.IsNullOrEmpty(ValueName))
            {
                GetValue = null;

                return;
            }

            var configType = ConfigData.GetType();

            // Try property first
            var property = configType.GetProperty(ValueName, BindingFlags.Public | BindingFlags.Instance);

            if (property != null && property.PropertyType == typeof(T) && property.CanRead)
            {
                GetValue = () => (T) property.GetValue(ConfigData);

                return;
            }

            // Try field
            var field = configType.GetField(ValueName, BindingFlags.Public | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(T))
            {
                GetValue = () => (T) field.GetValue(ConfigData);

                return;
            }

            GetValue = null;
        }


        private void OnDisable()
        {
            if (SubscribeToChanges && ConfigData != null)
            {
                ConfigData.Unsubscribe(ValueName, _onValueChanged);
            }
        }
    }
}