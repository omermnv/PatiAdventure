using System.Collections.Generic;
using UnityEngine;
namespace PatiAdventure
{
    public sealed class CreatureMotion : MonoBehaviour
    {
        private PortalEnemy enemy;
        private Transform rig,head;
        private Vector3 home,lastPosition;
        private float stride,hitAt=-10;
        private Transform[] hips,knees,arms;
        private Transform[] eyes;
        private Vector3[] eyeScales;
        private Renderer[] renderers;
        private Color[] colors;
        private MaterialPropertyBlock block;
        private bool flashing;
        private void Awake(){block=new MaterialPropertyBlock();}
        public void HitFeedback(){hitAt=Time.time;flashing=true;}
        private void Start()
        {
            enemy=GetComponent<PortalEnemy>();rig=transform.Find("Enemy Rig");if(!rig){enabled=false;return;}home=rig.localPosition;lastPosition=transform.position;
            var h=new List<Transform>();var k=new List<Transform>();var a=new List<Transform>();
            foreach(var t in rig.GetComponentsInChildren<Transform>()){if(t.name.EndsWith(" Hip"))h.Add(t);if(t.name.EndsWith(" Knee"))k.Add(t);if(t.name.EndsWith("Arm"))a.Add(t);if(t.name=="Head Joint")head=t;}
            hips=h.ToArray();knees=k.ToArray();arms=a.ToArray();renderers=rig.GetComponentsInChildren<Renderer>();colors=new Color[renderers.Length];for(int i=0;i<colors.Length;i++)colors[i]=renderers[i].sharedMaterial.color;
            var eyelids=new List<Transform>();foreach(var t in rig.GetComponentsInChildren<Transform>())if(t.name=="Left Eye"||t.name=="Right Eye"||t.name=="Eye"||t.name=="Boar pupil")eyelids.Add(t);
            eyes=eyelids.ToArray();eyeScales=new Vector3[eyes.Length];for(int i=0;i<eyes.Length;i++)eyeScales[i]=eyes[i].localScale;
        }
        private void LateUpdate()
        {
            if(!rig||!enemy||!enemy.director||!enemy.director.IsPlaying||Time.deltaTime<=0)return;
            float speed=(transform.position-lastPosition).magnitude/Time.deltaTime;lastPosition=transform.position;
            float moving=Mathf.Clamp01(speed),hit=Mathf.Clamp01(1-(Time.time-hitAt)/.28f);stride+=Time.deltaTime*7*moving;
            rig.localPosition=home+Vector3.up*(Mathf.Abs(Mathf.Sin(stride))*.025f*moving-(enemy.IsPreparingAttack?.025f:0));
            rig.localRotation=Quaternion.Euler(enemy.IsPreparingAttack?-12:0,0,Mathf.Sin(hit*Mathf.PI)*12);
            if(head)head.localRotation=Quaternion.Euler(enemy.IsPreparingAttack?-18:Mathf.Sin(Time.time)*2,0,0);
            for(int i=0;i<hips.Length;i++)hips[i].localRotation=Quaternion.Euler(Mathf.Sin(stride+(i%2)*Mathf.PI)*moving*20,0,0);
            for(int i=0;i<knees.Length;i++)knees[i].localRotation=Quaternion.Euler(Mathf.Max(0,-Mathf.Sin(stride+(i%2)*Mathf.PI))*moving*15,0,0);
            for(int i=0;i<arms.Length;i++)arms[i].localRotation=Quaternion.Euler(enemy.IsPreparingAttack?-65:Mathf.Sin(stride+i*Mathf.PI)*moving*15,0,0);
            float blink=Mathf.Repeat(Time.time+home.x*.17f,4.7f),lid=blink<.14f?Mathf.Max(.08f,Mathf.Abs(blink-.07f)/.07f):1;
            for(int i=0;i<eyes.Length;i++){var scale=eyeScales[i];scale.y*=lid;eyes[i].localScale=scale;}
            if(flashing)
            {
                for(int i=0;i<renderers.Length;i++){block.Clear();block.SetColor("_BaseColor",Color.Lerp(colors[i],Color.white,hit*.65f));renderers[i].SetPropertyBlock(block);}
                if(Time.time-hitAt>=.35f){foreach(var renderer in renderers)renderer.SetPropertyBlock(null);flashing=false;}
            }
        }
    }
}
