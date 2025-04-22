using UnityEngine;

namespace JL_GameProdEnv_CustomPackage.Runtime.RBController
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
