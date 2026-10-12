using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class WaterSurfaceMotion : MonoBehaviour
    {
        private MeshRenderer surface;
        private MaterialPropertyBlock properties;
        private static readonly int TextureTransform=Shader.PropertyToID("_BaseMap_ST");
        private void Awake(){surface=GetComponent<MeshRenderer>();properties=new MaterialPropertyBlock();}
        private void Update()
        {
            if(Time.deltaTime<=0)return;
            surface.GetPropertyBlock(properties);
            properties.SetVector(TextureTransform,new Vector4(3,3,Mathf.Repeat(Time.time*.015f,1),Mathf.Repeat(Time.time*.009f,1)));
            surface.SetPropertyBlock(properties);
        }
    }
}
