using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
namespace PatiAdventure
{
    [DefaultExecutionOrder(-100)]
    public sealed class MobileControls : MonoBehaviour
    {
        public FoxController fox;
        public FollowCamera followCamera;
        public AdventureDirector director;
        private RectTransform safeRoot;
        private CanvasGroup controls;
        private Font font;
        private Text runText, jumpText, biteText, spinText;
        private Rect lastSafe;
        private int lastWidth, lastHeight;
        private string lastLanguage;
        private bool wasLocked;
        private void Start()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (!EventSystem.current)
            {
                var events = new GameObject("Pati Event System"); events.transform.SetParent(transform);
                events.AddComponent<EventSystem>(); var module = events.AddComponent<InputSystemUIInputModule>(); module.AssignDefaultActions();
            }
            var canvasObject = new GameObject("Pati Touch Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.GetComponent<Canvas>().sortingOrder = 10;
            var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280,720); scaler.matchWidthOrHeight = .5f;
            safeRoot = new GameObject("Safe Area", typeof(RectTransform)).GetComponent<RectTransform>(); safeRoot.SetParent(canvasObject.transform,false); safeRoot.offsetMin = safeRoot.offsetMax = Vector2.zero;
            var group = new GameObject("Movement Controls", typeof(RectTransform), typeof(CanvasGroup)); group.transform.SetParent(safeRoot,false);
            var rect = group.GetComponent<RectTransform>(); rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.offsetMin=rect.offsetMax=Vector2.zero;
            controls = group.GetComponent<CanvasGroup>();
            var pad = Panel("Joystick", rect, new Vector2(0,0), new Vector2(135,135), new Vector2(185,185), new Color(.15f,.35f,.25f,.65f));
            var knob = Panel("Joystick Knob",pad.rectTransform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(75,75),new Color(.65f,.9f,.45f,.9f)); knob.raycastTarget=false;
            var joystick = pad.gameObject.AddComponent<PatiJoystick>(); joystick.fox=fox; joystick.knob=knob.rectTransform;
            var jump = Panel("Jump",rect,new Vector2(1,0),new Vector2(-105,105),new Vector2(140,115),new Color(.2f,.65f,.9f,.85f));
            var jumpButton = jump.gameObject.AddComponent<PatiHoldButton>(); jumpButton.fox=fox; jumpButton.jump=true; jumpText=Label(jump.rectTransform,"↑",32);
            var run = Panel("Run",rect,new Vector2(1,0),new Vector2(-265,95),new Vector2(125,100),new Color(.95f,.6f,.15f,.85f));
            var runButton=run.gameObject.AddComponent<PatiHoldButton>(); runButton.fox=fox; runText=Label(run.rectTransform,">>",30);
            var combat=fox.GetComponent<FoxCombat>();
            if(combat)
            {
                var bite=Panel("Bite",rect,new Vector2(1,0),new Vector2(-105,250),new Vector2(140,100),new Color(.77f,.32f,.16f,.9f));
                bite.gameObject.AddComponent<PatiAttackButton>().combat=combat;biteText=Label(bite.rectTransform,"",25);
                var spin=Panel("Tail Spin",rect,new Vector2(1,0),new Vector2(-270,235),new Vector2(140,100),new Color(.48f,.3f,.68f,.9f));
                var attack=spin.gameObject.AddComponent<PatiAttackButton>();attack.combat=combat;attack.spin=true;spinText=Label(spin.rectTransform,"",23);
            }
            var look=Panel("Camera Drag",rect,new Vector2(.5f,.5f),new Vector2(270,30),new Vector2(370,320),new Color(0,0,0,0));
            var drag=look.gameObject.AddComponent<PatiCameraDrag>(); drag.followCamera=followCamera;
            look.transform.SetAsFirstSibling();
        }
        private Image Panel(string name,RectTransform parent,Vector2 anchor,Vector2 position,Vector2 size,Color color)
        {
            var obj=new GameObject(name,typeof(RectTransform),typeof(Image)); var rect=obj.GetComponent<RectTransform>(); rect.SetParent(parent,false); rect.anchorMin=rect.anchorMax=anchor; rect.anchoredPosition=position; rect.sizeDelta=size;
            var image=obj.GetComponent<Image>(); image.color=color; return image;
        }
        private Text Label(RectTransform parent,string value,int size)
        {
            var obj=new GameObject("Label",typeof(RectTransform),typeof(Text)); var rect=obj.GetComponent<RectTransform>(); rect.SetParent(parent,false); rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.offsetMin=rect.offsetMax=Vector2.zero;
            var text=obj.GetComponent<Text>(); text.font=font; text.fontSize=size; text.alignment=TextAnchor.MiddleCenter; text.text=value; text.raycastTarget=false; return text;
        }
        private void Update()
        {
            if (!safeRoot || !fox) return;
            var safe=Screen.safeArea;
            if(safe!=lastSafe || lastWidth!=Screen.width || lastHeight!=Screen.height)
            { lastSafe=safe; lastWidth=Screen.width; lastHeight=Screen.height; safeRoot.anchorMin=safe.position/new Vector2(Screen.width,Screen.height); safeRoot.anchorMax=(safe.position+safe.size)/new Vector2(Screen.width,Screen.height); }
            if(fox.InputLocked!=wasLocked){wasLocked=fox.InputLocked;controls.gameObject.SetActive(!wasLocked);}
            controls.alpha=fox.InputLocked?0:1; controls.blocksRaycasts=controls.interactable=!fox.InputLocked;
            if(fox.InputLocked) { fox.TouchMove=Vector2.zero; fox.TouchRun=false; }
            string language=PatiLocalization.Language;
            if(language!=lastLanguage) { lastLanguage=language; runText.text=PatiLocalization.Pick(">> KOŞ",">> RUN"); jumpText.text=PatiLocalization.Pick("↑ ZIPLA","↑ JUMP");if(biteText)biteText.text=PatiLocalization.Pick("ISIR","BITE");if(spinText)spinText.text=PatiLocalization.Pick("KUYRUK","TAIL SPIN"); }
        }
    }
    public sealed class PatiAttackButton : MonoBehaviour, IPointerDownHandler
    {
        public FoxCombat combat;
        public bool spin;
        public void OnPointerDown(PointerEventData data){if(combat)combat.RequestAttack(spin);}
    }
    public sealed class PatiJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public FoxController fox;
        public RectTransform knob;
        private int pointer=int.MinValue;
        public void OnPointerDown(PointerEventData data) { if(pointer!=int.MinValue || !fox || fox.InputLocked) return; pointer=data.pointerId; OnDrag(data); }
        public void OnDrag(PointerEventData data)
        {
            if(data.pointerId!=pointer || !fox || fox.InputLocked) return;
            var rect=(RectTransform)transform;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,data.position,data.pressEventCamera,out var point))
            { fox.TouchMove=Vector2.ClampMagnitude(point/(rect.rect.width*.4f),1); if(knob)knob.anchoredPosition=fox.TouchMove*rect.rect.width*.3f; }
        }
        public void OnPointerUp(PointerEventData data) { if(data.pointerId==pointer) ResetPad(); }
        private void ResetPad() { pointer=int.MinValue; if(fox) fox.TouchMove=Vector2.zero; if(knob) knob.anchoredPosition=Vector2.zero; }
        private void OnDisable() { ResetPad(); }
        private void OnApplicationFocus(bool focused) { if(!focused)ResetPad(); }
        private void OnApplicationPause(bool paused) { if(paused)ResetPad(); }
    }
    public sealed class PatiHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public FoxController fox;
        public bool jump;
        private int pointer=int.MinValue;
        public void OnPointerDown(PointerEventData data)
        { if(pointer!=int.MinValue || !fox || fox.InputLocked)return; pointer=data.pointerId; if(jump)fox.RequestJump(); else fox.TouchRun=true; }
        public void OnPointerUp(PointerEventData data) { if(data.pointerId==pointer) { pointer=int.MinValue; if(fox&&!jump)fox.TouchRun=false; } }
        private void ResetButton() { pointer=int.MinValue; if(fox&&!jump)fox.TouchRun=false; }
        private void OnDisable() { ResetButton(); }
        private void OnApplicationFocus(bool focused) { if(!focused)ResetButton(); }
        private void OnApplicationPause(bool paused) { if(paused)ResetButton(); }
    }
    public sealed class PatiCameraDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public FollowCamera followCamera;
        private int pointer=int.MinValue;
        public void OnPointerDown(PointerEventData data) { if(pointer==int.MinValue)pointer=data.pointerId; }
        public void OnDrag(PointerEventData data)
        {
            if(data.pointerId!=pointer || !followCamera)return;
            var canvas=GetComponentInParent<Canvas>();
            float scale=canvas?Mathf.Max(.01f,canvas.scaleFactor):1;
            followCamera.TouchLook+=data.delta/scale;
        }
        public void OnPointerUp(PointerEventData data) { if(data.pointerId==pointer)pointer=int.MinValue; }
        private void ResetDrag() { pointer=int.MinValue; if(followCamera)followCamera.TouchLook=Vector2.zero; }
        private void OnDisable() { ResetDrag(); }
        private void OnApplicationFocus(bool focused) { if(!focused)ResetDrag(); }
        private void OnApplicationPause(bool paused) { if(paused)ResetDrag(); }
    }
}
