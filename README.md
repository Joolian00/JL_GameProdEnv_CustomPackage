# Custom Replay Tool for Unity
A Unity package for recording, saving, and replaying physics-based gameplay.

Records Rigidbody transformations at configurable frequencies, with higher precision for player objects. (Though I think this is currently bugged)

Provides an editor window for easy recording management and visualization of replays.


## Documentation

- [GitHub](https://github.com/Joolian00/JL_GameProdEnv_CustomPackage)
- [GitHub Pages](https://joolian00.github.io/JL_GameProdEnv_CustomPackage/)
- [Doxygen Documentation](https://joolian00.github.io/JL_GameProdEnv_CustomPackage/docs/html/index.html)

## Key Components

### Editor
- **ReplayToolsWindow**: Editor window for managing gameplay recordings and replays.

### Runtime
- **ReplayRecorder**: Handles recording of Rigidbody and Transform data.
- **ReplayPlayer**: Manages playback of recorded sessions with controls for play, pause, and speed.
- **ReplaySession**: Contains recorded data for various rigidbody objects during gameplay.
- **ReplaySessionManager**: Orchestrates persistence and management of replay sessions.
- **RigidbodyRecordingData**: Stores transformation data for a specific Rigidbody.
- **TransformData**: Represents position, rotation, velocity data at specific timestamps.

## Installation
Unity Package Manager: Click on the plus → Add from Custom Git Url

```
https://github.com/Joolian00/JL_GameProdEnv_CustomPackage.git
```
    
