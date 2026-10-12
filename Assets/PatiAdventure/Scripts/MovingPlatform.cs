using UnityEngine;
namespace PatiAdventure
{
    [DefaultExecutionOrder(-80)]
    public sealed class MovingPlatform : MonoBehaviour
    {
        public Vector3 travel = new Vector3(2, 0, 0);
        [Min(.1f)] public float period = 5;
        private Vector3 origin;
        private float elapsed;
        private void Awake() { origin = transform.position; }
        private void Update()
        { elapsed += Time.deltaTime; transform.position = origin + travel * (.5f - .5f * Mathf.Cos(elapsed * Mathf.PI * 2 / period)); }
    }
}
