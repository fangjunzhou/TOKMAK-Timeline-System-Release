using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    [System.Serializable]
    public class AnimatorDict : SerializableDictionary<string, Animator>
    {
        
    }

    [System.Serializable]
    public class AnimEventDict : SerializableDictionary<string, AnimEventInvoker>
    {
        
    }

    public class TimelineSystem : MonoBehaviour
    {
        #region Public Field

        /// <summary>
        /// The string-Animator dictionary that keep track of
        /// all the Animators used by this Timeline System
        /// </summary>
        public AnimatorDict animatorDict;

        /// <summary>
        /// The string-AnimEventInvoker dictionary that keep track of
        /// all the AnimEventInvoker used by this Timeline System
        /// </summary>
        public AnimEventDict animEventDict;

        #endregion
        
        #region Private Field

        /// <summary>
        /// A dict for the event system to call the event by event name.
        /// </summary>
        private Dictionary<string, Action> _eventTable;

        #endregion
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        #region Public Methods

        /// <summary>
        /// The method for the upper system register timeline events.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="regEvent">The event to register.</param>
        public void RegisterEvent(string name, Action regEvent)
        {
            // Check if the eventTable has the event with the same name.
            // If not, create a event table key-value pair.
            if (!_eventTable.ContainsKey(name))
            {
                _eventTable.Add(name, () => { });
            }
            
            // Register the event.
            _eventTable[name] += regEvent;
        }

        /// <summary>
        /// The method to unregister a exist event.
        /// </summary>
        /// <param name="name">The name of event to unregister.</param>
        /// <param name="unregEvent">The target event to unregister.</param>
        public void UnRegisterEvent(string name, Action unregEvent)
        {
            // Check if there's a event with the certain name.
            if (!_eventTable.ContainsKey(name))
            {
                throw new InvalidOperationException($"No event with name {name}!");
            }

            _eventTable[name] -= unregEvent;
        }

        /// <summary>
        /// The method to invoke a event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        public void InvokeEvent(string name)
        {
            _eventTable[name]?.Invoke();
        }

        #endregion
    }
}
