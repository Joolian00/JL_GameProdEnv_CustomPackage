using UnityEngine;
using Julian.RBController.ScriptableObjects;

namespace Julian.RBController.Components
{
    [System.Serializable]
    public class RbcShooting
    {
        private readonly Transform _shotOrigin;
        private readonly Transform _playerCamera;
        
        [Header("Shooting Settings")]
        [SerializeField] private float _shootForce;
        [SerializeField] private float _shootCooldown;
        [SerializeField] private GameObject _projectilePrefab;
        
        private float _lastShootTime;
        
        
        public RbcShooting(Transform shotOrigin, Transform playerCamera, ShootingSettings settings)
        {
            _shotOrigin = shotOrigin;
            _playerCamera = playerCamera;
            
            _shootForce = settings.ShootForce;
            _shootCooldown = settings.ShootCooldown;
            _projectilePrefab = settings.ProjectilePrefab;
            
            _lastShootTime = -_shootCooldown; // Allow immediate first shot
        }
        
        public void HandleShooting(bool shootPressed)
        {
            if (!shootPressed) return;
            if (Time.time - _lastShootTime < _shootCooldown) return;
            
            Shoot();
        }
        
        private void Shoot()
        {
            _lastShootTime = Time.time;
            
            // Use camera's forward direction for shooting
            Vector3 shootDirection = _playerCamera.forward;
            // Offset the shot position slightly forward to avoid collision with the player
            Vector3 shotPosition = _shotOrigin.position + shootDirection * 2f;
            
            // Instantiate projectile at shoot origin
            GameObject projectile = Object.Instantiate(
                _projectilePrefab,
                shotPosition,
                Quaternion.LookRotation(shootDirection)
            );
            
            // Add force to projectile
            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(shootDirection * _shootForce, ForceMode.Impulse);
            }
        }
    }
}