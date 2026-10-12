#if UNITY_EDITOR
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
namespace PatiAdventure.Tests
{
    public sealed class PortalAdventureTests
    {
        private Scene scene;
        private AdventureDirector director;
        private string previous,language;
        private bool hadSave,entered;
        [UnitySetUp]public IEnumerator Load()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Generate the portal scene in the isolated validation project first.");
            entered=true;hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);previous=PlayerPrefs.GetString(ExplorationProgress.Key);language=PlayerPrefs.GetString("Pati.Language","tr");PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));yield return null;yield return null;
            foreach(var root in scene.GetRootGameObjects()){director=root.GetComponentInChildren<AdventureDirector>();if(director)break;}
            Assert.NotNull(director);director.ChooseLanguage("en");director.StartAdventure();director.fox.readKeyboard=false;yield return null;
        }
        [UnityTearDown]public IEnumerator Unload()
        {
            if(!entered)yield break;Time.timeScale=1;if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,previous);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);PlayerPrefs.SetString("Pati.Language",language);PatiLocalization.SetLanguage(language);PlayerPrefs.Save();
        }
        private PortalEnemy Enemy(){foreach(var root in scene.GetRootGameObjects()){var enemy=root.GetComponentInChildren<PortalEnemy>();if(enemy)return enemy;}return null;}
        private IEnumerator WalkTo(Vector3 target,bool running=false)
        {
            var fox=director.fox;fox.TouchRun=running;float limit=Time.time+10;
            while(Time.time<limit)
            {
                var delta=target-fox.transform.position;delta.y=0;if(delta.magnitude<.18f)break;
                fox.TouchMove=new Vector2(delta.x,delta.z).normalized*Mathf.Clamp01(delta.magnitude*2);yield return null;
            }
            fox.TouchMove=Vector2.zero;fox.TouchRun=false;yield return new WaitForSeconds(.25f);
            var error=fox.transform.position-target;error.y=0;Assert.Less(error.magnitude,.45f,"Route blocked near "+target+"; actual "+fox.transform.position);
        }
        private void FightNearby()
        {
            PortalEnemy nearest=null;float distance=2.5f;
            foreach(var enemy in director.GetComponentsInChildren<PortalEnemy>())if(!enemy.IsDefeated)
            {var delta=enemy.transform.position-director.fox.transform.position;delta.y=0;if(delta.magnitude<distance){nearest=enemy;distance=delta.magnitude;}}
            if(nearest){var delta=nearest.transform.position-director.fox.transform.position;delta.y=0;director.fox.GetComponent<FoxCombat>().RequestAttack(Vector3.Dot(director.fox.transform.forward,delta.normalized)<.25f);}
        }
        private IEnumerator ExploreTowards(System.Func<Vector3> destination,System.Func<bool> complete,bool jumpForPickup=false)
        {
            var fox=director.fox;fox.TouchRun=true;float deadline=Time.time+30,stuckSince=Time.time,detourUntil=-1;var previous=fox.transform.position;Vector3 detour=Vector3.zero;
            while(!complete()&&Time.time<deadline)
            {
                FightNearby();var target=destination();var delta=target-fox.transform.position;delta.y=0;
                if(Time.time<detourUntil)delta=detour-fox.transform.position;
                delta.y=0;fox.TouchMove=new Vector2(delta.x,delta.z).normalized*Mathf.Clamp01(delta.magnitude*2);
                if(jumpForPickup&&target.y-fox.transform.position.y>.7f&&delta.magnitude<2.7f&&fox.IsGrounded)fox.RequestJump();
                if(Vector3.Distance(fox.transform.position,previous)>.2f){previous=fox.transform.position;stuckSince=Time.time;}
                else if(Time.time-stuckSince>.65f&&Time.time>detourUntil)
                {
                    if(fox.IsGrounded&&target.y-fox.transform.position.y>.6f&&delta.magnitude<3){fox.RequestJump();stuckSince=Time.time;}
                    else{var side=Vector3.Cross(Vector3.up,delta.normalized);detour=fox.transform.position+side*1.5f;detourUntil=Time.time+.6f;stuckSince=Time.time;}
                }
                yield return null;
            }
            fox.TouchMove=Vector2.zero;fox.TouchRun=false;Assert.IsTrue(complete(),"Automated exploration stalled at "+fox.transform.position+" towards "+destination());
        }
        [UnityTest,Timeout(300000)]public IEnumerator CompleteThreeWorldAdventureThroughMovementPickupsAndCombat()
        {
            var camera=director.fox.cameraTransform;director.fox.cameraTransform=null;
            try
            {
                for(int region=1;region<=3;region++)
                {
                    Assert.IsTrue(director.TravelTo(region));yield return null;
                    for(int index=0;index<8;index++)
                    {
                        AdventureActor star=null;foreach(var actor in director.GetComponentsInChildren<AdventureActor>())if(actor.role==AdventureActor.Role.Star&&actor.actorId=="star"+index)star=actor;
                        if(!star)continue;
                        if(index==4)
                        {
                            var approach=new Vector3(-7,0,24);yield return ExploreTowards(()=>approach,()=>{var d=director.fox.transform.position-approach;d.y=0;return d.magnitude<.35f;});
                            var summit=new Vector3(-7,0,38);yield return ExploreTowards(()=>summit,()=>{var d=director.fox.transform.position-summit;d.y=0;return d.magnitude<.4f;});
                        }
                        yield return ExploreTowards(()=>star.transform.position,()=>!star.gameObject.activeInHierarchy,true);
                    }
                    foreach(var enemy in director.GetComponentsInChildren<PortalEnemy>())if(!enemy.IsDefeated)yield return ExploreTowards(()=>enemy.transform.position,()=>enemy.IsDefeated);
                    var progress=ExplorationProgress.Load();int stars=0,enemies=0;foreach(var id in progress.stars)if(id.StartsWith(region+":"))stars++;foreach(var id in progress.enemies)if(id.StartsWith(region+":"))enemies++;
                    Assert.AreEqual(8,stars);Assert.AreEqual(4,enemies);Debug.Log("PATI_PLAYTHROUGH region="+region+" stars="+stars+" enemies="+enemies);
                }
                Assert.IsTrue(director.TravelTo(0));yield return null;int seeds=0;
                foreach(var part in director.GetComponentsInChildren<Transform>())if(part.name=="Restored light seed")
                {var color=part.GetComponent<Renderer>().sharedMaterial.color;Assert.Greater(color.r,.85f);Assert.Greater(color.g,.6f);seeds++;}
                Assert.AreEqual(3,seeds);Assert.AreEqual(24,ExplorationProgress.Load().stars.Count);Assert.AreEqual(12,ExplorationProgress.Load().enemies.Count);
            }
            finally {director.fox.cameraTransform=camera;director.fox.TouchMove=Vector2.zero;director.fox.TouchRun=false;}
        }
        [UnityTest]public IEnumerator AllRegionalStairsHavePawContactAndCanBeAscendedAndDescended()
        {
            var cameraTransform=director.fox.cameraTransform;director.fox.cameraTransform=null;
            try
            {
                for(int region=1;region<=3;region++)
                {
                    Assert.IsTrue(director.TravelTo(region));yield return null;
                    foreach(var enemy in director.GetComponentsInChildren<PortalEnemy>())enemy.enabled=false;
                    foreach(bool running in new[]{false,true})
                    {
                        director.fox.SetCheckpoint(new Vector3(-7,.2f,24));director.fox.Respawn();yield return new WaitForSeconds(.3f);
                        yield return WalkTo(new Vector3(-7,0,29.1f),running);yield return new WaitForSeconds(.8f);
                        if(!running)CaptureStairPose(cameraTransform.GetComponent<Camera>(),region);
                        int paws=0;float worst=0;
                        foreach(var part in director.fox.GetComponentsInChildren<Transform>())if(part.name=="Paw pad")
                        {
                            var sole=part.TransformPoint(new Vector3(0,-.5f,0));Assert.IsTrue(Physics.Raycast(sole+Vector3.up*.4f,Vector3.down,out var hit,.9f,director.fox.groundMask,QueryTriggerInteraction.Ignore));
                            float error=Mathf.Abs(sole.y-hit.point.y);worst=Mathf.Max(worst,error);Debug.Log("PATI_PAW region="+region+" run="+running+" sole="+sole+" ground="+hit.point+" error="+error+" body="+director.fox.transform.position);paws++;
                        }
                        Assert.AreEqual(4,paws);
                        Assert.Less(worst,.06f,"Floating/buried paw in region "+region);
                        yield return WalkTo(new Vector3(-7,0,38),running);Assert.Greater(director.fox.transform.position.y,1.95f);
                        yield return WalkTo(new Vector3(-7,0,24),running);Assert.Less(director.fox.transform.position.y,.25f);
                    }
                }
            }
            finally {director.fox.cameraTransform=cameraTransform;director.fox.TouchMove=Vector2.zero;director.fox.TouchRun=false;}
        }
        private void CaptureStairPose(Camera camera,int region)
        {
            if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null)return;
            var position=camera.transform.position;var rotation=camera.transform.rotation;float fov=camera.fieldOfView;var target=new RenderTexture(1280,720,24);target.Create();var previous=RenderTexture.active;var image=new Texture2D(1280,720,TextureFormat.RGB24,false);
            try
            {
                camera.transform.position=director.fox.transform.position+new Vector3(2.4f,1.4f,-1.9f);camera.transform.LookAt(director.fox.transform.position+Vector3.up*.7f);camera.fieldOfView=45;
                for(int i=0;i<3;i++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();File.WriteAllBytes("ValidationResults/StairPaws"+region+".png",image.EncodeToPNG());
            }
            finally {camera.transform.SetPositionAndRotation(position,rotation);camera.fieldOfView=fov;RenderTexture.active=previous;target.Release();Object.Destroy(target);Object.Destroy(image);}
        }
        [UnityTest]public IEnumerator PlatformStarFollowsItsSupport()
        {
            director.TravelTo(1);yield return null;var platform=director.GetComponentInChildren<MovingPlatform>();AdventureActor star=null;
            foreach(var actor in platform.GetComponentsInChildren<AdventureActor>())if(actor.role==AdventureActor.Role.Star)star=actor;
            Assert.NotNull(star);var relative=star.transform.position-platform.transform.position;yield return new WaitForSeconds(.8f);
            var now=star.transform.position-platform.transform.position;Assert.AreEqual(relative.x,now.x,.01f);Assert.AreEqual(relative.z,now.z,.01f);
        }
        [UnityTest]public IEnumerator PawsRestOnSlopingSurface()
        {
            var ramp=GameObject.CreatePrimitive(PrimitiveType.Cube);SceneManager.MoveGameObjectToScene(ramp,scene);var material=new Material(director.surfaceMaterial);material.color=new Color(.35f,.43f,.29f);ramp.GetComponent<Renderer>().sharedMaterial=material;
            ramp.transform.position=new Vector3(0,1,9);ramp.transform.localScale=new Vector3(4,.3f,4);ramp.transform.rotation=Quaternion.Euler(-12,0,0);Physics.SyncTransforms();
            try
            {
                director.fox.SetCheckpoint(new Vector3(0,1.4f,9));director.fox.Respawn();yield return new WaitForSeconds(1.2f);Assert.IsTrue(director.fox.IsGrounded);int count=0;
                foreach(var part in director.fox.GetComponentsInChildren<Transform>())if(part.name=="Paw pad")
                {var sole=part.TransformPoint(new Vector3(0,-.5f,0));Assert.IsTrue(Physics.Raycast(sole+Vector3.up*.4f,Vector3.down,out var ground,.9f,director.fox.groundMask,QueryTriggerInteraction.Ignore));Assert.Less(Mathf.Abs(sole.y-ground.point.y),.04f,"Paw contact on slope");count++;}
                Assert.AreEqual(4,count);CaptureStairPose(director.fox.cameraTransform.GetComponent<Camera>(),0);
            }
            finally {Object.Destroy(ramp);Object.Destroy(material);}
        }
        [UnityTest]public IEnumerator FrozenLakeHasTheSameWalkingSurfaceAsItsVisibleIce()
        {
            director.TravelTo(3);yield return null;Transform lake=null;foreach(var part in director.GetComponentsInChildren<Transform>())if(part.name=="Frozen lake")lake=part;
            Assert.NotNull(lake);Assert.NotNull(lake.GetComponent<MeshCollider>());director.fox.SetCheckpoint(new Vector3(16,.25f,22));director.fox.Respawn();yield return new WaitForSeconds(.7f);
            Assert.IsTrue(director.fox.IsGrounded);Assert.IsTrue(Physics.Raycast(director.fox.transform.position+Vector3.up*.4f,Vector3.down,out var surface,1,director.fox.groundMask,QueryTriggerInteraction.Ignore));Assert.AreEqual(lake.gameObject,surface.collider.gameObject);
            Assert.AreEqual(lake.GetComponent<Renderer>().bounds.max.y,surface.point.y,.01f);Assert.Greater(director.fox.transform.position.y,surface.point.y);
        }
        [UnityTest]public IEnumerator GuardianTitleFollowsTheEnemyAndDoesNotOverlapItsHealth()
        {
            director.TravelTo(3);yield return null;PortalEnemy guardian=null;foreach(var enemy in director.GetComponentsInChildren<PortalEnemy>())if(enemy.enemyId=="enemy3")guardian=enemy;
            Assert.NotNull(guardian);TextMesh title=null;foreach(var text in guardian.GetComponentsInChildren<TextMesh>())if(text.text=="Shadow Guardian")title=text;
            Assert.NotNull(title);Assert.Greater(title.transform.localPosition.y-guardian.healthDisplay.transform.localPosition.y,.2f);
            guardian.Hit(99,director.fox.transform.position);yield return new WaitForSeconds(.5f);Assert.IsFalse(title.gameObject.activeInHierarchy);
        }
        [UnityTest]public IEnumerator PortalTriggerTeleportsAndAllWorldsHaveEnemies()
        {
            AdventurePortal portal=null;foreach(var root in scene.GetRootGameObjects())foreach(var candidate in root.GetComponentsInChildren<AdventurePortal>())if(candidate.destination==1)portal=candidate;
            Assert.NotNull(portal);director.fox.SetCheckpoint(portal.transform.position+Vector3.up*.1f);director.fox.Respawn();yield return new WaitForSeconds(.2f);
            Assert.AreEqual(1,director.CurrentRegion);Assert.NotNull(Enemy());
            for(int region=2;region<=3;region++){yield return new WaitForSeconds(1.6f);Assert.IsTrue(director.TravelTo(region));yield return null;Assert.AreEqual(region,director.CurrentRegion);Assert.NotNull(Enemy());}
        }
        [UnityTest]public IEnumerator EachCornerGatewayCanBeWalkedThrough()
        {
            for(int destination=1;destination<=3;destination++)
            {
                if(destination>1){yield return new WaitForSeconds(1.6f);Assert.IsTrue(director.TravelTo(0));yield return new WaitForSeconds(1.6f);}
                AdventurePortal portal=null;foreach(var candidate in director.GetComponentsInChildren<AdventurePortal>())if(candidate.destination==destination)portal=candidate;
                Assert.NotNull(portal);var forward=portal.transform.forward;
                director.fox.SetCheckpoint(portal.transform.position-forward*3.5f+Vector3.up*.2f);director.fox.Respawn();Physics.SyncTransforms();
                var controller=director.fox.GetComponent<CharacterController>();
                for(int step=0;step<55&&director.CurrentRegion==0;step++){controller.Move(forward*.1f);yield return null;}
                Assert.AreEqual(destination,director.CurrentRegion,"The themed entrance must remain walkable.");
            }
        }
        [UnityTest]public IEnumerator GatewayColumnsAreSolidWhileTheirOpeningsRemainClear()
        {
            foreach(var portal in director.GetComponentsInChildren<AdventurePortal>())
            {
                var gate=portal.transform;
                Assert.IsTrue(Physics.SphereCast(gate.TransformPoint(new Vector3(-4,.8f,0)),.25f,gate.right,out var column,2.5f,director.fox.groundMask,QueryTriggerInteraction.Ignore),"Missing solid column for gateway "+portal.destination);
                Assert.NotNull(column.collider.GetComponentInParent<AdventurePortal>());
                Assert.IsFalse(Physics.SphereCast(gate.TransformPoint(new Vector3(0,.9f,-2)),.3f,gate.forward,out var blocker,4,director.fox.groundMask,QueryTriggerInteraction.Ignore),"Gateway opening is obstructed by "+(blocker.collider?blocker.collider.name:"unknown"));
            }
            yield return null;
        }
        [UnityTest]public IEnumerator BiteDefeatsEnemyAndPersistsAcrossReturn()
        {
            Assert.IsTrue(director.TravelTo(1));yield return null;var enemy=Enemy();Assert.NotNull(enemy);string id=enemy.enemyId;
            director.fox.SetCheckpoint(enemy.transform.position-Vector3.forward*1.6f-Vector3.up*.6f);director.fox.Respawn();director.fox.transform.rotation=Quaternion.identity;Physics.SyncTransforms();
            var combat=director.fox.GetComponent<FoxCombat>();Assert.IsTrue(combat.RequestAttack(false));Assert.IsTrue(enemy.IsDefeated);Assert.AreEqual(1,director.DefeatedEnemies);
            Assert.Contains("1:"+id,ExplorationProgress.Load().enemies);yield return new WaitForSeconds(1.6f);director.TravelTo(0);yield return new WaitForSeconds(1.6f);director.TravelTo(1);yield return null;
            foreach(var root in scene.GetRootGameObjects())foreach(var candidate in root.GetComponentsInChildren<PortalEnemy>())Assert.AreNotEqual(id,candidate.enemyId);
        }
        [UnityTest]public IEnumerator SpinHitsBehindAndAttackCooldownAndPauseAreEnforced()
        {
            director.TravelTo(2);yield return null;var enemy=Enemy();var combat=director.fox.GetComponent<FoxCombat>();
            director.fox.SetCheckpoint(enemy.transform.position+Vector3.forward*1.4f-Vector3.up*.6f);director.fox.Respawn();director.fox.transform.rotation=Quaternion.identity;Physics.SyncTransforms();
            Assert.IsTrue(combat.RequestAttack(true));Assert.AreEqual(1,enemy.health);Assert.IsFalse(combat.RequestAttack(true));
            yield return new WaitForSeconds(.9f);director.TogglePause();Assert.IsFalse(combat.RequestAttack(false));Assert.IsFalse(director.TravelTo(0));director.ResumeGame();Assert.IsTrue(director.IsPlaying);
        }
        [UnityTest]public IEnumerator StarTriggerPersistsAndTouchAttackButtonsExist()
        {
            director.TravelTo(3);yield return null;AdventureActor star=null;MobileControls controls=null;
            foreach(var root in scene.GetRootGameObjects()){if(!star)foreach(var actor in root.GetComponentsInChildren<AdventureActor>())if(actor.role==AdventureActor.Role.Star){star=actor;break;}if(!controls)controls=root.GetComponentInChildren<MobileControls>();}
            Assert.NotNull(star);Assert.NotNull(controls);Assert.AreEqual(2,controls.GetComponentsInChildren<PatiAttackButton>(true).Length);
            director.fox.SetCheckpoint(star.transform.position-Vector3.up*.7f);director.fox.Respawn();yield return new WaitForSeconds(.2f);
            Assert.GreaterOrEqual(director.ExplorationStars,1);Assert.Contains("3:"+star.actorId,ExplorationProgress.Load().stars);
        }
    }
}
#endif
