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
    public sealed class PortalTransitionTests
    {
        private Scene scene;
        private AdventureDirector director;
        private bool entered,hadSave,hadLanguage;
        private string savedProgress,savedLanguage;
        private float previousTimeScale;

        [UnitySetUp] public IEnumerator Load()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Generate the portal scene in the isolated validation project first.");
            entered=true;previousTimeScale=Time.timeScale;Time.timeScale=1;
            hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);savedProgress=PlayerPrefs.GetString(ExplorationProgress.Key);
            hadLanguage=PlayerPrefs.HasKey("Pati.Language");savedLanguage=PlayerPrefs.GetString("Pati.Language");
            PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));
            yield return null;yield return null;
            foreach(var root in scene.GetRootGameObjects()){director=root.GetComponentInChildren<AdventureDirector>();if(director)break;}
            Assert.NotNull(director);director.ChooseLanguage("en");director.StartAdventure();director.fox.readKeyboard=false;
            yield return null;
        }

        [UnityTearDown] public IEnumerator Unload()
        {
            if(!entered)yield break;
            Time.timeScale=1;
            if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,savedProgress);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            if(hadLanguage)PlayerPrefs.SetString("Pati.Language",savedLanguage);else PlayerPrefs.DeleteKey("Pati.Language");
            PatiLocalization.SetLanguage(hadLanguage?savedLanguage:"tr");PlayerPrefs.Save();Time.timeScale=previousTimeScale;
        }

        [UnityTest] public IEnumerator ReturnPortalEnteredDuringCooldownTravelsWhenReady()
        {
            for(int region=1;region<=3;region++)
            {
                if(region>1)yield return new WaitForSeconds(1.6f);
                float departedAt=Time.time;
                Assert.IsTrue(director.TravelTo(region));
                // The retry must not make a direct trip to another world bypass
                // the existing 1.5 second transition cooldown.
                Assert.IsFalse(director.TravelTo(region==3?1:region+1));
                AdventurePortal portal=null;
                foreach(var candidate in director.GetComponentsInChildren<AdventurePortal>())if(candidate.destination==0)portal=candidate;
                Assert.NotNull(portal);
                var opening=portal.GetComponent<BoxCollider>();
                var fox=director.fox;
                var controller=fox.GetComponent<CharacterController>();
                // Enter through CharacterController.Move, then leave movement
                // input at zero so the fix is exercised by a genuine Stay event.
                fox.SetCheckpoint(portal.transform.position-portal.transform.forward*1.6f+Vector3.up*.2f);fox.Respawn();Physics.SyncTransforms();
                controller.Move(portal.transform.forward*.9f);
                Physics.SyncTransforms();
                Assert.IsTrue(opening.bounds.Intersects(controller.bounds));
                yield return new WaitForSeconds(.3f);
                Assert.Less(Time.time-departedAt,1.5f,"The test must enter the return portal before its cooldown expires.");
                Assert.AreEqual(region,director.CurrentRegion,"Trigger entry must respect the travel cooldown.");
                Assert.IsFalse(director.TravelTo(0));
                float limit=departedAt+2.5f;
                while(director.CurrentRegion==region&&Time.time<limit)yield return null;
                Assert.AreEqual(0,director.CurrentRegion,"Waiting inside the return portal should travel after the cooldown in region "+region);
                Assert.GreaterOrEqual(Time.time-departedAt,1.5f,"Trigger stay must retain the existing cooldown.");
            }
        }
    }
}
#endif
