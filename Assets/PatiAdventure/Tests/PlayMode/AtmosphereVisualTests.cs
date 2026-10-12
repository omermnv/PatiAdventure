#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;

namespace PatiAdventure.Tests
{
    public sealed class AtmosphereVisualTests
    {
        private Scene scene;
        private Scene unrelatedScene;
        private AdventureDirector director;
        private string oldSave, oldLanguage;
        private bool entered, hadSave;

        [UnitySetUp] public IEnumerator Load()
        {
            if (!File.Exists("ValidationResults/GeneratedPortalScene.txt")) Assert.Ignore("Run in isolated validation project.");
            entered = true;
            hadSave = PlayerPrefs.HasKey(ExplorationProgress.Key);
            oldSave = PlayerPrefs.GetString(ExplorationProgress.Key);
            oldLanguage = PlayerPrefs.GetString("Pati.Language", "tr");
            PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            scene = EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(), new LoadSceneParameters(LoadSceneMode.Additive));
            yield return null;
            yield return null;
            foreach (var root in scene.GetRootGameObjects())
            {
                director = root.GetComponentInChildren<AdventureDirector>();
                if (director) break;
            }
            Assert.NotNull(director);
            director.ChooseLanguage("en");
            director.StartAdventure();
            director.fox.readKeyboard = false;
            yield return null;
        }

        [UnityTearDown] public IEnumerator Restore()
        {
            if (!entered) yield break;
            Time.timeScale = 1;
            if (unrelatedScene.IsValid() && unrelatedScene.isLoaded) yield return SceneManager.UnloadSceneAsync(unrelatedScene);
            if (scene.IsValid() && scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
            if (hadSave) PlayerPrefs.SetString(ExplorationProgress.Key, oldSave);
            else PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            PlayerPrefs.SetString("Pati.Language", oldLanguage);
            PatiLocalization.SetLanguage(oldLanguage);
            PlayerPrefs.Save();
        }

        private void AssertSkyAndLocalSun()
        {
            Assert.NotNull(RenderSettings.skybox);
            Assert.AreEqual("Skybox/Procedural", RenderSettings.skybox.shader.name);
            Assert.Contains(RenderSettings.skybox, new System.Collections.Generic.List<Material>(director.GeneratedMaterials));
            Assert.Less(Vector4.Distance(RenderSettings.fogColor, RenderSettings.skybox.GetColor("_GroundColor")), .0001f, "The lower sky hemisphere must match distant haze within material color precision.");
            Assert.NotNull(typeof(AdventureDirector).GetField("atmosphereSkyShader", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(director), "Generated scenes must retain the sky shader as a build dependency.");
            Assert.NotNull(RenderSettings.sun);
            Assert.AreEqual(scene, RenderSettings.sun.gameObject.scene);
            Assert.AreEqual(CameraClearFlags.Skybox, director.fox.cameraTransform.GetComponent<Camera>().clearFlags);
        }

        [UnityTest] public IEnumerator RegionalSkiesKeepTheirShaderAndWarmColdPalette()
        {
            AssertSkyAndLocalSun();
            Assert.IsTrue(director.TravelTo(1));
            yield return null;
            AssertSkyAndLocalSun();
            var forestHaze = RenderSettings.fogColor;
            yield return new WaitForSeconds(1.6f);
            Assert.IsTrue(director.TravelTo(2));
            yield return null;
            AssertSkyAndLocalSun();
            Assert.Greater(RenderSettings.sun.color.r, RenderSettings.sun.color.b);
            Assert.Greater(RenderSettings.fogColor.r, forestHaze.r);
            yield return new WaitForSeconds(1.6f);
            Assert.IsTrue(director.TravelTo(3));
            yield return null;
            AssertSkyAndLocalSun();
            Assert.Greater(RenderSettings.sun.color.b, RenderSettings.sun.color.r);
            Assert.Greater(RenderSettings.fogColor.b, RenderSettings.fogColor.r);
        }

        [UnityTest] public IEnumerator RegionChangeDoesNotAlterAnotherScenesDirectionalLight()
        {
            var otherScene = unrelatedScene = SceneManager.CreateScene("Unrelated lighting regression");
            var otherSun = new GameObject("Unrelated sun").AddComponent<Light>();
            SceneManager.MoveGameObjectToScene(otherSun.gameObject, otherScene);
            otherSun.type = LightType.Directional;
            otherSun.intensity = 100;
            otherSun.color = Color.magenta;
            otherSun.transform.rotation = Quaternion.Euler(11, 12, 13);
            var rotation = otherSun.transform.rotation;
            try
            {
                RenderSettings.sun = otherSun;
                Assert.IsTrue(director.TravelTo(1));
                yield return null;
                AssertSkyAndLocalSun();
                Assert.AreEqual(Color.magenta, otherSun.color);
                Assert.AreEqual(100, otherSun.intensity);
                Assert.Less(Quaternion.Angle(rotation, otherSun.transform.rotation), .001f);
            }
            finally
            {
                Object.Destroy(otherSun.gameObject);
            }
            yield return SceneManager.UnloadSceneAsync(otherScene);
        }
    }
}
#endif
