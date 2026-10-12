using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class StoryNarrator : MonoBehaviour
    {
        public string Status { get; private set; } = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject speech;
        private volatile bool initialized;
        private volatile bool failed;
        private volatile bool disposed;
        private string pendingText, pendingLanguage;
        private sealed class InitListener : AndroidJavaProxy
        {
            private readonly StoryNarrator owner;
            public InitListener(StoryNarrator owner) : base("android.speech.tts.TextToSpeech$OnInitListener") { this.owner = owner; }
            public void onInit(int status) { if(owner.disposed)return;owner.initialized = status == 0; owner.failed = status != 0; }
        }
        private void Awake()
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            { var activity = player.GetStatic<AndroidJavaObject>("currentActivity"); activity.Call("runOnUiThread", new AndroidJavaRunnable(() => { if(!disposed)speech = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, new InitListener(this));activity.Dispose(); })); }
        }
        private void Update()
        {
            if (failed) { Status = pendingLanguage == "tr" ? "Cihaz seslendirmesi açılamadı; anlatım metni ekranda." : "Device speech unavailable; story text is on screen."; pendingText = null; }
            if (!initialized || speech == null || pendingText == null) return;
            string text = pendingText; pendingText = null;
            using (var locale = new AndroidJavaObject("java.util.Locale", pendingLanguage))
            {
                int result = speech.Call<int>("setLanguage", locale);
                if (result < 0) { Status = pendingLanguage == "tr" ? "Türkçe ses paketi cihazda bulunamadı." : "English voice data is missing on this device."; return; }
            }
            using (var parameters = new AndroidJavaObject("android.os.Bundle")) speech.Call<int>("speak", text, 0, parameters, "PatiStory"); Status = "";
        }
#endif
        public void Speak(string language, string key, string text)
        {
            Stop();
            var clip = Resources.Load<AudioClip>("Narration/" + language + "/" + key);
            if (clip) { GetComponent<AudioSource>().clip = clip; GetComponent<AudioSource>().Play(); Status = ""; return; }
#if UNITY_ANDROID && !UNITY_EDITOR
            pendingLanguage = language; pendingText = text;
            Status = language == "tr" ? "Ses hazırlanıyor…" : "Preparing narration…";
#else
            Status = language == "tr" ? "Bu dilin ses kaydı henüz eklenmedi. Android'de cihaz sesi kullanılır." : "Voice recording unavailable. Android uses device speech.";
#endif
        }
        public void Stop()
        {
            GetComponent<AudioSource>().Stop();
#if UNITY_ANDROID && !UNITY_EDITOR
            pendingText = null; if (speech != null && initialized) speech.Call<int>("stop");
#endif
        }
        private void OnDestroy()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            disposed=true;
            if (speech != null) { speech.Call("shutdown"); speech.Dispose(); }
#endif
        }
    }
}
