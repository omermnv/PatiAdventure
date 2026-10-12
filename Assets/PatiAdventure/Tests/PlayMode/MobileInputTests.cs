using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
namespace PatiAdventure.Tests
{
    public sealed class MobileInputTests
    {
        private readonly List<GameObject> objects=new List<GameObject>();
        private EventSystem events;
        private FoxController fox;
        private Canvas canvas;
        private PatiJoystick pad;
        private PatiHoldButton run,jump;
        private PatiCameraDrag drag;
        private FollowCamera camera;
        private GameObject Create(string name,params System.Type[] types)
        {var obj=new GameObject(name,types);objects.Add(obj);return obj;}
        [SetUp]public void Setup()
        {
            Time.timeScale=1;
            events=Create("Mobile test events",typeof(EventSystem)).GetComponent<EventSystem>();
            fox=Create("Mobile test fox",typeof(CharacterController)).AddComponent<FoxController>();fox.readKeyboard=false;fox.gameObject.layer=2;fox.groundMask=~(1<<2);
            canvas=Create("Mobile test canvas",typeof(RectTransform),typeof(Canvas)).GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var padObject=Create("Test pad",typeof(RectTransform));padObject.transform.SetParent(canvas.transform,false);
            var rect=(RectTransform)padObject.transform;rect.sizeDelta=new Vector2(200,200);
            pad=padObject.AddComponent<PatiJoystick>();pad.fox=fox;
            var knob=Create("Test knob",typeof(RectTransform));knob.transform.SetParent(pad.transform,false);pad.knob=(RectTransform)knob.transform;
            run=Create("Test run").AddComponent<PatiHoldButton>();run.fox=fox;
            jump=Create("Test jump").AddComponent<PatiHoldButton>();jump.fox=fox;jump.jump=true;
            camera=Create("Test touch camera").AddComponent<FollowCamera>();camera.enabled=false;
            var look=Create("Test camera drag",typeof(RectTransform));look.transform.SetParent(canvas.transform,false);
            drag=look.AddComponent<PatiCameraDrag>();drag.followCamera=camera;
            Canvas.ForceUpdateCanvases();
        }
        [UnityTearDown]public IEnumerator Cleanup()
        {Time.timeScale=1;foreach(var obj in objects)if(obj)Object.Destroy(obj);objects.Clear();yield return null;}
        private PointerEventData Pointer(int id,Vector2 local)
        {
            return new PointerEventData(events){pointerId=id,position=RectTransformUtility.WorldToScreenPoint(null,pad.transform.TransformPoint(local)),delta=new Vector2(40,-20)};
        }
        [Test]public void SecondFingerCannotStealOrReleaseJoystick()
        {
            var first=Pointer(11,new Vector2(80,0));var second=Pointer(12,new Vector2(0,80));
            pad.OnPointerDown(first);Assert.Greater(fox.TouchMove.x,.95f);
            pad.OnPointerDown(second);pad.OnDrag(second);pad.OnPointerUp(second);Assert.Greater(fox.TouchMove.x,.95f);Assert.AreEqual(0,fox.TouchMove.y,.001f);
            pad.OnPointerUp(first);Assert.AreEqual(Vector2.zero,fox.TouchMove);Assert.AreEqual(Vector2.zero,pad.knob.anchoredPosition);
        }
        [Test]public void LockedTouchDoesNotKeepOwnershipAfterUnlock()
        {
            fox.InputLocked=true;pad.OnPointerDown(Pointer(1,new Vector2(80,0)));run.OnPointerDown(Pointer(2,Vector2.zero));
            fox.InputLocked=false;pad.OnPointerDown(Pointer(3,new Vector2(0,80)));run.OnPointerDown(Pointer(4,Vector2.zero));
            Assert.Greater(fox.TouchMove.y,.95f);Assert.IsTrue(fox.TouchRun);
        }
        [TestCase("OnApplicationFocus",false)]
        [TestCase("OnApplicationPause",true)]
        public void LosingApplicationInputReleasesAndAcceptsNewFingers(string message,bool value)
        {
            pad.OnPointerDown(Pointer(1,new Vector2(80,0)));run.OnPointerDown(Pointer(2,Vector2.zero));drag.OnPointerDown(Pointer(3,Vector2.zero));drag.OnDrag(Pointer(3,Vector2.zero));
            pad.SendMessage(message,value);run.SendMessage(message,value);drag.SendMessage(message,value);
            Assert.AreEqual(Vector2.zero,fox.TouchMove);Assert.IsFalse(fox.TouchRun);Assert.AreEqual(Vector2.zero,camera.TouchLook);
            pad.OnPointerDown(Pointer(4,new Vector2(0,80)));run.OnPointerDown(Pointer(5,Vector2.zero));drag.OnPointerDown(Pointer(6,Vector2.zero));drag.OnDrag(Pointer(6,Vector2.zero));
            Assert.Greater(fox.TouchMove.y,.95f);Assert.IsTrue(fox.TouchRun);Assert.Greater(camera.TouchLook.x,0);
        }
        [Test]public void CameraDragUsesCanvasScaleAndKeepsItsOwnFinger()
        {
            canvas.scaleFactor=2;drag.OnPointerDown(Pointer(1,Vector2.zero));drag.OnDrag(Pointer(1,Vector2.zero));
            Assert.AreEqual(new Vector2(20,-10),camera.TouchLook);
            drag.OnPointerUp(Pointer(2,Vector2.zero));drag.OnDrag(Pointer(2,Vector2.zero));Assert.AreEqual(new Vector2(20,-10),camera.TouchLook);
            drag.OnDrag(Pointer(1,Vector2.zero));Assert.AreEqual(new Vector2(40,-20),camera.TouchLook);
        }
        [UnityTest]public IEnumerator JoystickRunAndJumpWorkTogetherWithoutReleasingEachOther()
        {
            var floor=GameObject.CreatePrimitive(PrimitiveType.Cube);objects.Add(floor);floor.transform.position=new Vector3(0,-.5f,0);floor.transform.localScale=new Vector3(20,1,20);
            var controller=fox.GetComponent<CharacterController>();controller.height=1.6f;controller.center=new Vector3(0,.8f,0);controller.radius=.35f;
            fox.SetCheckpoint(new Vector3(0,.1f,0));fox.Respawn();yield return new WaitForSeconds(.3f);Assert.IsTrue(fox.IsGrounded);
            pad.OnPointerDown(Pointer(11,new Vector2(0,80)));run.OnPointerDown(Pointer(12,Vector2.zero));jump.OnPointerDown(Pointer(13,Vector2.zero));
            yield return null;Assert.AreEqual(1,fox.JumpCount);Assert.IsTrue(fox.TouchRun);Assert.Greater(fox.TouchMove.y,.95f);
            jump.OnPointerUp(Pointer(13,Vector2.zero));run.OnPointerUp(Pointer(11,Vector2.zero));Assert.IsTrue(fox.TouchRun);
            run.OnPointerUp(Pointer(12,Vector2.zero));Assert.IsFalse(fox.TouchRun);Assert.Greater(fox.TouchMove.y,.95f);
            pad.OnPointerUp(Pointer(11,Vector2.zero));Assert.AreEqual(Vector2.zero,fox.TouchMove);
        }
    }
}
