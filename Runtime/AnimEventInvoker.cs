using System;
using FinTOKMAK.EventSystem.Runtime;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    public class AnimEventInvoker : MonoBehaviour
    {
        #region Public Field

        /// <summary>
        /// The event handler for the timeline system to register.
        /// </summary>
        public Action<string, IEventData> eventHandler;

        #endregion

        #region Public Methods

        /// <summary>
        /// The method for the animation event to invoke an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        public void InvokeEvent(string eventName)
        {
            eventHandler?.Invoke(eventName, new EventData());
        }

        #endregion
    }
}