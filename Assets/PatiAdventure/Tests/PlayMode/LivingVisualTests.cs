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
    public sealed class LivingVisualTests
    {
        private Scene scene;
        private AdventureDirector director;
        private string oldSave,oldLanguage;
        private bool entered,hadSave;
        [UnitySetUp]public IEnumerator Load()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Run in isolated validation project.");
            entered=true;hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);oldSave=PlayerPrefs.GetString(ExplorationProgress.Key);oldLanguage=PlayerPrefs.GetString("Pati.Language","tr");PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));yield return null;yield return null;
            foreach(var root in scene.GetRootGameObjects()){director=root.GetComponentInChildren<AdventureDirector>();if(director)break;}Assert.NotNull(director);
            director.ChooseLanguage("en");director.StartAdventure();director.fox.readKeyboard=false;yield return null;
        }
        [UnityTearDown]public IEnumerator Restore()
        {
            if(!entered)yield break;Time.timeScale=1;if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,oldSave);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);PlayerPrefs.SetString("Pati.Language",oldLanguage);PatiLocalization.SetLanguage(oldLanguage);PlayerPrefs.Save();
        }
        private Transform Named(Transform root,string name){foreach(var t in root.GetComponentsInChildren<Transform>())if(t.name==name)return t;return null;}
        [UnityTest]public IEnumerator FoxHasArticulatedJawLegsAndTail()
        {
            var rig=Named(director.fox.transform,"Fox Rig");Assert.NotNull(rig);
            foreach(string name in new[]{"Jaw Joint","Tail Base","Tail Middle","Tail Tip","Front Left Hip","Front Right Hip","Back Left Hip","Back Right Hip","Amber iris","Nose"})Assert.NotNull(Named(rig,name),name);
            var torso=Named(rig,"Torso").GetComponent<MeshFilter>().sharedMesh;Assert.Greater(torso.vertexCount,120);Assert.Greater(torso.bounds.size.z,torso.bounds.size.x);Assert.AreEqual(torso.vertexCount,torso.normals.Length);
            yield return null;
        }
        [UnityTest]public IEnumerator TailIsOneContinuousSkinAndBendsWithoutSeparating()
        {
            var tail=Named(director.fox.transform,"Tail Base");var skin=tail.GetComponent<SkinnedMeshRenderer>();Assert.NotNull(skin);
            Assert.AreEqual(0,tail.GetComponentsInChildren<MeshRenderer>().Length,"Tail must not be a chain of rigid pieces.");
            Assert.AreEqual(3,skin.bones.Length);Assert.AreEqual(2,skin.sharedMesh.subMeshCount);
            var pose=director.fox.GetComponent<FoxLocomotionPose>();pose.enabled=false;
            var before=new Mesh();var after=new Mesh();var middle=skin.bones[1];var rotation=middle.localRotation;
            try
            {
                skin.BakeMesh(before);middle.localRotation=rotation*Quaternion.Euler(0,35,0);skin.BakeMesh(after);
                var a=before.vertices;var b=after.vertices;float movement=0;
                for(int i=0;i<b.Length;i++)
                {
                    Assert.IsFalse(float.IsNaN(b[i].x)||float.IsNaN(b[i].y)||float.IsNaN(b[i].z));
                    movement=Mathf.Max(movement,Vector3.Distance(a[i],b[i]));
                }
                Assert.Greater(movement,.1f,"Joint rotation must deform the tail mesh.");
                // Every neighbouring ring remains joined by short edges after bending.
                for(int i=0;i<b.Length-25;i++)Assert.Less(Vector3.Distance(b[i],b[i+25]),.18f,"A gap or sharp joint appeared in the skin.");
            }
            finally {middle.localRotation=rotation;pose.enabled=true;Object.Destroy(before);Object.Destroy(after);}
            yield return null;
        }
        [UnityTest]public IEnumerator BiteMovesJawAndSpinRotatesBodyThenReturnsToRest()
        {
            var rig=Named(director.fox.transform,"Fox Rig");var jaw=Named(rig,"Jaw Joint");Assert.NotNull(jaw);var combat=director.fox.GetComponent<FoxCombat>();
            Assert.IsTrue(combat.RequestAttack(false));yield return new WaitForSeconds(.12f);Assert.Greater(Quaternion.Angle(Quaternion.identity,jaw.localRotation),12);
            yield return new WaitForSeconds(.38f);Assert.Less(Quaternion.Angle(Quaternion.identity,jaw.localRotation),4);
            Assert.IsTrue(combat.RequestAttack(true));yield return new WaitForSeconds(.2f);Assert.Greater(Quaternion.Angle(Quaternion.identity,rig.localRotation),45);
            yield return new WaitForSeconds(.4f);Assert.Less(Quaternion.Angle(Quaternion.identity,rig.localRotation),5);
        }
        [UnityTest]public IEnumerator EnemiesHaveRecognisableRegionalAnatomy()
        {
            for(int region=1;region<=3;region++)
            {
                Assert.IsTrue(director.TravelTo(region));yield return null;var enemy=director.GetComponentInChildren<PortalEnemy>();Assert.NotNull(enemy);Assert.NotNull(enemy.GetComponent<CreatureMotion>());
                string expected=region==1?"Boar face":region==2?"Upper muzzle":"Ice chest plate";Assert.NotNull(Named(enemy.transform,expected));Assert.NotNull(Named(enemy.transform,"Head Joint"));
                Assert.Greater(enemy.GetComponentsInChildren<MeshRenderer>().Length,20);yield return new WaitForSeconds(1.6f);
            }
        }
        [UnityTest]public IEnumerator PlatformDecorationFollowsMovingSupport()
        {
            director.TravelTo(1);yield return null;var platform=director.GetComponentInChildren<MovingPlatform>();Assert.NotNull(platform);var plank=Named(platform.transform,"Platform wooden plank");Assert.NotNull(plank);
            var local=plank.localPosition;var world=plank.position;var platformStart=platform.transform.position;yield return new WaitForSeconds(.7f);
            Assert.AreEqual(platform.GetComponent<Collider>().bounds.max.y,plank.GetComponent<Renderer>().bounds.max.y,.002f,"Deck collider must match the visible planks.");
            Assert.AreEqual(local,plank.localPosition);Assert.Less(((plank.position-world)-(platform.transform.position-platformStart)).magnitude,.001f);Assert.IsNull(plank.GetComponent<Collider>());
        }
    }
}
#endif
