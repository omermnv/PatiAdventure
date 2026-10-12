using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(FoxController))]
    public sealed class FoxVisualFeedback : MonoBehaviour
    {
        public Transform visualRoot;
        private FoxController fox;
        private Vector3 target = Vector3.one;
        private float resetAt;
        private void Awake() { fox = GetComponent<FoxController>(); }
        private void OnEnable() { fox.Jumped += Jump; fox.Landed += Land; fox.Respawned += ResetPose; }
        private void OnDisable() { fox.Jumped -= Jump; fox.Landed -= Land; fox.Respawned -= ResetPose; }
        private void Jump() { target = visualRoot&&visualRoot.Find("Fox Rig")?new Vector3(.98f,1.025f,.98f):new Vector3(.9f,1.15f,.9f);resetAt=Time.time+.16f; }
        private void Land() { target = visualRoot&&visualRoot.Find("Fox Rig")?new Vector3(1.025f,.97f,1.025f):new Vector3(1.15f,.8f,1.15f);resetAt=Time.time+.15f; }
        private void ResetPose() { target = Vector3.one; }
        private void LateUpdate()
        {
            if (!visualRoot) visualRoot = transform.Find("Fox Visual Pose");
            if (!visualRoot) return;
            if (Time.time >= resetAt) target = Vector3.one;
            visualRoot.localScale = Vector3.Lerp(visualRoot.localScale, target, 14 * Time.deltaTime);
        }
    }
}
