using UnityEngine;
using UnityEngine.Rendering;
namespace PatiAdventure
{
    public sealed class MobileQualityPolicy : MonoBehaviour
    {
        public RenderPipelineAsset mobilePipeline;
        private RenderPipelineAsset previous=null;
        private bool applied=false;
        private void Awake()
        {
            DebugManager.instance.displayRuntimeUI=false;
            DebugManager.instance.displayPersistentRuntimeUI=false;
            DebugManager.instance.enableRuntimeUI=false;
            // Use the scene's own profile in Editor too; never modify the project's profiles.
            if(mobilePipeline){previous=QualitySettings.renderPipeline;QualitySettings.renderPipeline=mobilePipeline;applied=true;}
#if UNITY_ANDROID || UNITY_IOS
            QualitySettings.vSyncCount=0;Application.targetFrameRate=30;
            Screen.autorotateToPortrait=Screen.autorotateToPortraitUpsideDown=false;
            Screen.autorotateToLandscapeLeft=Screen.autorotateToLandscapeRight=true;Screen.orientation=ScreenOrientation.AutoRotation;
#endif
        }
        private void OnDestroy() { if(applied)QualitySettings.renderPipeline=previous; }
    }
}
