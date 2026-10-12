using UnityEngine;
using UnityEngine.Rendering;

namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        // Keep the shader as a scene dependency so Shader.Find is only a legacy fallback.
        // Runtime materials belong to the normal generated-material lifecycle.
        [SerializeField, HideInInspector] private Shader atmosphereSkyShader;

        private void ApplyRegionalAtmosphere()
        {
            bool canyon = region == 2, glacier = region == 3, forest = region == 1;
            var haze = canyon ? new Color(.83f, .70f, .55f)
                : glacier ? new Color(.72f, .83f, .91f)
                : forest ? new Color(.66f, .77f, .77f) : new Color(.72f, .81f, .82f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = canyon ? .010f : glacier ? .008f : .0085f;
            RenderSettings.fogColor = haze;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = glacier ? new Color(.59f, .71f, .85f)
                : canyon ? new Color(.63f, .66f, .73f) : new Color(.57f, .69f, .77f);
            RenderSettings.ambientEquatorColor = glacier ? new Color(.49f, .62f, .72f)
                : canyon ? new Color(.57f, .45f, .34f) : new Color(.47f, .55f, .41f);
            RenderSettings.ambientGroundColor = glacier ? new Color(.30f, .40f, .50f)
                : canyon ? new Color(.29f, .20f, .13f) : new Color(.22f, .26f, .17f);

            var sun = FindRegionalSun();
            if (sun)
            {
                sun.color = canyon ? new Color(1f, .86f, .67f)
                    : glacier ? new Color(.89f, .95f, 1f) : new Color(1f, .95f, .83f);
                sun.intensity = canyon ? 1.22f : glacier ? 1.15f : 1.25f;
                sun.shadows = LightShadows.Soft;
                sun.transform.rotation = canyon ? Quaternion.Euler(39f, -43f, 0)
                    : glacier ? Quaternion.Euler(43f, -28f, 0)
                    : forest ? Quaternion.Euler(47f, -35f, 0) : Quaternion.Euler(42f, -32f, 0);
                RenderSettings.sun = sun;
            }

            var shader = atmosphereSkyShader ? atmosphereSkyShader : Shader.Find("Skybox/Procedural");
            var camera = fox && fox.cameraTransform ? fox.cameraTransform.GetComponent<Camera>() : null;
            if (shader)
            {
                var sky = new Material(shader) { name = canyon ? "Canyon warm sky"
                    : glacier ? "Glacier clear sky" : forest ? "Forest morning sky" : "Village morning sky" };
                materials.Add(sky);
                sky.SetColor("_SkyTint", canyon ? new Color(.56f, .60f, .66f)
                    : glacier ? new Color(.45f, .57f, .71f) : new Color(.48f, .61f, .72f));
                // A downward-looking camera sees the ground hemisphere. Its default gray
                // exposed the void beyond the mountains; match it to the distant haze.
                sky.SetColor("_GroundColor", haze);
                sky.SetFloat("_AtmosphereThickness", canyon ? 1.15f : glacier ? .80f : .96f);
                sky.SetFloat("_SunSize", .025f);
                sky.SetFloat("_SunDisk", 1f);
                sky.EnableKeyword("_SUNDISK_SIMPLE");
                sky.SetFloat("_SunSizeConvergence", 5f);
                sky.SetFloat("_Exposure", 1.05f);
                RenderSettings.skybox = sky;
                if (!Application.isPlaying) atmosphereSkyShader = shader;
                if (camera) camera.clearFlags = CameraClearFlags.Skybox;
            }
            if (camera) camera.backgroundColor = haze;
        }

        private Light FindRegionalSun()
        {
            var configured = RenderSettings.sun;
            if (configured && configured.type == LightType.Directional && configured.isActiveAndEnabled
                && configured.gameObject.scene == gameObject.scene) return configured;
            Light selected = null;
            // Scene-local selection avoids changing lights from another additive scene.
            foreach (var root in gameObject.scene.GetRootGameObjects())
                foreach (var light in root.GetComponentsInChildren<Light>())
                    if (light.type == LightType.Directional && light.isActiveAndEnabled
                        && (!selected || light.intensity > selected.intensity)) selected = light;
            return selected;
        }
    }
}
