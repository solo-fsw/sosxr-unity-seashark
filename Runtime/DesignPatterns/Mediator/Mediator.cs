using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark
{
    public static class Mediator
    {
        static Mediator()
        {
            LoadRegistry();
        }


        public static Dictionary<string, Action<Medium>> Mediations { get; } = new();

        private static MediatorRegistry _registry;


        private static void LoadRegistry()
        {
            _registry = Resources.Load<MediatorRegistry>(nameof(MediatorRegistry));

            if (_registry == null)
            {
                Debug.LogErrorFormat("{0} not found in Resources!", nameof(MediatorRegistry));
            }
        }


        public static void Subscribe(Medium medium, Action<Medium> listener)
        {
            var listenerGameObject = listener.Target as Object;
            var listenerGameObjectName = listenerGameObject?.name ?? "UnknownGameObject";
            var listenerClass = listener.Target?.GetType().Name ?? "UnknownClass";
            var listenerMethod = listener.Method.Name;
            var listenerString = $"{listenerGameObjectName}.{listenerClass}.{listenerMethod}";

            if (!Mediations.ContainsKey(medium.Channel))
            {
                Mediations[medium.Channel] = delegate { };
            }

            if (medium.Channel == MediatorRegistry.EmptyName)
            {
                Debug.LogWarningFormat("Cannot subscribe to medium with name {0}, since this is a placeholder for 'no medium'", MediatorRegistry.EmptyName);

                return;
            }

            Mediations[medium.Channel] += listener;

            _registry.RegisterListener(medium, listenerString);
        }


        public static void Publish(Medium medium, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            if (string.IsNullOrEmpty(medium.Channel))
            {
                Debug.LogWarning("Medium name is null or empty");

                return;
            }

            if (medium.Channel == MediatorRegistry.EmptyName)
            {
                return;
            }

            var className = Path.GetFileNameWithoutExtension(filePath);
            var callerName = $"{className}.{methodName}";

            _registry.RegisterCaller(medium, callerName);
            _registry.UpdateData(medium);

            if (!Mediations.TryGetValue(medium.Channel, out var evt))
            {
                Debug.LogWarning($"No listeners for Medium: {medium.Channel}");

                return;
            }

            evt.Invoke(medium);
        }


        public static void Unsubscribe(Medium medium, Action<Medium> listener)
        {
            var listenerGameObject = listener.Target as Object;
            var listenerGameObjectName = listenerGameObject?.name ?? "UnknownGameObject";
            var listenerClass = listener.Target?.GetType().Name ?? "UnknownClass";
            var listenerMethod = listener.Method.Name;
            var listenerString = $"{listenerGameObjectName}.{listenerClass}.{listenerMethod}";

            _registry.UnregisterListener(medium, listenerString);

            if (Mediations.ContainsKey(medium.Channel))
            {
                Mediations[medium.Channel] -= listener;
            }

            if (Mediations.TryGetValue(medium.Channel, out var evt) && evt == null)
            {
                Mediations.Remove(medium.Channel);
            }
        }
    }
}