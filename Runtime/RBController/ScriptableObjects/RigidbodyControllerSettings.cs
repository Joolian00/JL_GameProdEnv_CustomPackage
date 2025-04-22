using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RigidbodyControllerSettings", menuName = "RBController/Settings", order = 1)]
    public class RigidbodyControllerSettings : ScriptableObject
    {
        [Header("Physics")]
        [SerializeField] private PhysicsSettings physics = new();
        
        [Header("Ground Checker")]
        [SerializeField] private GroundCheckerSettings groundChecker = new();
        
        [Header("Height Spring")]
        [SerializeField] private HeightSpringSettings heightSpring = new();
        
        [Header("Movement")]
        [SerializeField] private MovementSettings movement = new();
        
        [Header("Camera")]
        [SerializeField] private CameraSettings camera = new();
        
        [Header("Shooting")]
        [SerializeField] private ShootingSettings shooting = new();
        
        // Public properties for accessing the settings sections
        public PhysicsSettings Physics => physics;
        public GroundCheckerSettings GroundChecker => groundChecker;
        public HeightSpringSettings HeightSpring => heightSpring;
        public MovementSettings Movement => movement;
        public CameraSettings Camera => camera;
        public ShootingSettings Shooting => shooting;
    }
}