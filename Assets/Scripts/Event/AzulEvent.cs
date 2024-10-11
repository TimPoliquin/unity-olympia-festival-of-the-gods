using System;
using System.Collections;
using System.Collections.Generic;
using Azul.PreviewEvents;
using UnityEngine;
using UnityEngine.Events;


namespace Azul
{
    namespace Event
    {
        [Serializable]
        public class EventTracker<T>
        {
            public int Total { get; init; }
            public int Progress { get; private set; } = 0;
            public T Payload { get; init; }

            public void Done()
            {
                this.Progress++;
            }

            public bool IsComplete()
            {
                return this.Progress == Total;
            }

            public WaitUntil WaitUntilCompleted()
            {
                return new WaitUntil(() => this.Progress == this.Total);
            }
        }
        [Serializable]
        public class AzulEvent<T>
        {
            [SerializeField] private UnityEvent<EventTracker<T>> unityEvent = new();
            private int registeredListeners = 0;

            public void AddListener(UnityAction<EventTracker<T>> listener)
            {
                this.registeredListeners++;
                this.unityEvent.AddListener(listener);
            }

            public EventTracker<T> Invoke(T payload)
            {
                EventTracker<T> tracker = new()
                {
                    Total = this.registeredListeners,
                    Payload = payload
                };
                this.unityEvent.Invoke(tracker);
                return tracker;
            }

            public void RemoveAllListeners()
            {
                this.registeredListeners = 0;
                this.unityEvent.RemoveAllListeners();
            }

        }
    }
}
