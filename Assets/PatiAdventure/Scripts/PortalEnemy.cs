using UnityEngine;
namespace PatiAdventure
{
    public sealed class PortalEnemy : MonoBehaviour
    {
        public AdventureDirector director;
        public string enemyId;
        public int health=2,kind;
        public TextMesh healthDisplay;
        public bool IsDefeated {get;private set;}
        public bool IsPreparingAttack=>attackAt>=0;
        private Vector3 home,originalScale=Vector3.one;
        private float stunnedUntil,attackAt=-1,defeatedAt;
        private CombatCue attackWarning;
        private int maximumHealth;
        private readonly RaycastHit[] obstructionHits=new RaycastHit[32];
        private void Start(){home=transform.position;originalScale=transform.localScale;maximumHealth=health;ShowHealth();}
        private void ShowHealth(){if(healthDisplay){healthDisplay.text=IsDefeated?"":new string('■',Mathf.Max(0,health))+new string('□',Mathf.Max(0,maximumHealth-health));healthDisplay.color=new Color(1,.8f,.3f);}}
        private void CancelWarning(){if(attackWarning)Destroy(attackWarning.gameObject);attackWarning=null;}
        private void OnDisable(){CancelWarning();attackAt=-1;}
        private bool IsSolidCover(Collider collider)
        {
            return collider&&!collider.isTrigger&&!collider.GetComponentInParent<FoxController>()&&!collider.GetComponentInParent<PortalEnemy>();
        }
        public bool HasClearAttackPath(FoxController target)
        {
            if(!target)return false;
            // Enemy origins sit at torso height; trace to the fox's chest rather than its feet.
            var chest=target.transform.position+Vector3.up*.8f;
            var from=transform.position+Vector3.up*.2f;
            var delta=chest-from;if(delta.sqrMagnitude<.0001f)return true;
            int count=Physics.RaycastNonAlloc(from,delta.normalized,obstructionHits,delta.magnitude,target.groundMask,QueryTriggerInteraction.Ignore);
            if(count==obstructionHits.Length)return false;
            for(int i=0;i<count;i++)if(IsSolidCover(obstructionHits[i].collider))return false;
            return true;
        }
        public void Hit(int damage,Vector3 origin)
        {
            if(IsDefeated||!director||!director.IsPlaying)return;
            if(damage<=0)return;
            int dealt=Mathf.Min(health,damage);
            health=Mathf.Max(0,health-damage);stunnedUntil=Time.time+.65f;attackAt=-1;CancelWarning();
            GetComponent<CreatureMotion>()?.HitFeedback();transform.localScale=originalScale;
            ShowHealth();
            var away=transform.position-origin;away.y=0;
            if(away.sqrMagnitude>.0001f)MoveAlongGround(away.normalized,.7f,true);
            if(health<=0){IsDefeated=true;defeatedAt=Time.time;director.EnemyDefeated(enemyId);foreach(var c in GetComponentsInChildren<Collider>())c.enabled=false;}
            ShowHealth();director.ShowEnemyImpact(transform.position,dealt,IsDefeated);
        }
        private void Update()
        {
            if(!director||!director.IsPlaying)return;
            if(IsDefeated){transform.localScale=originalScale*Mathf.Max(0,1-(Time.time-defeatedAt)/.4f);if(Time.time-defeatedAt>.4f)gameObject.SetActive(false);return;}
            if(Time.time<stunnedUntil)return;
            var target=director.fox.transform.position;bool sameHeight=Mathf.Abs(target.y+.8f-transform.position.y)<1.1f;target.y=transform.position.y;
            float distance=Vector3.Distance(transform.position,target);
            if(attackAt>=0)
            {
                float pulse=1+Mathf.Sin((Time.time-attackAt)*25)*.08f;
                transform.localScale=originalScale*pulse;
                if(Time.time>=attackAt+.8f){if(distance<1.9f&&sameHeight&&HasClearAttackPath(director.fox))director.Bump();CancelWarning();attackAt=-1;stunnedUntil=Time.time+1.2f;transform.localScale=originalScale;ShowHealth();}
                return;
            }
            if(distance<1.55f&&sameHeight&&HasClearAttackPath(director.fox)){attackAt=Time.time;attackWarning=director.ShowEnemyWarning(transform.position);if(healthDisplay){healthDisplay.text="!";healthDisplay.color=new Color(1,.3f,.12f);}return;}
            if(distance<7&&Vector3.Distance(target,home)<9)
                MoveOnGround(target,kind==2?1.6f:1.1f);
            else MoveOnGround(home+new Vector3(Mathf.Sin(Time.time*.8f+home.x)*1.2f,0,0),.65f);
            Vector3 direction=target-transform.position;direction.y=0;
            if(direction.sqrMagnitude>.01f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(direction),100*Time.deltaTime);
        }
        private void MoveOnGround(Vector3 target,float speed)
        {
            var direction=target-transform.position;direction.y=0;if(direction.sqrMagnitude<.0001f)return;
            float distance=Mathf.Min(direction.magnitude,speed*Time.deltaTime);direction.Normalize();
            MoveAlongGround(direction,distance,false);
        }
        private void MoveAlongGround(Vector3 direction,float distance,bool knockback)
        {
            float radius=.3f;
            var center=transform.position+Vector3.up*.1f;
            if(knockback&&TryGetComponent<SphereCollider>(out var body))
            {
                var scale=transform.lossyScale;
                radius=Mathf.Max(radius,body.radius*Mathf.Max(Mathf.Abs(scale.x),Mathf.Abs(scale.y),Mathf.Abs(scale.z)));
                center=transform.TransformPoint(body.center);
            }
            // Large guardian triggers extend below the floor. Start the sweep clear of its
            // supporting surface so an initial overlap cannot masquerade as a wall hit.
            if(knockback&&Physics.Raycast(center+Vector3.up*2,Vector3.down,out var support,5,director.fox.groundMask,QueryTriggerInteraction.Ignore)&&support.normal.y>=.65f)
                center.y=Mathf.Max(center.y,support.point.y+(radius+.025f)/support.normal.y);
            int count=Physics.SphereCastNonAlloc(center,radius,direction,obstructionHits,distance+.06f,director.fox.groundMask,QueryTriggerInteraction.Ignore);
            if(count==obstructionHits.Length)return;
            for(int i=0;i<count;i++)
            {
                var obstacle=obstructionHits[i];
                if(IsSolidCover(obstacle.collider)&&obstacle.normal.y<.65f)distance=Mathf.Min(distance,Mathf.Max(0,obstacle.distance-.06f));
            }
            if(distance<=.0001f)return;
            var next=transform.position+direction*distance;
            if(!Physics.Raycast(next+Vector3.up*2,Vector3.down,out var ground,5,director.fox.groundMask,QueryTriggerInteraction.Ignore)||ground.normal.y<.65f)return;
            float y=ground.point.y+.9f;if(y-transform.position.y>.35f||(knockback&&transform.position.y-y>.35f))return;
            next.y=knockback?y:Mathf.MoveTowards(transform.position.y,y,4*Time.deltaTime);transform.position=next;
        }
    }
}
