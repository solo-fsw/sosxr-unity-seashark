using System;


//Modified from: https://github.com/FuzzyHobo/UnityCallbackAndEventTutorial/blob/master/Assets/Scenes/EventCallbackScene/Event.cs
//Now doesn't generate garbage
//How to at bottom


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>Generic, garbage-free event system using a singleton pattern with disposal-based firing.</summary>
    /// <typeparam name="T">The concrete event type deriving from GlobalEvent&lt;T&gt;.</typeparam>
    public class GlobalEvent<T> where T : GlobalEvent<T>, IDisposable, new()
    {
        /// <summary>Delegate type for event listeners. Receives the event data of type <typeparamref name="T"/>.</summary>
        public delegate void EventListener(T info);


        private bool inUse;
        // InUse signals whether this particular event instance is currently owned/being fired.
        // This prevents re-entrancy: firing the event while another fire is still in progress would
        // otherwise cause nested callbacks to run on the same instance and could lead to infinite loops.
        // Access via the static InUse property to guard against concurrent usage.
        /// <summary>Indicates whether the event instance is currently in use, preventing concurrent access.</summary>
        public static bool InUse => instance.inUse;
        private static T instance = new();


        /// <summary>Returns the singleton instance of the event. Only one instance can be in use at a time—the InUse flag prevents concurrent access.</summary>
        public static T Get()
        {
            // Always construct a fresh instance for ownership semantics. A previous instance,
            // if any, should have been disposed and released via the pattern below.
            instance = new T();

            if (InUse)
            {
                throw new Exception("Cannot get object twice! Please fire event before grabbing the instance again");
            }

            instance.inUse = true;

            return instance;
        }


        private static event EventListener listeners;


        /// <summary>Registers a listener callback to be invoked when the event fires.</summary>
        public static void RegisterListener(EventListener listener)
        {
            listeners += listener;
        }


        /// <summary>Removes a listener callback from the event.</summary>
        public static void UnregisterListener(EventListener listener)
        {
            listeners -= listener;
        }


        /// <summary>Immediately invokes all registered listeners with the event data, then resets the event state.</summary>
        public void FireEvent()
        {
            // Fire only if we are actively in use. This check helps avoid accidental re-fire loops.
            if (!inUse)
            {
                throw new Exception("This event has already fired, to prevent infinite loops you can't refire an event");
            }

            listeners?.Invoke(this as T);

            inUse = false;
            Reset();
        }


        /// <summary>Resets the event data to its default state. This method is protected virtual and may be overridden by subclasses to implement custom reset behavior.</summary>
        protected virtual void Reset()
        {
        }


        #region IDisposable Implementation

        // Reflects the disposal state. When Disposed is true, the instance is considered available
        // for reuse by subsequent Get() calls. The getter/setter translate between 'inUse' and 'disposed'
        // semantics to keep the public API intuitive.
        private bool Disposed
        {
            get => !inUse;
            set => inUse = !value;
        }


        /// <summary>Fires the event and then disposes the instance, making it available for reuse via Get().</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        // Protected implementation of Dispose pattern.
        // The disposing flag ensures the event isn't fired after being disposed, and coordinates with
        // the lifecycle so that a new instance can be obtained again from Get().
        /// <summary>Protected virtual implementation of the IDisposable pattern.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            // Fire listeners before we mark as disposed; this aligns with the pattern that the
            // act of disposing an instance is the moment the event is fired to observers.
            FireEvent();
            Disposed = true;
        }

        #endregion
    }
}
/*Example event
 *
 *	public class DebugEvent : GlobalEvent<DebugEvent>, IDisposable {
 *		public int VerbosityLevel;
 *	}
 */


/* Example use case
 *	using (var e = DebugEvent.Get()) { //Grab ownership of event instance
 *		e.VerbosityLevel = 3;
 *		e.Description = "Something happened!";
 *	} //Event get's fired on dispose
 */
