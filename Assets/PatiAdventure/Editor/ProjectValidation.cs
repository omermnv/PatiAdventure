using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace PatiAdventure.Editor
{
    public static class ProjectValidation
    {
        private static bool IsValidationProject => Application.dataPath.Replace('\\','/').Contains("/.codex-validation/UnityValidation-") || Application.dataPath.Replace('\\','/').Contains("/Temp/UnityValidation-");
        public static void CreateFixedPortalScene()
        {
            try
            {
                if(!IsValidationProject)throw new InvalidOperationException("Use the isolated validation project.");
                var active=SceneManager.GetActiveScene();
                if(string.IsNullOrEmpty(active.path))EditorSceneManager.SaveScene(active,AssetDatabase.GenerateUniqueAssetPath("Assets/PatiAdventure/CameraValidationEmpty.unity"));
                SunnyForestBuilder.CreatePortalAdventure();
                File.WriteAllText("ValidationResults/GeneratedPortalScene.txt",AssetDatabase.GetAssetPath(Selection.activeObject));
                Debug.Log("PATI_FIXED_PORTAL_SCENE_CREATED");
            }
            catch(Exception exception){Debug.LogException(exception);EditorApplication.Exit(1);}
        }
        public static void BuildLivingAndroid()
        {BuildPortalAndroid("PatiAdventure_Living_Fixed",3,true);}
        public static void BuildWorldPolishAndroid()
        {BuildPortalAndroid("PatiAdventure_World_Polish",4,false);}
        public static void BuildTerrainAndroid()
        {BuildPortalAndroid("PatiAdventure_Terrain_Ready",5,false);}
        public static void BuildMobileAndroid()
        {BuildPortalAndroid("PatiAdventure_Mobile_Ready",6,false);}
        public static void BuildDaylightAndroid()
        {BuildPortalAndroid("PatiAdventure_Daylight_Ready",7,false);}
        private static void BuildPortalAndroid(string outputName,int versionCode,bool generate)
        {
            try
            {
                if(!IsValidationProject)throw new InvalidOperationException("Android build must run in the isolated validation copy.");
                if(generate)CreateFixedPortalScene();
                string path=File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim();
                string company=PlayerSettings.companyName,product=PlayerSettings.productName;
                int originalQuality=QualitySettings.GetQualityLevel();
                int mobileQuality=Array.IndexOf(QualitySettings.names,"Mobile");if(mobileQuality<0)mobileQuality=originalQuality;
                QualitySettings.SetQualityLevel(mobileQuality,false);
                var originalPipeline=QualitySettings.renderPipeline;
                var originalBuildScenes=EditorBuildSettings.scenes;
                try
                {
                    // Register the scene's actual pipeline on the Android quality level before
                    // URP strips shader variants. Runtime switching alone cannot retain shadows.
                    QualitySettings.renderPipeline=ScenePipeline(path);
                    EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(path,true)};
                    PlayerSettings.companyName="DefaultCompany";PlayerSettings.productName="PatiAdventure";
                    PlayerSettings.Android.bundleVersionCode=versionCode;
                    PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
                    PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                    EditorUserBuildSettings.buildAppBundle=false;EditorUserBuildSettings.exportAsGoogleAndroidProject=false;
                    string apk="ValidationResults/"+outputName+".apk";
                    var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{path},locationPathName=apk,target=BuildTarget.Android,options=BuildOptions.None});
                    if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("Living Android APK build failed: "+report.summary.result);
                    string reportName=generate?"LivingFixedAndroidBuild":outputName+"Build";
                    File.WriteAllText("ValidationResults/"+reportName+".txt","Android APK built successfully.\nScene: "+path+"\nAPK size: "+new FileInfo(apk).Length+" bytes\nARM64 / IL2CPP. Version code "+versionCode+". Device installation and performance not tested.");
                }
                finally {PlayerSettings.companyName=company;PlayerSettings.productName=product;QualitySettings.SetQualityLevel(mobileQuality,false);QualitySettings.renderPipeline=originalPipeline;QualitySettings.SetQualityLevel(originalQuality,false);EditorBuildSettings.scenes=originalBuildScenes;}
                Debug.Log("PATI_LIVING_ANDROID_BUILD_PASSED");
            }
            catch(Exception exception){Debug.LogException(exception);EditorApplication.Exit(1);}
        }
        private static UnityEngine.Rendering.RenderPipelineAsset ScenePipeline(string path)
        {
            var scene=SceneManager.GetSceneByPath(path);bool opened=!scene.IsValid()||!scene.isLoaded;
            if(opened)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
            try
            {
                foreach(var root in scene.GetRootGameObjects())
                {var policy=root.GetComponentInChildren<MobileQualityPolicy>();if(policy&&policy.mobilePipeline)return policy.mobilePipeline;}
                throw new InvalidOperationException("The tested scene has no mobile rendering pipeline.");
            }
            finally {if(opened)EditorSceneManager.CloseScene(scene,true);}
        }
        public static void CapturePolishedPortals()
        {
            if(!File.Exists("ValidationResults/GeneratedScene.txt"))Run();
            CreateFixedPortalScene();
            CapturePortals();
        }
        public static void CapturePortals()
        {
            var originalPipeline=QualitySettings.renderPipeline;
            try
            {
                string path=File.ReadAllText("ValidationResults/GeneratedPortalScene.txt").Trim();
                var active=SceneManager.GetActiveScene();if(string.IsNullOrEmpty(active.path))EditorSceneManager.SaveScene(active,AssetDatabase.GenerateUniqueAssetPath("Assets/PatiAdventure/PortalCaptureEmpty.unity"));
                var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
                AdventureDirector director=null;Camera camera=null;
                foreach(var root in scene.GetRootGameObjects()){if(root.TryGetComponent<Camera>(out var c))camera=c;if(root.TryGetComponent<AdventureDirector>(out var d))director=d;}
                if(!camera||!director)throw new InvalidOperationException("Portal preview references missing.");
                var quality=director.GetComponent<MobileQualityPolicy>();if(quality&&quality.mobilePipeline)QualitySettings.renderPipeline=quality.mobilePipeline;
                var texture=new RenderTexture(1280,720,24);texture.Create();var previous=RenderTexture.active;
                var pipeline=UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
                bool batching=pipeline&&pipeline.useSRPBatcher;if(pipeline)pipeline.useSRPBatcher=false;
                for(int region=0;region<=3;region++)
                {
                    director.BuildRegionPreview(region);camera.transform.position=region==0?new Vector3(24,26,-8):new Vector3(21,24,-3);camera.transform.LookAt(new Vector3(0,0,20));camera.fieldOfView=65;
                    foreach(var label in director.GetComponentsInChildren<PortalLabel>())label.transform.rotation=camera.transform.rotation;
                    for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                    RenderTexture.active=texture;var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
                    File.WriteAllBytes("ValidationResults/PortalWorld"+region+".png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);
                    if(region==0)foreach(var portal in director.GetComponentsInChildren<AdventurePortal>())
                    {
                        var gate=portal.transform;camera.transform.position=gate.TransformPoint(new Vector3(5,4.3f,-10));camera.transform.LookAt(gate.TransformPoint(new Vector3(0,2.4f,0)));camera.fieldOfView=55;
                        foreach(var label in director.GetComponentsInChildren<PortalLabel>())label.transform.rotation=camera.transform.rotation;
                        for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                        RenderTexture.active=texture;var entrance=new Texture2D(1280,720,TextureFormat.RGB24,false);entrance.ReadPixels(new Rect(0,0,1280,720),0,0);entrance.Apply();File.WriteAllBytes("ValidationResults/Gateway"+portal.destination+".png",entrance.EncodeToPNG());UnityEngine.Object.DestroyImmediate(entrance);
                    }
                    if(region==1)
                    {
                        camera.transform.position=director.fox.transform.position+new Vector3(2.7f,1.9f,3.4f);camera.transform.LookAt(director.fox.transform.position+Vector3.up*.8f);camera.fieldOfView=50;
                        for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                        RenderTexture.active=texture;var portrait=new Texture2D(1280,720,TextureFormat.RGB24,false);portrait.ReadPixels(new Rect(0,0,1280,720),0,0);portrait.Apply();File.WriteAllBytes("ValidationResults/PortalFox.png",portrait.EncodeToPNG());UnityEngine.Object.DestroyImmediate(portrait);
                        camera.transform.position=director.fox.transform.position+new Vector3(3.1f,1.25f,-1.7f);camera.transform.LookAt(director.fox.transform.position+new Vector3(0,.85f,-.45f));camera.fieldOfView=42;
                        for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                        RenderTexture.active=texture;var tailPortrait=new Texture2D(1280,720,TextureFormat.RGB24,false);tailPortrait.ReadPixels(new Rect(0,0,1280,720),0,0);tailPortrait.Apply();File.WriteAllBytes("ValidationResults/FoxTailProfile.png",tailPortrait.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tailPortrait);
                    }
                    if(region>0)
                    {
                        var enemy=director.GetComponentInChildren<PortalEnemy>();
                        if(enemy)
                        {
                            camera.transform.position=enemy.transform.position+new Vector3(2.2f,1.2f,3.5f);camera.transform.LookAt(enemy.transform.position+Vector3.up*(region==3?.65f:.12f));camera.fieldOfView=48;
                            for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                            RenderTexture.active=texture;var enemyImage=new Texture2D(1280,720,TextureFormat.RGB24,false);enemyImage.ReadPixels(new Rect(0,0,1280,720),0,0);enemyImage.Apply();File.WriteAllBytes("ValidationResults/EnemyRegion"+region+".png",enemyImage.EncodeToPNG());UnityEngine.Object.DestroyImmediate(enemyImage);
                        }
                    }
                }
                RenderTexture.active=previous;if(pipeline)pipeline.useSRPBatcher=batching;texture.Release();UnityEngine.Object.DestroyImmediate(texture);EditorSceneManager.CloseScene(scene,true);
                Debug.Log("PATI_PORTAL_CAPTURE_PASSED");
            }
            catch(Exception exception){Debug.LogException(exception);EditorApplication.Exit(1);}
            finally {QualitySettings.renderPipeline=originalPipeline;}
        }
        public static void Capture()
        {
            try
            {
                string path=File.ReadAllText("ValidationResults/GeneratedScene.txt").Trim();
                var active=SceneManager.GetActiveScene();if(string.IsNullOrEmpty(active.path))EditorSceneManager.SaveScene(active,AssetDatabase.GenerateUniqueAssetPath("Assets/PatiAdventure/CaptureEmpty.unity"));
                var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
                Camera camera=null;foreach(var root in scene.GetRootGameObjects())if(root.TryGetComponent<Camera>(out var found))camera=found;
                if(!camera)throw new InvalidOperationException("Capture camera not found.");
                camera.transform.position=new Vector3(21,24,-1);camera.transform.LookAt(new Vector3(0,0,28));camera.fieldOfView=65;
                var texture=new RenderTexture(1280,720,24);texture.Create();
                var pipeline=UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
                bool batching=pipeline&&pipeline.useSRPBatcher;if(pipeline)pipeline.useSRPBatcher=false;
                for(int attempt=0;attempt<3;attempt++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=texture});
                if(pipeline)pipeline.useSRPBatcher=batching;
                var previous=RenderTexture.active;RenderTexture.active=texture;
                var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
                File.WriteAllBytes("ValidationResults/ForestOverview.png",image.EncodeToPNG());RenderTexture.active=previous;
                UnityEngine.Object.DestroyImmediate(image);texture.Release();UnityEngine.Object.DestroyImmediate(texture);EditorSceneManager.CloseScene(scene,true);
                Debug.Log("PATI_CAPTURE_PASSED");
            }
            catch(Exception exception){Debug.LogException(exception);EditorApplication.Exit(1);}
        }
        public static void CompileAndroid()
        {
            try
            {
                var settings=new UnityEditor.Build.Player.ScriptCompilationSettings
                {target=BuildTarget.Android,group=BuildTargetGroup.Android,options=UnityEditor.Build.Player.ScriptCompilationOptions.None};
                Directory.CreateDirectory("Temp/AndroidScriptValidation");
                var result=UnityEditor.Build.Player.PlayerBuildInterface.CompilePlayerScripts(settings,"Temp/AndroidScriptValidation");
                if(result.assemblies==null||result.assemblies.Count==0)throw new InvalidOperationException("Android script compilation produced no assemblies.");
                File.WriteAllText("ValidationResults/AndroidScripts.txt","Android player scripts compiled: "+result.assemblies.Count+" assemblies. APK and device test not performed.");
                Debug.Log("PATI_ANDROID_SCRIPTS_PASSED");
            }
            catch(Exception exception){Debug.LogException(exception);EditorApplication.Exit(1);}
        }
        // Run this only in the isolated validation project. No existing scene is overwritten.
        public static void Run()
        {
            try
            {
                // Unity starts batch mode with an untitled scene; additive scene creation
                // requires a saved active scene. Save only that disposable validation scene.
                var active=SceneManager.GetActiveScene();
                if(string.IsNullOrEmpty(active.path))EditorSceneManager.SaveScene(active,AssetDatabase.GenerateUniqueAssetPath("Assets/PatiAdventure/ValidationEmpty.unity"));
                SunnyForestBuilder.CreateAdventure();
                string path=AssetDatabase.GetAssetPath(Selection.activeObject);
                var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
                int scripts=0,actors=0;
                AdventureDirector director=null;
                foreach(var root in scene.GetRootGameObjects())
                {
                    foreach(var transform in root.GetComponentsInChildren<Transform>(true))
                    {
                        scripts+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                        if(transform.TryGetComponent<AdventureDirector>(out var found))director=found;
                    }
                    actors+=root.GetComponentsInChildren<AdventureActor>(true).Length;
                }
                if(scripts!=0||!director||!director.fox||!director.surfaceMaterial||actors<14)
                    throw new InvalidOperationException("Generated scene has missing references or insufficient gameplay actors.");
                Directory.CreateDirectory("ValidationResults");
                File.WriteAllText("ValidationResults/GeneratedScene.txt",path);
                File.WriteAllText("ValidationResults/SceneValidation.txt","Unity "+Application.unityVersion+"\nMissing scripts: "+scripts+"\nGameplay actors: "+actors+"\nScene: "+path);
                EditorSceneManager.CloseScene(scene,true);
                SunnyForestBuilder.CreatePortalAdventure();
                string portalPath=AssetDatabase.GetAssetPath(Selection.activeObject);
                var portalScene=EditorSceneManager.OpenScene(portalPath,OpenSceneMode.Additive);
                int portalCount=0,missing=0;
                foreach(var root in portalScene.GetRootGameObjects())
                {
                    portalCount+=root.GetComponentsInChildren<AdventurePortal>(true).Length;
                    foreach(var t in root.GetComponentsInChildren<Transform>(true))missing+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                }
                if(missing!=0||portalCount!=3)throw new InvalidOperationException("Portal village validation failed.");
                File.WriteAllText("ValidationResults/GeneratedPortalScene.txt",portalPath);
                File.WriteAllText("ValidationResults/PortalSceneValidation.txt","Missing scripts: "+missing+"\nVillage portals: "+portalCount+"\nScene: "+portalPath);
                EditorSceneManager.CloseScene(portalScene,true);
                Debug.Log("PATI_VALIDATION_PASSED: "+path);
            }
            catch(Exception exception)
            { Debug.LogException(exception);EditorApplication.Exit(1); }
        }
    }
}
