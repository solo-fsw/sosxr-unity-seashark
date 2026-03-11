using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// Static mediator pattern implementation that enables decoupled communication between
    /// objects via mediums. Objects subscribe to mediums to receive notifications and publish
    /// to mediums to send notifications.
    /// </summary>
    public static class Mediator
    {
        static Mediator()
        {
            LoadRegistry();
        }


        /// <summary>
        /// Gets a dictionary mapping medium channel names to their registered listener delegates.
        /// </summary>
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


        /// <summary>
        /// Registers a listener callback for a specific medium. The listener will be invoked
        /// whenever that medium is published. Uses caller context (method name and file path)
        /// to create a unique listener identifier.
        /// </summary>
        /// <param name="medium">The medium to subscribe to.</param>
        /// <param name="listener">The callback to invoke when the medium is published.</param>
        public static void Subscribe(Medium medium, Action<Medium> listener)
        {
            // Build a unique listener identifier from the target object, class, and method names
            var listenerGameObject = listener.Target as Object;
            var listenerGameObjectName = listenerGameObject?.name ?? "UnknownGameObject";
            var listenerClass = listener.Target?.GetType().Name ?? "UnknownClass";
            var listenerMethod = listener.Method.Name;
            var listenerString = $"{listenerGameObjectName}.{listenerClass}.{listenerMethod}";

            // Initialize the medium's delegate chain if it doesn't exist yet
            if (!Mediations.ContainsKey(medium.Channel))
            {
                Mediations[medium.Channel] = delegate { };
            }

            // Guard against subscribing to the placeholder "empty" medium
            if (medium.Channel == MediatorRegistry.EmptyName)
            {
                Debug.LogWarningFormat("Cannot subscribe to medium with name {0}, since this is a placeholder for 'no medium'", MediatorRegistry.EmptyName);

                return;
            }

            // Add the listener to the delegate chain for this medium
            Mediations[medium.Channel] += listener;

            // Register the listener in the mediator registry for tracking/debugging
            _registry.RegisterListener(medium, listenerString);
        }


        /// <summary>
        /// Publishes a medium to all listeners registered for that medium, invoking each with the
        /// medium as the argument.
        /// </summary>
        /// <param name="medium">The medium to publish.</param>
        /// <param name="methodName">The calling method name (auto-populated).</param>
        /// <param name="filePath">The calling file path (auto-populated).</param>
        public static void Publish(Medium medium, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            // Validate the medium has a valid channel name
            if (string.IsNullOrEmpty(medium.Channel))
            {
                Debug.LogWarning("Medium name is null or empty");

                return;
            }

            // Guard against publishing to the placeholder "empty" medium
            if (medium.Channel == MediatorRegistry.EmptyName)
            {
                return;
            }

            // Extract the caller's class name from the file path and combine with method name
            var className = Path.GetFileNameWithoutExtension(filePath);
            var callerName = $"{className}.{methodName}";

            // Register the caller in the mediator registry for tracking/debugging
            _registry.RegisterCaller(medium, callerName);
            _registry.UpdateData(medium);

            // Retrieve the delegate chain for this medium
            if (!Mediations.TryGetValue(medium.Channel, out var evt))
            {
                Debug.LogWarning($"No listeners for Medium: {medium.Channel}");

                return;
            }

            // Invoke all registered listeners with the medium data
            evt.Invoke(medium);
        }


        /// <summary>
        /// Removes a listener callback from a medium. If no listeners remain for that medium,
        /// the medium entry is removed from the Mediations dictionary.
        /// </summary>
        /// <param name="medium">The medium to unsubscribe from.</param>
        /// <param name="listener">The callback to remove.</param>
        public static void Unsubscribe(Medium medium, Action<Medium> listener)
        {
            // Build the same listener identifier used during subscription
            var listenerGameObject = listener.Target as Object;
            var listenerGameObjectName = listenerGameObject?.name ?? "UnknownGameObject";
            var listenerClass = listener.Target?.GetType().Name ?? "UnknownClass";
            var listenerMethod = listener.Method.Name;
            var listenerString = $"{listenerGameObjectName}.{listenerClass}.{listenerMethod}";

            // Unregister from the mediator registry
            _registry.UnregisterListener(medium, listenerString);

            // Remove the listener from the delegate chain
            if (Mediations.ContainsKey(medium.Channel))
            {
                Mediations[medium.Channel] -= listener;
            }

            // Clean up empty delegate chains to avoid memory leaks
            if (Mediations.TryGetValue(medium.Channel, out var evt) && evt == null)
            {
                Mediations.Remove(medium.Channel);
            }
        }
    }
}
