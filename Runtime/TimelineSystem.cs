using System;
using System.Collections;
using System.Collections.Generic;
using FinTOKMAK.EventSystem.Runtime;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    [System.Serializable]
    public class AnimatorDict : SerializableDictionary<string, Animator>
    {
        
    }

    [RequireComponent(typeof(TimelineEventManager))]
    public class TimelineSystem : MonoBehaviour
    {
        #region Public Field

        /// <summary>
        /// The string-Animator dictionary that keep track of
        /// all the Animators used by this Timeline System
        /// </summary>
        public AnimatorDict animatorDict = new AnimatorDict();

        /// <summary>
        /// If current TimelineSystem use debug.
        /// </summary>
        public bool useDebug = false;

        #endregion

        #region Hide Public Field

        /// <summary>
        /// The event manager of the Timeline System.
        /// </summary>
        [HideInInspector]
        public TimelineEventManager eventManager;

        #endregion
        
        #region Private Field

        /// <summary>
        /// The string-AnimEventInvoker dictionary that keep track of
        /// all the AnimEventInvoker used by this Timeline System
        /// </summary>
        private Dictionary<string, AnimEventInvoker> _animEventTable = new Dictionary<string, AnimEventInvoker>();

        /// <summary>
        /// The listener of local event invoke system.
        /// </summary>
        private Action<string, IEventData> _eventSystemInvokeHook;

        /// <summary>
        /// All the playing timeline.
        /// </summary>
        private Dictionary<Timeline, Coroutine> _playingTimeline = new Dictionary<Timeline, Coroutine>();

        #endregion

        private void Awake()
        {
            eventManager = gameObject.GetComponent<TimelineEventManager>();
            
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
        /// <param name="eventName">The name of the event.</param>
        /// <param name="regEvent">The event to register.</param>
        public void RegisterEvent(string eventName, Action<IEventData> regEvent)
        {
            eventManager.RegisterEvent(eventName, regEvent);
        }

        /// <summary>
        /// The method to unregister a exist event.
        /// </summary>
        /// <param name="name">The name of event to unregister.</param>
        /// <param name="unregEvent">The target event to unregister.</param>
        public void UnRegisterEvent(string eventName, Action<IEventData> unregEvent)
        {
            eventManager.UnRegisterEvent(eventName, unregEvent);
        }

        /// <summary>
        /// The method to invoke a event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        public void InvokeEvent(string eventName, IEventData data)
        {
            eventManager.InvokeEvent(eventName, data);
            _eventSystemInvokeHook?.Invoke(eventName, data);
        }

        /// <summary>
        /// The method for SkillSystem and WeaponSystem to play a timeline.
        /// </summary>
        /// <param name="timeline">The timeline to play.</param>
        /// <param name="allowDuplicate">If the timeline player allow playing duplicate timeline.</param>
        public void PlayTimeline(Timeline timeline, bool allowDuplicate = false)
        {
            if(!allowDuplicate && _playingTimeline.ContainsKey(timeline))
            {
                Debug.Log("Duplicate timeline.");
                return;
            }
            
            // Finish implement PlayTimeline
            Coroutine coroutine = StartCoroutine(TimelineCoroutine(timeline));
            if (!_playingTimeline.ContainsKey(timeline))
                _playingTimeline.Add(timeline, coroutine);
            else
                _playingTimeline[timeline] = coroutine;
        }

        /// <summary>
        /// Stop a certain playing timeline.
        /// </summary>
        /// <param name="timeline"></param>
        public void StopTimeline(Timeline timeline)
        {
            if (!_playingTimeline.ContainsKey(timeline))
                throw new InvalidOperationException($"Timeline: {timeline} is not playing currently.");
            
            StopCoroutine(_playingTimeline[timeline]);
            _playingTimeline.Remove(timeline);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A coroutine to play the timeline.
        /// </summary>
        /// <param name="timeline">The timeline to be played.</param>
        /// <returns>enumerator</returns>
        private IEnumerator TimelineCoroutine(Timeline timeline)
        {
            // All the event to be checked.
            HashSet<string> checkEventNames = new HashSet<string>();
            // All the animations to be checked.
            HashSet<string> checkAnimNames = new HashSet<string>();

            // Initialize the checkEventNames and checkAnimNames

            foreach (CheckNode checkNode in timeline.checkNodes)
            {
                if (checkNode.nodeType == CheckNodeType.CheckAnim)
                {
                    checkAnimNames.Add(checkNode.targetAnimVar);
                }
                else if (checkNode.nodeType == CheckNodeType.CheckEvent)
                {
                    checkEventNames.Add(checkNode.targetEvent);
                }
            }

            // The internal function to remove the event from the event set
            void RemoveEventSet(string s, IEventData data)
            {
                checkEventNames.Remove(s);
            }
            
            // Start listening to all the event invoke when timeline is played
            _eventSystemInvokeHook += RemoveEventSet;

            // The timer
            float startTime = Time.realtimeSinceStartup;

            // Play the nodes
            foreach (PlayableNode playableNode in timeline.playableNodes)
            {
                // Calculate the wait time
                float waitTime = playableNode.time - (Time.realtimeSinceStartup - startTime);
                // If the node should have been played, play immediately.
                if (waitTime <= 0)
                {
                    if (playableNode.nodeType == PlayableNodeType.EndMark)
                    {
                        // End check here
                        EndCheck(checkEventNames, checkAnimNames);

                        break;
                    }
                    PlayNode(playableNode, checkEventNames, checkAnimNames);
                    // Continue to the next node.
                    continue;
                }
                // wait until the time to play the node
                yield return new WaitForSeconds(waitTime);
                
                if (playableNode.nodeType == PlayableNodeType.EndMark)
                {
                    // End check here
                    EndCheck(checkEventNames, checkAnimNames);
                    
                    break;
                }
                PlayNode(playableNode, checkEventNames, checkAnimNames);
            }
            
            // When finished, stop listening to the event hook.
            _eventSystemInvokeHook -= RemoveEventSet;
            _playingTimeline.Remove(timeline);
        }

        /// <summary>
        /// The method to do the end check.
        /// </summary>
        /// <param name="checkEventNames">The HashSet to store all the event names to be checked.</param>
        /// <param name="checkAnimNames">The HashSet to store all the animation names to be checked.</param>
        /// <exception cref="MissingMemberException">When required event or animation is not played.</exception>
        private void EndCheck(HashSet<string> checkEventNames, HashSet<string> checkAnimNames)
        {
            if (checkEventNames.Count != 0 || checkAnimNames.Count != 0)
            {
                foreach (string eventName in checkEventNames)
                {
                    Debug.LogWarning($"Event {eventName} is not invoked!");
                }

                foreach (string animName in checkAnimNames)
                {
                    Debug.LogWarning($"Animation {animName} is not invoked.");
                }

                throw new MissingMemberException("Required Event or Animation is not played.");
            }

            if (useDebug)
            {
                Debug.Log("All the required events are invoked. All the required animation is played.");
            }
        }

        /// <summary>
        /// The method to play a PlayableNode.
        /// </summary>
        /// <param name="node">The PlayableNode to be played.</param>
        /// <param name="checkEvent">The event to be checked.</param>
        /// <param name="checkAnim">The animation to be checked.</param>
        private void PlayNode(PlayableNode node, HashSet<string> checkEvent, HashSet<string> checkAnim)
        {
            if (node.nodeType == PlayableNodeType.EndMark)
            {
                return;
            }

            // Invoke the event
            if (node.nodeType == PlayableNodeType.InvokeEvent)
            {
                // Remove the name of the event from the event hash set
                checkEvent.Remove(node.eventName);
                // TODO: Invoke the event here.
                InvokeEvent(node.eventName, new EventData());
            }
            // Trigger the animation
            else if (node.nodeType == PlayableNodeType.PlayAnim)
            {
                // Remove the name of animation from the anim hash set
                checkAnim.Remove(node.targetVar);

                switch (node.animOperationType)
                {
                    case AnimOperationType.SetTrigger:
                        animatorDict[node.targetAnimator].SetTrigger(node.targetVar);
                        break;
                    
                    case AnimOperationType.SetBool:
                        animatorDict[node.targetAnimator].SetBool(node.targetVar, node.boolValue);
                        break;
                    
                    case AnimOperationType.SetFloat:
                        animatorDict[node.targetAnimator].SetFloat(node.targetVar, node.floatValue);
                        break;
                    
                    case AnimOperationType.SetInt:
                        animatorDict[node.targetAnimator].SetInteger(node.targetVar, node.intValue);
                        break;
                    
                    case AnimOperationType.PlayDirectly:
                        animatorDict[node.targetAnimator].Play(node.targetAnimation, node.targetLayer);
                        break;

                    default:
                        throw new ArgumentException("AnimOperationType not supported.");
                }
            }
        }

        #endregion
    }
}
