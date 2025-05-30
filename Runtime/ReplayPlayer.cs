using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JL_GameProdEnv_CustomPackage.Runtime
{
    /// <summary>
    /// Handles the playback of recorded replay sessions within the Unity Editor.
    /// Manages visualization objects, playback controls, and frame interpolation.
    /// </summary>
    public class ReplayPlayer
    {
        /// <summary>
        /// Color used for visualizing player-controlled objects during replay.
        /// </summary>
        private Color playerColor = new Color(0.2f, 0.6f, 1f, 0.8f);
        
        /// <summary>
        /// Color used for visualizing normal (non-player) objects during replay.
        /// </summary>
        private Color normalObjectColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        
        /// <summary>
        /// The currently loaded replay session data.
        /// </summary>
        private ReplaySession currentSession;
        
        /// <summary>
        /// List of visualization GameObjects representing the recorded objects during playback.
        /// </summary>
        private List<GameObject> visualObjects = new List<GameObject>();
        
        /// <summary>
        /// Flag indicating whether replay playback is currently active.
        /// </summary>
        private bool isPlaying = false;
        
        /// <summary>
        /// The editor time when playback was started, used for calculating playback time.
        /// </summary>
        private float playbackStartTime;
        
        /// <summary>
        /// Current playback time in seconds from the start of the replay.
        /// </summary>
        private float playbackTime = 0f;
        
        /// <summary>
        /// Playback speed multiplier (1.0 = normal speed).
        /// </summary>
        private float playbackSpeed = 1f;
        
        /// <summary>
        /// The time of the last update, used for calculating delta time in editor mode.
        /// </summary>
        private float lastUpdateTime;
        
        /// <summary>
        /// Singleton instance of the ReplayPlayer class.
        /// </summary>
        private static ReplayPlayer instance;
        
        /// <summary>
        /// Gets the singleton instance of the ReplayPlayer, creating it if it doesn't exist.
        /// </summary>
        public static ReplayPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReplayPlayer();
                }
                return instance;
            }
        }
        
        /// <summary>
        /// Gets whether replay playback is currently active.
        /// </summary>
        public bool IsPlaying => isPlaying;
        
        /// <summary>
        /// Gets the current playback progress as a normalized value between 0 and 1.
        /// </summary>
        public float PlaybackProgress => currentSession != null ? Mathf.Clamp01(playbackTime / currentSession.duration) : 0f;

        /// <summary>
        /// Gets the current playback time in seconds.
        /// </summary>
        public float PlaybackTime => playbackTime;
        
        /// <summary>
        /// Gets the total duration of the current session in seconds.
        /// </summary>
        public float SessionDuration => currentSession != null ? currentSession.duration : 0f;
        
        /// <summary>
        /// Gets the name of the currently loaded replay session.
        /// </summary>
        public string CurrentSessionName => currentSession != null ? currentSession.name : string.Empty;
        
        /// <summary>
        /// The parent GameObject that contains all visualization objects for the replay.
        /// </summary>
        private GameObject replayParentObject;

        /// <summary>
        /// Loads a replay session and creates the necessary visualization objects.
        /// </summary>
        /// <param name="session">The replay session to load and prepare for playback.</param>
        public void LoadSession(ReplaySession session)
        {
            StopPlayback();
            ClearVisualObjects();
    
            currentSession = session;
            CreateVisualObjects();
    
            Debug.Log($"Loaded replay session: {session.name} with {session.recordedObjects.Count} objects");
    
            // Immediately update visualization to show frame 0
            playbackTime = 0f;
            UpdateVisualization(0f);
    
            // Force repaint of scene view to show the visualization updates
            SceneView.RepaintAll();
        }
        
        /// <summary>
        /// Starts playback of the currently loaded replay session.
        /// </summary>
        /// <remarks>
        /// This method initializes playback from the beginning, subscribes to editor updates,
        /// and sets the visual objects to their initial positions.
        /// </remarks>
        public void StartPlayback()
        {
            if (currentSession == null || isPlaying) return;
            
            playbackStartTime = (float)EditorApplication.timeSinceStartup;
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            playbackTime = 0f;
            isPlaying = true;
            
            // Set initial positions
            UpdateVisualization(0f);
            
            // Start the editor update
            EditorApplication.update += EditorUpdate;
            
            Debug.Log($"Started playback of session: {currentSession.name}");
        }
        
        /// <summary>
        /// Pauses the current playback without resetting the playback time.
        /// </summary>
        public void PausePlayback()
        {
            isPlaying = false;
        }
        
        /// <summary>
        /// Resumes playback from the current position after being paused.
        /// </summary>
        public void ResumePlayback()
        {
            if (currentSession == null) return;
            
            playbackStartTime = (float)EditorApplication.timeSinceStartup - playbackTime / playbackSpeed;
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            isPlaying = true;
            
            // Make sure the update is subscribed
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }
        
        /// <summary>
        /// Stops playback and resets the playback time to the beginning.
        /// </summary>
        /// <remarks>
        /// This method unsubscribes from editor updates and resets the visualization
        /// to the initial frame.
        /// </remarks>
        public void StopPlayback()
        {
            isPlaying = false;
            playbackTime = 0f;
            
            // Unsubscribe from editor update
            EditorApplication.update -= EditorUpdate;
            
            if (currentSession != null)
            {
                UpdateVisualization(0f);
            }
        }
        
        /// <summary>
        /// Sets the playback progress to a specific normalized time.
        /// </summary>
        /// <param name="normalizedTime">Normalized time value between 0 and 1.</param>
        public void SetPlaybackProgress(float normalizedTime)
        {
            if (currentSession == null) return;
            
            playbackTime = normalizedTime * currentSession.duration;
            playbackStartTime = (float)EditorApplication.timeSinceStartup - playbackTime / playbackSpeed;
            UpdateVisualization(playbackTime);
        }
        
        /// <summary>
        /// Sets the playback speed multiplier.
        /// </summary>
        /// <param name="speed">The speed multiplier, clamped between 0.1 and 3.0.</param>
        public void SetPlaybackSpeed(float speed)
        {
            playbackSpeed = Mathf.Clamp(speed, 0.1f, 3f);
            
            if (isPlaying)
            {
                // Adjust the playback start time to maintain the current position with the new speed
                playbackStartTime = (float)EditorApplication.timeSinceStartup - (playbackTime / playbackSpeed);
            }
        }
        
        /// <summary>
        /// Editor update callback that advances playback time and updates visualizations.
        /// </summary>
        private void EditorUpdate()
        {
            if (!isPlaying || currentSession == null) return;
            
            float deltaTime = (float)EditorApplication.timeSinceStartup - lastUpdateTime;
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            
            // Calculate current playback time
            playbackTime += deltaTime * playbackSpeed;
            
            // Check if we've reached the end of the replay
            if (playbackTime >= currentSession.duration)
            {
                playbackTime = currentSession.duration;
                isPlaying = false;
                EditorApplication.update -= EditorUpdate;
            }
            
            UpdateVisualization(playbackTime);
            
            // Force repaint of scene view to show the visualization updates
            SceneView.RepaintAll();
        }
        
        /// <summary>
        /// Updates the position and rotation of all visual objects based on the current playback time.
        /// </summary>
        /// <param name="time">The current playback time in seconds.</param>
        /// <remarks>
        /// This method handles interpolation between recorded frames to provide smooth playback.
        /// </remarks>
        private void UpdateVisualization(float time)
        {
            for (int i = 0; i < currentSession.recordedObjects.Count; i++)
            {
                if (i >= visualObjects.Count) continue;
                
                var recordedObject = currentSession.recordedObjects[i];
                var visualObject = visualObjects[i];
                
                if (recordedObject.recordedFrames.Count == 0) continue;
                
                // Find the appropriate frames to interpolate between
                TransformData prevFrame = recordedObject.recordedFrames[0];
                TransformData nextFrame = recordedObject.recordedFrames[0];
                
                for (int j = 0; j < recordedObject.recordedFrames.Count - 1; j++)
                {
                    if (recordedObject.recordedFrames[j].timestamp <= time && 
                        recordedObject.recordedFrames[j + 1].timestamp >= time)
                    {
                        prevFrame = recordedObject.recordedFrames[j];
                        nextFrame = recordedObject.recordedFrames[j + 1];
                        break;
                    }
                }
                
                // If we're beyond the last frame, use the last one
                if (time > recordedObject.recordedFrames[recordedObject.recordedFrames.Count - 1].timestamp)
                {
                    prevFrame = nextFrame = recordedObject.recordedFrames[recordedObject.recordedFrames.Count - 1];
                }
                
                // Interpolate between frames
                float t = Mathf.InverseLerp(prevFrame.timestamp, nextFrame.timestamp, time);
                
                // Apply interpolated transform
                visualObject.transform.position = Vector3.Lerp(prevFrame.position, nextFrame.position, t);
                visualObject.transform.rotation = Quaternion.Slerp(prevFrame.rotation, nextFrame.rotation, t);
            }
        }
        
        /// <summary>
        /// Creates visual representation objects for all recorded objects in the current session.
        /// </summary>
        /// <remarks>
        /// This method attempts to clone original objects when available, or creates primitive
        /// representations as a fallback. It also handles material setup for visualization.
        /// </remarks>
        private void CreateVisualObjects()
        {
            if (currentSession == null) return;
            
            // Create a parent object for all replay visualizations
            replayParentObject = new GameObject("Replay_Visualization");
            replayParentObject.hideFlags = HideFlags.DontSave;
            
            foreach (var recordedObject in currentSession.recordedObjects)
            {
                GameObject visualObject = null;
                
                // Try to find the original object in the scene to use its shape
                GameObject originalObject = GameObject.Find(recordedObject.objectName);
                
                if (originalObject != null)
                {
                    // Create a clone of the original object
                    visualObject = GameObject.Instantiate(originalObject);
                    visualObject.name = $"Replay_{recordedObject.objectName}";
                    
                    // Reset the transform before applying recorded data
                    // This is crucial for objects with parent-based scale modifications
                    visualObject.transform.SetParent(replayParentObject.transform, false);
                    
                    // Reset local scale to remove parent scale influence
                    visualObject.transform.localScale = Vector3.one;
                    
                    // Calculate the global scale from the original object by traversing its hierarchy
                    Vector3 globalScale = originalObject.transform.lossyScale;
                    
                    // Apply the global scale to our independent replay object
                    visualObject.transform.localScale = globalScale;
                    
                    // Disable any scripts or components that might interfere
                    MonoBehaviour[] scripts = visualObject.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour script in scripts)
                    {
                        script.enabled = false;
                    }
                    
                    // Remove rigidbody to prevent physics interactions
                    Rigidbody rb = visualObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Object.DestroyImmediate(rb);
                    }
                    
                    // Remove colliders to prevent physics interactions
                    Collider[] colliders = visualObject.GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        Object.DestroyImmediate(collider);
                    }
                    
                    // Apply semi-transparent material to all renderers
                    Renderer[] renderers = visualObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        // Create a new material instance based on the original
                        Material originalMat = renderer.sharedMaterial;
                        if (originalMat != null)
                        {
                            Material newMat = new Material(originalMat);
                            
                            // Set appropriate color with transparency
                            Color color = recordedObject.isPlayer ? playerColor : normalObjectColor;
                            newMat.color = color;
                            
                            // Apply the material
                            renderer.sharedMaterial = newMat;
                        }
                    }
                }
                else
                {
                    // Fallback to primitives if original object cannot be found
                    if (recordedObject.isPlayer)
                    {
                        // Create a capsule for players
                        visualObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        visualObject.name = $"Replay_{recordedObject.objectName}";
                        visualObject.transform.localScale = new Vector3(1f, 1f, 1f);
                        visualObject.transform.SetParent(replayParentObject.transform, false);
                        
                        // Set the color in edit mode
                        var renderer = visualObject.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                            renderer.sharedMaterial.color = playerColor;
                        }
                    }
                    else
                    {
                        // Create a box for normal objects
                        visualObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        visualObject.name = $"Replay_{recordedObject.objectName}";
                        visualObject.transform.localScale = new Vector3(1f, 1f, 1f);
                        visualObject.transform.SetParent(replayParentObject.transform, false);
                        
                        // Set the color in edit mode
                        var renderer = visualObject.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                            renderer.sharedMaterial.color = normalObjectColor;
                        }
                    }
                    
                    // Remove colliders to prevent physics interactions
                    Collider collider = visualObject.GetComponent<Collider>();
                    if (collider != null)
                    {
                        Object.DestroyImmediate(collider);
                    }
                }
                
                // Add to visualization group
                visualObject.hideFlags = HideFlags.DontSave;
                visualObjects.Add(visualObject);
            }
        }
        
        /// <summary>
        /// Removes all visual objects created for replay visualization.
        /// </summary>
        public void ClearVisualObjects()
        {
            foreach (var obj in visualObjects)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            
            visualObjects.Clear();
            
            // Also destroy the parent object
            if (replayParentObject != null)
            {
                Object.DestroyImmediate(replayParentObject);
                replayParentObject = null;
            }
        }
        
        /// <summary>
        /// Performs complete cleanup of the replay player, stopping playback,
        /// removing visual objects, and resetting the singleton instance.
        /// </summary>
        public void CleanUp()
        {
            StopPlayback();
            ClearVisualObjects();
            instance = null;
        }
    }
}
