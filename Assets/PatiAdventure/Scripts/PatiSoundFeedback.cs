using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class PatiSoundFeedback : MonoBehaviour
    {
        private AudioSource source;
        private AudioClip star,help,beacon;
        private void Awake()
        {
            source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.volume=.35f;
            star=Tone("Star chime",660);help=Tone("Friend chime",520);beacon=Tone("Beacon chime",880);
        }
        private AudioClip Tone(string name,float frequency)
        {
            const int sampleRate=22050,count=5512;var samples=new float[count];
            for(int i=0;i<count;i++) { float t=(float)i/sampleRate;float fade=1-(float)i/count;samples[i]=Mathf.Sin(t*frequency*Mathf.PI*2)*fade*fade*.25f; }
            var clip=AudioClip.Create(name,count,1,sampleRate,false);clip.SetData(samples,0);return clip;
        }
        public void Play(AdventureActor.Role role) { if(source)source.PlayOneShot(role==AdventureActor.Role.Star?star:role==AdventureActor.Role.Beacon?beacon:help); }
        private void OnDestroy() { if(star)Destroy(star);if(help)Destroy(help);if(beacon)Destroy(beacon); }
    }
}
