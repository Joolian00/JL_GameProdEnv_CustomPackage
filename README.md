# Custom Replay Tool for Unity - [GitHub](https://joolian00.github.io/JL_GameProdEnv_CustomPackage/)
A Unity package for recording, saving, and replaying physics-based gameplay.

Records Rigidbody transformations at configurable frequencies, with higher precision for player objects. (Though I think this is currently bugged)

Provides an editor window for easy recording management and visualization of replays.


## Documentation

[Doxygen Documentation](./docs/html/index.html)

## Key Components

### Editor
- **[ReplayToolsWindow](./docs/html/class_j_l___game_prod_env___custom_package_1_1_editor_1_1_replay_tools_window.html)**: Editor window for managing gameplay recordings and replays.

### Runtime
- **[ReplayRecorder](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_recorder.html)**: Handles recording of Rigidbody and Transform data.
- **[ReplayPlayer](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_player.html)**: Manages playback of recorded sessions with controls for play, pause, and speed.
- **[ReplaySession](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session.html)**: Contains recorded data for various rigidbody objects during gameplay.
- **[ReplaySessionManager](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_replay_session_manager.html)**: Orchestrates persistence and management of replay sessions.
- **[RigidbodyRecordingData](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_rigidbody_recording_data.html)**: Stores transformation data for a specific Rigidbody.
- **[TransformData](./docs/html/class_j_l___game_prod_env___custom_package_1_1_runtime_1_1_transform_data.html)**: Represents position, rotation, velocity data at specific timestamps.

## Installation
Unity Package Manager: Click on the plus → Add from Custom Git Url

```
https://github.com/Joolian00/JL_GameProdEnv_CustomPackage.git
```
    
