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
        /// <summary>Indicates whether the event instance is currently in use, preventing concurrent access.</summary>
        public static bool InUse => instance.inUse;
        private static T instance = new();


        /// <summary>Returns the singleton instance of the event. Only one instance can be in use at a time—the InUse flag prevents concurrent access.</summary>
        public static T Get()
        {
            //if (instance == null)
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
            //Automatically fires when 'disposed'
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
