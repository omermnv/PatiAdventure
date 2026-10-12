using UnityEngine;
namespace PatiAdventure
{
    public sealed class FoliageSway : MonoBehaviour
    {
        public float phase;
        private void Update(){transform.localRotation=Quaternion.Euler(Mathf.Sin(Time.time*.7f+phase)*1.1f,0,Mathf.Sin(Time.time*.85f+phase)*1.4f);}
    }
}
