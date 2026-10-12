using UnityEngine;
using UnityEngine.InputSystem;
namespace PatiAdventure
{
    public sealed class ForestHUD : MonoBehaviour
    {
        public FoxController fox;
        public bool controlsOnly;
        private int stars;
        private Rect pad, run, jump;
        private bool jumpHeld;
        public void Collect() { stars = Mathf.Min(5, stars + 1); }
        private Vector2 GuiPoint(Vector2 screen) => new Vector2(screen.x, Screen.height - screen.y);
        private void ReadPointer(Vector2 screen, ref bool running, ref bool jumping)
        {
            Vector2 point = GuiPoint(screen);
            if (pad.Contains(point)) fox.TouchMove = Vector2.ClampMagnitude(new Vector2((point.x - pad.center.x) / (pad.width * .4f), (pad.center.y - point.y) / (pad.height * .4f)), 1);
            running |= run.Contains(point); jumping |= jump.Contains(point);
        }
        private void Update()
        {
            if(!fox)return;
            float size = Mathf.Min(Screen.width * .22f, Screen.height * .32f);
            Rect safe = Screen.safeArea;
            float bottom = Screen.height - safe.yMin - 15;
            pad = new Rect(safe.xMin + 15, bottom - size, size, size);
            jump = new Rect(safe.xMax - size - 15, bottom - size * .5f, size, size * .5f);
            run = new Rect(safe.xMax - size - 15, bottom - size - 10, size, size * .45f);
            fox.TouchMove = Vector2.zero;
            bool running = false, jumping = false;
            var touch = Touchscreen.current;
            bool anyTouch = false;
            if (touch != null) foreach (var finger in touch.touches)
                if (finger.press.isPressed) { anyTouch = true; ReadPointer(finger.position.ReadValue(), ref running, ref jumping); }
            if (!anyTouch && Mouse.current != null && Mouse.current.leftButton.isPressed) ReadPointer(Mouse.current.position.ReadValue(), ref running, ref jumping);
            fox.TouchRun = running;
            if (jumping && !jumpHeld) fox.RequestJump();
            jumpHeld = jumping;
        }
        private void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.box) { fontSize = Mathf.Clamp(Screen.height / 30, 18, 36), alignment = TextAnchor.MiddleCenter };
            if (!fox || fox.InputLocked) return;
            if (!controlsOnly) GUI.Box(new Rect(15, 15, Mathf.Min(650, Screen.width - 30), 65), stars == 5 ? "Harika! 5 yıldızı da topladın!" : "SunnyForest  |  Yıldız: " + stars + " / 5", style);
            GUI.Box(pad, "↑\n←   →\n↓", style);
            bool english = PlayerPrefs.GetString("Pati.Language", "tr") == "en";
            GUI.Box(run, english ? "RUN" : "KOŞ", style); GUI.Box(jump, english ? "JUMP" : "ZIPLA", style);
        }
        private void OnDisable() { if (fox) { fox.TouchMove = Vector2.zero; fox.TouchRun = false; } }
    }
}
