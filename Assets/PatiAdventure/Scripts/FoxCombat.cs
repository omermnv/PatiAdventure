using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace PatiAdventure
{
    [RequireComponent(typeof(FoxController))]
    public sealed class FoxCombat : MonoBehaviour
    {
        public AdventureDirector director;
        public float NextAttackAt {get;private set;}
        public float AttackStarted {get;private set;}=-10;
        public bool IsSpinning {get;private set;}
        public float AttackDuration=>IsSpinning?.55f:.38f;
        public float AttackFraction=>Mathf.Clamp01((Time.time-AttackStarted)/AttackDuration);
        public bool IsAttacking=>Time.time-AttackStarted<AttackDuration;
        private FoxController fox;
        private readonly Collider[] hits=new Collider[64];
        private readonly List<PortalEnemy> struck=new List<PortalEnemy>(16);
        private void Awake(){fox=GetComponent<FoxController>();}
        private void Update()
        {
            if(!fox.readKeyboard || Keyboard.current==null)return;
            if(Keyboard.current.jKey.wasPressedThisFrame)RequestAttack(false);
            if(Keyboard.current.kKey.wasPressedThisFrame)RequestAttack(true);
        }
        public bool RequestAttack(bool spin)
        {
            if(!director||!director.IsPlaying||fox.InputLocked||Time.time<NextAttackAt)return false;
            IsSpinning=spin;AttackStarted=Time.time;NextAttackAt=Time.time+(spin?.85f:.42f);
            var center=transform.position+Vector3.up*.8f+(spin?Vector3.zero:transform.forward*.85f);
            int count=Physics.OverlapSphereNonAlloc(center,spin?2.3f:1.35f,hits,~(1<<2),QueryTriggerInteraction.Collide);
            struck.Clear();
            for(int i=0;i<count;i++)
            {
                var enemy=hits[i].GetComponentInParent<PortalEnemy>();
                if(!enemy||enemy.director!=director||struck.Contains(enemy))continue;
                struck.Add(enemy);
                var delta=enemy.transform.position-transform.position;delta.y=0;
                if((spin||Vector3.Dot(transform.forward,delta.normalized)>.1f)&&enemy.HasClearAttackPath(fox))enemy.Hit(spin?1:2,transform.position);
            }
            director.ShowAttackBurst(center,spin);
            return true;
        }
    }
}
