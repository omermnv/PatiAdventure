using System;
using UnityEngine;
namespace PatiAdventure
{
    public static class CharacterSculptor
    {
        private sealed class Parts
        {
            public Mesh round,ear,canineEar,innerEar,skull,nose,lip,body,muzzle,limb,tuft;
            public readonly Action<Mesh> register;
            public Parts(Action<Mesh> register)
            {
                this.register=register;
                round=OrganicGeometry.Rounded();ear=OrganicGeometry.Ear();
                body=OrganicGeometry.Sweep("Canine torso",OrganicGeometry.CurvedSections(new[]{new Vector4(-.7f,.025f,.03f,0),new Vector4(-.55f,.25f,.3f,-.02f),new Vector4(-.3f,.29f,.32f,0),new Vector4(0,.24f,.3f,.03f),new Vector4(.28f,.29f,.37f,.06f),new Vector4(.5f,.17f,.27f,.08f),new Vector4(.6f,.02f,.04f,.1f)}));
                muzzle=OrganicGeometry.Sweep("Tapered canine muzzle",OrganicGeometry.CurvedSections(new[]{new Vector4(-.08f,.13f,.13f,0),new Vector4(.03f,.16f,.115f,0),new Vector4(.19f,.105f,.08f,-.025f),new Vector4(.36f,.075f,.065f,-.035f),new Vector4(.4f,.015f,.025f,-.035f)}));
                limb=OrganicGeometry.Sweep("Tapered jointed limb",OrganicGeometry.CurvedSections(new[]{new Vector4(0,.085f,.09f,0),new Vector4(.12f,.095f,.1f,0),new Vector4(.3f,.062f,.065f,.015f),new Vector4(.43f,.045f,.05f,.025f)}),12);
                tuft=OrganicGeometry.Sweep("Soft pointed fur tuft",new[]{new Vector4(0,.055f,.045f,0),new Vector4(.065f,.042f,.035f,0),new Vector4(.145f,.003f,.003f,-.014f)},10);
                register(round);register(ear);register(body);register(muzzle);register(limb);register(tuft);
            }
            public void PrepareCanineFace()
            {
                canineEar=BuildCanineEar(false);innerEar=BuildCanineEar(true);lip=BuildLipSeam();
                skull=OrganicGeometry.Sweep("Sculpted canine forehead",OrganicGeometry.CurvedSections(new[]{new Vector4(-.27f,.008f,.012f,0),new Vector4(-.2f,.15f,.16f,.01f),new Vector4(-.1f,.23f,.206f,.025f),new Vector4(.04f,.236f,.194f,.028f),new Vector4(.16f,.184f,.142f,.018f),new Vector4(.26f,.093f,.102f,-.015f),new Vector4(.31f,.008f,.009f,-.03f)}));
                nose=OrganicGeometry.Sweep("Soft canine nose pad",OrganicGeometry.CurvedSections(new[]{new Vector4(-.04f,.043f,.032f,.004f),new Vector4(-.018f,.064f,.043f,.002f),new Vector4(.009f,.055f,.033f,-.006f),new Vector4(.027f,.03f,.019f,-.01f),new Vector4(.032f,.007f,.005f,-.012f)}),16);
                register(canineEar);register(innerEar);register(skull);register(nose);register(lip);
            }
            public Transform Part(Transform parent,string name,Mesh mesh,Vector3 position,Vector3 scale,Material material)
            {var obj=new GameObject(name);obj.transform.SetParent(parent,false);obj.transform.localPosition=position;obj.transform.localScale=scale;obj.AddComponent<MeshFilter>().sharedMesh=mesh;obj.AddComponent<MeshRenderer>().sharedMaterial=material;return obj.transform;}
            public Transform Volume(Transform parent,string name,Vector3 position,Vector3 scale,Material material)=>Part(parent,name,round,position,scale,material);
        }
        private static Transform Joint(Transform parent,string name,Vector3 p)
        {var joint=new GameObject(name).transform;joint.SetParent(parent,false);joint.localPosition=p;return joint;}
        private static Mesh BuildCanineEar(bool inset)
        {
            const int rows=9,columns=9;int layer=rows*columns;
            var vertices=new Vector3[inset?layer:layer*2];var uv=new Vector2[vertices.Length];
            var triangles=new System.Collections.Generic.List<int>();
            // A curved pinna with thickness, a recessed centre and a rounded taper.
            // The inset follows the same surface, leaving a visible fur rim at every edge.
            for(int row=0;row<rows;row++)for(int column=0;column<columns;column++)
            {
                float v=row/(float)(rows-1),u=column/(float)(columns-1),across=u*2-1;
                float y=inset?Mathf.Lerp(.052f,.365f,v):v*.43f,t=y/.43f;
                float width=.15f*(1-t)*(.95f+.18f*Mathf.Sin(t*Mathf.PI))+.003f;
                float x=across*width*(inset?.7f:1);
                float rimDistance=x/width;
                float front=.065f*(1-t)+.018f*t-.022f*Mathf.Sin(t*Mathf.PI)*(1-rimDistance*rimDistance);
                int index=row*columns+column;
                vertices[index]=new Vector3(x,y,front+(inset?.0015f:0));uv[index]=new Vector2(u,v);
                if(!inset){vertices[index+layer]=new Vector3(x,y,-.06f*(1-t)+.006f*t);uv[index+layer]=new Vector2(u,v);}
            }
            void Quad(int a,int b,int c,int d){triangles.Add(a);triangles.Add(b);triangles.Add(c);triangles.Add(b);triangles.Add(d);triangles.Add(c);}
            for(int row=0;row<rows-1;row++)for(int column=0;column<columns-1;column++)
            {
                int a=row*columns+column,b=a+1,c=a+columns,d=c+1;Quad(a,b,c,d);
                if(!inset)Quad(a+layer,c+layer,b+layer,d+layer);
            }
            if(!inset)
            {
                for(int row=0;row<rows-1;row++)
                {
                    int left=row*columns,right=left+columns-1;
                    Quad(left,left+columns,left+layer,left+columns+layer);
                    Quad(right,right+layer,right+columns,right+columns+layer);
                }
                for(int column=0;column<columns-1;column++)
                {
                    int top=(rows-1)*columns+column;
                    Quad(column,column+layer,column+1,column+1+layer);
                    Quad(top,top+1,top+layer,top+1+layer);
                }
            }
            var mesh=new Mesh{name=inset?"Recessed canine ear lining":"Soft curved canine pinna",vertices=vertices,uv=uv,triangles=triangles.ToArray()};
            mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        private static Mesh BuildLipSeam()
        {
            const int rings=17,sides=8;int sideVertices=rings*(sides+1);
            var vertices=new Vector3[sideVertices*2];var uv=new Vector2[vertices.Length];
            var triangles=new int[(rings-1)*sides*6*2];int triangle=0;
            for(int side=0;side<2;side++)for(int ring=0;ring<rings;ring++)
            {
                float t=ring/(float)(rings-1),sign=side==0?-1:1;
                float z=Mathf.Lerp(.064f,.39f,t),x=Mathf.Lerp(.142f,.061f,Mathf.SmoothStep(0,1,t));
                float y=Mathf.Lerp(.018f,-.008f,t),radius=.0045f*Mathf.Sin(Mathf.PI*t)+.0006f;
                for(int edge=0;edge<=sides;edge++)
                {
                    float angle=edge*Mathf.PI*2/sides;int index=side*sideVertices+ring*(sides+1)+edge;
                    vertices[index]=new Vector3(sign*x+Mathf.Cos(angle)*radius,y+Mathf.Sin(angle)*radius,z);uv[index]=new Vector2(edge/(float)sides,t);
                    if(ring==rings-1||edge==sides)continue;
                    int a=index,b=a+1,c=a+sides+1,d=c+1;
                    triangles[triangle++]=a;triangles[triangle++]=b;triangles[triangle++]=c;triangles[triangle++]=b;triangles[triangle++]=d;triangles[triangle++]=c;
                }
            }
            var mesh=new Mesh{name="Paired fine canine lip seams",vertices=vertices,uv=uv,triangles=triangles};
            mesh.RecalculateNormals();mesh.RecalculateBounds();
            var normals=mesh.normals;
            for(int side=0;side<2;side++)for(int ring=0;ring<rings;ring++)
            {
                int first=side*sideVertices+ring*(sides+1),last=first+sides;
                normals[first]=normals[last]=(normals[first]+normals[last]).normalized;
            }
            mesh.normals=normals;return mesh;
        }
        private static void Leg(Parts p,Transform rig,string name,Vector3 origin,Material fur,Material paws,bool hind=false,bool hoof=false)
        {
            var hip=Joint(rig,name+" Hip",origin);
            p.Part(hip,"Upper leg",p.limb,new Vector3(0,.025f,hind?-.03f:0),new Vector3(hind?1.35f:1.05f,1,1),fur).localRotation=Quaternion.Euler(90,0,0);
            var knee=Joint(hip,name+" Knee",new Vector3(0,-.34f,0));
            p.Part(knee,"Lower leg",p.limb,new Vector3(0,.045f,0),new Vector3(.72f,.78f,.95f),paws).localRotation=Quaternion.Euler(90,0,0);
            var paw=Joint(knee,name+" Paw",new Vector3(0,-.35f,.04f));
            p.Volume(paw,"Paw pad",new Vector3(0,hoof?-.035f:-.025f,hoof?.055f:.035f),hoof?new Vector3(.17f,.15f,.23f):new Vector3(.14f,.12f,.18f),paws);
            int toes=hoof?2:4;for(int i=0;i<toes;i++)p.Volume(paw,hoof?"Cloven hoof tip":"Toe",new Vector3((i-(toes-1)*.5f)*(hoof?.075f:.032f),hoof?-.065f:-.055f,hoof?.14f:.10f),hoof?new Vector3(.075f,.09f,.1f):new Vector3(.041f,.05f,.05f),paws);
        }
        private static void Canine(Parts p,Transform rig,Material fur,Material cream,Material paws,Material inner,Material amber,bool fox)
        {
            p.PrepareCanineFace();
            p.Part(rig,"Torso",p.body,new Vector3(0,.88f,-.05f),Vector3.one,fur);
            p.Volume(rig,"Chest ruff",new Vector3(0,.95f,.4f),new Vector3(.43f,.62f,.39f),cream);
            p.Volume(rig,"Neck",new Vector3(0,1.04f,.44f),new Vector3(.42f,.46f,.43f),fur);
            var head=Joint(rig,"Head Joint",new Vector3(0,1.18f,.62f));
            head.localScale=fox?new Vector3(.84f,.88f,.88f):new Vector3(.86f,.9f,.9f);
            p.Part(head,"Skull",p.skull,Vector3.zero,Vector3.one,fur);
            for(int s=-1;s<=1;s+=2)
            {
                p.Volume(head,"Cheek ruff",new Vector3(s*.145f,-.08f,.075f),new Vector3(.17f,.18f,.28f),cream);
                for(int i=0;i<3;i++)p.Part(head,"Cheek fur tuft",p.tuft,new Vector3(s*.19f,-.07f-i*.025f,.035f-i*.04f),new Vector3(.72f,.65f,.75f),cream).localRotation=Quaternion.Euler(12,s*65,0);
                var ear=Joint(head,s<0?"Left Ear":"Right Ear",new Vector3(s*.17f,.15f,-.07f));ear.localRotation=Quaternion.Euler(-12,0,-s*13);ear.localScale=new Vector3(.85f,.75f,.9f);
                p.Part(ear,"Ear shell",p.canineEar,Vector3.zero,Vector3.one,fur);
                p.Part(ear,"Inner ear",p.innerEar,Vector3.zero,Vector3.one,inner);
                var eye=Joint(head,s<0?"Left Eye":"Right Eye",new Vector3(s*.165f,.038f,.191f));eye.localRotation=Quaternion.Euler(0,s*32,0);
                p.Volume(eye,"Eye socket",Vector3.zero,new Vector3(.103f,.061f,.034f),paws);
                p.Volume(eye,"Amber iris",new Vector3(0,0,.018f),new Vector3(.053f,.045f,.014f),amber);
                p.Volume(eye,"Pupil",new Vector3(0,0,.027f),new Vector3(.014f,.035f,.008f),paws);
                p.Volume(eye,"Catchlight",new Vector3(-.011f,.011f,.032f),Vector3.one*.009f,cream);
                p.Volume(eye,"Lower eyelid",new Vector3(0,-.028f,.005f),new Vector3(.086f,.016f,.02f),fur);
                p.Volume(head,"Canine brow",new Vector3(s*.169f,.079f,.18f),new Vector3(.107f,.026f,.033f),fur).localRotation=Quaternion.Euler(0,s*29,-s*4);
            }
            for(int i=0;i<5;i++)p.Part(rig,"Chest fur tuft",p.tuft,new Vector3((i-2)*.065f,.8f+Mathf.Abs(i-2)*.025f,.46f),new Vector3(.9f,1,1.15f),cream).localRotation=Quaternion.Euler(65,(i-2)*12,0);
            p.Part(head,"Upper muzzle",p.muzzle,new Vector3(0,-.08f,.18f),Vector3.one,cream);
            p.Part(head,"Nasal bridge",p.muzzle,new Vector3(0,.004f,.14f),new Vector3(.64f,.18f,.46f),fur);
            p.Part(head,"Nose",p.nose,new Vector3(0,-.112f,.575f),Vector3.one,paws);
            for(int s=-1;s<=1;s+=2)p.Volume(head,"Nostril",new Vector3(s*.039f,-.108f,.595f),new Vector3(.025f,.014f,.009f),inner);
            var jaw=Joint(head,"Jaw Joint",new Vector3(0,-.16f,.13f));
            p.Part(jaw,"Lower jaw",p.muzzle,new Vector3(0,-.01f,.01f),new Vector3(.87f,.42f,.91f),cream);
            p.Part(jaw,"Lower lip seam",p.lip,Vector3.zero,Vector3.one,paws);
            p.Volume(jaw,"Mouth interior",new Vector3(0,.027f,.2f),new Vector3(.15f,.027f,.24f),inner);
            for(int s=-1;s<=1;s+=2){var tooth=p.Part(jaw,"Canine tooth",p.ear,new Vector3(s*.055f,.033f,.25f),new Vector3(.15f,.11f,.22f),cream);tooth.localRotation=Quaternion.Euler(0,0,s*5);}
            Leg(p,rig,"Front Left",new Vector3(-.235f,.83f,.35f),fur,paws);Leg(p,rig,"Front Right",new Vector3(.235f,.83f,.35f),fur,paws);
            Leg(p,rig,"Back Left",new Vector3(-.235f,.83f,-.48f),fur,paws,true);Leg(p,rig,"Back Right",new Vector3(.235f,.83f,-.48f),fur,paws,true);
            var tail=Joint(rig,"Tail Base",new Vector3(0,.87f,-.66f));tail.localRotation=Quaternion.Euler(15,180,0);
            var middle=Joint(tail,"Tail Middle",new Vector3(0,0,.52f));middle.localRotation=Quaternion.Euler(-15,0,0);
            var tip=Joint(middle,"Tail Tip",new Vector3(0,0,.5f));tip.localRotation=Quaternion.Euler(-18,0,0);
            BuildContinuousTail(tail,middle,tip,fur,fox?cream:fur,p.register);
        }

        private static void BuildContinuousTail(Transform root,Transform middle,Transform tip,Material fur,Material cream,Action<Mesh> register)
        {
            const int rings=33,sides=24;const float length=1.45f;
            var vertices=new Vector3[rings*(sides+1)];var uv=new Vector2[vertices.Length];var weights=new BoneWeight[vertices.Length];
            var orange=new System.Collections.Generic.List<int>();var white=new System.Collections.Generic.List<int>();
            for(int ring=0;ring<rings;ring++)for(int side=0;side<=sides;side++)
            {
                float t=ring/(float)(rings-1),a=side*Mathf.PI*2/sides;
                // A single fusiform brush, with a soft droop and a feathered colour boundary.
                if(ring==24)t+=Mathf.Sin(a*5+.4f)*.016f;
                float radius=.205f*Mathf.Pow(Mathf.Max(0,Mathf.Sin(t*Mathf.PI)),.92f)+.075f*Mathf.Pow(1-t,3);
                radius*=1+Mathf.Sin(a*7+t*9)*.025f*Mathf.Sin(t*Mathf.PI);
                float y=-.22f*Mathf.Sin(t*Mathf.PI)+.12f*t*t;
                int index=ring*(sides+1)+side;
                vertices[index]=new Vector3(Mathf.Cos(a)*radius,Mathf.Sin(a)*radius*.88f+y,t*length);
                uv[index]=new Vector2(side/(float)sides,t);
                float z=t*length;
                // Linear skin weights share vertices across joints: no separate rotating tail lumps.
                if(z<.65f){float blend=Mathf.SmoothStep(0,1,z/.65f);weights[index]=new BoneWeight{boneIndex0=0,boneIndex1=1,weight0=1-blend,weight1=blend};}
                else{float blend=Mathf.SmoothStep(0,1,(z-.65f)/.65f);weights[index]=new BoneWeight{boneIndex0=1,boneIndex1=2,weight0=1-blend,weight1=blend};}
            }
            for(int ring=0;ring<rings-1;ring++)for(int side=0;side<sides;side++)
            {
                int a=ring*(sides+1)+side,b=a+1,c=a+sides+1,d=c+1;var triangles=ring<24?orange:white;
                triangles.Add(a);triangles.Add(b);triangles.Add(c);triangles.Add(b);triangles.Add(d);triangles.Add(c);
            }
            var bones=new[]{root,middle,tip};var poses=new Matrix4x4[bones.Length];
            for(int i=0;i<bones.Length;i++)poses[i]=bones[i].worldToLocalMatrix*root.localToWorldMatrix;
            var mesh=new Mesh{name="Continuous skinned fox brush",vertices=vertices,uv=uv,boneWeights=weights,bindposes=poses,subMeshCount=2};
            mesh.SetTriangles(orange,0);mesh.SetTriangles(white,1);mesh.RecalculateNormals();mesh.RecalculateBounds();
            var normals=mesh.normals;for(int ring=0;ring<rings;ring++){int first=ring*(sides+1),last=first+sides;normals[first]=normals[last]=(normals[first]+normals[last]).normalized;}mesh.normals=normals;
            register(mesh);
            var skin=root.gameObject.AddComponent<SkinnedMeshRenderer>();skin.sharedMesh=mesh;skin.sharedMaterials=new[]{fur,cream};skin.bones=bones;skin.rootBone=root;
            skin.localBounds=new Bounds(new Vector3(0,0,.6f),new Vector3(2.6f,2,3.2f));
        }
        public static void BuildFox(Transform visual,Material fur,Material cream,Material paws,Material inner,Material amber,Action<Mesh> register)
        {
            var p=new Parts(register);var rig=Joint(visual,"Fox Rig",Vector3.zero);Canine(p,rig,fur,cream,paws,inner,amber,true);
        }
        public static void BuildEnemy(Transform root,int kind,bool boss,Material body,Material cream,Material dark,Material amber,Action<Mesh> register)
        {
            var p=new Parts(register);var rig=Joint(root,"Enemy Rig",new Vector3(0,kind==1?-.92f:-.95f,0));
            if(kind==2)Canine(p,rig,body,cream,dark,dark,amber,false);
            else if(kind==1)
            {
                p.Part(rig,"Boar torso",p.body,new Vector3(0,.78f,-.07f),new Vector3(1.35f,1.15f,1.05f),body);
                var head=Joint(rig,"Head Joint",new Vector3(0,.77f,.66f));
                p.Volume(head,"Boar face",Vector3.zero,new Vector3(.53f,.5f,.55f),body);p.Volume(head,"Snout",new Vector3(0,-.07f,.27f),new Vector3(.34f,.24f,.36f),cream);
                for(int s=-1;s<=1;s+=2)
                {
                    p.Volume(head,"Nostril",new Vector3(s*.07f,-.035f,.437f),new Vector3(.052f,.042f,.015f),dark);
                    p.Volume(head,"Eye",new Vector3(s*.225f,.06f,.17f),new Vector3(.07f,.07f,.043f),amber);
                    p.Volume(head,"Boar pupil",new Vector3(s*.23f,.06f,.19f),new Vector3(.022f,.046f,.017f),dark);
                    p.Volume(head,"Boar brow",new Vector3(s*.23f,.105f,.17f),new Vector3(.11f,.055f,.08f),body);
                    var ear=p.Part(head,"Boar ear",p.ear,new Vector3(s*.23f,.2f,-.06f),new Vector3(.8f,.6f,.8f),body);ear.localRotation=Quaternion.Euler(0,0,-s*40);
                    p.Part(head,"Ivory tusk",p.muzzle,new Vector3(s*.2f,-.2f,.2f),new Vector3(.34f,.34f,.6f),cream).localRotation=Quaternion.Euler(-70,0,s*30);
                }
                for(int i=0;i<9;i++)p.Part(rig,"Boar back bristle",p.tuft,new Vector3(0,1.1f,-.55f+i*.12f),new Vector3(.8f,1,1.3f),dark).localRotation=Quaternion.Euler(-65,180,0);
                Leg(p,rig,"Front Left",new Vector3(-.29f,.82f,.32f),body,dark,false,true);Leg(p,rig,"Front Right",new Vector3(.29f,.82f,.32f),body,dark,false,true);
                Leg(p,rig,"Back Left",new Vector3(-.29f,.82f,-.45f),body,dark,true,true);Leg(p,rig,"Back Right",new Vector3(.29f,.82f,-.45f),body,dark,true,true);
            }
            else
            {
                var ice=OrganicGeometry.Crystal();register(ice);
                p.Part(rig,"Ice torso",ice,new Vector3(0,1.12f,0),new Vector3(1.05f,.75f,1.15f),body).localRotation=Quaternion.Euler(-90,0,0);
                p.Part(rig,"Ice chest plate",ice,new Vector3(0,1.2f,.3f),new Vector3(.72f,.14f,.8f),cream).localRotation=Quaternion.Euler(-90,0,0);
                var head=Joint(rig,"Head Joint",new Vector3(0,1.95f,0));p.Part(head,"Guardian head",ice,Vector3.zero,new Vector3(.73f,.75f,.7f),body).localRotation=Quaternion.Euler(-90,0,0);
                for(int s=-1;s<=1;s+=2)
                {
                    p.Volume(head,"Eye brow",new Vector3(s*.16f,.04f,.28f),new Vector3(.2f,.06f,.065f),dark);
                    p.Volume(head,"Glowing eye",new Vector3(s*.15f,-.015f,.28f),new Vector3(.1f,.065f,.075f),amber);
                    var arm=Joint(rig,s<0?"Left Arm":"Right Arm",new Vector3(s*.48f,1.55f,0));
                    p.Part(arm,"Shoulder",ice,Vector3.zero,new Vector3(.45f,.43f,.45f),cream).localRotation=Quaternion.Euler(-90,0,0);p.Part(arm,"Forearm",ice,new Vector3(s*.08f,-.35f,.07f),new Vector3(.35f,.4f,.65f),body).localRotation=Quaternion.Euler(-90,0,0);
                    p.Part(arm,"Fist",ice,new Vector3(s*.08f,-.66f,.09f),new Vector3(.43f,.48f,.4f),dark).localRotation=Quaternion.Euler(-90,0,0);
                    Leg(p,rig,s<0?"Front Left":"Front Right",new Vector3(s*.27f,.82f,0),body,dark);
                }
                for(int i=0;i<3;i++)p.Part(head,"Ice crown",p.ear,new Vector3((i-1)*.16f,.25f,0),new Vector3(.7f,.7f,.8f),cream);
            }
            if(boss)rig.localScale=Vector3.one*1.35f;
        }
    }
}
