using UnityEngine;
using UnityEngine.InputSystem;
namespace PatiAdventure
{
    public sealed class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 7, followTime = .22f;
        public LayerMask collisionMask = ~0;
        public Vector2 TouchLook { get; set; }
        private float yaw, pitch = 28, lastLook, actualDistance = 7;
        private Vector3 focus, velocity;
        private readonly RaycastHit[] hits = new RaycastHit[32];
        private readonly Collider[] overlaps = new Collider[16];
        private bool initialized;
        private FoxController fox;
        public void Snap()
        {
            if (!target) return;
            focus = target.position + Vector3.up * 1.1f; velocity = Vector3.zero; initialized = true;
            fox = target.GetComponent<FoxController>();
            TouchLook = Vector2.zero;
            PlaceCamera(true);
        }
        private void LateUpdate()
        {
            // A drag received while paused must not rotate the camera on resume.
            if (!target || Time.timeScale <= 0 || Time.deltaTime <= 0) { TouchLook = Vector2.zero; return; }
            if (!initialized || Vector3.Distance(focus, target.position) > 12) Snap();
            Vector2 look = TouchLook; TouchLook = Vector2.zero;
            if (Mouse.current != null && Mouse.current.rightButton.isPressed) look += Mouse.current.delta.ReadValue();
            if (fox && fox.InputLocked) look = Vector2.zero;
            if (look.sqrMagnitude > .01f)
            { yaw += Mathf.Clamp(look.x, -80, 80) * .16f; pitch = Mathf.Clamp(pitch - look.y * .12f, 15, 50); lastLook = Time.time; }
            else if (fox && fox.PlanarVelocity.sqrMagnitude > .5f && Time.time - lastLook > 3)
                yaw = Mathf.MoveTowardsAngle(yaw, target.eulerAngles.y, 25 * Time.deltaTime);
            focus = Vector3.SmoothDamp(focus, target.position + Vector3.up * 1.1f, ref velocity, followTime);
            int overlapCount=Physics.OverlapSphereNonAlloc(focus,.2f,overlaps,collisionMask,QueryTriggerInteraction.Ignore);
            for(int i=0;i<overlapCount;i++)if(overlaps[i]&&!overlaps[i].transform.IsChildOf(target))
            {focus=target.position+Vector3.up*1.1f;velocity=Vector3.zero;break;}
            PlaceCamera(false);
        }
        private void PlaceCamera(bool snap)
        {
            Vector3 direction = Quaternion.Euler(pitch, yaw, 0) * Vector3.back;
            float allowed = distance;
            // Ignore all player colliders, not just the first hit.
            int count = Physics.SphereCastNonAlloc(focus, .25f, direction, hits, distance, collisionMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
                if (hits[i].transform && !hits[i].transform.IsChildOf(target)) allowed = Mathf.Min(allowed, Mathf.Max(.1f, hits[i].distance - .15f));
            // Pull inward immediately. Smoothing through an obstruction would clip.
            actualDistance = snap || allowed < actualDistance ? allowed : Mathf.MoveTowards(actualDistance, allowed, 3 * Time.deltaTime);
            transform.position = focus + direction * actualDistance; transform.LookAt(focus);
        }
    }
}
