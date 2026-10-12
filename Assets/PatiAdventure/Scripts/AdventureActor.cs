using UnityEngine;
namespace PatiAdventure
{
    public sealed class AdventureActor : MonoBehaviour
    {
        public enum Role { Star, Gate, Checkpoint, Creature, Hazard, Friend, Beacon, Chest, Guide }
        public Role role;
        public AdventureDirector director;
        public int variant;
        public string actorId;
        private Vector3 home;
        private Vector3 localHome;
        private float cooldown;
        private bool collected;
        private void Start() { home = transform.position; localHome=transform.localPosition; }
        private void Update()
        {
            if (director == null || !director.IsPlaying) return;
            float phase = Time.time * (1.1f + variant * .13f) + home.z;
            if (role == Role.Star) { transform.Rotate(0, 65 * Time.deltaTime, 0);var bob=Vector3.up*Mathf.Sin(phase)*.15f;transform.localPosition=localHome+(transform.parent?transform.parent.InverseTransformVector(bob):bob); }
            if (role == Role.Creature)
            {
                if (variant % 4 == 3 && Vector3.Distance(director.fox.transform.position, home) < 6)
                {
                    Vector3 destination = director.fox.transform.position; destination.y = home.y;
                    transform.position = Vector3.MoveTowards(transform.position, destination, 1.2f * Time.deltaTime);
                }
                else if (variant % 3 == 0) transform.position = home + Vector3.right * Mathf.Sin(phase) * 2.3f;
                else if (variant % 3 == 1) transform.position = home + Vector3.up * Mathf.Abs(Mathf.Sin(phase)) * 1.5f;
                else transform.position = home + new Vector3(Mathf.Sin(phase) * 1.7f, .3f + Mathf.Cos(phase) * .3f, Mathf.Cos(phase) * .8f);
            }
        }
        private void OnTriggerEnter(Collider other) { Touch(other); }
        private void OnTriggerStay(Collider other) { if (role != Role.Star && role != Role.Checkpoint) Touch(other); }
        private void Touch(Collider other)
        {
            if (!director || !director.IsPlaying || !other.GetComponentInParent<FoxController>() || Time.time < cooldown) return;
            if(role==Role.Hazard&&actorId=="pond")
            {var point=director.fox.transform.position-transform.position;if(point.x*point.x/36+point.z*point.z/16>1)return;}
            cooldown = Time.time + 1.5f;
            if ((role == Role.Star || role == Role.Friend || role == Role.Beacon || role == Role.Chest || role == Role.Guide) && !collected)
            {
                if (!director.Interact(role,actorId)) return;
                collected = true;
                if(role==Role.Guide) return;
                if(role==Role.Beacon || role==Role.Chest)
                {
                    var glow=GetComponent<Renderer>(); if(glow) { var properties=new MaterialPropertyBlock(); properties.SetColor("_BaseColor",new Color(1,.9f,.2f)); glow.SetPropertyBlock(properties); }
                    var trigger=GetComponent<Collider>(); if(trigger)trigger.enabled=false;
                }
                else gameObject.SetActive(false);
            }
            else if (role == Role.Gate) director.TryFinish();
            else if (role == Role.Checkpoint) { director.ReachCheckpoint(transform.position + Vector3.up * .3f); }
            else if (role == Role.Creature || role == Role.Hazard) director.Bump();
        }
        public void RestoreCollected()
        {
            collected=true;
            if(role==Role.Beacon || role==Role.Chest)
            { var properties=new MaterialPropertyBlock(); properties.SetColor("_BaseColor",new Color(1,.9f,.2f)); var renderer=GetComponent<Renderer>(); if(renderer)renderer.SetPropertyBlock(properties); var trigger=GetComponent<Collider>(); if(trigger)trigger.enabled=false; }
            else if(role!=Role.Guide)gameObject.SetActive(false);
        }
    }
}
