using UnityEngine;

namespace Julian.RBController.ScriptableObjects
{
    [System.Serializable]
    public class PhysicsSettings
    {
        [Header("Gravity")]
        [SerializeField] private float gravityStrength = 9.81f;
        
        public float GravityStrength => gravityStrength;
    }

    [System.Serializable]
    public class MovementSettings
    {
        [Header("Locomotion")]
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float maxAccelForce = 150f;
        [SerializeField] private float frictionCoefficient = 10f;
        [SerializeField] private float staticFrictionThreshold = 0.1f;
        
        public float MaxSpeed => maxSpeed;
        public float MaxAccelForce => maxAccelForce;
        public float FrictionCoefficient => frictionCoefficient;
        public float StaticFrictionThreshold => staticFrictionThreshold;
        
        [Header("Jumping")]
        [SerializeField] private float jumpUpVelocity = 9f;
        [SerializeField] private AnimationCurve jumpUpVelocityFactorFromExistingY = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private AnimationCurve analogJumpUpForce  = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private float jumpTerminalVelocity = 22.5f;
        [SerializeField] private float jumpDuration = 0.6667f;
        [SerializeField] private float jumpFallFactor = 1.25f;
        
        public float JumpUpVelocity => jumpUpVelocity;
        public AnimationCurve JumpUpVelocityFactorFromExistingY => jumpUpVelocityFactorFromExistingY;
        public AnimationCurve AnalogJumpUpForce => analogJumpUpForce;
        public float JumpTerminalVelocity => jumpTerminalVelocity;
        public float JumpDuration => jumpDuration;
        public float JumpFallFactor => jumpFallFactor;


    }
    
    [System.Serializable]
    public class HeightSpringSettings
    {
        [Header("Height Spring")]
        [SerializeField] private LayerMask groundLayer = 1 << 0;
        [SerializeField] private float raycastDistance = 3f;
        [SerializeField] private float rideHeight = 2f;
        [SerializeField] private float springStrength = 50f;
        [SerializeField] private float springDamping = 5f;
        
        [Header("Upright Torque Settings")]
        [SerializeField] private Quaternion uprightJointTargetRot = Quaternion.identity;
        [SerializeField] private float uprightJointSpringStrength = 40f;
        [SerializeField] private float uprightJointSpringDamper = 5f;

        // Public properties
        public int GroundLayer => groundLayer;
        public float RaycastDistance => raycastDistance;
        public float RideHeight => rideHeight;
        public float SpringStrength => springStrength;
        public float SpringDamping => springDamping;
        public Quaternion UprightJointTargetRot => uprightJointTargetRot;
        public float UprightJointSpringStrength => uprightJointSpringStrength;
        public float UprightJointSpringDamper => uprightJointSpringDamper;
    }
    
    [System.Serializable]
    public class GroundCheckerSettings
    {
        [Header("Ground Checker")]
        [SerializeField] private float groundCheckRadiusMultiplier = 1.5f;
        [SerializeField] private float groundCheckDistanceTolerance = 0.1f;
        [SerializeField] private LayerMask groundLayer = 1 << 0;
        [SerializeField] private float maxGroundCheckDistance = 3f;
        
        // Public properties
        public float GroundCheckRadiusMultiplier => groundCheckRadiusMultiplier;
        public float GroundCheckDistanceTolerance => groundCheckDistanceTolerance;
        public LayerMask GroundLayer => groundLayer;
        public float MaxGroundCheckDistance => maxGroundCheckDistance;
    }
    
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Target Offsets in Local Space")]
        [SerializeField] private Vector3 lookTargetOffset = new(2.22f, 0.6f, 0);
        [SerializeField] private Vector3 positionTargetOffset = new(2f, 1.25f, -3f);
        
        public Vector3 LookTargetOffset => lookTargetOffset;
        public Vector3 PositionTargetOffset => positionTargetOffset;
        
        [Header("Camera Smoothing")]
        [SerializeField] private bool useSmoothing = true;
        [SerializeField] private float smoothingTime = 0.05f;
        [SerializeField] private float maxSmoothSpeed = 20f;
        
        public bool UseSmoothing => useSmoothing;
        public float SmoothingTime => smoothingTime;
        public float MaxSmoothSpeed => maxSmoothSpeed;
        
        [Header("Camera Rotation")]
        [SerializeField] private float horizontalSensitivity = 2f;
        [SerializeField] private float verticalSensitivity = 1f;
        
        public float HorizontalSensitivity => horizontalSensitivity;
        public float VerticalSensitivity => verticalSensitivity;
        
        
    }
    
    [System.Serializable]
    public class ShootingSettings
    {
        [Header("Shooting")]
        [SerializeField] private float shootForce = 30f;
        [SerializeField] private float shootCooldown = 0.5f;
        [SerializeField] private GameObject projectilePrefab;
        
        public float ShootForce => shootForce;
        public float ShootCooldown => shootCooldown;
        public GameObject ProjectilePrefab => projectilePrefab;
    }
}
