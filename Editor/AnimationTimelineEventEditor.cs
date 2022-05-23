using System;
using System.Collections.Generic;
using System.Linq;
using FinTOKMAK.EventSystem.Runtime;
using FinTOKMAK.TimelineSystem.Runtime;
using Hextant;
using UnityEditor;
using UnityEngine;

namespace Package.Editor
{
    public class AnimationTimelineEventEditor : EditorWindow
    {
        #region Prviate Field

        /// <summary>
        /// The Universal Event Config of the animation event.
        /// </summary>
        private UniversalEventConfig _eventConfig;

        /// <summary>
        /// The clip current operating.
        /// </summary>
        private AnimationClip _clip;

        /// <summary>
        /// All the animation events in the animation clip.
        /// </summary>
        private AnimationEvent[] _animationEvents = new AnimationEvent[]{};

        /// <summary>
        /// The current working AnimationWindow.
        /// </summary>
        private AnimationWindow _window;

        /// <summary>
        /// The frame current play head is pointing to.
        /// </summary>
        private int _frame;

        /// <summary>
        /// The time current play head is pointing to.
        /// </summary>
        private float _time;

        #endregion

        #region Editor Variables

        /// <summary>
        /// The position of the main scroll view.
        /// </summary>
        private Vector2 _scrollPos;

        #endregion
        
        [MenuItem("FinTOKMAK/Timeline System/Animation Timeline Event Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<AnimationTimelineEventEditor>();
            window.titleContent = new GUIContent("TITLE");
            window.Show();
        }

        private void OnEnable()
        {
            _eventConfig = Settings<TimelineEventSettings>.instance.universalEventConfig;
        }

        private void OnGUI()
        {
            UpdateWorkingTarget();
            
            titleContent.text = "Animation Timeline Event Editor";

            // Working window.
            EditorGUILayout.BeginVertical("Box");
            {
                if (_window == null)
                {
                    EditorGUILayout.HelpBox("No Animation Window!", MessageType.Warning);
                }

                if (_clip == null)
                {
                    EditorGUILayout.HelpBox("No Working Clip!", MessageType.Warning);
                }
            }
            EditorGUILayout.EndVertical();
            
            // Working clip
            EditorGUILayout.BeginVertical("Box");

            {
                GUILayout.Label("Working clip", EditorStyles.boldLabel);

                EditorGUILayout.ObjectField("Current clip", _clip, typeof(AnimationClip));
            }
            
            if (_eventConfig == null)
            {
                EditorGUI.HelpBox(position, "Global Event Settings missing.", MessageType.Error);
                EditorGUILayout.Space(position.height - 18);
                return;
            }

            EditorGUILayout.EndVertical();

            // Animation event editor

            EditorGUILayout.BeginVertical("Box");
            
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                {
                    GUILayout.Label("Animation Event Editor", EditorStyles.boldLabel);

                    for (int i = 0; i < _animationEvents.Length; i++)
                    {
                        AnimationEvent animEvent = _animationEvents[i];
                        
                        EditorGUILayout.BeginVertical("Box");
                        
                        {
                            GUILayout.Label($"Animation event {i}", EditorStyles.boldLabel);

                            EditorGUILayout.TextField("Time", $"{animEvent.time}");
                            
                            EditorGUILayout.TextField("Frame", $"{animEvent.time * _clip.frameRate}");

                            string eventName = animEvent.stringParameter;
                            string[] options = _eventConfig.eventNames.ToArray();
                            int index;
                            if (_eventConfig.eventNames.Contains(eventName))
                            {
                                index = _eventConfig.eventNames.IndexOf(eventName);
                            }
                            else
                            {
                                index = 0;
                                _animationEvents[i].stringParameter = options[index];
                            }

                            index = EditorGUILayout.Popup("Animation event", index, options);
                            _animationEvents[i].stringParameter = options[index];
                            AnimationUtility.SetAnimationEvents(_clip, _animationEvents);
                        }
                        
                        EditorGUILayout.EndVertical();
                    }

                    if (GUILayout.Button("Add Event"))
                    {
                        if (_window == null || _clip == null)
                        {
                            EditorUtility.DisplayDialog("No Working Clip",
                                "You have no working animation clip to operate! You cannot add Timeline Event!", "OK");
                            return;
                        }
                    
                        List<AnimationEvent> events = AnimationUtility.GetAnimationEvents(_clip).ToList();
                        events.Add(new AnimationEvent()
                        {
                            time = _time,
                            functionName = "InvokeEvent"
                        });
                        AnimationUtility.SetAnimationEvents(_clip, events.ToArray());
                        
                        // Update event display
                        _animationEvents = _clip.events;
                    }
                }
            
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Save"))
            {
                AnimationUtility.SetAnimationEvents(_clip, _animationEvents);
            }
        }
        
        #region Private Methods

        /// <summary>
        /// Call this method to get the current working AnimationWindow, anim clip and other info.
        /// </summary>
        private void UpdateWorkingTarget()
        {
            // Get window
            if (_window == null)
            {
                _window = AnimationWindow.GetWindow<AnimationWindow>();
            }
            // Get clip
            _clip = _window.animationClip;
            // Get frame
            _frame = _window.frame;
            // Get time
            _time = _window.time;
            
            // New animation clip is dragged in.
            if (_clip != null)
            {
                _animationEvents = _clip.events;
            }
            else
            {
                _animationEvents = Array.Empty<AnimationEvent>();
            }
        }

        #endregion
    }
}