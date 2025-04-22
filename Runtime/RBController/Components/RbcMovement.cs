using JL_GameProdEnv_CustomPackage.Runtime.RBController.ScriptableObjects;

using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.Components
{
    [System.Serializable]
    public class RbcMovement
    {
        private readonly Rigidbody _rb;
        private readonly Transform _modelPivot;
        private readonly Transform _playerCamera;
        
        [Header("Locomotion")]
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _maxAccelForce;
        [SerializeField] private float _frictionCoefficient;
        [SerializeField] private float _staticFrictionThreshold;
        
        [Header("Jumping")]
        [SerializeField] private float _jumpUpVelocity;

        [SerializeField] private AnimationCurve _jumpUpVelocityFactorFromExistingY;
        [SerializeField] private AnimationCurve _analogJumpUpForce;
        [SerializeField] private float _jumpTerminalVelocity;
        [SerializeField] private float _jumpDuration;
        [SerializeField] private float _jumpFallFactor;

        private bool _hasButtonBeenReleased = true;
        public bool IsPlayerFalling => _rb.linearVelocity.y < 0.1f;

        
        public bool IsJumping { get; private set; }
        public bool IsJumpForceActive { get; private set; }

        private float _jumpHoldTime;
        private Vector3? _lastShootDirection = null;  // Add this field to store the last shoot direction
        
        private float _lastShootTime;

        public RbcMovement(Rigidbody rb, Transform modelPivot, Transform playerCamera, MovementSettings settings)
        {
            _rb = rb;
            
            _modelPivot = modelPivot;
            _playerCamera = playerCamera;
            
            _maxSpeed = settings.MaxSpeed;
            _maxAccelForce = settings.MaxAccelForce;
            _frictionCoefficient = settings.FrictionCoefficient;
            _staticFrictionThreshold = settings.StaticFrictionThreshold;
            
            _jumpUpVelocity = settings.JumpUpVelocity;
            _jumpUpVelocityFactorFromExistingY = settings.JumpUpVelocityFactorFromExistingY;
            _analogJumpUpForce = settings.AnalogJumpUpForce;
            _jumpTerminalVelocity = settings.JumpTerminalVelocity;
            _jumpDuration = settings.JumpDuration;
            _jumpFallFactor = settings.JumpFallFactor;
        }

        public void HandleMovement(Vector2 moveInput)
        {
            // Get the current input and calculate the desired movement direction in the x z plane using the player's camera
            Vector3 inputDirection = GetInputDirection(moveInput);
            
            Vector3 transformedDirection = _rb.transform.TransformDirection(inputDirection);
            transformedDirection.y = 0; // Keep movement on the horizontal plane
            transformedDirection.Normalize();


            Vector3 goalVel = transformedDirection * _maxSpeed;
        
            // Calculate needed acceleration to reach the goal velocity
            Vector3 currentVelXZ = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            Vector3 neededAccel = (goalVel - currentVelXZ) / Time.fixedDeltaTime;
    
            // Limit the acceleration force if needed
            if (neededAccel.magnitude > _maxAccelForce)
            {
                neededAccel = neededAccel.normalized * _maxAccelForce;
            }
    
            // Apply the force
            _rb.AddForce(neededAccel * _rb.mass);
        }

        public void ApplyFriction()
        {
            // Get horizontal velocity
            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            float currentSpeed = horizontalVelocity.magnitude;
        
            // Stop completely if below threshold
            if (currentSpeed < _staticFrictionThreshold)
            {
                _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
                return;
            }
        
            // Calculate and apply drag force
            Vector3 dragForce = -horizontalVelocity.normalized * (_frictionCoefficient * currentSpeed);
            _rb.AddForce(dragForce, ForceMode.Acceleration);
        }

    public void HandleModelRotation(Vector2 moveInput, bool shootPressed)
    {
        Vector3 targetDirection;
        Vector3 inputDirection = GetInputDirection(moveInput);
        bool hasMovementInput = inputDirection.magnitude >= 0.1f;

        // Track time since last shoot button press
        if (shootPressed)
        {
            _lastShootTime = Time.time;
            // When shooting, store and use camera's forward direction (only horizontal component)
            _lastShootDirection = new Vector3(_playerCamera.forward.x, 0, _playerCamera.forward.z).normalized;
        }

        // Determine target direction based on priorities
        float timeSinceLastShoot = Time.time - _lastShootTime;
        // TODO: Replace magic number with field
        bool shouldUseShootDirection = _lastShootDirection.HasValue && timeSinceLastShoot < 0.5f;

        if (shouldUseShootDirection)
        {
            // Use last shoot direction within the time window
            targetDirection = _lastShootDirection.Value;
        }
        else if (hasMovementInput)
        {
            // Clear last shoot direction if outside time window and use movement direction
            if (!shootPressed)
            {
                _lastShootDirection = null;
            }
            targetDirection = inputDirection.normalized;
        }
        else if (_lastShootDirection.HasValue)
        {
            // Keep using last shoot direction when no movement
            targetDirection = _lastShootDirection.Value;
        } 
        else
        {
            return;
        }

        // Calculate target rotation based on the determined direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        // Ensure the target rotation has no X or Z component
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        
        // Smoothly rotate the model pivot toward the target rotation
        _modelPivot.rotation = Quaternion.Slerp(
            _modelPivot.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    // Helper method to ensure rotation has no X or Z component

        
        private Vector3 GetInputDirection(Vector2 moveInput)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            inputDirection = Quaternion.Euler(0, _playerCamera.eulerAngles.y, 0) * inputDirection;

            return inputDirection;
        }
        
        private void EnsureProperRotation()
        {
            if (_modelPivot.rotation.eulerAngles is { x: 0f, z: 0f })
            {
                return;
            }
            
            _modelPivot.rotation = Quaternion.Euler(0, _modelPivot.rotation.eulerAngles.y, 0);
        }

        public void HandleJumping(bool jumpPressed, bool jumpButtonWasReleased, bool isGrounded)
        {
            if (jumpPressed)
            {
                if (!IsJumping && _hasButtonBeenReleased && isGrounded)
                {
                    IsJumping = true;
                    IsJumpForceActive = true;
                    _hasButtonBeenReleased = false;
                    _jumpHoldTime = 0f;
                    
                    float currentYVelocity = _rb.linearVelocity.y;
                    float velocityFactor = _jumpUpVelocityFactorFromExistingY.Evaluate(currentYVelocity / _jumpTerminalVelocity);
                    float initialJumpForce = _jumpUpVelocity * velocityFactor;
                    
                    _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, initialJumpForce, _rb.linearVelocity.z);
                }
                
            }
            
            if (jumpButtonWasReleased)
            {
                _hasButtonBeenReleased = true;
            }
            
            if (IsJumpForceActive && !jumpButtonWasReleased)
            {
                _jumpHoldTime += Time.fixedDeltaTime;
                
                if (_jumpHoldTime <= _jumpDuration)
                {
                    float normalizedTime = _jumpHoldTime / _jumpDuration;
                    float jumpForce = _analogJumpUpForce.Evaluate(normalizedTime);
                    _rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                }
                else
                {
                    IsJumpForceActive = false;
                    
                }
            } else if (IsJumpForceActive && jumpButtonWasReleased)
            {
                IsJumpForceActive = false;
            }
            
            if (IsJumping && (_rb.linearVelocity.y < 0.1f || jumpButtonWasReleased))
            {
                // TODO: Replace magic number with the setting scriptable object
                _rb.AddForce(Vector3.down * (_jumpFallFactor * 9.81f), ForceMode.Acceleration);
            }
        }
        
        public void CheckJumpReset(bool isGrounded)
        {
            // Only reset jumping state when landing on the ground after a jump
            if (isGrounded && IsJumping && !IsJumpForceActive && IsPlayerFalling)
            {
                IsJumping = false;
            }
        }
        
        
    }
} 