using UnityEngine;
namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private Material attackCueMaterial,dangerCueMaterial,protectionCueMaterial;
        private Material CueMaterial(int type)
        {
            if(type==1)return dangerCueMaterial?dangerCueMaterial:(dangerCueMaterial=ColorMaterial(new Color(1,.29f,.12f)));
            if(type==2)return protectionCueMaterial?protectionCueMaterial:(protectionCueMaterial=ColorMaterial(new Color(.28f,.82f,1)));
            return attackCueMaterial?attackCueMaterial:(attackCueMaterial=ColorMaterial(new Color(1,.82f,.30f)));
        }
        private CombatCue CombatVisual(string name,Vector3 position,int type,float radius,float duration,string caption="")
        {
            if(!world)return null;
            var obj=new GameObject(name);obj.transform.SetParent(world,false);obj.transform.position=position;
            var cue=obj.AddComponent<CombatCue>();cue.radius=radius;cue.duration=duration;
            cue.Configure(CueMaterial(type),caption,fox?fox.cameraTransform:null);return cue;
        }
        public CombatCue ShowEnemyWarning(Vector3 position)
        {
            if(fox&&Physics.Raycast(position+Vector3.up,Vector3.down,out var hit,4,fox.groundMask,QueryTriggerInteraction.Ignore))position.y=hit.point.y+.07f;
            else position.y-=.8f;
            var cue=CombatVisual("Enemy attack warning",position,1,1.9f,.8f,"!");
            if(cue){cue.warning=true;cue.expansion=0;}return cue;
        }
        public void ShowEnemyImpact(Vector3 position,int damage,bool defeated)
        {
            var cue=CombatVisual("Enemy hit feedback",position,0,.35f,defeated?.65f:.4f,defeated?L("Dağıldı!","Cleared!"):"−"+damage);
            if(cue)cue.expansion=defeated?4:2;
        }
        public void ShowHazardMarker(AdventureActor actor)
        {
            var collider=actor.GetComponent<Collider>();if(!collider)return;
            var bounds=collider.bounds;
            var position=new Vector3(bounds.center.x,bounds.min.y+.07f,bounds.center.z);
            var cue=CombatVisual("Hazard boundary",position,1,1,float.PositiveInfinity);
            if(!cue)return;
            cue.warning=true;cue.expansion=0;
            cue.transform.localScale=actor.actorId=="pond"?new Vector3(6,1,4):new Vector3(Mathf.Max(.4f,bounds.extents.x),1,Mathf.Max(.4f,bounds.extents.z));
            cue.follow=actor.transform;cue.followOffset=position-actor.transform.position;
        }
        private void ShowSafeReturn(){CombatVisual("Player hurt feedback",fox.transform.position+Vector3.up*.15f,1,.6f,.4f);}
        private void ShowReturnProtection()
        {
            var cue=CombatVisual("Safe return protection",fox.transform.position+Vector3.up*.12f,2,.85f,2,L("Korunuyorsun","Protected"));
            if(cue){cue.follow=fox.transform;cue.expansion=0;cue.warning=true;}
        }
    }
}
