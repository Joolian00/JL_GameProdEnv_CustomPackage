using JL_GameProdEnv_CustomPackage.Runtime.RBController.ScriptableObjects;

using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.Components
{
    [System.Serializable]
    public class RbcPhysics
    {
        private readonly Rigidbody _rb;
        
        [SerializeField] private float _gravityStrength;
        public bool IsGravityEnabled { get; set; } = true;
        
        public RbcPhysics(Rigidbody rb, PhysicsSettings settings)
        {
            _rb = rb;
            // Make sure the rigidbody is not affected by the default gravity
            _rb.useGravity = false;
            
            _gravityStrength = settings.GravityStrength;
        }

        public void ApplyGravity()
        {
            if (!IsGravityEnabled) return;
            
            _rb.AddForce(Vector3.down * _gravityStrength, ForceMode.Acceleration);
        }
    }
}