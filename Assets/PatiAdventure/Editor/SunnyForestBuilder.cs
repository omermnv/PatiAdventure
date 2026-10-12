using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
namespace PatiAdventure.Editor
{
    public static class SunnyForestBuilder
    {
        [MenuItem("PatiAdventure/Create SunnyForest")]
        public static void Create() { Build("SunnyForest"); }
        [MenuItem("PatiAdventure/Create Story Adventure")]
        public static void CreateAdventure() { Build("PatiStory"); }
        [MenuItem("PatiAdventure/Create Portal Adventure")]
        public static void CreatePortalAdventure() { Build("PatiPortals",true); }

        private static void Build(string sceneName,bool portals=false)
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            if(string.IsNullOrEmpty(SceneManager.GetActiveScene().path))
                throw new System.InvalidOperationException("Açık adsız sahneyi önce kaydedin. Üretici mevcut sahnenizi kapatmadan yeni sahne oluşturur.");
            var shader=Shader.Find("Universal Render Pipeline/Lit");
            if(!shader)throw new System.InvalidOperationException("URP Lit shader bulunamadı.");
            const string root="Assets/PatiAdventure/Generated";
            if(!AssetDatabase.IsValidFolder(root))AssetDatabase.CreateFolder("Assets/PatiAdventure","Generated");
            string path=AssetDatabase.GenerateUniqueAssetPath(root+"/"+sceneName+".unity");
            string assets=AssetDatabase.GenerateUniqueAssetPath(root+"/"+sceneName+"Assets");
            AssetDatabase.CreateFolder(root,System.IO.Path.GetFileName(assets));
            var grass=Material(assets,"Grass",new Color(.35f,.75f,.24f),shader);
            var orange=Material(assets,"Fox",new Color(.7f,.29f,.11f),shader);
            var cream=Material(assets,"Cream",new Color(.82f,.77f,.64f),shader);
            var dark=Material(assets,"Eyes",new Color(.08f,.06f,.08f),shader);
            Scene previous=SceneManager.GetActiveScene();
            Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
            AssetDatabase.StartAssetEditing();
            try
            {
                SceneManager.SetActiveScene(scene);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.65f,.75f,.85f);
                var sun=new GameObject("Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.3f;sun.shadows=LightShadows.Hard;
                sun.transform.rotation=Quaternion.Euler(45,-30,0);
                var fox=new GameObject("Pati the Fox");fox.layer=2;fox.transform.position=new Vector3(0,.15f,-3);
                var collider=fox.AddComponent<CharacterController>();collider.height=1.6f;collider.radius=.35f;collider.center=new Vector3(0,.8f,0);collider.stepOffset=.35f;
                var player=fox.AddComponent<FoxController>();player.groundMask=~(1<<2);
                fox.AddComponent<FoxVisualFeedback>();
                var visual=new GameObject("Fox Visual Pose").transform;visual.SetParent(fox.transform,false);
                FoxPart(visual,"Body",new Vector3(0,.65f,0),new Vector3(.75f,.8f,.9f),orange);
                FoxPart(visual,"Head",new Vector3(0,1.25f,.25f),new Vector3(.85f,.7f,.7f),orange);
                FoxPart(visual,"Muzzle",new Vector3(0,1.08f,.58f),new Vector3(.5f,.3f,.35f),cream);
                FoxPart(visual,"Nose",new Vector3(0,1.17f,.76f),Vector3.one*.13f,dark);
                for(int s=-1;s<=1;s+=2)
                {
                    FoxPart(visual,"Ear",new Vector3(s*.29f,1.65f,.2f),new Vector3(.23f,.42f,.23f),orange);
                    FoxPart(visual,"Eye",new Vector3(s*.22f,1.32f,.56f),Vector3.one*.13f,dark);
                    FoxPart(visual,"Foot",new Vector3(s*.23f,.2f,.05f),new Vector3(.23f,.3f,.4f),dark);
                }
                FoxPart(visual,"Tail",new Vector3(0,.65f,-.7f),new Vector3(.4f,.4f,1),orange);
                FoxPart(visual,"Tail tip",new Vector3(0,.65f,-1.15f),Vector3.one*.35f,cream);
                if(portals)
                {
                    // These parts belong only to the new, unsaved generated scene.
                    while(visual.childCount>0)Object.DestroyImmediate(visual.GetChild(0).gameObject);
                    orange.color=new Color(.74f,.3f,.105f);cream.color=new Color(.93f,.88f,.75f);dark.color=new Color(.07f,.046f,.027f);
                    orange.SetFloat("_Smoothness",.2f);cream.SetFloat("_Smoothness",.17f);dark.SetFloat("_Smoothness",.2f);
                    var furTexture=LivingSurface.Fur();AssetDatabase.CreateAsset(furTexture,assets+"/FoxFur.asset");orange.SetTexture("_BaseMap",furTexture);
                    cream.SetTexture("_BaseMap",furTexture);dark.SetTexture("_BaseMap",furTexture);
                    var inner=Material(assets,"Ear Interior",new Color(.46f,.24f,.22f),shader);var amber=Material(assets,"Amber Eyes",new Color(.85f,.48f,.09f),shader);amber.SetFloat("_Smoothness",.6f);
                    int sculptIndex=0;CharacterSculptor.BuildFox(visual,orange,cream,dark,inner,amber,mesh=>AssetDatabase.CreateAsset(mesh,assets+"/FoxSculpt"+(sculptIndex++)+".asset"));
                }
                fox.GetComponent<FoxVisualFeedback>().visualRoot=visual;
                var camera=new GameObject("Main Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.nearClipPlane=.1f;camera.farClipPlane=120;
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.55f,.82f,1);
                camera.transform.position=fox.transform.position+new Vector3(0,4.7f,-7);camera.transform.LookAt(fox.transform.position+Vector3.up);
                camera.gameObject.AddComponent<AudioListener>();var follow=camera.gameObject.AddComponent<FollowCamera>();follow.target=fox.transform;follow.collisionMask=~(1<<2);player.cameraTransform=camera.transform;
                camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing=false;
                var adventure=new GameObject("Story Adventure").AddComponent<AdventureDirector>();adventure.fox=player;adventure.surfaceMaterial=grass;
                adventure.portalAdventure=portals;
                if(portals){fox.AddComponent<FoxCombat>().director=adventure;fox.AddComponent<FoxLocomotionPose>();}
                adventure.gameObject.AddComponent<PatiSoundFeedback>();
                var mobile=new GameObject("Mobile Controls").AddComponent<MobileControls>();mobile.fox=player;mobile.followCamera=follow;mobile.director=adventure;
                var quality=adventure.gameObject.AddComponent<MobileQualityPolicy>();
                var source=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/Mobile_RPAsset.asset");
                if(source)
                {
                    var pipeline=Object.Instantiate(source);pipeline.name="Pati Mobile 30fps";pipeline.supportsHDR=false;pipeline.renderScale=.8f;pipeline.msaaSampleCount=1;pipeline.useSRPBatcher=false;
                    pipeline.shadowDistance=portals?45:25;pipeline.shadowCascadeCount=1;pipeline.mainLightShadowmapResolution=portals?1024:512;
                    if(portals){pipeline.renderScale=1;pipeline.msaaSampleCount=2;}
                    var serialized=new SerializedObject(pipeline);serialized.FindProperty("m_AdditionalLightsRenderingMode").intValue=0;serialized.FindProperty("m_AdditionalLightsPerObjectLimit").intValue=0;serialized.ApplyModifiedPropertiesWithoutUndo();
                    if(portals){serialized.FindProperty("m_SoftShadowsSupported").boolValue=true;serialized.FindProperty("m_SoftShadowQuality").intValue=(int)SoftShadowQuality.Low;serialized.ApplyModifiedPropertiesWithoutUndo();}
                    AssetDatabase.CreateAsset(pipeline,assets+"/PatiMobilePipeline.asset");quality.mobilePipeline=pipeline;
                }
                adventure.BuildEditorPreview();int index=0;
                foreach(var texture in adventure.GeneratedTextures)AssetDatabase.CreateAsset(texture,assets+"/Surface"+(index++)+".asset");
                index=0;
                foreach(var material in adventure.GeneratedMaterials)AssetDatabase.CreateAsset(material,assets+"/ForestMaterial"+(index++)+".mat");
                index=0;foreach(var mesh in adventure.GeneratedMeshes)AssetDatabase.CreateAsset(mesh,assets+"/ForestMesh"+(index++)+".asset");
                if(!EditorSceneManager.SaveScene(scene,path))throw new System.IO.IOException("Orman sahnesi kaydedilemedi.");
                Debug.Log("Pati sahnesi hazır: "+path);
            }
            finally
            { try{AssetDatabase.StopAssetEditing();}finally{if(previous.IsValid())SceneManager.SetActiveScene(previous);EditorSceneManager.CloseScene(scene,true);} }
            AssetDatabase.SaveAssets();
            Selection.activeObject=AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        }
        private static Material Material(string folder,string name,Color color,Shader shader)
        { var material=new Material(shader){color=color,enableInstancing=true};AssetDatabase.CreateAsset(material,folder+"/"+name+".mat");return material; }
        private static void FoxPart(Transform parent,string name,Vector3 position,Vector3 scale,Material material)
        {
            var obj=GameObject.CreatePrimitive(PrimitiveType.Sphere);obj.name=name;Object.DestroyImmediate(obj.GetComponent<Collider>());
            obj.transform.SetParent(parent,false);obj.transform.localPosition=position;obj.transform.localScale=scale;obj.GetComponent<Renderer>().sharedMaterial=material;
        }
    }
}
