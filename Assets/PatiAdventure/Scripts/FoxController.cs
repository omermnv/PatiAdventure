using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace PatiAdventure
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FoxController : MonoBehaviour
    {
        public Transform cameraTransform;
        [Min(.1f)] public float walkSpeed = 3f, runSpeed = 5f, jumpHeight = 1.6f;
        [Min(.1f)] public float acceleration = 14f, deceleration = 18f, gravity = 20f;
        [Range(0, 1)] public float airControl = .65f;
        public LayerMask groundMask = ~0;
        public bool readKeyboard = true;
        [Range(0,.4f)] public float groundSnapDistance=.32f;
        public Vector2 TouchMove { get; set; }
        public bool TouchRun { get; set; }
        public bool InputLocked { get; set; }
        public float WalkedDistance { get; private set; }
        public float RanDistance { get; private set; }
        public int JumpCount { get; private set; }
        public int JumpsUsed => jumpsUsed;
        public bool IsGrounded { get; private set; }
        public Vector3 PlanarVelocity => planarVelocity;
        public event Action Jumped;
        public event Action Landed;
        public event Action Respawned;
        private CharacterController controller;
        private Vector3 checkpoint, planarVelocity;
        private float vertical, groundedUntil = -1, jumpUntil = -1;
        private int jumpsUsed;
        private Transform support;
        private Vector3 supportLocalPoint, supportWorldPoint;
        private void Awake() { controller = GetComponent<CharacterController>(); checkpoint = transform.position; }
        public void SetCheckpoint(Vector3 point) { checkpoint = point; }
        public void RequestJump() { if (!InputLocked) jumpUntil = Time.time + .15f; }
        public void Respawn()
        {
            if (!controller) controller = GetComponent<CharacterController>();
            controller.enabled = false; transform.position = checkpoint; controller.enabled = true;
            vertical = 0; planarVelocity = Vector3.zero; groundedUntil = jumpUntil = -1;
            TouchMove = Vector2.zero; TouchRun = false; jumpsUsed = 0; support = null;
            IsGrounded=false;
            Respawned?.Invoke();
        }
        private void Update()
        {
            if (InputLocked || Time.deltaTime <= 0) { jumpUntil = -1; return; }
            bool wasGrounded = IsGrounded;
            IsGrounded = controller.isGrounded && vertical <= 0;
            if (IsGrounded)
            {
                groundedUntil = Time.time + .12f; vertical = -2; jumpsUsed = 0;
            }
            if (support && IsGrounded) controller.Move(support.TransformPoint(supportLocalPoint) - supportWorldPoint);
            Vector2 input = TouchMove; bool running = TouchRun;
            var keys = readKeyboard ? Keyboard.current : null;
            if (keys != null)
            {
                input += new Vector2((keys.dKey.isPressed || keys.rightArrowKey.isPressed ? 1 : 0) - (keys.aKey.isPressed || keys.leftArrowKey.isPressed ? 1 : 0),
                    (keys.wKey.isPressed || keys.upArrowKey.isPressed ? 1 : 0) - (keys.sKey.isPressed || keys.downArrowKey.isPressed ? 1 : 0));
                running |= keys.leftShiftKey.isPressed || keys.rightShiftKey.isPressed;
                if (keys.spaceKey.wasPressedThisFrame) RequestJump();
            }
            if (jumpUntil >= Time.time && jumpsUsed < 2)
            {
                if (!IsGrounded && groundedUntil < Time.time && jumpsUsed == 0) jumpsUsed = 1;
                jumpsUsed++; JumpCount++;
                vertical = Mathf.Sqrt(2 * gravity * jumpHeight) * (jumpsUsed == 2 ? .9f : 1);
                jumpUntil = groundedUntil = -1; IsGrounded = false; support = null; Jumped?.Invoke();
            }
            Vector3 forward = cameraTransform ? cameraTransform.forward : Vector3.forward;
            forward.y = 0; if (forward.sqrMagnitude < .001f) forward = Vector3.forward; forward.Normalize();
            Vector3 direction = Vector3.ClampMagnitude(forward * input.y + Vector3.Cross(Vector3.up, forward) * input.x, 1);
            Vector3 desired = direction * (running ? runSpeed : walkSpeed);
            float rate = direction.sqrMagnitude > .001f ? acceleration : deceleration;
            planarVelocity = Vector3.MoveTowards(planarVelocity, desired, rate * (IsGrounded ? 1 : airControl) * Time.deltaTime);
            if (direction.sqrMagnitude > .01f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 480 * Time.deltaTime);
            vertical = Mathf.Max(vertical - gravity * Time.deltaTime, -25);
            Vector3 before = transform.position;
            CollisionFlags collisions = controller.Move((planarVelocity + Vector3.up * vertical) * Time.deltaTime);
            if ((collisions & CollisionFlags.Above) != 0 && vertical > 0) vertical = 0;
            // Follow small downward steps without a falling frame or snapping a jump to the floor.
            if(vertical<=0&&!controller.isGrounded&&(wasGrounded||groundedUntil>=Time.time))
            {
                float radius=controller.radius*.85f;
                var foot=transform.TransformPoint(controller.center)-Vector3.up*(controller.height*.5f);
                if(Physics.SphereCast(foot+Vector3.up*(radius+.06f),radius,Vector3.down,out var landing,.06f+groundSnapDistance,groundMask,QueryTriggerInteraction.Ignore)
                    &&landing.normal.y>=Mathf.Cos(controller.slopeLimit*Mathf.Deg2Rad))
                    controller.Move(Vector3.down*Mathf.Max(0,landing.distance-.06f+.025f));
            }
            IsGrounded=controller.isGrounded&&vertical<=0;
            if(IsGrounded){groundedUntil=Time.time+.12f;jumpsUsed=0;vertical=-2;if(!wasGrounded)Landed?.Invoke();}
            Vector3 traveled = transform.position - before; traveled.y = 0;
            if (running) RanDistance += traveled.magnitude; else WalkedDistance += traveled.magnitude;
            if (controller.isGrounded && vertical <= 0 && Physics.Raycast(transform.position + Vector3.up * .3f, Vector3.down, out var hit, .65f, groundMask, QueryTriggerInteraction.Ignore))
            {
                support = hit.transform; supportLocalPoint = support.InverseTransformPoint(transform.position); supportWorldPoint = transform.position;
            }
            else support = null;
            if (transform.position.y < -8) Respawn();
        }
    }
}
