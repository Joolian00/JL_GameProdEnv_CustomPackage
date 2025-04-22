using JL_GameProdEnv_CustomPackage.Runtime.RBController.ScriptableObjects;

using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.Components
{
    [System.Serializable]
    public class RbcCamera
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform cameraPositionTarget;
        [SerializeField] private Transform cameraLookTarget;
    
        [SerializeField] private Vector3 lookTargetOffset;
        [SerializeField] private Vector3 positionTargetOffset;
    
        // Smoothing settings
        [SerializeField] private bool useSmoothing;
        [SerializeField] private float smoothingTime;
        [SerializeField] private float maxSmoothSpeed;
        
        [SerializeField] private float horizontalSensitivity;
        [SerializeField] private float verticalSensitivity;
        

        // Smoothing variables
        private Vector3 _targetPosition;
        private Vector3 _smoothedPosition;
        private Vector3 _smoothVelocity;

        private bool _hasInitializedPosition = false;

        public RbcCamera(Transform camPivot, Transform camPosTarget, Transform camLookTarget, CameraSettings settings)
        {
            cameraPivot = camPivot;
            cameraPositionTarget = camPosTarget;
            cameraLookTarget = camLookTarget;
        
            lookTargetOffset = settings.LookTargetOffset;
            positionTargetOffset = settings.PositionTargetOffset;
        
            useSmoothing = settings.UseSmoothing;
            smoothingTime = settings.SmoothingTime;
            maxSmoothSpeed = settings.MaxSmoothSpeed;

            horizontalSensitivity = settings.HorizontalSensitivity;
            verticalSensitivity = settings.VerticalSensitivity;
        
            if (cameraPivot != null)
            {
                _targetPosition = cameraPivot.position;
                _smoothedPosition = _targetPosition;
            }
        }
    
        public void PositionCameraPivot(Vector3 position)
        {
            _targetPosition = position;
            
            // If this is the first position update after initialization, snap directly
            if (!_hasInitializedPosition)
            {
                cameraPivot.position = _targetPosition;
                _smoothedPosition = _targetPosition;
                _hasInitializedPosition = true;
                return;
            }

            if (!useSmoothing)
            {
                cameraPivot.position = _targetPosition;
                return;
            }
        
            _smoothedPosition = Vector3.SmoothDamp(
                _smoothedPosition,
                _targetPosition,
                ref _smoothVelocity,
                smoothingTime,
                maxSmoothSpeed
            );
        
            cameraPivot.position = _smoothedPosition;
        }

        public void HandleLookInput(Vector2 lookInput)
        {
            // Rotate the camera pivot horizontally using horizontal sensitivity
            cameraPivot.Rotate(0, lookInput.x * horizontalSensitivity, 0);

            // Get current local position of the look target
            Vector3 localPos = cameraLookTarget.localPosition;

            // Update y position based on input using vertical sensitivity
            localPos.y += lookInput.y * verticalSensitivity * 0.01f; // Added 0.01f scaling factor to maintain similar feel

            // Clamp the y position between 0 and 2 in local space
            localPos.y = Mathf.Clamp(localPos.y, 0f, 2f);

            // Apply the updated local position
            cameraLookTarget.localPosition = localPos;
        }

        public void ResetPositionInitialization()
        {
            _hasInitializedPosition = false;
        }
    }
}
