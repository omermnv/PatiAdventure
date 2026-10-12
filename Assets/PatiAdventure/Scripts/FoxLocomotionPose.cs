using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(FoxController))]
    public sealed class FoxLocomotionPose : MonoBehaviour
    {
        private FoxController fox;
        private FoxCombat combat;
        private CharacterController character;
        private Transform visual,head,tail,tip;
        private Transform[] feet;
        private Vector3[] origins;
        private Vector3 headOrigin;
        private float stride;
        private Transform rig,jaw,leftEye,rightEye,leftEar,rightEar,tailMiddle,tailEnd;
        private Transform[] hips,knees,paws;
        private Vector3 rigOrigin;
        private Quaternion headRest,tailRest;
        private float terrainPitch,terrainHeight;
        private readonly Transform[] pawSupport=new Transform[4];
        private readonly Vector3[] pawAnchor=new Vector3[4],pawNormal=new Vector3[4];
        private readonly Vector3[] soleOffsets=new Vector3[4];
        private void Start()
        {
            fox=GetComponent<FoxController>();combat=GetComponent<FoxCombat>();character=GetComponent<CharacterController>();visual=transform.Find("Fox Visual Pose");
            if(!visual)return;head=visual.Find("Head");tail=visual.Find("Tail");tip=visual.Find("Tail tip");if(head)headOrigin=head.localPosition;
            var list=new System.Collections.Generic.List<Transform>();foreach(Transform part in visual)if(part.name=="Foot")list.Add(part);
            feet=list.ToArray();origins=new Vector3[feet.Length];for(int i=0;i<feet.Length;i++)origins[i]=feet[i].localPosition;
            rig=visual.Find("Fox Rig");
            if(rig)
            {
                rigOrigin=rig.localPosition;head=Find(rig,"Head Joint");headOrigin=head.localPosition;headRest=head.localRotation;
                jaw=Find(rig,"Jaw Joint");leftEye=Find(rig,"Left Eye");rightEye=Find(rig,"Right Eye");leftEar=Find(rig,"Left Ear");rightEar=Find(rig,"Right Ear");
                tail=Find(rig,"Tail Base");tailRest=tail.localRotation;tailMiddle=Find(rig,"Tail Middle");tailEnd=Find(rig,"Tail Tip");
                string[] names={"Front Left","Front Right","Back Left","Back Right"};hips=new Transform[4];knees=new Transform[4];paws=new Transform[4];
                for(int i=0;i<4;i++){hips[i]=Find(rig,names[i]+" Hip");knees[i]=Find(rig,names[i]+" Knee");paws[i]=Find(rig,names[i]+" Paw");var pad=Find(paws[i],"Paw pad");soleOffsets[i]=paws[i].InverseTransformPoint(pad.TransformPoint(Vector3.down*.5f));}
            }
        }
        private static Transform Find(Transform root,string name){foreach(var t in root.GetComponentsInChildren<Transform>())if(t.name==name)return t;return null;}
        private void LateUpdate()
        {
            if(!visual)return;
            if(rig){AnimateRig();return;}
            if(fox.InputLocked)return;
            float speed=fox.PlanarVelocity.magnitude;stride+=Time.deltaTime*speed*3;
            float swing=Mathf.Sin(stride)*Mathf.Clamp01(speed);
            for(int i=0;i<feet.Length;i++){feet[i].localPosition=origins[i]+new Vector3(0,Mathf.Max(0,swing*(i%2==0?1:-1))*.12f,swing*(i%2==0?1:-1)*.2f);}
            float attack=combat&&combat.IsAttacking?Mathf.Sin((Time.time-combat.AttackStarted)/.32f*Mathf.PI):0;
            if(head)head.localPosition=headOrigin+Vector3.forward*attack*.2f;
            float tailAngle=Mathf.Sin(Time.time*4)*12;
            if(tail)tail.localRotation=Quaternion.Euler(0,tailAngle,0);
            if(tip)tip.localPosition=new Vector3(Mathf.Sin(tailAngle*Mathf.Deg2Rad)*-.6f,.65f,-1.15f);
            visual.localRotation=combat&&combat.IsAttacking&&combat.IsSpinning?Quaternion.Euler(0,attack*330,0):Quaternion.Euler(0,0,swing*3);
        }
        private void AnimateRig()
        {
            if(Time.deltaTime<=0)return;
            float speed=fox.InputLocked?0:fox.PlanarVelocity.magnitude,moving=Mathf.Clamp01(speed/2),run=Mathf.Clamp01((speed-3)/2);
            stride+=Time.deltaTime*Mathf.Lerp(6,12,run)*moving;
            float bob=fox.IsGrounded?Mathf.Abs(Mathf.Sin(stride))*moving*.025f:0;
            UpdateTerrainPose();
            rig.localPosition=rigOrigin+Vector3.up*(-.16f+terrainHeight+bob+Mathf.Sin(Time.time*2.5f)*.007f);
            bool attack=combat&&combat.IsAttacking;float t=attack?combat.AttackFraction:0;
            float bite=attack&&!combat.IsSpinning?Mathf.Sin(t*Mathf.PI):0;
            float turn=attack&&combat.IsSpinning?Mathf.SmoothStep(0,1,t)*360:0;
            float bodyPitch=bite*5+terrainPitch;
            rig.localRotation=Quaternion.Euler(bodyPitch,turn,Mathf.Sin(stride)*moving*1.5f);
            var bodyPivot=Vector3.up*.85f;rig.localPosition+=bodyPivot-rig.localRotation*bodyPivot;
            head.localPosition=headOrigin+Vector3.forward*(bite*.16f);
            head.localRotation=headRest*Quaternion.Euler(-bite*9,Mathf.Sin(Time.time*.9f)*3*(1-moving),0);
            // Open, then close on the lunge, instead of moving a solid muzzle block.
            float gape=attack&&!combat.IsSpinning?Mathf.Sin(Mathf.Clamp01(t/.65f)*Mathf.PI)*32:Mathf.Sin(Time.time*2)*.7f;
            if(jaw)jaw.localRotation=Quaternion.Euler(gape,0,0);
            for(int i=0;i<4;i++)
            {
                float phase=stride+((i==0||i==3)?0:Mathf.PI),swing=Mathf.Sin(phase)*moving;
                float upper=swing*Mathf.Lerp(19,30,run),lower=Mathf.Max(0,-swing)*26;
                if(!fox.IsGrounded&&moving>0){upper=i<2?-18:24;lower=22;}
                hips[i].localRotation=Quaternion.Euler(upper-(fox.IsGrounded?bodyPitch:0),0,0);knees[i].localRotation=Quaternion.Euler(lower,0,0);paws[i].localRotation=Quaternion.Euler(-upper-lower,0,0);
                if(fox.IsGrounded&&(!attack||!combat.IsSpinning))PlacePaw(i,Mathf.Max(0,-Mathf.Cos(phase))*.12f*moving);
                else pawSupport[i]=null;
            }
            float wag=Mathf.Sin(Time.time*3.1f)*Mathf.Lerp(13,7,moving);
            tail.localRotation=tailRest*Quaternion.Euler(0,wag+(attack&&combat.IsSpinning?45*Mathf.Sin(t*Mathf.PI):0),0);
            tailMiddle.localRotation=Quaternion.Euler(-15,Mathf.Sin(Time.time*3.1f-.45f)*10,0);
            tailEnd.localRotation=Quaternion.Euler(-18,Mathf.Sin(Time.time*3.1f-.9f)*12,0);
            float blink=Mathf.Repeat(Time.time+1.7f,4.6f);float eyelid=blink<.14f?Mathf.Max(.08f,Mathf.Abs(blink-.07f)/.07f):1;
            leftEye.localScale=rightEye.localScale=new Vector3(1,eyelid,1);
            leftEar.localRotation=Quaternion.Euler(-12+Mathf.Sin(Time.time*.7f)*3,0,13);rightEar.localRotation=Quaternion.Euler(-12+Mathf.Sin(Time.time*.7f+.8f)*3,0,-13);
        }
        private void UpdateTerrainPose()
        {
            float pitch=0,height=0;
            if(fox.IsGrounded)
            {
                var origin=transform.position+Vector3.up*.7f;
                if(Physics.Raycast(origin+transform.forward*.4f,Vector3.down,out var front,1.35f,fox.groundMask,QueryTriggerInteraction.Ignore)
                    &&Physics.Raycast(origin-transform.forward*.48f,Vector3.down,out var back,1.35f,fox.groundMask,QueryTriggerInteraction.Ignore))
                {
                    // A capsule can already stand on the next step while its centre ray sees the previous one.
                    float supportHeight=transform.TransformPoint(character.center).y-character.height*.5f-character.skinWidth;
                    pitch=-Mathf.Clamp(Mathf.Atan2(front.point.y-back.point.y,.88f)*Mathf.Rad2Deg,-40,40);height=Mathf.Clamp((front.point.y+back.point.y)*.5f-supportHeight,-.16f,.12f);
                }
            }
            float blend=1-Mathf.Exp(-Time.deltaTime*18);terrainPitch=Mathf.Lerp(terrainPitch,pitch,blend);terrainHeight=Mathf.Lerp(terrainHeight,height,blend);
        }
        private void PlacePaw(int index,float lift)
        {
            var hip=hips[index];var knee=knees[index];var paw=paws[index];
            var sole=Vector3.Scale(soleOffsets[index],paw.lossyScale);var probe=paw.TransformPoint(soleOffsets[index]);Vector3 point,normal;
            var anchor=pawSupport[index]?pawSupport[index].TransformPoint(pawAnchor[index]):probe;
            var drift=anchor-probe;drift.y=0;
            float reach=Vector3.Distance(hip.position,knee.position)+Vector3.Distance(knee.position,paw.position);
            if(lift<.015f&&pawSupport[index]&&drift.sqrMagnitude<.35f*.35f&&Vector3.Distance(hip.position,anchor-Vector3.up*sole.y)<reach+.005f)
            {point=anchor;normal=pawSupport[index].TransformDirection(pawNormal[index]).normalized;}
            else
            {
                pawSupport[index]=null;
                if(!Physics.Raycast(probe+Vector3.up*.65f,Vector3.down,out var ground,1.35f,fox.groundMask,QueryTriggerInteraction.Ignore)||ground.normal.y<.55f)return;
                point=ground.point;normal=ground.normal;
                if(lift<.015f){pawSupport[index]=ground.transform;pawAnchor[index]=ground.transform.InverseTransformPoint(point);pawNormal[index]=ground.transform.InverseTransformDirection(normal);}
            }
            var forward=Vector3.ProjectOnPlane(transform.forward,normal);if(forward.sqrMagnitude<.01f)return;
            var soleRotation=Quaternion.LookRotation(forward,normal);
            var target=point-soleRotation*sole+Vector3.up*lift;
            float upper=Vector3.Distance(hip.position,knee.position),lower=Vector3.Distance(knee.position,paw.position);
            var delta=target-hip.position;float distance=Mathf.Clamp(delta.magnitude,Mathf.Abs(upper-lower)+.005f,upper+lower-.003f);var direction=delta.normalized;
            var bend=Vector3.ProjectOnPlane(transform.forward*(index<2?-1:1),direction).normalized;
            if(bend.sqrMagnitude<.01f)bend=transform.forward;
            float along=(upper*upper-lower*lower+distance*distance)/(2*distance);
            var joint=hip.position+direction*along+bend*Mathf.Sqrt(Mathf.Max(0,upper*upper-along*along));
            hip.rotation=Quaternion.FromToRotation(knee.position-hip.position,joint-hip.position)*hip.rotation;
            knee.rotation=Quaternion.FromToRotation(paw.position-knee.position,target-knee.position)*knee.rotation;
            paw.rotation=soleRotation;
        }
    }
}
