using JL_GameProdEnv_CustomPackage.Runtime.RBController.ScriptableObjects;

using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController.Components
{
    [System.Serializable]
    public class RbcHeightSpring
    {
        private readonly Rigidbody _rb;
        
        [Header("Height Spring")]
        [SerializeField] private int _groundLayer;
        [SerializeField] private float _raycastDistance;
        [SerializeField] private float _rideHeight;
        [SerializeField] private float _springStrength;
        [SerializeField] private float _springDamping;

        [Header("Upright Torque Settings")] [SerializeField]
        private Quaternion _uprightJointTargetRot;
        [SerializeField] private float _uprightJointSpringStrength;
        [SerializeField] private float _uprightJointSpringDamper;

        private RbcGroundChecker _rbcGroundChecker;

        public RbcHeightSpring(Rigidbody rb, RbcGroundChecker groundChecker, HeightSpringSettings settings)
        {
            _rb = rb;
            
            _rbcGroundChecker = groundChecker;
            
            _groundLayer = settings.GroundLayer;
            _raycastDistance = settings.RaycastDistance;
            _rideHeight = settings.RideHeight;
            _springStrength = settings.SpringStrength;
            _springDamping = settings.SpringDamping;
            _uprightJointTargetRot = settings.UprightJointTargetRot;
            _uprightJointSpringStrength = settings.UprightJointSpringStrength;
            _uprightJointSpringDamper = settings.UprightJointSpringDamper;
        }

        public void HandleUprightTorque()
        {
            Quaternion characterCurrentRotation = _rb.rotation;
        
            Quaternion targetRotation = UtilsMath.ShortestRotation(_uprightJointTargetRot, characterCurrentRotation);

            Vector3 rotationAxis;
            float rotationDegrees;

            targetRotation.ToAngleAxis(out rotationDegrees, out rotationAxis);
        
            rotationAxis.Normalize();

            float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

            _rb.AddTorque((rotationAxis * (rotationRadians * -_uprightJointSpringStrength)) - (_rb.angularVelocity * _uprightJointSpringDamper));

        }
        

        public void HandleHeightSpring()
        {
            if (!_rbcGroundChecker.IsGrounded) return;
            
            Vector3 vel = _rb.linearVelocity;
            Vector3 rayDir = Vector3.down;

            Vector3 otherVel = Vector3.zero;
            
            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = _rbcGroundChecker.GroundCheckHit.distance - _rideHeight;
            
            float springForce = (x * _springStrength) - (relVel * _springDamping);
            
            
            _rb.AddForce(rayDir * springForce);
        }
    }
}