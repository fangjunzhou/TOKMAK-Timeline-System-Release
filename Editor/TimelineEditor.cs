using System;
using System.Linq;
using FinTOKMAK.EventSystem.Runtime;
using FinTOKMAK.TimelineSystem.Runtime;
using Hextant;
using UnityEditor;
using UnityEngine;

namespace Package.Editor
{
    public class TimelineEditor : EditorWindow
    {
        #region Private Field

        /// <summary>
        /// The working timeline object.
        /// </summary>
        private Timeline _timeline;

        /// <summary>
        /// The event config of Timeline Event System
        /// </summary>
        private UniversalEventConfig _eventConfig;

        #endregion

        #region Prviate Field

        private Vector2 _playableNodesScrollView;
        
        private Vector2 _checkNodesScrollView;
        
        private Vector2 _interruptNodesScrollView;

        private float _titleHeight = 60;

        #endregion
        
        [MenuItem("FinTOKMAK/Timeline System/Timeline Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<TimelineEditor>();
            window.titleContent = new GUIContent("Timeline Editor");
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        private void OnEnable()
        {
            _eventConfig = Settings<TimelineEventSettings>.instance.universalEventConfig;
        }

        private void OnGUI()
        {
            // The working timeline object
            EditorGUILayout.BeginVertical("Box");
            {
                GUILayout.Label("Working Timeline", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                _timeline = (Timeline) EditorGUILayout.ObjectField(_timeline, typeof(Timeline), false);
                if (GUILayout.Button("Save", GUILayout.Width(100)))
                {
                    // Set the timeline to dirty
                    EditorUtility.SetDirty(_timeline);
                    // Save the timeline
                    AssetDatabase.SaveAssetIfDirty(_timeline);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if (_timeline == null)
            {
                EditorGUILayout.HelpBox("No working timeline", MessageType.Warning);
                return;
            }

            if (_eventConfig == null)
            {
                EditorGUILayout.HelpBox("No Timeline Event Config", MessageType.Error);
                return;
            }
            
            if (_eventConfig.eventNames.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No event in the event config. There should be at least one event in the config.",
                    MessageType.Error);
                return;
            }

            // The vertical layout for Playable and Check Nodes
            EditorGUILayout.BeginVertical();

            Rect windowRect = this.position;

            EditorGUILayout.BeginVertical("Box", GUILayout.Height((windowRect.height - _titleHeight) / 3));
            {
                GUILayout.Label("Playable Nodes", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                {
                    _playableNodesScrollView = EditorGUILayout.BeginScrollView(_playableNodesScrollView);
                    {
                        EditorGUILayout.BeginHorizontal();
                
                        for (int i = 0; i < _timeline.playableNodes.Count; i++)
                        {
                            EditorGUILayout.BeginVertical("Box", GUILayout.Width(250), GUILayout.ExpandHeight(true));
                            {
                                GUILayout.Label($"Node {i}", EditorStyles.boldLabel);
                                _timeline.playableNodes[i].time =
                                    EditorGUILayout.FloatField("Time", _timeline.playableNodes[i].time);
                                _timeline.playableNodes[i].nodeType =
                                    (PlayableNodeType) EditorGUILayout.EnumPopup("Node Type",
                                        _timeline.playableNodes[i].nodeType);
                                
                                // Anim Node
                                if (_timeline.playableNodes[i].nodeType == PlayableNodeType.PlayAnim)
                                {
                                    _timeline.playableNodes[i].targetAnimator =
                                        EditorGUILayout.TextField("Target Animator",
                                            _timeline.playableNodes[i].targetAnimator);
                                    _timeline.playableNodes[i].animOperationType =
                                        (AnimOperationType) EditorGUILayout.EnumPopup("Anim Operation Type",
                                            _timeline.playableNodes[i].animOperationType);
                                    if (_timeline.playableNodes[i].animOperationType != AnimOperationType.PlayDirectly)
                                    {
                                        _timeline.playableNodes[i].targetVar = EditorGUILayout.TextField(
                                            "Target Animator Variable",
                                            _timeline.playableNodes[i].targetVar);
                                    }

                                    // Different anim operation
                                    if (_timeline.playableNodes[i].animOperationType == AnimOperationType.SetBool)
                                    {
                                        _timeline.playableNodes[i].boolValue = EditorGUILayout.Toggle("Bool Value",
                                            _timeline.playableNodes[i].boolValue);
                                    }
                                    else if (_timeline.playableNodes[i].animOperationType == AnimOperationType.SetFloat)
                                    {
                                        _timeline.playableNodes[i].floatValue = EditorGUILayout.FloatField("Float Value",
                                            _timeline.playableNodes[i].floatValue);
                                    }
                                    else if (_timeline.playableNodes[i].animOperationType == AnimOperationType.SetInt)
                                    {
                                        _timeline.playableNodes[i].intValue = EditorGUILayout.IntField("Int Value",
                                            _timeline.playableNodes[i].intValue);
                                    }
                                    else if (_timeline.playableNodes[i].animOperationType == AnimOperationType.PlayDirectly)
                                    {
                                        _timeline.playableNodes[i].targetAnimation = EditorGUILayout.TextField("Target Animation",
                                            _timeline.playableNodes[i].targetAnimation);
                                        _timeline.playableNodes[i].targetLayer = EditorGUILayout.IntField("Target Layer",
                                            _timeline.playableNodes[i].targetLayer);
                                    }
                                }
                                // Event
                                else if (_timeline.playableNodes[i].nodeType == PlayableNodeType.InvokeEvent)
                                {
                                    String[] options = _eventConfig.eventNames.ToArray();
                                    int index;
                                    if (_eventConfig.eventNames.Contains(_timeline.playableNodes[i].eventName))
                                    {
                                        index = _eventConfig.eventNames.IndexOf(_timeline.playableNodes[i].eventName);
                                    }
                                    else
                                    {
                                        index = 0;
                                        _timeline.playableNodes[i].eventName = options[index];
                                    }
                                    EditorGUI.BeginChangeCheck();
                                    index = EditorGUILayout.Popup("Event", index, options);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        _timeline.playableNodes[i].eventName = options[index];
                                        EditorUtility.SetDirty(_timeline);
                                    }

                                    EditorGUILayout.TextArea(_timeline.playableNodes[i].eventName);
                                }
                                
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("-"))
                                {
                                    _timeline.playableNodes.RemoveAt(i);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    // Left buttons
                    
                    GUILayout.FlexibleSpace();
                    
                    // Right buttons
                    
                    if (GUILayout.Button("Sort", EditorStyles.miniButtonRight, GUILayout.Width(50)))
                    {
                        _timeline.playableNodes = _timeline.playableNodes.OrderBy(x => x.time).ToList();
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                    {
                        if (_timeline.playableNodes.Count > 0)
                        {
                            _timeline.playableNodes.Add(new PlayableNode()
                            {
                                time = _timeline.playableNodes[_timeline.playableNodes.Count-1].time
                            });
                        }
                        else
                        {
                            _timeline.playableNodes.Add(new PlayableNode()
                            {
                                time = 0
                            });
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Check nodes
            EditorGUILayout.BeginVertical("Box", GUILayout.Height((windowRect.height - _titleHeight) / 3));
            {
                GUILayout.Label("Check Nodes", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                {
                    _checkNodesScrollView = EditorGUILayout.BeginScrollView(_checkNodesScrollView);
                    {
                        EditorGUILayout.BeginHorizontal();
                
                        for (int i = 0; i < _timeline.checkNodes.Count; i++)
                        {
                            EditorGUILayout.BeginVertical("Box", GUILayout.Width(250), GUILayout.ExpandHeight(true));
                            {
                                GUILayout.Label($"Node {i}", EditorStyles.boldLabel);

                                _timeline.checkNodes[i].nodeType =
                                    (CheckNodeType) EditorGUILayout.EnumPopup("Node Type",
                                        _timeline.checkNodes[i].nodeType);

                                if (_timeline.checkNodes[i].nodeType == CheckNodeType.CheckAnim)
                                {
                                    _timeline.checkNodes[i].targetAnimVar =
                                        EditorGUILayout.TextField("Target Anim Variable",
                                            _timeline.checkNodes[i].targetAnimVar);
                                }
                                else if (_timeline.checkNodes[i].nodeType == CheckNodeType.CheckEvent)
                                {
                                    String[] options = _eventConfig.eventNames.ToArray();
                                    int index;
                                    if (_eventConfig.eventNames.Contains(_timeline.checkNodes[i].targetEvent))
                                    {
                                        index = _eventConfig.eventNames.IndexOf(_timeline.checkNodes[i].targetEvent);
                                    }
                                    else
                                    {
                                        index = 0;
                                        _timeline.checkNodes[i].targetEvent = options[index];
                                    }
                                    index = EditorGUILayout.Popup("Target Event Name", index, options);
                                    _timeline.checkNodes[i].targetEvent = options[index];

                                    EditorGUILayout.TextArea(_timeline.checkNodes[i].targetEvent);
                                }
                                
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("-"))
                                {
                                    _timeline.checkNodes.RemoveAt(i);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();
                {
                    // Left buttons
                    
                    GUILayout.FlexibleSpace();
                    
                    // Right buttons
                    
                    GUILayout.Space(10);
                    if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                    {
                        _timeline.checkNodes.Add(new CheckNode());
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            // Interrupt Node
            EditorGUILayout.BeginVertical("Box", GUILayout.Height((windowRect.height - _titleHeight) / 3));
            {
                GUILayout.Label("Interrupt Nodes", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                {
                    _interruptNodesScrollView = EditorGUILayout.BeginScrollView(_interruptNodesScrollView);
                    {
                        EditorGUILayout.BeginHorizontal();
                
                        for (int i = 0; i < _timeline.interruptNodes.Count; i++)
                        {
                            EditorGUILayout.BeginVertical("Box", GUILayout.Width(250), GUILayout.ExpandHeight(true));
                            {
                                GUILayout.Label($"Node {i}", EditorStyles.boldLabel);
                                _timeline.interruptNodes[i].nodeType =
                                    (PlayableNodeType) EditorGUILayout.EnumPopup("Node Type",
                                        _timeline.interruptNodes[i].nodeType);
                                
                                // Anim Node
                                if (_timeline.interruptNodes[i].nodeType == PlayableNodeType.PlayAnim)
                                {
                                    _timeline.interruptNodes[i].targetAnimator =
                                        EditorGUILayout.TextField("Target Animator",
                                            _timeline.interruptNodes[i].targetAnimator);
                                    _timeline.interruptNodes[i].animOperationType =
                                        (AnimOperationType) EditorGUILayout.EnumPopup("Anim Operation Type",
                                            _timeline.interruptNodes[i].animOperationType);
                                    if (_timeline.interruptNodes[i].animOperationType != AnimOperationType.PlayDirectly)
                                    {
                                        _timeline.interruptNodes[i].targetVar = EditorGUILayout.TextField(
                                            "Target Animator Variable",
                                            _timeline.interruptNodes[i].targetVar);
                                    }

                                    // Different anim operation
                                    if (_timeline.interruptNodes[i].animOperationType == AnimOperationType.SetBool)
                                    {
                                        _timeline.interruptNodes[i].boolValue = EditorGUILayout.Toggle("Bool Value",
                                            _timeline.interruptNodes[i].boolValue);
                                    }
                                    else if (_timeline.interruptNodes[i].animOperationType == AnimOperationType.SetFloat)
                                    {
                                        _timeline.interruptNodes[i].floatValue = EditorGUILayout.FloatField("Float Value",
                                            _timeline.interruptNodes[i].floatValue);
                                    }
                                    else if (_timeline.interruptNodes[i].animOperationType == AnimOperationType.SetInt)
                                    {
                                        _timeline.interruptNodes[i].intValue = EditorGUILayout.IntField("Int Value",
                                            _timeline.interruptNodes[i].intValue);
                                    }
                                    else if (_timeline.interruptNodes[i].animOperationType == AnimOperationType.PlayDirectly)
                                    {
                                        _timeline.interruptNodes[i].targetAnimation = EditorGUILayout.TextField("Target Animation",
                                            _timeline.interruptNodes[i].targetAnimation);
                                        _timeline.interruptNodes[i].targetLayer = EditorGUILayout.IntField("Target Layer",
                                            _timeline.interruptNodes[i].targetLayer);
                                    }
                                }
                                // Event
                                else if (_timeline.interruptNodes[i].nodeType == PlayableNodeType.InvokeEvent)
                                {
                                    String[] options = _eventConfig.eventNames.ToArray();
                                    int index;
                                    if (_eventConfig.eventNames.Contains(_timeline.interruptNodes[i].eventName))
                                    {
                                        index = _eventConfig.eventNames.IndexOf(_timeline.interruptNodes[i].eventName);
                                    }
                                    else
                                    {
                                        index = 0;
                                        _timeline.interruptNodes[i].eventName = options[index];
                                    }
                                    EditorGUI.BeginChangeCheck();
                                    index = EditorGUILayout.Popup("Event", index, options);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        _timeline.interruptNodes[i].eventName = options[index];
                                        EditorUtility.SetDirty(_timeline);
                                    }

                                    EditorGUILayout.TextArea(_timeline.interruptNodes[i].eventName);
                                }
                                
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("-"))
                                {
                                    _timeline.interruptNodes.RemoveAt(i);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    // Left buttons
                    
                    GUILayout.FlexibleSpace();
                    
                    // Right buttons
                    
                    if (GUILayout.Button("Sort", EditorStyles.miniButtonRight, GUILayout.Width(50)))
                    {
                        _timeline.interruptNodes = _timeline.interruptNodes.OrderBy(x => x.time).ToList();
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                    {
                        if (_timeline.interruptNodes.Count > 0)
                        {
                            _timeline.interruptNodes.Add(new PlayableNode()
                            {
                                time = _timeline.interruptNodes[_timeline.interruptNodes.Count-1].time
                            });
                        }
                        else
                        {
                            _timeline.interruptNodes.Add(new PlayableNode()
                            {
                                time = 0
                            });
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
    }
}