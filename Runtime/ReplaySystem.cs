using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace JL_GameProdEnv_CustomPackage.Runtime
{
    /// <summary>
    /// Represents the transformation data for an object, including position, rotation, velocity, angular velocity, and a timestamp.
    /// </summary>
    [Serializable]
    public class TransformData
    {
        /// <summary>
        /// Represents the position of an object in 3D space.
        /// </summary>
        /// <remarks>
        /// This variable is part of the TransformData class and stores the positional coordinates
        /// of a transform at a specific point in time within the replay system.
        /// </remarks>
        public Vector3 position;

        /// Represents the rotation of a transform at a specific point in time during recording.
        /// This property stores the orientation of the object as a quaternion.
        /// Used in conjunction with position, velocity, and angularVelocity to record and
        /// replay the state of a Rigidbody.
        public Quaternion rotation;

        /// Represents the linear velocity of a rigidbody at a specific point in time.
        /// This property captures and stores the current linear velocity of the rigidbody
        /// as part of the `TransformData`. Used to record and analyze movement over time,
        /// typically during a replay recording or playback system.
        public Vector3 velocity;

        /// Represents the angular velocity of an object in 3D space.
        /// This is typically used to record and store the rotational speed of a rigidbody
        /// during gameplay and replay systems. It defines the rate of rotation around each axis (X, Y, Z)
        /// in radians per second.
        public Vector3 angularVelocity;

        /// <summary>
        /// Represents the time at which a specific frame of transform data was recorded during a replay session.
        /// This variable is used to timestamp each frame, allowing for accurate playback of recorded animations or events.
        /// </summary>
        public float timestamp;

        /// Represents the state and motion of a Transform and Rigidbody at a specific point in time.
        /// This class encapsulates the position, rotation, velocity, angular velocity, and timestamp of a Transform and Rigidbody.
        /// It is primarily used for recording gameplay state during a replay session.
        public TransformData(Transform transform, Rigidbody rigidbody, float time)
        {
            position = transform.position;
            rotation = transform.rotation;
            velocity = rigidbody != null ? rigidbody.linearVelocity : Vector3.zero;
            angularVelocity = rigidbody != null ? rigidbody.angularVelocity : Vector3.zero;
            timestamp = time;
        }
    }

    /// <summary>
    /// Represents a recording of transform data for a Rigidbody component.
    /// This is used to capture and store the movement data of a Rigidbody over time for replay purposes.
    /// </summary>
    [Serializable]
    public class RigidbodyRecordingData
    {
        /// Represents a unique identifier or name associated with an object in a replay system during its recording and playback.
        public string objectName;

        /// A flag indicating whether the object associated with the recording data represents a player.
        public bool isPlayer;

        /// Stores the recorded frames of transform data for a specific Rigidbody.
        /// This list holds the sequential snapshots of the Rigidbody's transformation,
        /// including position, rotation, velocity, and angular velocity, at different timestamps.
        public List<TransformData> recordedFrames;

        /// <summary>
        /// Represents the runtime reference to the Rigidbody component
        /// of a game object. Used mainly for tracking and recording
        /// physical properties such as position, rotation, velocity,
        /// and angular velocity during runtime.
        /// </summary>
        public Rigidbody rigidbody; // Runtime reference

        /// Represents recording data for a Rigidbody, including its state over time during a replay session.
        public RigidbodyRecordingData(Rigidbody rb, bool playerType)
        {
            objectName = rb.name;
            isPlayer = playerType;
            rigidbody = rb;
            recordedFrames = new List<TransformData>();
        }

        /// <summary>
        /// Adds a frame containing the current transform and physics data to the recording.
        /// </summary>
        /// <param name="currentTime">The current time at which the frame is being recorded.</param>
        /// <returns>None. Adds a frame to the recordedFrames list if the Rigidbody is not null.</returns>
        public void AddFrame(float currentTime)
        {
            if (rigidbody != null)
            {
                recordedFrames.Add(new TransformData(rigidbody.transform, rigidbody, currentTime));
            }
        }

        /// <summary>
        /// Clears all recorded frames from the current object.
        /// </summary>
        /// <returns>None.</returns>
        public void ClearFrames()
        {
            recordedFrames.Clear();
        }
    }

    /// <summary>
    /// Represents a replay session containing recorded data for various rigidbody objects during gameplay.
    /// </summary>
    /// <remarks>
    /// Each replay session records the name, duration, timestamp of recording, and a list of rigidbody recordings.
    /// This data is typically used to replay object movements and interactions that occurred during gameplay.
    /// </remarks>
    [Serializable]
    public class ReplaySession
    {
        /// <summary>
        /// The name of the replay session.
        /// Used to uniquely identify the session and can be provided by the user or automatically generated.
        /// </summary>
        public string name;

        /// <summary>
        /// Represents the total duration of the replay session in seconds.
        /// It is calculated as the difference between the session's start and end time,
        /// corresponding to the total time span of the recorded data.
        /// </summary>
        public float duration;

        /// <summary>
        /// Represents the date and time at which the replay session was recorded.
        /// </summary>
        /// <remarks>
        /// This variable stores the exact timestamp when the replay session begins or is initialized.
        /// It uses the <see cref="DateTime"/> structure to encapsulate both date and time information.
        /// </remarks>
        public DateTime recordedAt;

        /// <summary>
        /// A collection that holds the recorded data of rigidbody objects tracked during a replay session.
        /// </summary>
        /// <remarks>
        /// Each entry in the collection contains rigidbody recording data, which includes object name, type (player or normal),
        /// and a list of recorded frames capturing the object's transform and physics data over time.
        /// </remarks>
        public List<RigidbodyRecordingData> recordedObjects;

        /// <summary>
        /// Represents a replay session that contains recorded data for multiple objects over a set duration.
        /// </summary>
        /// <remarks>
        /// A replay session stores the state and movement data (position, rotation, velocity, and angular velocity)
        /// for rigidbodies captured during a recording. It includes metadata such as session name, recording
        /// duration, and the time at which the session was recorded.
        /// </remarks>
        public ReplaySession(string sessionName)
        {
            name = string.IsNullOrEmpty(sessionName) ? $"Replay_{DateTime.Now:HH_mm_ss}" : sessionName;
            recordedAt = DateTime.Now;
            duration = 0f;
            recordedObjects = new List<RigidbodyRecordingData>();
        }
    }

    /// <summary>
    /// Handles the recording of Rigidbody and Transform data for replay functionality.
    /// Manages the initiation, termination, and data management of a recording session.
    /// </summary>
    public class ReplayRecorder : MonoBehaviour
    {
        /// <summary>
        /// The interval, in seconds, between recording frames for player objects during a replay recording session.
        /// Determines the frequency at which player transform data is captured.
        /// </summary>
        [Header("Recording Settings")] [SerializeField]
        private float playerRecordingInterval = 0.02f; // 50 FPS for players

        /// <summary>
        /// Defines the interval, in seconds, at which normal objects are recorded during the replay session.
        /// This variable determines the frequency of data collection, where a smaller value increases
        /// the recording frequency (capturing more frames) and a larger value reduces it, optimizing performance.
        /// Default value is set to 0.1 seconds, equivalent to 10 FPS for normal objects, aiming for a balance between
        /// recording detail and computational efficiency.
        /// </summary>
        [SerializeField] private float normalRecordingInterval = 0.1f; // 10 FPS for normal objects

        /// <summary>
        /// Represents a collection of recording data for all rigidbodies currently being tracked during a replay session.
        /// </summary>
        /// <remarks>
        /// This list stores instances of <see cref="RigidbodyRecordingData"/>, which contains data about the position,
        /// rotation, and other relevant physics states of rigidbodies over a period of time.
        /// It is used during recording to collect data, during saving to write the data to a replay session,
        /// and during playback to replay the recorded object's state changes.
        /// </remarks>
        private List<RigidbodyRecordingData> currentRecordingData = new List<RigidbodyRecordingData>();

        /// <summary>
        /// Indicates whether the replay recording process is currently active.
        /// </summary>
        /// <remarks>
        /// This variable is set to true when recording starts and is set to false when the recording stops.
        /// </remarks>
        private bool isRecording = false;

        /// <summary>
        /// Holds the start time of the recording process, represented in seconds since the start of the application.
        /// This value is used to calculate the duration of a recording session by subtracting it from the current time
        /// when the recording is in progress, or from the end time once the recording is stopped.
        /// </summary>
        private float recordingStartTime;

        /// <summary>
        /// The timestamp indicating when the recording session ended, expressed in seconds since the start of the application run.
        /// This variable is updated when the recording process is stopped and serves as the end point for calculating the total recording duration.
        /// </summary>
        private float recordingEndTime;

        /// <summary>
        /// Represents the time in seconds of the last recorded frame for the player's Rigidbody during an active recording session.
        /// </summary>
        /// <remarks>
        /// This value is reset to 0 when a recording session starts and is updated during each recording cycle.
        /// It is used to track the time elapsed since the last player's Rigidbody frame was recorded.
        /// </remarks>
        private float lastPlayerRecordTime;

        /// <summary>
        /// Stores the last recorded timestamp for normal objects during the recording process.
        /// </summary>
        /// <remarks>
        /// This variable is used internally by the ReplayRecorder class to track the time of the most recent
        /// recording update for normal (non-player) objects. It is reset to 0 when recording starts, and updated
        /// dynamically during the recording session to ensure accurate timing data is captured.
        /// </remarks>
        private float lastNormalRecordTime;

        /// <summary>
        /// Indicates whether the recorder is currently in the process of recording.
        /// </summary>
        public bool IsRecording => isRecording;

        /// <summary>
        /// Gets the duration of the current recording session in seconds.
        /// </summary>
        /// <remarks>
        /// The duration is calculated as the difference between the current time and the start time
        /// if a recording is in progress. If the recording has stopped, it returns the duration
        /// between the start and end times of the last recorded session.
        /// </remarks>
        /// <value>
        /// A floating-point value representing the duration of the recording in seconds.
        /// </value>
        public float RecordingDuration
        {
            get
            {
                if (isRecording)
                    return Time.time - recordingStartTime;
                else
                    return recordingEndTime - recordingStartTime;
            }
        }

        /// <summary>
        /// Gets the number of objects currently being recorded.
        /// </summary>
        /// <remarks>
        /// This property reflects the total count of objects that are actively included in the current recording session.
        /// </remarks>
        public int RecordedObjectsCount => currentRecordingData.Count;

        /// <summary>
        /// Singleton instance of the ReplayRecorder class, providing global access to the replay recording functionality.
        /// </summary>
        private static ReplayRecorder instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="ReplayRecorder"/> class.
        /// If no instance exists, a new object will be instantiated and persisted across scenes.
        /// </summary>
        /// <remarks>
        /// The Instance property ensures a single global instance of the ReplayRecorder class,
        /// utilizing lazy initialization. If accessed for the first time and no instance exists,
        /// it creates a new GameObject named "ReplayRecorder" and attaches the ReplayRecorder component to it.
        /// </remarks>
        public static ReplayRecorder Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("ReplayRecorder");
                    instance = go.AddComponent<ReplayRecorder>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        /// Initializes the ReplayRecorder instance and ensures only one instance exists at a time.
        /// If no instance exists, it assigns the current object as the instance and marks it to not be destroyed on scene load.
        /// If an instance already exists that is not the current object, the current object will be destroyed.
        /// <return>Void.</return>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Starts the recording process for specified rigidbodies.
        /// </summary>
        /// <param name="playerRigidbodies">A list of Rigidbody objects considered as player-controlled objects.</param>
        /// <param name="normalRigidbodies">A list of Rigidbody objects considered as non-player-controlled objects.</param>
        public void StartRecording(List<Rigidbody> playerRigidbodies, List<Rigidbody> normalRigidbodies)
        {
            if (isRecording) return;

            currentRecordingData.Clear();
            recordingStartTime = Time.time;
            recordingEndTime = recordingStartTime; // Initialize end time
            lastPlayerRecordTime = 0f;
            lastNormalRecordTime = 0f;

            // Setup recording data for all rigidbodies
            foreach (var rb in playerRigidbodies)
            {
                if (rb != null)
                {
                    currentRecordingData.Add(new RigidbodyRecordingData(rb, true));
                }
            }

            foreach (var rb in normalRigidbodies)
            {
                if (rb != null)
                {
                    currentRecordingData.Add(new RigidbodyRecordingData(rb, false));
                }
            }

            isRecording = true;
            Debug.Log($"Started recording {currentRecordingData.Count} objects at time {recordingStartTime}");
        }

        /// Stops the current recording process if it is ongoing.
        /// Resets the recording status and records the final duration
        /// of the recording for reference. No effect if recording is not active.
        /// <returns>Void. There is no return value for this method.</returns>
        public void StopRecording()
        {
            if (!isRecording) return;

            recordingEndTime = Time.time;
            isRecording = false;

            float finalDuration = recordingEndTime - recordingStartTime;
            Debug.Log($"Stopped recording. Duration: {finalDuration:F2}s");
        }

        /// Saves the current recording session as a replay session.
        /// <param name="sessionName">The name to assign to the replay session. If not provided, the session will remain unnamed.</param>
        /// <returns>A ReplaySession object containing the saved recording data. Returns null if no recording data exists.</returns>
        public ReplaySession SaveCurrentRecording(string sessionName = "")
        {
            if (currentRecordingData.Count == 0)
            {
                Debug.LogWarning("No recording data to save");

                return null;
            }

            var session = new ReplaySession(sessionName);
            session.duration = RecordingDuration; // This will now use the correct duration

            // Deep copy the recording data
            foreach (var recordingData in currentRecordingData)
            {
                var sessionData = new RigidbodyRecordingData(recordingData.rigidbody, recordingData.isPlayer);
                sessionData.objectName = recordingData.objectName;
                sessionData.recordedFrames = new List<TransformData>(recordingData.recordedFrames);
                session.recordedObjects.Add(sessionData);
            }

            // Add to session manager
            ReplaySessionManager.Instance.AddSession(session);

            Debug.Log($"Saved replay session: {session.name} with duration {session.duration:F2}s and {session.recordedObjects.Count} objects");

            return session;
        }

        /// Clears all current recording data and resets the recording state.
        /// This method stops any ongoing recording, clears the stored recorded data,
        /// and resets the internal recording start and end time to their initial values.
        /// <returns>Void. No value is returned.</returns>
        public void ClearCurrentRecording()
        {
            isRecording = false;

            foreach (var data in currentRecordingData)
            {
                data.ClearFrames();
            }

            currentRecordingData.Clear();
            recordingStartTime = 0f;
            recordingEndTime = 0f;
        }

        /// Updates the recording data for the current frame during an active recording session.
        /// This method captures and stores frame data for player objects and normal objects based
        /// on their respective recording intervals. It ensures that data is only recorded during an
        /// active recording session and while the application is running.
        /// If the recording session is inactive, or the application is not in play mode, this method
        /// exits without performing any operations.
        /// <returns>
        /// No value is returned, as this method performs its operations directly on the recording
        /// data maintained by the ReplayRecorder instance.
        /// </returns>
        private void Update()
        {
            if (!isRecording || !Application.isPlaying) return;

            float currentTime = Time.time - recordingStartTime;

            // Record player objects at higher frequency
            if (currentTime - lastPlayerRecordTime >= playerRecordingInterval)
            {
                foreach (var data in currentRecordingData)
                {
                    if (data.isPlayer && data.rigidbody != null)
                    {
                        data.AddFrame(currentTime);
                    }
                }

                lastPlayerRecordTime = currentTime;
            }

            // Record normal objects at lower frequency
            if (currentTime - lastNormalRecordTime >= normalRecordingInterval)
            {
                foreach (var data in currentRecordingData)
                {
                    if (!data.isPlayer && data.rigidbody != null)
                    {
                        data.AddFrame(currentTime);
                    }
                }

                lastNormalRecordTime = currentTime;
            }
        }

        /// <summary>
        /// Retrieves the current recording data as a list of rigidbody recording entries.
        /// </summary>
        /// <returns>
        /// A list of <see cref="RigidbodyRecordingData"/> containing the recording data collected during the current recording session.
        /// Returns an empty list if no recording data is available.
        /// </returns>
        public List<RigidbodyRecordingData> GetCurrentRecordingData()
        {
            return new List<RigidbodyRecordingData>(currentRecordingData);
        }

        /// Checks if the current recording contains any valid recorded data.
        /// <returns>
        /// true if there is at least one object with recorded frames; otherwise, false.
        /// </returns>
        public bool HasRecordingData()
        {
            return currentRecordingData.Count > 0 && currentRecordingData.Exists(data => data.recordedFrames.Count > 0);
        }
    }

    /// <summary>
    /// Manages the orchestration and persistence of replay sessions, allowing for addition,
    /// retrieval, removal, and cleanup of recorded gameplay sessions.
    /// </summary>
    public class ReplaySessionManager
    {
        /// <summary>
        /// Represents an instance of an object or a class.
        /// </summary>
        private static ReplaySessionManager instance;

        /// <summary>
        /// Gets the singleton instance of the associated class.
        /// This property is used to ensure that only one instance of the class exists
        /// and provides a global point of access to it.
        /// </summary>
        public static ReplaySessionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReplaySessionManager();
                    instance.LoadSessionsFromDisk();
                }

                return instance;
            }
        }

        /// <summary>
        /// A private list storing all ReplaySession objects managed by ReplaySessionManager.
        /// Represents the internal collection of recorded replay sessions.
        /// </summary>
        private List<ReplaySession> sessions = new List<ReplaySession>();

        /// <summary>
        /// The directory path where replay session data is saved to and loaded from.
        /// </summary>
        private const string SAVE_FOLDER = "Assets/ReplaySessions";

        /// <summary>
        /// The file extension used for saving and loading replay session data files.
        /// This constant is used to specify the format of files
        /// handled by the ReplaySessionManager when performing operations
        /// such as saving or loading replay sessions to and from disk.
        /// </summary>
        private const string FILE_EXTENSION = ".json";

        /// Retrieves all replay sessions currently available in the system.
        /// <returns>
        /// A list of ReplaySession objects representing all recorded sessions.
        /// </returns>
        public List<ReplaySession> GetAllSessions() => new List<ReplaySession>(sessions);

        /// <summary>
        /// Adds a new replay session to the session manager and saves it to disk.
        /// </summary>
        /// <param name="session">The replay session to be added.</param>
        public void AddSession(ReplaySession session)
        {
            sessions.Add(session);
            SaveSessionToDisk(session);
        }

        /// <summary>
        /// Removes the specified replay session from the session list and deletes its associated file from storage.
        /// </summary>
        /// <param name="session">The <see cref="ReplaySession"/> instance to be removed.</param>
        public void RemoveSession(ReplaySession session)
        {
            sessions.Remove(session);
            DeleteSessionFile(session);
        }

        /// Clears all recorded replay sessions from memory and deletes their corresponding files from disk.
        /// This method iterates through all stored replay sessions, removes their associated files from disk,
        /// and clears the session list in memory to ensure no session data remains.
        /// <returns>Void. No return value.</returns>
        public void ClearAllSessions()
        {
            foreach (var session in sessions)
            {
                DeleteSessionFile(session);
            }

            sessions.Clear();
        }

        /// Retrieves a replay session by its name.
        /// <param name="name">The name of the replay session to retrieve.</param>
        /// <returns>The <see cref="ReplaySession"/> that matches the given name, or null if no match is found.</returns>
        public ReplaySession GetSession(string name)
        {
            return sessions.Find(s => s.name == name);
        }

        /// <summary>
        /// Gets the total number of replay sessions currently managed by the system.
        /// </summary>
        public int SessionCount => sessions.Count;

        /// Saves the specified replay session to disk as a JSON file.
        /// <param name="session">The replay session to save to disk.</param>
        /// <returns>Void. The replay session is saved to the specified directory.</returns>
        private void SaveSessionToDisk(ReplaySession session)
        {
            // Ensure directory exists
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }

            string json = JsonUtility.ToJson(session, true);
            string filePath = Path.Combine(SAVE_FOLDER, GetSafeFileName(session.name) + FILE_EXTENSION);
            File.WriteAllText(filePath, json);

            // Make Unity aware of the new file
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        /// Loads replay sessions from disk into the manager's session list.
        /// This method reads serialized session files from the designated save folder
        /// and deserializes them into ReplaySession objects. The method clears any
        /// previously loaded sessions and handles file read errors gracefully.
        /// <returns>Void. This method does not return a value.</returns>
        private void LoadSessionsFromDisk()
        {
            sessions.Clear();

            if (!Directory.Exists(SAVE_FOLDER))
            {
                return;
            }

            string[] files = Directory.GetFiles(SAVE_FOLDER, "*" + FILE_EXTENSION);

            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    ReplaySession session = JsonUtility.FromJson<ReplaySession>(json);

                    if (session != null)
                    {
                        sessions.Add(session);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading replay session from {file}: {e.Message}");
                }
            }
        }

        /// Deletes the file associated with the specified replay session from disk.
        /// <param name="session">The replay session whose associated file is to be deleted.</param>
        /// <returns>Returns void, does not provide any value or response.</returns>
        private void DeleteSessionFile(ReplaySession session)
        {
            string filePath = Path.Combine(SAVE_FOLDER, GetSafeFileName(session.name) + FILE_EXTENSION);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                
                // Also delete the meta file
                string metaFilePath = filePath + ".meta";
                if (File.Exists(metaFilePath))
                {
                    File.Delete(metaFilePath);
                }

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
        }

        /// Generates a safe file name by replacing invalid characters in the provided file name with underscores.
        /// <param name="fileName">The original file name that may contain invalid characters.</param>
        /// <returns>A modified file name that is safe for file system operations by replacing invalid characters with underscores.</returns>
        private string GetSafeFileName(string fileName)
        {
            // Remove invalid characters from filename
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }
    }
}