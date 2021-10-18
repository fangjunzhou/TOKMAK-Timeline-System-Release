using System;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    public class AnimEventInvoker : MonoBehaviour
    {
        #region Public Field

        /// <summary>
        /// The event handler for the timeline system to register.
        /// </summary>
        public Action<string> eventHandler;

        #endregion

        #region Public Methods

        /// <summary>
        /// The method for the animation event to invoke an event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        public void InvokeEvent(string name)
        {
            eventHandler?.Invoke(name);
        }

        #endregion
    }
}