using UnityEngine;
namespace PatiAdventure
{
    public sealed class StarCollectible : MonoBehaviour
    {
        public ForestHUD hud;
        private Vector3 origin;
        private bool collected;
        private void Start() { origin = transform.position; }
        private void Update()
        { transform.Rotate(0, 70 * Time.deltaTime, 0, Space.World); transform.position = origin + Vector3.up * (Mathf.Sin(Time.time * 2) * .12f); }
        private void OnTriggerEnter(Collider other)
        {
            if (collected || !hud || !other.GetComponentInParent<FoxController>()) return;
            collected = true; hud.Collect(); gameObject.SetActive(false);
        }
    }
}
