#if UNITY_EDITOR
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace PatiAdventure.Tests
{
    public sealed class StartupRenderingTests
    {
        private Scene scene;
        private bool entered,hadLanguage,hadSave;
        private string language,save;
        [UnityTearDown] public IEnumerator Restore()
        {
            if(!entered)yield break;
            Time.timeScale=1;
            if(scene.IsValid()&&scene.isLoaded)yield return SceneManager.UnloadSceneAsync(scene);
            if(hadLanguage)PlayerPrefs.SetString("Pati.Language",language);else PlayerPrefs.DeleteKey("Pati.Language");
            if(hadSave)PlayerPrefs.SetString(ExplorationProgress.Key,save);else PlayerPrefs.DeleteKey(ExplorationProgress.Key);
            PatiLocalization.SetLanguage(hadLanguage?language:"tr");PlayerPrefs.Save();
        }
        [UnityTest] public IEnumerator ActualFollowCameraSeesVillageAndEachWorld()
        {
            if(!File.Exists("ValidationResults/GeneratedPortalScene.txt"))Assert.Ignore("Isolated validation project required.");
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null)Assert.Ignore("Run with a graphics device to validate pixels.");
            entered=true;hadLanguage=PlayerPrefs.HasKey("Pati.Language");language=PlayerPrefs.GetString("Pati.Language");
            hadSave=PlayerPrefs.HasKey(ExplorationProgress.Key);save=PlayerPrefs.GetString(ExplorationProgress.Key);
            scene=EditorSceneManager.LoadSceneInPlayMode(File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim(),new LoadSceneParameters(LoadSceneMode.Additive));
                yield return null;yield return new WaitForSeconds(.5f);
                AdventureDirector director=null;
                foreach(var root in scene.GetRootGameObjects())if(root.TryGetComponent<AdventureDirector>(out var found))director=found;
                Assert.NotNull(director);
                var camera=director.fox.cameraTransform.GetComponent<Camera>();
                Assert.Greater(Vector3.Distance(camera.transform.position,director.fox.transform.position+Vector3.up*1.1f),4f,"Village spawn must leave space for the follow camera.");
                Capture(camera,"StartupLanguage");
                director.ChooseLanguage("en");director.StartAdventure();director.fox.readKeyboard=false;
                for(int region=0;region<=3;region++)
                {
                    if(region>0){yield return new WaitForSeconds(1.6f);Assert.IsTrue(director.TravelTo(region));}
                    yield return new WaitForSeconds(.7f);
                    Assert.Greater(Vector3.Distance(camera.transform.position,director.fox.transform.position+Vector3.up*1.1f),4f,"Spawn camera is too close in region "+region);
                    Capture(camera,"StartupWorld"+region);
                }
        }
        private static void Capture(Camera camera,string name)
        {
            var target=new RenderTexture(960,540,24);target.Create();var previous=RenderTexture.active;
            var image=new Texture2D(960,540,TextureFormat.RGB24,false);
            try
            {
                for(int i=0;i<3;i++)RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;image.ReadPixels(new Rect(0,0,960,540),0,0);image.Apply();
                File.WriteAllBytes("ValidationResults/"+name+".png",image.EncodeToPNG());
                Debug.Log("PATI_RUNTIME_CAMERA "+name+" position="+camera.transform.position+" rotation="+camera.transform.eulerAngles);
                var pixels=image.GetPixels32();int different=0;var first=pixels[0];
                foreach(var p in pixels)if(Mathf.Abs(p.r-first.r)+Mathf.Abs(p.g-first.g)+Mathf.Abs(p.b-first.b)>45)different++;
                Assert.Greater(different,pixels.Length/5,"Camera produced a mostly uniform image: "+name);
            }
            finally {RenderTexture.active=previous;target.Release();Object.Destroy(target);Object.Destroy(image);}
        }
    }
}
#endif
