#if UNITY_EDITOR
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PatiAdventure.Tests
{
    public sealed class WorldLookTests
    {
        private Scene scene;private AdventureDirector director;private bool entered,hadSave,hadLanguage;private string save,language;
        [UnitySetUp]public IEnumerator Setup()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Generate an isolated portal scene first.");
            entered=true;Time.timeScale=1;hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);save=PlayerPrefs.GetString(ExplorationProgress.Key);hadLanguage=PlayerPrefs.HasKey("Pati.Language");language=PlayerPrefs.GetString("Pati.Language");PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));yield return null;yield return null;
            foreach(var root in scene.GetRootGameObjects())if(root.TryGetComponent(out director))break;
            Assert.NotNull(director);director.ChooseLanguage("en");director.StartAdventure();director.fox.readKeyboard=false;yield return null;
        }
        [UnityTearDown]public IEnumerator Cleanup()
        {
            if(!entered)yield break;Time.timeScale=1;if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,save);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            if(hadLanguage)PlayerPrefs.SetString("Pati.Language",language);else PlayerPrefs.DeleteKey("Pati.Language");PatiLocalization.SetLanguage(hadLanguage?language:"tr");PlayerPrefs.Save();
        }
        [UnityTest]public IEnumerator VisibleOuterTerrainSupportsWalkingBeyondTheFormerSquareEdge()
        {
            for(int region=0;region<=3;region++)
            {
                if(region>0){yield return new WaitForSeconds(1.6f);Assert.IsTrue(director.TravelTo(region));yield return null;}
                foreach(var enemy in director.GetComponentsInChildren<PortalEnemy>())enemy.enabled=false;
                float edge=17-(region==0?60:96)*.5f;var fox=director.fox;var camera=fox.cameraTransform;fox.cameraTransform=null;
                try
                {
                    fox.SetCheckpoint(new Vector3(0,.25f,edge+.65f));fox.Respawn();yield return new WaitForSeconds(.35f);
                    float limit=Time.time+3;fox.TouchMove=Vector2.down;
                    while(fox.transform.position.z>edge-.7f&&Time.time<limit)yield return null;
                    fox.TouchMove=Vector2.zero;yield return new WaitForSeconds(.4f);
                    Assert.Less(fox.transform.position.z,edge-.6f,"Former landscape edge is blocked in region "+region);
                    Assert.IsTrue(fox.IsGrounded,"Visible surrounding terrain must support the player.");
                    Assert.IsTrue(Physics.Raycast(fox.transform.position+Vector3.up*.3f,Vector3.down,out var ground,1,fox.groundMask,QueryTriggerInteraction.Ignore));
                    Assert.AreEqual("Continuous foothill landscape",ground.collider.name);
                    var collider=ground.collider as MeshCollider;Assert.NotNull(collider);Assert.AreSame(collider.GetComponent<MeshFilter>().sharedMesh,collider.sharedMesh);
                }
                finally{fox.TouchMove=Vector2.zero;fox.cameraTransform=camera;}
            }
        }
    }
}
#endif
