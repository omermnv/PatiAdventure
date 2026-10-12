using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace PatiAdventure.Tests
{
    public sealed class MovementTests
    {
        private readonly List<GameObject> objects=new List<GameObject>();
        private FoxController fox;
        private GameObject Cube(Vector3 position,Vector3 scale)
        { var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);objects.Add(obj);obj.transform.position=position;obj.transform.localScale=scale;return obj; }
        [UnitySetUp] public IEnumerator Setup()
        {
            Time.timeScale=1;Cube(new Vector3(0,-.5f,0),new Vector3(50,1,50));
            var obj=new GameObject("Test Fox");objects.Add(obj);obj.layer=2;obj.transform.position=new Vector3(0,.15f,0);
            var controller=obj.AddComponent<CharacterController>();controller.height=1.6f;controller.center=new Vector3(0,.8f,0);controller.radius=.35f;controller.stepOffset=.35f;
            fox=obj.AddComponent<FoxController>();fox.groundMask=~(1<<2);fox.readKeyboard=false;
            yield return new WaitForSeconds(.4f);
        }
        [UnityTearDown] public IEnumerator Cleanup()
        { Time.timeScale=1;foreach(var obj in objects)if(obj)Object.Destroy(obj);objects.Clear();yield return null; }
        [UnityTest] public IEnumerator AccelerationAndStoppingAreSmooth()
        {
            fox.TouchMove=Vector2.up;yield return null;
            Assert.Greater(fox.PlanarVelocity.magnitude,0);Assert.Less(fox.PlanarVelocity.magnitude,fox.walkSpeed);
            yield return new WaitForSeconds(.5f);Assert.AreEqual(fox.walkSpeed,fox.PlanarVelocity.magnitude,.15f);
            fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.5f);Assert.Less(fox.PlanarVelocity.magnitude,.05f);
        }
        [UnityTest] public IEnumerator DoubleJumpWorksButThirdJumpDoesNot()
        {
            Assert.IsTrue(fox.IsGrounded);fox.RequestJump();yield return null;Assert.AreEqual(1,fox.JumpCount);
            yield return new WaitForSeconds(.12f);fox.RequestJump();yield return null;Assert.AreEqual(2,fox.JumpCount);
            fox.RequestJump();yield return null;Assert.AreEqual(2,fox.JumpCount);Assert.AreEqual(2,fox.JumpsUsed);
        }
        [UnityTest] public IEnumerator SolidWallBlocksMovement()
        {
            Cube(new Vector3(0,1.5f,2),new Vector3(5,3,.5f));Physics.SyncTransforms();
            fox.TouchMove=Vector2.up;fox.TouchRun=true;yield return new WaitForSeconds(1);
            Assert.Greater(fox.transform.position.z,.5f);Assert.Less(fox.transform.position.z,1.6f);
        }
        [UnityTest] public IEnumerator FallingReturnsToCheckpoint()
        {
            var point=new Vector3(3,.2f,4);fox.SetCheckpoint(point);fox.GetComponent<CharacterController>().enabled=false;
            fox.transform.position=new Vector3(1,-10,1);fox.GetComponent<CharacterController>().enabled=true;yield return null;
            Assert.AreEqual(point.x,fox.transform.position.x,.05f);Assert.AreEqual(point.z,fox.transform.position.z,.05f);Assert.Greater(fox.transform.position.y,-1);
        }
        [UnityTest] public IEnumerator MovingPlatformCarriesStandingFox()
        {
            var platform=Cube(new Vector3(0,.5f,0),new Vector3(5,.5f,5));var motion=platform.AddComponent<MovingPlatform>();motion.travel=new Vector3(2,0,0);motion.period=4;
            fox.SetCheckpoint(new Vector3(0,.9f,0));fox.Respawn();yield return new WaitForSeconds(.5f);
            float start=fox.transform.position.x;yield return new WaitForSeconds(1);
            Assert.Greater(fox.transform.position.x-start,.35f);Assert.Greater(fox.transform.position.y,.6f);
        }
        [UnityTest] public IEnumerator CameraStaysInFrontOfBlockingWall()
        {
            Cube(new Vector3(0,2,-2),new Vector3(8,4,.4f));Physics.SyncTransforms();
            var obj=new GameObject("Test Camera");objects.Add(obj);obj.AddComponent<Camera>();var follow=obj.AddComponent<FollowCamera>();follow.target=fox.transform;follow.collisionMask=~(1<<2);
            yield return new WaitForSeconds(.3f);Assert.Greater(obj.transform.position.z,-1.7f);Assert.IsFalse(float.IsNaN(obj.transform.position.y));
        }
        private IEnumerator StairRoute(bool running)
        {
            for(int i=0;i<8;i++){float h=(i+1)*.25f;Cube(new Vector3(0,h*.5f,2+i*.75f),new Vector3(3,h,.8f));}
            Cube(new Vector3(0,1,8.5f),new Vector3(3,2,2));Physics.SyncTransforms();
            fox.SetCheckpoint(new Vector3(0,.15f,1));fox.Respawn();yield return new WaitForSeconds(.25f);
            fox.TouchRun=running;fox.TouchMove=Vector2.up;float limit=Time.time+5;
            while(fox.transform.position.z<8.1f&&Time.time<limit)yield return null;
            fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.3f);
            Assert.Greater(fox.transform.position.z,8.05f,"Pati stalled on the ascending stairs.");Assert.Greater(fox.transform.position.y,1.95f);
            fox.TouchMove=Vector2.down;limit=Time.time+5;int grounded=0,samples=0;
            while(fox.transform.position.z>1.2f&&Time.time<limit){if(fox.transform.position.z<7.5f&&fox.transform.position.z>2){samples++;if(fox.IsGrounded)grounded++;}yield return null;}
            fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.3f);
            Assert.Less(fox.transform.position.z,1.3f,"Pati stalled while descending.");Assert.Less(fox.transform.position.y,.2f);
            Assert.Greater(samples,5);Assert.Greater(grounded/(float)samples,.8f,"Descending should retain contact on normal steps.");
        }
        [UnityTest]public IEnumerator WalksUpAndDownQuarterMetreStairs(){yield return StairRoute(false);}
        [UnityTest]public IEnumerator RunsUpAndDownQuarterMetreStairs(){yield return StairRoute(true);}
        [UnityTest]public IEnumerator WalksUpSlopingGround()
        {
            var ramp=Cube(new Vector3(0,.7f,3),new Vector3(4,.3f,5));ramp.transform.rotation=Quaternion.Euler(-18,0,0);Physics.SyncTransforms();
            fox.TouchMove=Vector2.up;float limit=Time.time+4;while(fox.transform.position.z<4.7f&&Time.time<limit)yield return null;
            fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.3f);Assert.Greater(fox.transform.position.z,4.65f);Assert.Greater(fox.transform.position.y,1.2f);Assert.IsTrue(fox.IsGrounded);
        }
        [UnityTest]public IEnumerator CrossesStairsDiagonallyWithoutGettingCaught()
        {
            for(int i=0;i<8;i++){float h=(i+1)*.25f;Cube(new Vector3(0,h*.5f,2+i*.75f),new Vector3(3,h,.8f));}
            Cube(new Vector3(0,1,8.5f),new Vector3(3,2,2));Physics.SyncTransforms();fox.SetCheckpoint(new Vector3(-1,.15f,1));fox.Respawn();yield return new WaitForSeconds(.3f);
            var target=new Vector3(1,0,8.1f);float limit=Time.time+5;
            while(Time.time<limit){var delta=target-fox.transform.position;delta.y=0;if(delta.magnitude<.2f)break;fox.TouchMove=new Vector2(delta.x,delta.z).normalized;yield return null;}
            fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.3f);Assert.Greater(fox.transform.position.z,7.95f);Assert.Greater(fox.transform.position.y,1.95f);
        }
    }
}
