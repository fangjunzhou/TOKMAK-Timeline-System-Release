using System;
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
            titleContent.text = "Animation Timeline Event Editor";

            // Working clip
            EditorGUILayout.BeginVertical("Box");

            {
                GUILayout.Label("Working clip", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                _clip = (AnimationClip) EditorGUILayout.ObjectField("Current clip", _clip, typeof(AnimationClip),
                    false);
                if (EditorGUI.EndChangeCheck())
                {
                    // New animation clip is dragged in.
                    if (_clip != null)
                    {
                        _animationEvents = _clip.events;
                    }
                }
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

                        }
                        
                        EditorGUILayout.EndVertical();
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
    }
}