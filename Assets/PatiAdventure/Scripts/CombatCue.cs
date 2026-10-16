using UnityEngine;
using UnityEngine.Rendering;
namespace PatiAdventure
{
    public sealed class CombatCue : MonoBehaviour
    {
        public Transform follow;
        public Vector3 followOffset=Vector3.up*.12f;
        public float duration=.4f,radius=1,arc=360,expansion=.3f;
        public bool warning;
        private float started;
        private LineRenderer line;
        private TextMesh label;
        private readonly Vector3[] points=new Vector3[49];
        private void Awake(){started=Time.time;}
        public void Configure(Material material,string caption,Transform cameraTransform)
        {
            line=gameObject.AddComponent<LineRenderer>();line.sharedMaterial=material;
            line.useWorldSpace=false;line.positionCount=points.Length;
            line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;line.numCornerVertices=2;
            if(!string.IsNullOrEmpty(caption))
            {
                var text=new GameObject("Combat caption");text.transform.SetParent(transform,false);
                label=text.AddComponent<TextMesh>();label.text=caption;label.fontSize=48;label.characterSize=.09f;
                label.anchor=TextAnchor.MiddleCenter;label.color=material.color;
                text.AddComponent<PortalLabel>().cameraTransform=cameraTransform;
            }
            Draw(0);
        }
        private void Draw(float t)
        {
            float r=radius*(1+expansion*t);
            line.widthMultiplier=warning?.045f+.075f*t:.10f*(1-t);
            for(int i=0;i<points.Length;i++)
            {
                float angle=(-arc*.5f+arc*i/(points.Length-1))*Mathf.Deg2Rad;
                points[i]=new Vector3(Mathf.Sin(angle)*r,0,Mathf.Cos(angle)*r);
            }
            line.SetPositions(points);
            if(label)label.transform.localPosition=Vector3.up*(.7f+t*.65f);
        }
        private void Update()
        {
            float t=(Time.time-started)/duration;
            if(t>=1){Destroy(gameObject);return;}
            if(follow)transform.position=follow.position+followOffset;
            if(line)Draw(t);
        }
    }
}
