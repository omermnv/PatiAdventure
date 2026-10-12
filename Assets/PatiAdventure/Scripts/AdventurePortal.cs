using UnityEngine;
namespace PatiAdventure
{
    public sealed class AdventurePortal : MonoBehaviour
    {
        public AdventureDirector director;
        public int destination;
        private void OnTriggerEnter(Collider other) { TryTravel(other); }
        // An early entry can be rejected by the travel cooldown. Retry while the
        // fox remains in the opening; TravelTo keeps the pause and cooldown guards.
        private void OnTriggerStay(Collider other) { TryTravel(other); }
        private void TryTravel(Collider other)
        {if(director&&other.GetComponentInParent<FoxController>()==director.fox)director.TravelTo(destination);}
    }
    public sealed class PortalBurst : MonoBehaviour
    {
        private float created;
        private Vector3 scale;
        private void Start(){created=Time.time;scale=transform.localScale;}
        private void Update(){float t=(Time.time-created)/.35f;transform.localScale=scale*(1+t*2);if(t>=1)Destroy(gameObject);}
    }
}
