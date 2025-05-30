﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using JL_GameProdEnv_CustomPackage.Runtime;

namespace JL_GameProdEnv_CustomPackage.Editor
{
    /// <summary>
    /// Custom Unity Editor window for managing gameplay recordings and replays.
    /// Provides functionality to record, save, and replay physics-based gameplay with
    /// support for both player and non-player objects.
    /// </summary>
    public class ReplayToolsWindow : EditorWindow
    {
        /// <summary>
        /// List of rigidbodies associated with player-controlled objects.
        /// These are recorded at a higher frequency for better precision.
        /// </summary>
        private List<Rigidbody> playerRigidbodies = new List<Rigidbody>();
        
        /// <summary>
        /// List of regular rigidbodies in the scene that are not player-controlled.
        /// These are recorded at a lower frequency to optimize performance.
        /// </summary>
        private List<Rigidbody> normalRigidbodies = new List<Rigidbody>();
    
        /// <summary>Scroll position for the main window content.</summary>
        private Vector2 scrollPos;
        
        /// <summary>Scroll position for the saved sessions list.</summary>
        private Vector2 sessionScrollPos;
        
        /// <summary>Currently selected rigidbody for detailed information display.</summary>
        private Rigidbody selectedRigidbody;
        
        // Foldout state variables
        private bool playerFoldout = true;
        private bool normalFoldout = true;
        private bool recordingFoldout = true;
        private bool sessionsFoldout = true;
        private bool replayFoldout = true;
        
        /// <summary>Name to be used when saving the current recording session.</summary>
        private string sessionName = "";
        
        /// <summary>Reference to the ReplayRecorder instance that manages recording.</summary>
        private ReplayRecorder recorder;
        
        /// <summary>Currently selected replay session for playback.</summary>
        private ReplaySession selectedSession;
        
        /// <summary>Reference to the ReplayPlayer instance that manages playback.</summary>
        private ReplayPlayer player;
        
        /// <summary>Playback speed multiplier (1.0 = normal speed).</summary>
        private float playbackSpeed = 1f;
        
        /// <summary>Whether to show the replay control UI elements.</summary>
        private bool showReplayControls = false;

        /// <summary>
        /// Creates and shows the Replay Tools window.
        /// </summary>
        [MenuItem("Tools/Replay Tools")]
        public static void ShowWindow()
        {
            GetWindow<ReplayToolsWindow>("Replay Tools");
        }

        /// <summary>
        /// Called when the window is enabled. Initializes the window by refreshing rigidbody lists,
        /// subscribing to editor events, and initializing the replay player.
        /// </summary>
        private void OnEnable()
        {
            RefreshRigidbodyLists();
            EditorApplication.hierarchyChanged += RefreshRigidbodyLists;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += OnEditorUpdate;
            
            // Initialize player
            player = ReplayPlayer.Instance;
        }

        /// <summary>
        /// Called when the window is disabled. Unsubscribes from editor events
        /// and stops any active playback.
        /// </summary>
        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= RefreshRigidbodyLists;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.update -= OnEditorUpdate;
            
            // Clean up player
            if (player != null)
            {
                player.StopPlayback();
            }
        }
        
        /// <summary>
        /// Called when the window is destroyed. Performs final cleanup
        /// of the replay player instance.
        /// </summary>
        private void OnDestroy()
        {
            if (player != null)
            {
                player.CleanUp();
            }
        }

        /// <summary>
        /// Handles state changes between edit mode and play mode.
        /// Initializes the recorder when entering play mode and cleans up when exiting.
        /// </summary>
        /// <param name="state">The current play mode state.</param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                recorder = ReplayRecorder.Instance;
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                recorder = null;
                // Don't nullify player as it should work in edit mode too
            }
            
            Repaint();
        }
        
        /// <summary>
        /// Called during regular editor updates. Used to repaint the window
        /// during replay playback to show updated playback progress.
        /// </summary>
        private void OnEditorUpdate()
        {
            // Repaint for replay playback updates
            if (player != null && player.IsPlaying)
            {
                Repaint();
            }
        }

        /// <summary>
        /// Refreshes the lists of player and non-player rigidbodies in the scene.
        /// Rigidbodies with a PlayerInput component are classified as player rigidbodies.
        /// </summary>
        private void RefreshRigidbodyLists()
        {
            playerRigidbodies.Clear();
            normalRigidbodies.Clear();
        
            Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        
            foreach (Rigidbody rb in allRigidbodies)
            {
                if (rb.GetComponent<PlayerInput>() != null)
                {
                    playerRigidbodies.Add(rb);
                }
                else
                {
                    normalRigidbodies.Add(rb);
                }
            }
        }

        /// <summary>
        /// Main GUI method that draws the entire window content.
        /// Organizes the window into distinct functional sections.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Replay Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Recording Section
            DrawRecordingSection();
            EditorGUILayout.Space();

            // Saved Sessions Section
            DrawSessionsSection();
            EditorGUILayout.Space();
            
            // Replay Section
            DrawReplaySection();
            EditorGUILayout.Space();

            // Refresh button
            if (GUILayout.Button("Refresh Rigidbody Lists"))
            {
                RefreshRigidbodyLists();
            }
            EditorGUILayout.Space();

            // Rigidbody Lists
            DrawRigidbodyLists();

            // Selected Rigidbody Info
            DrawSelectedRigidbodyInfo();
        }

        /// <summary>
        /// Draws the recording controls section of the window.
        /// Includes recording start/stop buttons, recording status, and save controls.
        /// </summary>
        private void DrawRecordingSection()
        {
            recordingFoldout = EditorGUILayout.Foldout(recordingFoldout, "Recording Controls", true, EditorStyles.foldoutHeader);
            if (!recordingFoldout) return;

            EditorGUI.indentLevel++;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Recording is only available during Play Mode", MessageType.Info);
                EditorGUI.indentLevel--;
                return;
            }

            if (recorder == null)
            {
                EditorGUILayout.HelpBox("Recorder not initialized", MessageType.Warning);
                EditorGUI.indentLevel--;
                return;
            }

            // Recording status
            if (recorder.IsRecording)
            {
                // Use a colored background for recording status
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                EditorGUILayout.BeginVertical("box");
                GUI.backgroundColor = originalColor;
                
                GUILayout.Label("● RECORDING", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Duration: {recorder.RecordingDuration:F2}s", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Objects being recorded: {recorder.RecordedObjectsCount}");
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Stop Recording", GUILayout.Height(30)))
                {
                    recorder.StopRecording();
                }
                if (GUILayout.Button("Clear Recording", GUILayout.Height(30)))
                {
                    recorder.ClearCurrentRecording();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // Show last recording duration if exists
                if (recorder.HasRecordingData())
                {
                    EditorGUILayout.LabelField($"Last recording duration: {recorder.RecordingDuration:F2}s", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("Ready to record", EditorStyles.miniLabel);
                }
                
                GUI.enabled = playerRigidbodies.Count > 0 || normalRigidbodies.Count > 0;
                if (GUILayout.Button("Start Recording", GUILayout.Height(30)))
                {
                    RefreshRigidbodyLists();
                    recorder.StartRecording(playerRigidbodies, normalRigidbodies);
                }
                GUI.enabled = true;
                
                if (playerRigidbodies.Count == 0 && normalRigidbodies.Count == 0)
                {
                    EditorGUILayout.HelpBox("No rigidbodies found in scene", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();

            // Save section
            EditorGUILayout.LabelField("Save Recording:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Session Name:", GUILayout.Width(100));
            sessionName = EditorGUILayout.TextField(sessionName);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = !recorder.IsRecording && recorder.HasRecordingData();
            if (GUILayout.Button("Save Session"))
            {
                var session = recorder.SaveCurrentRecording(sessionName);
                if (session != null)
                {
                    sessionName = "";
                    Debug.Log($"Session saved successfully: {session.name}");
                }
            }
            GUI.enabled = true;

            if (!recorder.HasRecordingData() && !recorder.IsRecording)
            {
                EditorGUILayout.HelpBox("No recording data to save. Record something first.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws the saved sessions section of the window.
        /// Displays a list of all saved replay sessions with options to select, delete, or clear all.
        /// </summary>
        private void DrawSessionsSection()
        {
            sessionsFoldout = EditorGUILayout.Foldout(sessionsFoldout, $"Saved Sessions ({ReplaySessionManager.Instance.SessionCount})", true, EditorStyles.foldoutHeader);
            if (!sessionsFoldout) return;

            EditorGUI.indentLevel++;

            var sessions = ReplaySessionManager.Instance.GetAllSessions();
            
            if (sessions.Count == 0)
            {
                EditorGUILayout.LabelField("No saved sessions", EditorStyles.miniLabel);
            }
            else
            {
                // Increased max height from 150 to 250 to show more entries
                sessionScrollPos = EditorGUILayout.BeginScrollView(sessionScrollPos, GUILayout.MaxHeight(250));
                
                for (int i = sessions.Count - 1; i >= 0; i--)
                {
                    var session = sessions[i];
                    bool isSelected = selectedSession == session;
                    
                    // Create a style for the selectable row that changes when selected
                    GUIStyle rowStyle = new GUIStyle("box");
                    if (isSelected)
                    {
                        rowStyle.normal.background = EditorGUIUtility.whiteTexture;
                        Color selectionColor = new Color(0.24f, 0.49f, 0.91f, 0.5f);
                        GUI.backgroundColor = selectionColor;
                    }
                    
                    EditorGUILayout.BeginHorizontal(rowStyle);
                    GUI.backgroundColor = Color.white;
                    
                    // Make a clickable area for the entire row content (except delete button)
                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    
                    // Detect if the current row is clicked
                    Rect rowRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));
                    rowRect.height = 50; // Make the clickable area taller
                    
                    // Display the session info
                    EditorGUILayout.LabelField(session.name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Duration: {session.duration:F2}s | Objects: {session.recordedObjects.Count} | Recorded: {session.recordedAt:HH:mm:ss}");
                    
                    // Show frame counts
                    int totalFrames = 0;
                    foreach (var obj in session.recordedObjects)
                    {
                        totalFrames += obj.recordedFrames.Count;
                    }
                    EditorGUILayout.LabelField($"Total frames: {totalFrames}", EditorStyles.miniLabel);
                    
                    EditorGUILayout.EndVertical();
                    
                    // Handle click event for the row
                    if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                    {
                        // If clicking the already selected session, deselect it and clear visualizations
                        if (isSelected)
                        {
                            selectedSession = null;
                            showReplayControls = false;
                            
                            if (player != null)
                            {
                                player.StopPlayback();
                                player.ClearVisualObjects();
                            }
                        }
                        else
                        {
                            // If selecting a different session
                            // First clear any existing visualizations from the previous session
                            if (player != null && selectedSession != null)
                            {
                                player.StopPlayback();
                                player.ClearVisualObjects();
                            }
                            
                            // Then select the new session
                            selectedSession = session;
                            showReplayControls = true;
                            
                            if (player != null)
                            {
                                player.LoadSession(session);
                            }
                        }
                        
                        Event.current.Use();
                        Repaint();
                    }
                    
                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Session", 
                            $"Are you sure you want to delete '{session.name}'?", 
                            "Delete", "Cancel"))
                        {
                            if (selectedSession == session)
                            {
                                selectedSession = null;
                                showReplayControls = false;
                                if (player != null)
                                {
                                    player.StopPlayback();
                                    player.ClearVisualObjects();
                                }
                            }
                            
                            ReplaySessionManager.Instance.RemoveSession(session);
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Clear All Sessions"))
                {
                    if (EditorUtility.DisplayDialog("Clear All Sessions", 
                        "Are you sure you want to delete all saved replay sessions?", 
                        "Yes", "Cancel"))
                    {
                        selectedSession = null;
                        showReplayControls = false;
                        if (player != null)
                        {
                            // Call CleanUp instead of just StopPlayback to properly remove all visual objects
                            player.CleanUp();
                            // Reinitialize the player since CleanUp sets instance to null
                            player = ReplayPlayer.Instance;
                        }
                        
                        ReplaySessionManager.Instance.ClearAllSessions();
                    }
                }
                
                // Add "Clear All Visualizations" button
                if (GUILayout.Button("Clear All Visualizations"))
                {
                    if (player != null)
                    {
                        player.ClearVisualObjects();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        
        /// <summary>
        /// Draws the replay controls section of the window.
        /// Provides playback controls for the selected replay session including
        /// play/pause, stop, and playback speed adjustment.
        /// </summary>
        private void DrawReplaySection()
        {
            replayFoldout = EditorGUILayout.Foldout(replayFoldout, "Replay Controls", true, EditorStyles.foldoutHeader);
            if (!replayFoldout) return;

            EditorGUI.indentLevel++;

            if (player == null)
            {
                EditorGUILayout.HelpBox("Replay player not initialized", MessageType.Warning);
                EditorGUI.indentLevel--;
                return;
            }
            
            if (selectedSession == null)
            {
                EditorGUILayout.HelpBox("Select a session to replay from the Saved Sessions section", MessageType.Info);
                EditorGUI.indentLevel--;
                return;
            }
            
            // Session info
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Selected Session: {selectedSession.name}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Duration: {selectedSession.duration:F2}s | Objects: {selectedSession.recordedObjects.Count}");
            EditorGUILayout.EndVertical();
            
            // Playback controls
            EditorGUILayout.Space();
            
            // Playback progress
            float progress = player.PlaybackProgress;
            EditorGUI.BeginChangeCheck();
            float newProgress = EditorGUILayout.Slider(progress, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                player.SetPlaybackProgress(newProgress);
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Time: {player.PlaybackTime:F2}s / {player.SessionDuration:F2}s");
            
            // Playback speed
            EditorGUILayout.LabelField("Speed:", GUILayout.Width(50));
            EditorGUI.BeginChangeCheck();
            playbackSpeed = EditorGUILayout.Slider(playbackSpeed, 0.1f, 3f, GUILayout.Width(120));
            if (EditorGUI.EndChangeCheck())
            {
                player.SetPlaybackSpeed(playbackSpeed);
            }
            EditorGUILayout.EndHorizontal();
            
            // Control buttons
            EditorGUILayout.BeginHorizontal();
            
            if (!player.IsPlaying)
            {
                if (GUILayout.Button("Play", GUILayout.Height(30)))
                {
                    if (Mathf.Approximately(player.PlaybackTime, player.SessionDuration))
                    {
                        player.StopPlayback();
                    }
                    player.StartPlayback();
                }
            }
            else
            {
                if (GUILayout.Button("Pause", GUILayout.Height(30)))
                {
                    player.PausePlayback();
                }
            }
            
            if (GUILayout.Button("Stop", GUILayout.Height(30)))
            {
                player.StopPlayback();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Add a Clear Visualization button
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws the lists of player and normal rigidbodies detected in the scene.
        /// Allows selection of individual rigidbodies for detailed information.
        /// </summary>
        private void DrawRigidbodyLists()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Player Rigidbodies Section
            playerFoldout = EditorGUILayout.Foldout(playerFoldout, $"Player Rigidbodies ({playerRigidbodies.Count})", true, EditorStyles.foldoutHeader);
            if (playerFoldout)
            {
                EditorGUI.indentLevel++;
                if (playerRigidbodies.Count == 0)
                {
                    EditorGUILayout.LabelField("No player rigidbodies found", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("Recording frequency: 50 FPS (High precision)", EditorStyles.miniLabel);
                    foreach (Rigidbody rb in playerRigidbodies)
                    {
                        if (rb != null)
                        {
                            DrawRigidbodyItem(rb, true);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();

            // Normal Rigidbodies Section
            normalFoldout = EditorGUILayout.Foldout(normalFoldout, $"Normal Rigidbodies ({normalRigidbodies.Count})", true, EditorStyles.foldoutHeader);
            if (normalFoldout)
            {
                EditorGUI.indentLevel++;
                if (normalRigidbodies.Count == 0)
                {
                    EditorGUILayout.LabelField("No normal rigidbodies found", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("Recording frequency: 10 FPS (Performance optimized)", EditorStyles.miniLabel);
                    foreach (Rigidbody rb in normalRigidbodies)
                    {
                        if (rb != null)
                        {
                            DrawRigidbodyItem(rb, false);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws detailed information about the currently selected rigidbody.
        /// Shows properties like name, type, mass, position, and velocity (in play mode).
        /// </summary>
        private void DrawSelectedRigidbodyInfo()
        {
            if (selectedRigidbody != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Selected Rigidbody:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Name: {selectedRigidbody.name}");
                EditorGUILayout.LabelField($"Type: {(selectedRigidbody.GetComponent<PlayerInput>() != null ? "Player" : "Normal")}");
                EditorGUILayout.LabelField($"Mass: {selectedRigidbody.mass}");
                EditorGUILayout.LabelField($"Position: {selectedRigidbody.transform.position}");
                
                if (Application.isPlaying)
                {
                    EditorGUILayout.LabelField($"Velocity: {selectedRigidbody.linearVelocity}");
                    EditorGUILayout.LabelField($"Angular Velocity: {selectedRigidbody.angularVelocity}");
                }
            }
        }

        /// <summary>
        /// Draws a single rigidbody item in the list with selection toggle and basic information.
        /// </summary>
        /// <param name="rb">The rigidbody to display</param>
        /// <param name="isPlayer">Whether this is a player-controlled rigidbody</param>
        private void DrawRigidbodyItem(Rigidbody rb, bool isPlayer)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Selection toggle
            bool isSelected = selectedRigidbody == rb;
            bool newSelection = GUILayout.Toggle(isSelected, GUIContent.none, GUILayout.Width(20));
            
            if (newSelection != isSelected)
            {
                selectedRigidbody = newSelection ? rb : null;
                if (newSelection)
                {
                    Selection.activeGameObject = rb.gameObject;
                    EditorGUIUtility.PingObject(rb.gameObject);
                }
            }
            
            // Rigidbody name button
            string displayName = rb.name;
            if (isPlayer)
            {
                displayName += " [PLAYER]";
            }
            
            if (GUILayout.Button(displayName, EditorStyles.label))
            {
                selectedRigidbody = rb;
                Selection.activeGameObject = rb.gameObject;
                EditorGUIUtility.PingObject(rb.gameObject);
                
                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView)
                {
                    sceneView.FrameSelected();
                    sceneView.size = Mathf.Max(sceneView.size, 5f);
                }
            }
            
            // Mass info
            EditorGUILayout.LabelField($"Mass: {rb.mass:F2}", GUILayout.Width(80));
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called every frame during editor updates. Repaints the window
        /// when necessary to reflect current recording or playback status.
        /// </summary>
        private void Update()
        {
            // Repaint the window during play mode to show live recording info
            if (Application.isPlaying && recorder != null && recorder.IsRecording)
            {
                Repaint();
            }
            
            // Also repaint when replaying in editor mode
            if (player != null && player.IsPlaying)
            {
                Repaint();
            }
        }
    }
}
