using System;
using System.Collections.Generic;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     The Service Locator provides a global registry for services (timing, logging, audio, input devices).
    ///     Unlike Singleton, it decouples the interface from the implementation — callers request 'an IAudioService' without knowing the concrete class.
    ///     This enables: (1) swapping implementations at runtime (real audio vs. muted for testing), (2) no MonoBehaviour requirement for services,
    ///     (3) explicit registration instead of implicit FindObjectOfType.
    ///     <b>Trade-off vs Singleton:</b> More flexible but less discoverable. Prefer Service Locator when you need experiment code independent from concrete service providers.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();


        /// <summary>
        ///     Registers a service instance by its type.
        /// </summary>
        /// <typeparam name="T">The contract type used as the service key.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
        public static void Register<T>(T service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), $"Cannot register a null service for type {typeof(T).Name}.");
            }

            _services[typeof(T)] = service;
        }


        /// <summary>
        ///     Retrieves a previously registered service instance.
        /// </summary>
        /// <typeparam name="T">The contract type used as the service key.</typeparam>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no matching service has been registered.</exception>
        public static T Get<T>()
        {
            if (TryGet(out T service))
            {
                return service;
            }

            throw new InvalidOperationException($"No service registered for type {typeof(T).FullName}. Register a service before calling Get<{typeof(T).Name}>().");
        }


        /// <summary>
        ///     Attempts to retrieve a previously registered service instance.
        /// </summary>
        /// <typeparam name="T">The contract type used as the service key.</typeparam>
        /// <param name="service">When this method returns, contains the resolved service or default when not found.</param>
        /// <returns><see langword="true"/> when a service is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet<T>(out T service)
        {
            if (_services.TryGetValue(typeof(T), out object registeredService) && registeredService is T typedService)
            {
                service = typedService;
                return true;
            }

            service = default;
            return false;
        }


        /// <summary>
        ///     Removes a registered service for the specified contract type.
        /// </summary>
        /// <typeparam name="T">The contract type used as the service key.</typeparam>
        public static void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }


        /// <summary>
        ///     Removes all registered services.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}
