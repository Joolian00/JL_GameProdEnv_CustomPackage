# Summary

| Members                                                                                                            | Descriptions |
|--------------------------------------------------------------------------------------------------------------------|--------------|
| `namespace `[`JL_GameProdEnv_CustomPackage::Editor`](#namespace_j_l___game_prod_env___custom_package_1_1_editor)   |              |
| `namespace `[`JL_GameProdEnv_CustomPackage::Runtime`](#namespace_j_l___game_prod_env___custom_package_1_1_runtime) |              |

# namespace `JL_GameProdEnv_CustomPackage::Editor` 

## Summary

| Members                                                                                                                                             | Descriptions                                                                                                                                                                                                                                                          |
|-----------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `class `[`JL_GameProdEnv_CustomPackage::Editor::ReplayToolsWindow`](#class_j_l___game_prod_env___custom_package_1_1_editor_1_1_replay_tools_window) | Custom Unity [Editor](#namespace_j_l___game_prod_env___custom_package_1_1_editor) window for managing gameplay recordings and replays. Provides functionality to record, save, and replay physics-based gameplay with support for both player and non-player objects. |

# class `JL_GameProdEnv_CustomPackage::Editor::ReplayToolsWindow` 

```
class JL_GameProdEnv_CustomPackage::Editor::ReplayToolsWindow
  : public EditorWindow
```  

Custom Unity [Editor](#namespace_j_l___game_prod_env___custom_package_1_1_editor) window for managing gameplay recordings and replays. Provides functionality to record, save, and replay physics-based gameplay with support for both player and non-player objects.

## Summary

| Members                        | Descriptions                                |
|--------------------------------|---------------------------------------------|

## Members

# namespace `JL_GameProdEnv_CustomPackage::Runtime` 

## Summary

| Members                                                                                                                                                         | Descriptions                                                                                                                                                                                                       |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::ReplayPlayer`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player)                      | Handles the playback of recorded replay sessions within the Unity [Editor](#namespace_j_l___game_prod_env___custom_package_1_1_editor). Manages visualization objects, playback controls, and frame interpolation. |
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::ReplayRecorder`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder)                  | Handles the recording of Rigidbody and Transform data for replay functionality. Manages the initiation, termination, and data management of a recording session.                                                   |
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)                    | Represents a replay session containing recorded data for various rigidbody objects during gameplay.                                                                                                                |
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::ReplaySessionManager`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager)     | Manages the orchestration and persistence of replay sessions, allowing for addition, retrieval, removal, and cleanup of recorded gameplay sessions.                                                                |
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data) | Represents a recording of transform data for a Rigidbody component. This is used to capture and store the movement data of a Rigidbody over time for replay purposes.                                              |
| `class `[`JL_GameProdEnv_CustomPackage::Runtime::TransformData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data)                    | Represents the transformation data for an object, including position, rotation, velocity, angular velocity, and a timestamp.                                                                                       |

# class `JL_GameProdEnv_CustomPackage::Runtime::ReplayPlayer` 

Handles the playback of recorded replay sessions within the Unity [Editor](#namespace_j_l___game_prod_env___custom_package_1_1_editor). Manages visualization objects, playback controls, and frame interpolation.

## Summary

| Members                                                                                                                                                                                                                                                       | Descriptions                                                                                                                                                   |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `{property} `[`ReplayPlayer`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a8eeaf3aca2f6b5afbd6ea4fad6368714)                         | Gets the singleton instance of the [ReplayPlayer](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player), creating it if it doesn't exist. |
| `{property} bool `[`IsPlaying`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ae0e4cbd7988fa5ceaf77544f6be58e07)                                                                                                                 | Gets whether replay playback is currently active.                                                                                                              |
| `{property} float `[`PlaybackProgress`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a7525bb4811bb1dc39c6e53d8d96fcf3f)                                                                                                         | Gets the current playback progress as a normalized value between 0 and 1.                                                                                      |
| `{property} float `[`PlaybackTime`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a90404353340dc99eed90ec008563b8d0)                                                                                                             | Gets the current playback time in seconds.                                                                                                                     |
| `{property} float `[`SessionDuration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a2387f2a618fd7e484add6eda5f378ab9)                                                                                                          | Gets the total duration of the current session in seconds.                                                                                                     |
| `{property} string `[`CurrentSessionName`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1aec1ea3af8577ed604eff70ecadbd9a36)                                                                                                      | Gets the name of the currently loaded replay session.                                                                                                          |
| `public inline void `[`LoadSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a8a06e1ee70db22ff6760708c282b054c)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)` | Loads a replay session and creates the necessary visualization objects.                                                                                        |
| `public inline void `[`StartPlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ad8c2d65419944906f391d10c724bdfc9)`()`                                                                                                      | Starts playback of the currently loaded replay session.                                                                                                        |
| `public inline void `[`PausePlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ab709a668396c8f35d967d06a8143b75a)`()`                                                                                                      | Pauses the current playback without resetting the playback time.                                                                                               |
| `public inline void `[`ResumePlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a6230724758b2d36bf7568c1b25877724)`()`                                                                                                     | Resumes playback from the current position after being paused.                                                                                                 |
| `public inline void `[`StopPlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ac68541951439bebc4617c94ef7827979)`()`                                                                                                       | Stops playback and resets the playback time to the beginning.                                                                                                  |
| `public inline void `[`SetPlaybackProgress`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1afd996f093f4b07637e609e9986c1a63c)`(float normalizedTime)`                                                                            | Sets the playback progress to a specific normalized time.                                                                                                      |
| `public inline void `[`SetPlaybackSpeed`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1af42e67fc2045ff91c0f5f81232f82957)`(float speed)`                                                                                        | Sets the playback speed multiplier.                                                                                                                            |
| `public inline void `[`ClearVisualObjects`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a7aa8fbf97e18dab5d117cedc4004101e)`()`                                                                                                 | Removes all visual objects created for replay visualization.                                                                                                   |
| `public inline void `[`CleanUp`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a08fde0b50bc1b88cb36c0b849915f757)`()`                                                                                                            | Performs complete cleanup of the replay player, stopping playback, removing visual objects, and resetting the singleton instance.                              |

## Members

#### `{property} `[`ReplayPlayer`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a8eeaf3aca2f6b5afbd6ea4fad6368714) 

Gets the singleton instance of the [ReplayPlayer](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player), creating it if it doesn't exist.

#### `{property} bool `[`IsPlaying`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ae0e4cbd7988fa5ceaf77544f6be58e07) 

Gets whether replay playback is currently active.

#### `{property} float `[`PlaybackProgress`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a7525bb4811bb1dc39c6e53d8d96fcf3f) 

Gets the current playback progress as a normalized value between 0 and 1.

#### `{property} float `[`PlaybackTime`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a90404353340dc99eed90ec008563b8d0) 

Gets the current playback time in seconds.

#### `{property} float `[`SessionDuration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a2387f2a618fd7e484add6eda5f378ab9) 

Gets the total duration of the current session in seconds.

#### `{property} string `[`CurrentSessionName`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1aec1ea3af8577ed604eff70ecadbd9a36) 

Gets the name of the currently loaded replay session.

#### `public inline void `[`LoadSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a8a06e1ee70db22ff6760708c282b054c)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)` 

Loads a replay session and creates the necessary visualization objects.

#### Parameters
* `session` The replay session to load and prepare for playback.

#### `public inline void `[`StartPlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ad8c2d65419944906f391d10c724bdfc9)`()` 

Starts playback of the currently loaded replay session.

This method initializes playback from the beginning, subscribes to editor updates, and sets the visual objects to their initial positions.

#### `public inline void `[`PausePlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ab709a668396c8f35d967d06a8143b75a)`()` 

Pauses the current playback without resetting the playback time.

#### `public inline void `[`ResumePlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a6230724758b2d36bf7568c1b25877724)`()` 

Resumes playback from the current position after being paused.

#### `public inline void `[`StopPlayback`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1ac68541951439bebc4617c94ef7827979)`()` 

Stops playback and resets the playback time to the beginning.

This method unsubscribes from editor updates and resets the visualization to the initial frame.

#### `public inline void `[`SetPlaybackProgress`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1afd996f093f4b07637e609e9986c1a63c)`(float normalizedTime)` 

Sets the playback progress to a specific normalized time.

#### Parameters
* `normalizedTime` Normalized time value between 0 and 1.

#### `public inline void `[`SetPlaybackSpeed`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1af42e67fc2045ff91c0f5f81232f82957)`(float speed)` 

Sets the playback speed multiplier.

#### Parameters
* `speed` The speed multiplier, clamped between 0.1 and 3.0.

#### `public inline void `[`ClearVisualObjects`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a7aa8fbf97e18dab5d117cedc4004101e)`()` 

Removes all visual objects created for replay visualization.

#### `public inline void `[`CleanUp`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player_1a08fde0b50bc1b88cb36c0b849915f757)`()` 

Performs complete cleanup of the replay player, stopping playback, removing visual objects, and resetting the singleton instance.

# class `JL_GameProdEnv_CustomPackage::Runtime::ReplayRecorder` 

```
class JL_GameProdEnv_CustomPackage::Runtime::ReplayRecorder
  : public MonoBehaviour
```  

Handles the recording of Rigidbody and Transform data for replay functionality. Manages the initiation, termination, and data management of a recording session.

## Summary

| Members                                                                                                                                                                                                                                                                                    | Descriptions                                                                                                                                                                                                                  |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `{property} bool `[`IsRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1aaae9a7c53bba55e2cc8de28c8e5578a1)                                                                                                                                          | Indicates whether the recorder is currently in the process of recording.                                                                                                                                                      |
| `{property} float `[`RecordingDuration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a520ff14541bc329768b5b024a78a7061)                                                                                                                                   | Gets the duration of the current recording session in seconds.                                                                                                                                                                |
| `{property} int `[`RecordedObjectsCount`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1ad1484903dfce4fc62b999a53f1577024)                                                                                                                                  | Gets the number of objects currently being recorded.                                                                                                                                                                          |
| `{property} `[`ReplayRecorder`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a1f622b7b06add9d6aa0ca10bc24b4ca5)                                                | Gets the singleton instance of the [ReplayRecorder](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder) class. If no instance exists, a new object will be instantiated and persisted across scenes. |
| `public inline void `[`StartRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a81b33592f8b87340dff79b5d86bd87b5)`(List< Rigidbody > playerRigidbodies,List< Rigidbody > normalRigidbodies)`                                                         | Starts the recording process for specified rigidbodies.                                                                                                                                                                       |
| `public inline void `[`StopRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a87503d07e8cd54d8dc160b6710b5b5f5)`()`                                                                                                                                 | Stops the current recording process if it is ongoing. Resets the recording status and records the final duration of the recording for reference. No effect if recording is not active.                                        |
| `public inline `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` `[`SaveCurrentRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1ad6b46ecad59b392e9faab96244714ee7)`(string sessionName)`             | Saves the current recording session as a replay session.                                                                                                                                                                      |
| `public inline void `[`ClearCurrentRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a3695300c646725b167940a0e6e7b1a8b)`()`                                                                                                                         | Clears all current recording data and resets the recording state. This method stops any ongoing recording, clears the stored recorded data, and resets the internal recording start and end time to their initial values.     |
| `public inline List< `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data)` > `[`GetCurrentRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1aa807e1e48fdeaee1c467d6a6f3cd2c4d)`()` | Retrieves the current recording data as a list of rigidbody recording entries.                                                                                                                                                |
| `public inline bool `[`HasRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a1222e44c87862c27eb92303a56e27ced)`()`                                                                                                                              | Checks if the current recording contains any valid recorded data.                                                                                                                                                             |

## Members

#### `{property} bool `[`IsRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1aaae9a7c53bba55e2cc8de28c8e5578a1) 

Indicates whether the recorder is currently in the process of recording.

#### `{property} float `[`RecordingDuration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a520ff14541bc329768b5b024a78a7061) 

Gets the duration of the current recording session in seconds.

The duration is calculated as the difference between the current time and the start time if a recording is in progress. If the recording has stopped, it returns the duration between the start and end times of the last recorded session. 

A floating-point value representing the duration of the recording in seconds.

#### `{property} int `[`RecordedObjectsCount`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1ad1484903dfce4fc62b999a53f1577024) 

Gets the number of objects currently being recorded.

This property reflects the total count of objects that are actively included in the current recording session.

#### `{property} `[`ReplayRecorder`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a1f622b7b06add9d6aa0ca10bc24b4ca5) 

Gets the singleton instance of the [ReplayRecorder](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder) class. If no instance exists, a new object will be instantiated and persisted across scenes.

The Instance property ensures a single global instance of the [ReplayRecorder](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder) class, utilizing lazy initialization. If accessed for the first time and no instance exists, it creates a new GameObject named "ReplayRecorder" and attaches the [ReplayRecorder](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder) component to it.

#### `public inline void `[`StartRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a81b33592f8b87340dff79b5d86bd87b5)`(List< Rigidbody > playerRigidbodies,List< Rigidbody > normalRigidbodies)` 

Starts the recording process for specified rigidbodies.

#### Parameters
* `playerRigidbodies` A list of Rigidbody objects considered as player-controlled objects.

* `normalRigidbodies` A list of Rigidbody objects considered as non-player-controlled objects.

#### `public inline void `[`StopRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a87503d07e8cd54d8dc160b6710b5b5f5)`()` 

Stops the current recording process if it is ongoing. Resets the recording status and records the final duration of the recording for reference. No effect if recording is not active. 
#### Returns
Void. There is no return value for this method.

#### `public inline `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` `[`SaveCurrentRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1ad6b46ecad59b392e9faab96244714ee7)`(string sessionName)` 

Saves the current recording session as a replay session. 
#### Parameters
* `sessionName` The name to assign to the replay session. If not provided, the session will remain unnamed.

#### Returns
A [ReplaySession](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session) object containing the saved recording data. Returns null if no recording data exists.

#### `public inline void `[`ClearCurrentRecording`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a3695300c646725b167940a0e6e7b1a8b)`()` 

Clears all current recording data and resets the recording state. This method stops any ongoing recording, clears the stored recorded data, and resets the internal recording start and end time to their initial values. 
#### Returns
Void. No value is returned.

#### `public inline List< `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data)` > `[`GetCurrentRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1aa807e1e48fdeaee1c467d6a6f3cd2c4d)`()` 

Retrieves the current recording data as a list of rigidbody recording entries.

#### Returns
A list of [RigidbodyRecordingData](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data) containing the recording data collected during the current recording session. Returns an empty list if no recording data is available.

#### `public inline bool `[`HasRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder_1a1222e44c87862c27eb92303a56e27ced)`()` 

Checks if the current recording contains any valid recorded data. 
#### Returns
true if there is at least one object with recorded frames; otherwise, false.

# class `JL_GameProdEnv_CustomPackage::Runtime::ReplaySession` 

Represents a replay session containing recorded data for various rigidbody objects during gameplay.

Each replay session records the name, duration, timestamp of recording, and a list of rigidbody recordings. This data is typically used to replay object movements and interactions that occurred during gameplay.

## Summary

| Members                                                                                                                                                                                                                                                                | Descriptions                                                                                                                                                                                            |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `public string `[`name`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a0d61e0c40e0025533c126a9f6d9a1fe5)                                                                                                                                | The name of the replay session. Used to uniquely identify the session and can be provided by the user or automatically generated.                                                                       |
| `public float `[`duration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a2eb0b46197c40fa108ec6b043b56a354)                                                                                                                             | Represents the total duration of the replay session in seconds. It is calculated as the difference between the session's start and end time, corresponding to the total time span of the recorded data. |
| `public DateTime `[`recordedAt`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1ae9180948141f2e54e6e5ec70d7d71e51)                                                                                                                        | Represents the date and time at which the replay session was recorded.                                                                                                                                  |
| `public List< `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data)` > `[`recordedObjects`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a85c10817c09719352fdc59b4ac18076d) | A collection that holds the recorded data of rigidbody objects tracked during a replay session.                                                                                                         |
| `public inline  `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a7724daf7a1889c48807cd2bec09e4c2f)`(string sessionName)`                                                                                                | Represents a replay session that contains recorded data for multiple objects over a set duration.                                                                                                       |

## Members

#### `public string `[`name`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a0d61e0c40e0025533c126a9f6d9a1fe5) 

The name of the replay session. Used to uniquely identify the session and can be provided by the user or automatically generated.

#### `public float `[`duration`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a2eb0b46197c40fa108ec6b043b56a354) 

Represents the total duration of the replay session in seconds. It is calculated as the difference between the session's start and end time, corresponding to the total time span of the recorded data.

#### `public DateTime `[`recordedAt`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1ae9180948141f2e54e6e5ec70d7d71e51) 

Represents the date and time at which the replay session was recorded.

This variable stores the exact timestamp when the replay session begins or is initialized. It uses the DateTime structure to encapsulate both date and time information.

#### `public List< `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data)` > `[`recordedObjects`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a85c10817c09719352fdc59b4ac18076d) 

A collection that holds the recorded data of rigidbody objects tracked during a replay session.

Each entry in the collection contains rigidbody recording data, which includes object name, type (player or normal), and a list of recorded frames capturing the object's transform and physics data over time.

#### `public inline  `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_1a7724daf7a1889c48807cd2bec09e4c2f)`(string sessionName)` 

Represents a replay session that contains recorded data for multiple objects over a set duration.

A replay session stores the state and movement data (position, rotation, velocity, and angular velocity) for rigidbodies captured during a recording. It includes metadata such as session name, recording duration, and the time at which the session was recorded.

# class `JL_GameProdEnv_CustomPackage::Runtime::ReplaySessionManager` 

Manages the orchestration and persistence of replay sessions, allowing for addition, retrieval, removal, and cleanup of recorded gameplay sessions.

## Summary

| Members                                                                                                                                                                                                                                                                  | Descriptions                                                                                                                                                                                                                                                                 |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `{property} `[`ReplaySessionManager`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ac3ace7710d37e7d7dc0bb9c76b356751)          | Gets the singleton instance of the associated class. This property is used to ensure that only one instance of the class exists and provides a global point of access to it.                                                                                                 |
| `{property} int `[`SessionCount`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1a1c9a007b4ba2238274843379022a69ff)                                                                                                                 | Gets the total number of replay sessions currently managed by the system.                                                                                                                                                                                                    |
| `public List< `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` > `[`GetAllSessions`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ab7bb7aec3cff52fb0cf1a4d4b59d58bd)`()`           | Retrieves all replay sessions currently available in the system.                                                                                                                                                                                                             |
| `public inline void `[`AddSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ad3d1c68d40a466bf70f87dff1351cc55)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)`    | Adds a new replay session to the session manager and saves it to disk.                                                                                                                                                                                                       |
| `public inline void `[`RemoveSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1a1f2503c2009257530be0f92142b34ddb)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)` | Removes the specified replay session from the session list and deletes its associated file from storage.                                                                                                                                                                     |
| `public inline void `[`ClearAllSessions`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1af1aafe23b9c017d0877610448521b40f)`()`                                                                                                     | Clears all recorded replay sessions from memory and deletes their corresponding files from disk. This method iterates through all stored replay sessions, removes their associated files from disk, and clears the session list in memory to ensure no session data remains. |
| `public inline `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` `[`GetSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1aeb272f582a30e40b4406ffd90e82b298)`(string name)`     | Retrieves a replay session by its name.                                                                                                                                                                                                                                      |

## Members

#### `{property} `[`ReplaySessionManager`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager)` `[`Instance`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ac3ace7710d37e7d7dc0bb9c76b356751) 

Gets the singleton instance of the associated class. This property is used to ensure that only one instance of the class exists and provides a global point of access to it.

#### `{property} int `[`SessionCount`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1a1c9a007b4ba2238274843379022a69ff) 

Gets the total number of replay sessions currently managed by the system.

#### `public List< `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` > `[`GetAllSessions`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ab7bb7aec3cff52fb0cf1a4d4b59d58bd)`()` 

Retrieves all replay sessions currently available in the system. 
#### Returns
A list of [ReplaySession](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session) objects representing all recorded sessions.

#### `public inline void `[`AddSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1ad3d1c68d40a466bf70f87dff1351cc55)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)` 

Adds a new replay session to the session manager and saves it to disk.

#### Parameters
* `session` The replay session to be added.

#### `public inline void `[`RemoveSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1a1f2503c2009257530be0f92142b34ddb)`(`[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` session)` 

Removes the specified replay session from the session list and deletes its associated file from storage.

#### Parameters
* `session` The [ReplaySession](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session) instance to be removed.

#### `public inline void `[`ClearAllSessions`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1af1aafe23b9c017d0877610448521b40f)`()` 

Clears all recorded replay sessions from memory and deletes their corresponding files from disk. This method iterates through all stored replay sessions, removes their associated files from disk, and clears the session list in memory to ensure no session data remains. 
#### Returns
Void. No return value.

#### `public inline `[`ReplaySession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session)` `[`GetSession`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager_1aeb272f582a30e40b4406ffd90e82b298)`(string name)` 

Retrieves a replay session by its name. 
#### Parameters
* `name` The name of the replay session to retrieve.

#### Returns
The [ReplaySession](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session) that matches the given name, or null if no match is found.

# class `JL_GameProdEnv_CustomPackage::Runtime::RigidbodyRecordingData` 

Represents a recording of transform data for a Rigidbody component. This is used to capture and store the movement data of a Rigidbody over time for replay purposes.

## Summary

| Members                                                                                                                                                                                                                                                      | Descriptions                                                                                                                                                                                                                              |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `public string `[`objectName`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1ac8643ed0a9c9d7215b2981ae55e9c2dc)                                                                                                      | Represents a unique identifier or name associated with an object in a replay system during its recording and playback.                                                                                                                    |
| `public bool `[`isPlayer`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a92b0725e55ebd29e9096d9ecf4eea537)                                                                                                          | A flag indicating whether the object associated with the recording data represents a player.                                                                                                                                              |
| `public List< `[`TransformData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data)` > `[`recordedFrames`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a86a551e5ac195008b10caa78f370cd42) | Stores the recorded frames of transform data for a specific Rigidbody. This list holds the sequential snapshots of the Rigidbody's transformation, including position, rotation, velocity, and angular velocity, at different timestamps. |
| `public Rigidbody `[`rigidbody`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a7b661ee8e53b9645afb3d63ccd3ff611)                                                                                                    | Represents the runtime reference to the Rigidbody component of a game object. Used mainly for tracking and recording physical properties such as position, rotation, velocity, and angular velocity during runtime.                       |
| `public inline  `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a8c8b2f0a00f7f3b390662d2936037109)`(Rigidbody rb,bool playerType)`                                                         | Represents recording data for a Rigidbody, including its state over time during a replay session.                                                                                                                                         |
| `public inline void `[`AddFrame`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a0ada28128aaa938f9048ae17674ce929)`(float currentTime)`                                                                              | Adds a frame containing the current transform and physics data to the recording.                                                                                                                                                          |
| `public inline void `[`ClearFrames`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a3b605b8e4cb9363f640e625148ebefe1)`()`                                                                                            | Clears all recorded frames from the current object.                                                                                                                                                                                       |

## Members

#### `public string `[`objectName`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1ac8643ed0a9c9d7215b2981ae55e9c2dc) 

Represents a unique identifier or name associated with an object in a replay system during its recording and playback.

#### `public bool `[`isPlayer`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a92b0725e55ebd29e9096d9ecf4eea537) 

A flag indicating whether the object associated with the recording data represents a player.

#### `public List< `[`TransformData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data)` > `[`recordedFrames`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a86a551e5ac195008b10caa78f370cd42) 

Stores the recorded frames of transform data for a specific Rigidbody. This list holds the sequential snapshots of the Rigidbody's transformation, including position, rotation, velocity, and angular velocity, at different timestamps.

#### `public Rigidbody `[`rigidbody`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a7b661ee8e53b9645afb3d63ccd3ff611) 

Represents the runtime reference to the Rigidbody component of a game object. Used mainly for tracking and recording physical properties such as position, rotation, velocity, and angular velocity during runtime.

#### `public inline  `[`RigidbodyRecordingData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a8c8b2f0a00f7f3b390662d2936037109)`(Rigidbody rb,bool playerType)` 

Represents recording data for a Rigidbody, including its state over time during a replay session.

#### `public inline void `[`AddFrame`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a0ada28128aaa938f9048ae17674ce929)`(float currentTime)` 

Adds a frame containing the current transform and physics data to the recording.

#### Parameters
* `currentTime` The current time at which the frame is being recorded.

#### Returns
None. Adds a frame to the recordedFrames list if the Rigidbody is not null.

#### `public inline void `[`ClearFrames`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data_1a3b605b8e4cb9363f640e625148ebefe1)`()` 

Clears all recorded frames from the current object.

#### Returns
None.

# class `JL_GameProdEnv_CustomPackage::Runtime::TransformData` 

Represents the transformation data for an object, including position, rotation, velocity, angular velocity, and a timestamp.

## Summary

| Members                                                                                                                                                                                                 | Descriptions                                                                                                                                                                                                                                                                                                                                                                   |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `public Vector3 `[`position`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a44ea6817fc6593fc28e90ef9efb627c2)                                                            | Represents the position of an object in 3D space.                                                                                                                                                                                                                                                                                                                              |
| `public Quaternion `[`rotation`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1acef71fdf197048c82bf78d6e13b7df2e)                                                         | Represents the rotation of a transform at a specific point in time during recording. This property stores the orientation of the object as a quaternion. Used in conjunction with position, velocity, and angularVelocity to record and replay the state of a Rigidbody.                                                                                                       |
| `public Vector3 `[`velocity`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a3078458340ad83383ec7c7e3987280f5)                                                            | Represents the linear velocity of a rigidbody at a specific point in time. This property captures and stores the current linear velocity of the rigidbody as part of the `[TransformData](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data)`. Used to record and analyze movement over time, typically during a replay recording or playback system. |
| `public Vector3 `[`angularVelocity`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a3b2ff08ebdf45f325792d70ecdf7e297)                                                     | Represents the angular velocity of an object in 3D space. This is typically used to record and store the rotational speed of a rigidbody during gameplay and replay systems. It defines the rate of rotation around each axis (X, Y, Z) in radians per second.                                                                                                                 |
| `public float `[`timestamp`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1adf20a3990d8455fcd04b974e32036b6b)                                                             | Represents the time at which a specific frame of transform data was recorded during a replay session. This variable is used to timestamp each frame, allowing for accurate playback of recorded animations or events.                                                                                                                                                          |
| `public inline  `[`TransformData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a554016864e2e12b17a2682e1702c70c7)`(Transform transform,Rigidbody rigidbody,float time)` | Represents the state and motion of a Transform and Rigidbody at a specific point in time. This class encapsulates the position, rotation, velocity, angular velocity, and timestamp of a Transform and Rigidbody. It is primarily used for recording gameplay state during a replay session.                                                                                   |

## Members

#### `public Vector3 `[`position`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a44ea6817fc6593fc28e90ef9efb627c2) 

Represents the position of an object in 3D space.

This variable is part of the [TransformData](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data) class and stores the positional coordinates of a transform at a specific point in time within the replay system.

#### `public Quaternion `[`rotation`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1acef71fdf197048c82bf78d6e13b7df2e) 

Represents the rotation of a transform at a specific point in time during recording. This property stores the orientation of the object as a quaternion. Used in conjunction with position, velocity, and angularVelocity to record and replay the state of a Rigidbody.

#### `public Vector3 `[`velocity`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a3078458340ad83383ec7c7e3987280f5) 

Represents the linear velocity of a rigidbody at a specific point in time. This property captures and stores the current linear velocity of the rigidbody as part of the `[TransformData](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data)`. Used to record and analyze movement over time, typically during a replay recording or playback system.

#### `public Vector3 `[`angularVelocity`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a3b2ff08ebdf45f325792d70ecdf7e297) 

Represents the angular velocity of an object in 3D space. This is typically used to record and store the rotational speed of a rigidbody during gameplay and replay systems. It defines the rate of rotation around each axis (X, Y, Z) in radians per second.

#### `public float `[`timestamp`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1adf20a3990d8455fcd04b974e32036b6b) 

Represents the time at which a specific frame of transform data was recorded during a replay session. This variable is used to timestamp each frame, allowing for accurate playback of recorded animations or events.

#### `public inline  `[`TransformData`](#class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data_1a554016864e2e12b17a2682e1702c70c7)`(Transform transform,Rigidbody rigidbody,float time)` 

Represents the state and motion of a Transform and Rigidbody at a specific point in time. This class encapsulates the position, rotation, velocity, angular velocity, and timestamp of a Transform and Rigidbody. It is primarily used for recording gameplay state during a replay session.

Generated by [Moxygen](https://sourcey.com/moxygen)