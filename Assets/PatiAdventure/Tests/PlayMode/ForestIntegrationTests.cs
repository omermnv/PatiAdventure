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
    public sealed class ForestIntegrationTests
    {
        private Scene scene;
        private string originalSave,originalLanguage;
        private bool hadSave;
        private int originalProgress;
        private bool setupEntered;
        [UnitySetUp]public IEnumerator LoadForest()
        {
            if(!File.Exists("ValidationResults/GeneratedScene.txt"))Assert.Ignore("Run forest integration tests in the isolated validation copy created by Tools/Prepare-UnityValidation.ps1.");
            setupEntered=true;
            hadSave=PlayerPrefs.HasKey(AdventureSave.Key);originalSave=PlayerPrefs.GetString(AdventureSave.Key);originalProgress=PlayerPrefs.GetInt("Pati.Progress",0);originalLanguage=PlayerPrefs.GetString("Pati.Language","tr");
            // This runs inside the validation copy, with its separate company/product namespace.
            string path=File.ReadAllText("ValidationResults/GeneratedScene.txt").Trim();
            scene=EditorSceneManager.LoadSceneInPlayMode(path,new LoadSceneParameters(LoadSceneMode.Additive));
            yield return null;yield return null;
        }
        [UnityTearDown]public IEnumerator Restore()
        {
            if(!setupEntered)yield break;
            Time.timeScale=1;if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadSave)PlayerPrefs.SetString(AdventureSave.Key,originalSave);else PlayerPrefs.DeleteKey(AdventureSave.Key);
            PlayerPrefs.SetInt("Pati.Progress",originalProgress);PatiLocalization.SetLanguage(originalLanguage);PlayerPrefs.Save();yield return null;
        }
        private T Find<T>() where T:Component
        {foreach(var root in scene.GetRootGameObjects()){var found=root.GetComponentInChildren<T>();if(found)return found;}return null;}
        private IEnumerator ResumeFirstForest()
        {
            var save=new AdventureSaveData{trainingComplete=true};AdventureSave.Write(save);
            var director=Find<AdventureDirector>();director.ChooseLanguage("en");director.ResumeSaved();director.StartAdventure();yield return null;
        }
        [UnityTest]public IEnumerator LanguageSelectionLocksMovementAndCreatesCanvas()
        {
            var director=Find<AdventureDirector>();Assert.NotNull(director);Assert.IsTrue(director.fox.InputLocked);Assert.NotNull(Find<MobileControls>());
            Assert.Greater(Find<MobileControls>().GetComponentsInChildren<UnityEngine.UI.Image>(true).Length,3);
            director.ChooseLanguage("en");Assert.AreEqual("en",PatiLocalization.Language);Assert.IsTrue(director.fox.InputLocked);yield return null;
        }
        [UnityTest]public IEnumerator StaticPickupTriggersFromCharacterControllerAndPersists()
        {
            yield return ResumeFirstForest();var director=Find<AdventureDirector>();
            AdventureActor star=null;
            foreach(var candidate in Object.FindObjectsByType<AdventureActor>(FindObjectsSortMode.None))
                if(candidate.gameObject.scene==scene&&candidate.role==AdventureActor.Role.Star&&(!star||candidate.transform.position.z<star.transform.position.z))star=candidate;
            Assert.NotNull(star);
            Assert.AreEqual(AdventureActor.Role.Star,star.role);
            // Start on the supporting platform, outside the pickup trigger. Starting
            // beyond its edge made a slow graphics frame drop Pati below the platform.
            director.fox.readKeyboard=false;director.fox.SetCheckpoint(star.transform.position+Vector3.back*1.25f-Vector3.up*.7f);director.fox.Respawn();Physics.SyncTransforms();
            Assert.AreEqual(0,director.StarCount);director.fox.TouchMove=Vector2.up;
            yield return new WaitForSeconds(.9f);director.fox.TouchMove=Vector2.zero;
            Assert.GreaterOrEqual(director.StarCount,1);Assert.Contains(star.actorId,AdventureSave.Load().stars);
        }
        [UnityTest]public IEnumerator GateRequiresGuideFriendBeaconAndFiveStars()
        {
            yield return ResumeFirstForest();var director=Find<AdventureDirector>();director.TryFinish();Assert.IsTrue(director.IsPlaying);
            AdventureActor guide=null,friend=null,beacon=null;
            foreach(var actor in Object.FindObjectsByType<AdventureActor>(FindObjectsSortMode.None))
            {
                if(actor.gameObject.scene!=scene)continue;
                if(actor.role==AdventureActor.Role.Star)director.Interact(actor.role,actor.actorId);
                if(actor.role==AdventureActor.Role.Guide)guide=actor;
                if(actor.role==AdventureActor.Role.Friend)friend=actor;
                if(actor.role==AdventureActor.Role.Beacon)beacon=actor;
            }
            Assert.NotNull(guide);Assert.NotNull(friend);Assert.NotNull(beacon);director.TryFinish();Assert.IsTrue(director.IsPlaying);
            director.Interact(guide.role,guide.actorId);director.Interact(friend.role,friend.actorId);director.Interact(beacon.role,beacon.actorId);director.TryFinish();
            Assert.IsFalse(director.IsPlaying);Assert.AreEqual(1,AdventureSave.Load().level);director.ContinueAdventure();Assert.AreEqual(1,director.LevelIndex);yield return null;
        }
        [UnityTest]public IEnumerator PauseStopsMotionAndResumesSafely()
        {
            yield return ResumeFirstForest();var director=Find<AdventureDirector>();director.TogglePause();Assert.IsTrue(director.IsPaused);Assert.AreEqual(0,Time.timeScale);
            var point=director.fox.transform.position;yield return new WaitForSecondsRealtime(.2f);Assert.AreEqual(point,director.fox.transform.position);
            director.ResumeGame();Assert.AreEqual(1,Time.timeScale);Assert.IsFalse(director.fox.InputLocked);Assert.IsFalse(director.IsPaused);
            Assert.IsFalse(UnityEngine.Rendering.DebugManager.instance.displayRuntimeUI);
            Assert.IsFalse(UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI);
        }
        [UnityTest]public IEnumerator WakeWalkRunAndJumpUnlockFirstForest()
        {
            var director=Find<AdventureDirector>();director.ChooseLanguage("en");director.StartAdventure();
            Assert.IsTrue(director.fox.InputLocked);yield return new WaitForSeconds(2.7f);Assert.IsFalse(director.fox.InputLocked);
            director.fox.readKeyboard=false;director.fox.TouchMove=Vector2.up;yield return new WaitForSeconds(2.2f);
            director.fox.TouchRun=true;yield return new WaitForSeconds(1.8f);director.fox.TouchMove=Vector2.zero;director.fox.TouchRun=false;
            for(int i=0;i<3;i++){director.fox.RequestJump();yield return new WaitForSeconds(1.1f);}
            Assert.IsTrue(director.TrainingComplete,"Walk="+director.fox.WalkedDistance+", Run="+director.fox.RanDistance+", Jumps="+director.fox.JumpCount+", Position="+director.fox.transform.position);Assert.IsTrue(director.IsPlaying);Assert.IsTrue(AdventureSave.Load().trainingComplete);
        }
        [UnityTest]public IEnumerator ReloadRestoresCheckpointAndHidesCollectedStars()
        {
            yield return ResumeFirstForest();var saved=new AdventureSaveData{trainingComplete=true,hasCheckpoint=true,checkpoint=new Vector3(0,.34f,22)};
            foreach(var actor in Object.FindObjectsByType<AdventureActor>(FindObjectsSortMode.None))
                if(actor.gameObject.scene==scene&&actor.role==AdventureActor.Role.Star&&saved.stars.Count<5)saved.stars.Add(actor.actorId);
            yield return SceneManager.UnloadSceneAsync(scene);AdventureSave.Write(saved);
            string path=File.ReadAllText("ValidationResults/GeneratedScene.txt").Trim();scene=EditorSceneManager.LoadSceneInPlayMode(path,new LoadSceneParameters(LoadSceneMode.Additive));yield return null;yield return null;
            var director=Find<AdventureDirector>();director.ChooseLanguage("en");director.ResumeSaved();director.StartAdventure();yield return null;
            Assert.AreEqual(5,director.StarCount);Assert.AreEqual(22,director.fox.transform.position.z,.1f);
            int visibleStars=0;foreach(var actor in Object.FindObjectsByType<AdventureActor>(FindObjectsSortMode.None))if(actor.gameObject.scene==scene&&actor.role==AdventureActor.Role.Star)visibleStars++;
            Assert.AreEqual(3,visibleStars);
        }
    }
}
#endif
