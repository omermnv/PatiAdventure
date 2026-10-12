using UnityEngine;
namespace PatiAdventure
{
    public sealed class PortalLabel : MonoBehaviour
    {
        public Transform cameraTransform;
        private void LateUpdate(){if(cameraTransform)transform.rotation=cameraTransform.rotation;}
    }
}
