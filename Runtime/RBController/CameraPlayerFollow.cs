using UnityEngine;

namespace Julian.RBController
{
    public class CameraPlayerFollow : MonoBehaviour
    {
        [SerializeField] private Transform cameraPositionTarget;
        [SerializeField] private Transform cameraLookTarget;
    
        private void Update()
        {
            transform.position = cameraPositionTarget.position;
            transform.LookAt(cameraLookTarget);
        }
    }
}
