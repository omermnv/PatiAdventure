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
    public sealed class CombatCollisionTests
    {
        private Scene scene;
        private AdventureDirector director;
        private PortalEnemy enemy;
        private string previous,language;
        private bool hadSave,entered;
        // Remain outside the visible foothills as well as the original playable square.
        private readonly Vector3 arena=new Vector3(180,0,180);
        private GameObject Cube(string name,Vector3 position,Vector3 scale)
        {
            var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);cube.name=name;
            SceneManager.MoveGameObjectToScene(cube,scene);cube.transform.position=position;cube.transform.localScale=scale;return cube;
        }
        private GameObject Cover()
        {
            var wall=Cube("Combat test solid cover",arena+new Vector3(0,1,.7f),new Vector3(3,2,.2f));Physics.SyncTransforms();return wall;
        }
        [UnitySetUp]public IEnumerator Load()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Generate the portal scene in the isolated validation project first.");
            entered=true;Time.timeScale=1;hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);previous=PlayerPrefs.GetString(ExplorationProgress.Key);language=PlayerPrefs.GetString("Pati.Language","tr");PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));yield return null;yield return null;
            foreach(var root in scene.GetRootGameObjects()){director=root.GetComponentInChildren<AdventureDirector>();if(director)break;}
            Assert.NotNull(director);director.ChooseLanguage("en");director.StartAdventure();Assert.IsTrue(director.TravelTo(1));yield return null;
            foreach(var existing in director.GetComponentsInChildren<PortalEnemy>())existing.enabled=false;
            Cube("Combat test floor",arena+Vector3.down*.5f,new Vector3(10,1,10));
            var fox=director.fox;fox.readKeyboard=false;fox.SetCheckpoint(arena);fox.Respawn();fox.transform.rotation=Quaternion.identity;fox.enabled=false;
            var rootEnemy=new GameObject("Combat test guardian");SceneManager.MoveGameObjectToScene(rootEnemy,scene);rootEnemy.transform.position=arena+new Vector3(0,.9f,1.4f);
            enemy=rootEnemy.AddComponent<PortalEnemy>();enemy.director=director;enemy.enemyId="collisionTestGuardian";enemy.health=6;enemy.enabled=false;
            var collider=rootEnemy.AddComponent<SphereCollider>();collider.radius=1;collider.isTrigger=true;
            var body=rootEnemy.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;
            Physics.SyncTransforms();yield return null;
        }
        [UnityTearDown]public IEnumerator Unload()
        {
            if(!entered)yield break;Time.timeScale=1;if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,previous);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            PlayerPrefs.SetString("Pati.Language",language);PatiLocalization.SetLanguage(language);PlayerPrefs.Save();
        }
        private IEnumerator Ready(FoxCombat combat)
        {
            while(Time.time<combat.NextAttackAt)yield return null;
        }
        [UnityTest]public IEnumerator HittingAWindingUpEnemyCancelsItsWarningAndPreventsItsStrike()
        {
            int respawns=0;director.fox.Respawned+=()=>respawns++;
            enemy.enabled=true;
            float deadline=Time.time+2;
            while(!enemy.IsPreparingAttack&&Time.time<deadline)yield return null;
            Assert.IsTrue(enemy.IsPreparingAttack);
            Assert.NotNull(System.Array.Find(director.GetComponentsInChildren<CombatCue>(),cue=>cue.name=="Enemy attack warning"));
            enemy.Hit(2,director.fox.transform.position);
            Assert.IsFalse(enemy.IsPreparingAttack);
            yield return null;
            Assert.IsNull(System.Array.Find(director.GetComponentsInChildren<CombatCue>(),cue=>cue.name=="Enemy attack warning"));
            yield return new WaitForSeconds(.85f);
            Assert.AreEqual(0,respawns,"An interrupted attack must not damage Pati.");
        }
        [UnityTest]public IEnumerator SafeReturnProtectionMatchesTheDamageCooldown()
        {
            int respawns=0;director.fox.Respawned+=()=>respawns++;
            director.Bump();director.Bump();
            Assert.AreEqual(1,respawns);
            var protection=System.Array.Find(director.GetComponentsInChildren<CombatCue>(),cue=>cue.name=="Safe return protection");
            Assert.NotNull(protection);Assert.AreEqual(director.fox.transform,protection.follow);
            yield return new WaitForSeconds(2.1f);
            Assert.IsTrue(protection==null,"The visible shield must expire with protection.");
            director.Bump();Assert.AreEqual(2,respawns);
        }
        [UnityTest]public IEnumerator BiteAndSpinRespectSolidCoverButIgnoreActorBodiesAndTriggers()
        {
            var wall=Cover();var combat=director.fox.GetComponent<FoxCombat>();
            Assert.IsTrue(combat.RequestAttack(false));Assert.AreEqual(6,enemy.health,"A bite crossed the wall.");
            yield return Ready(combat);Assert.IsTrue(combat.RequestAttack(true));Assert.AreEqual(6,enemy.health,"A tail spin crossed the wall.");
            Object.Destroy(wall);yield return null;
            var trigger=Cube("Combat test nonblocking trigger",arena+new Vector3(0,1,.7f),new Vector3(1,1,.2f));trigger.GetComponent<Collider>().isTrigger=true;
            var foxBody=new GameObject("Combat test fox child collider");foxBody.transform.SetParent(director.fox.transform,false);foxBody.transform.localPosition=new Vector3(0,.8f,.4f);foxBody.AddComponent<BoxCollider>().size=new Vector3(.5f,.5f,.2f);
            var enemyBody=new GameObject("Combat test enemy child collider");enemyBody.transform.SetParent(enemy.transform,false);enemyBody.transform.localPosition=new Vector3(0,0,-.4f);enemyBody.AddComponent<BoxCollider>().size=new Vector3(.5f,.5f,.2f);
            Physics.SyncTransforms();yield return Ready(combat);
            Assert.IsTrue(combat.RequestAttack(false));Assert.AreEqual(4,enemy.health,"Clear bites must ignore actor child colliders and triggers.");
            yield return Ready(combat);Assert.IsTrue(combat.RequestAttack(true));Assert.AreEqual(3,enemy.health,"A clear tail spin must still hit after its normal cooldown.");
        }
        [UnityTest]public IEnumerator EnemyCannotStrikeThroughCoverIncludingCoverAddedDuringItsWindup()
        {
            int respawns=0;director.fox.Respawned+=()=>respawns++;
            var wall=Cover();enemy.enabled=true;yield return new WaitForSeconds(1);
            Assert.AreEqual(0,respawns);Assert.IsFalse(enemy.IsPreparingAttack,"A covered enemy should not begin an attack.");
            Object.Destroy(wall);yield return null;Physics.SyncTransforms();
            float deadline=Time.time+2;while(!enemy.IsPreparingAttack&&Time.time<deadline)yield return null;
            Assert.IsTrue(enemy.IsPreparingAttack,"The enemy should attack when cover is removed.");
            wall=Cover();yield return new WaitForSeconds(.9f);Assert.AreEqual(0,respawns,"Cover appearing during windup must block the strike.");
            yield return new WaitForSeconds(1.3f);Assert.IsFalse(enemy.IsPreparingAttack);
            Object.Destroy(wall);yield return null;Physics.SyncTransforms();
            deadline=Time.time+2;while(!enemy.IsPreparingAttack&&Time.time<deadline)yield return null;Assert.IsTrue(enemy.IsPreparingAttack);
            yield return new WaitForSeconds(.9f);Assert.AreEqual(1,respawns,"An unobstructed strike should retain its normal effect.");
        }
        [UnityTest]public IEnumerator GuardianKnockbackStopsBeforeStoneAndKeepsItsFeetOnTheFloor()
        {
            var wall=Cube("Combat test stone behind guardian",arena+new Vector3(0,1.2f,2.8f),new Vector3(4,2.4f,.2f));Physics.SyncTransforms();
            var before=enemy.transform.position;Assert.IsTrue(director.fox.GetComponent<FoxCombat>().RequestAttack(false));Assert.AreEqual(4,enemy.health);
            Assert.Greater(enemy.transform.position.z,before.z+.05f,"Clear space before the stone should allow a short knockback.");
            Physics.SyncTransforms();
            var body=enemy.GetComponent<SphereCollider>();
            Assert.LessOrEqual(body.bounds.max.z,wall.GetComponent<Collider>().bounds.min.z-.04f,"The guardian's body penetrated the stone.");
            Assert.AreEqual(.9f,enemy.transform.position.y,.01f,"Knockback should retain the grounded torso height.");
            Assert.IsFalse(Physics.ComputePenetration(body,enemy.transform.position,enemy.transform.rotation,wall.GetComponent<Collider>(),wall.transform.position,wall.transform.rotation,out _,out _));
            yield return null;
        }
    }
}
#endif
