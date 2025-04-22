using Julian.RBController.ScriptableObjects;
using UnityEngine;

namespace Julian.RBController.Components
{
    [System.Serializable]
    public class RbcGroundChecker
    {
        private readonly Rigidbody _rb;
        private readonly CapsuleCollider _capsuleCollider;

        [SerializeField] private float _groundCheckRadiusMultiplier;
        [SerializeField] private float _groundCheckDistanceTolerance;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _maxGroundCheckDistance = 3f; // Maximum distance to check for ground

        public float SphereCastRadius => _capsuleCollider.radius * _groundCheckRadiusMultiplier;

        private RaycastHit _groundCheckHit;
        public RaycastHit GroundCheckHit => _groundCheckHit;
        public float MaxGroundCheckDistance => _maxGroundCheckDistance;
        private float _distanceToGround;

        public bool IsGrounded => PlayerGroundCheck();
        public float DistanceToGround => _distanceToGround;
        public Vector3 LastGroundHitPoint => _groundCheckHit.point;

        public RbcGroundChecker(Rigidbody rb, GroundCheckerSettings settings)
        {
            _rb = rb;
            _capsuleCollider = rb.GetComponentInChildren<CapsuleCollider>();

            _groundCheckRadiusMultiplier = settings.GroundCheckRadiusMultiplier;
            _groundCheckDistanceTolerance = settings.GroundCheckDistanceTolerance;
            _groundLayer = settings.GroundLayer;
            _maxGroundCheckDistance = settings.MaxGroundCheckDistance;
        }

        public bool PlayerGroundCheck()
        {
            Vector3 playerCenterPoint = _rb.transform.position;
            float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
            
            bool hitGround = Physics.SphereCast(
                playerCenterPoint,
                sphereCastRadius,
                Vector3.down,
                out _groundCheckHit,
                _maxGroundCheckDistance,
                _groundLayer
            );

            if (hitGround)
            {
                _distanceToGround = _groundCheckHit.distance;
                
                // For debugging
                // Debug.Log($"Distance to ground: {_distanceToGround}");
                
                // Consider grounded if we're within the max check distance
                return _distanceToGround <= _maxGroundCheckDistance;
            }
            
            _distanceToGround = _maxGroundCheckDistance;
            return false;
        }
    }
}