using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace JL_GameProdEnv_CustomPackage.Runtime
{
    [Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public float timestamp;

        public TransformData(Transform transform, Rigidbody rigidbody, float time)
        {
            position = transform.position;
            rotation = transform.rotation;
            velocity = rigidbody != null ? rigidbody.linearVelocity : Vector3.zero;
            angularVelocity = rigidbody != null ? rigidbody.angularVelocity : Vector3.zero;
            timestamp = time;
        }
    }

    [Serializable]
    public class RigidbodyRecordingData
    {
        public string objectName;
        public bool isPlayer;
        public List<TransformData> recordedFrames;
        public Rigidbody rigidbody; // Runtime reference

        public RigidbodyRecordingData(Rigidbody rb, bool playerType)
        {
            objectName = rb.name;
            isPlayer = playerType;
            rigidbody = rb;
            recordedFrames = new List<TransformData>();
        }

        public void AddFrame(float currentTime)
        {
            if (rigidbody != null)
            {
                recordedFrames.Add(new TransformData(rigidbody.transform, rigidbody, currentTime));
            }
        }

        public void ClearFrames()
        {
            recordedFrames.Clear();
        }
    }

    [Serializable]
    public class ReplaySession
    {
        public string name;
        public float duration;
        public DateTime recordedAt;
        public List<RigidbodyRecordingData> recordedObjects;

        public ReplaySession(string sessionName)
        {
            name = string.IsNullOrEmpty(sessionName) ? $"Replay_{DateTime.Now:HH_mm_ss}" : sessionName;
            recordedAt = DateTime.Now;
            duration = 0f;
            recordedObjects = new List<RigidbodyRecordingData>();
        }
    }

    public class ReplayRecorder : MonoBehaviour
    {
        [Header("Recording Settings")] [SerializeField]
        private float playerRecordingInterval = 0.02f; // 50 FPS for players

        [SerializeField] private float normalRecordingInterval = 0.1f; // 10 FPS for normal objects

        private List<RigidbodyRecordingData> currentRecordingData = new List<RigidbodyRecordingData>();
        private bool isRecording = false;
        private float recordingStartTime;
        private float recordingEndTime;
        private float lastPlayerRecordTime;
        private float lastNormalRecordTime;

        public bool IsRecording => isRecording;

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

        public int RecordedObjectsCount => currentRecordingData.Count;

        private static ReplayRecorder instance;

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

        public void StopRecording()
        {
            if (!isRecording) return;

            recordingEndTime = Time.time;
            isRecording = false;

            float finalDuration = recordingEndTime - recordingStartTime;
            Debug.Log($"Stopped recording. Duration: {finalDuration:F2}s");
        }

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

        public List<RigidbodyRecordingData> GetCurrentRecordingData()
        {
            return new List<RigidbodyRecordingData>(currentRecordingData);
        }

        public bool HasRecordingData()
        {
            return currentRecordingData.Count > 0 && currentRecordingData.Exists(data => data.recordedFrames.Count > 0);
        }
    }

    public class ReplaySessionManager
    {
        private static ReplaySessionManager instance;

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

        private List<ReplaySession> sessions = new List<ReplaySession>();
        private const string SAVE_FOLDER = "Assets/ReplaySessions";
        private const string FILE_EXTENSION = ".json";

        public List<ReplaySession> GetAllSessions() => new List<ReplaySession>(sessions);

        public void AddSession(ReplaySession session)
        {
            sessions.Add(session);
            SaveSessionToDisk(session);
        }

        public void RemoveSession(ReplaySession session)
        {
            sessions.Remove(session);
            DeleteSessionFile(session);
        }

        public void ClearAllSessions()
        {
            foreach (var session in sessions)
            {
                DeleteSessionFile(session);
            }

            sessions.Clear();
        }

        public ReplaySession GetSession(string name)
        {
            return sessions.Find(s => s.name == name);
        }

        public int SessionCount => sessions.Count;

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