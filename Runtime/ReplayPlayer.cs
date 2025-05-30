using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JL_GameProdEnv_CustomPackage.Runtime
{
    public class ReplayPlayer
    {
        // Visualization settings
        private Color playerColor = new Color(0.2f, 0.6f, 1f, 0.8f);
        private Color normalObjectColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        
        private ReplaySession currentSession;
        private List<GameObject> visualObjects = new List<GameObject>();
        private bool isPlaying = false;
        private float playbackStartTime;
        private float playbackTime = 0f;
        private float playbackSpeed = 1f;
        private float lastUpdateTime;
        
        private static ReplayPlayer instance;
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
        
        public bool IsPlaying => isPlaying;
        public float PlaybackProgress => currentSession != null ? Mathf.Clamp01(playbackTime / currentSession.duration) : 0f;
        public float PlaybackTime => playbackTime;
        public float SessionDuration => currentSession != null ? currentSession.duration : 0f;
        public string CurrentSessionName => currentSession != null ? currentSession.name : string.Empty;
        
        public void LoadSession(ReplaySession session)
        {
            StopPlayback();
            ClearVisualObjects();
            
            currentSession = session;
            CreateVisualObjects();
            
            Debug.Log($"Loaded replay session: {session.name} with {session.recordedObjects.Count} objects");
        }
        
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
        
        public void PausePlayback()
        {
            isPlaying = false;
        }
        
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
        
        public void SetPlaybackProgress(float normalizedTime)
        {
            if (currentSession == null) return;
            
            playbackTime = normalizedTime * currentSession.duration;
            playbackStartTime = (float)EditorApplication.timeSinceStartup - playbackTime / playbackSpeed;
            UpdateVisualization(playbackTime);
        }
        
        public void SetPlaybackSpeed(float speed)
        {
            playbackSpeed = Mathf.Clamp(speed, 0.1f, 3f);
            
            if (isPlaying)
            {
                // Adjust the playback start time to maintain the current position with the new speed
                playbackStartTime = (float)EditorApplication.timeSinceStartup - (playbackTime / playbackSpeed);
            }
        }
        
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
        
        private void CreateVisualObjects()
        {
            if (currentSession == null) return;
            
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
                    visualObject.transform.SetParent(null);
                    
                    // Reset local scale to remove parent scale influence
                    // We'll calculate the correct global scale by traversing the original hierarchy
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
        
        private void ClearVisualObjects()
        {
            foreach (var obj in visualObjects)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            
            visualObjects.Clear();
        }
        
        public void CleanUp()
        {
            StopPlayback();
            ClearVisualObjects();
            instance = null;
        }
    }
}