using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    [System.Serializable]
    public class AnimatorDict : SerializableDictionary<string, Animator>
    {
        
    }

    public class TimelineSystem : MonoBehaviour
    {
        #region Public Field

        /// <summary>
        /// The string-Animator dictionary that keep track of
        /// all the Animators used by this Timeline System
        /// </summary>
        public AnimatorDict animatorDict = new AnimatorDict();

        #endregion
        
        #region Private Field

        /// <summary>
        /// A dict for the event system to call the event by event name.
        /// </summary>
        private Dictionary<string, Action> _eventTable = new Dictionary<string, Action>();

        /// <summary>
        /// The string-AnimEventInvoker dictionary that keep track of
        /// all the AnimEventInvoker used by this Timeline System
        /// </summary>
        private Dictionary<string, AnimEventInvoker> _animEventTable = new Dictionary<string, AnimEventInvoker>();

        #endregion

        private void Awake()
        {
            // Initialize the animEventTable
            foreach (string animatorDictKey in animatorDict.Keys)
            {
                // Check if the animator has the corresponding AnimEventInvoker
                AnimEventInvoker currEventInvoker =
                    animatorDict[animatorDictKey].gameObject.GetComponent<AnimEventInvoker>();
                
                // If no AnimEventInvoker was found, create a new one
                if (currEventInvoker == null)
                {
                    currEventInvoker = animatorDict[animatorDictKey].gameObject.AddComponent<AnimEventInvoker>();
                }
                
                // Register the event handler of all the event handlers
                currEventInvoker.eventHandler += InvokeEvent;
                
                // Add the event Invoker into the animEventTable
                _animEventTable.Add(animatorDictKey, currEventInvoker);
            }
        }

        private void OnDestroy()
        {
            // Unregister all the event handlers
            foreach (string key in _animEventTable.Keys)
            {
                _animEventTable[key].eventHandler -= InvokeEvent;
            }
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

        /// <summary>
        /// The method for SkillSystem and WeaponSystem to play a timeline.
        /// </summary>
        /// <param name="timeline">The timeline to play</param>
        public void PlayTimeline(Timeline timeline)
        {
            // All the event to be checked.
            HashSet<string> checkEventNames;
            // All the animations to be checked.
            HashSet<string> checkAnim;
            
            // TODO: Finish implement PlayTimeline
        }

        #endregion

        #region Private Methods

        

        #endregion
    }
}
