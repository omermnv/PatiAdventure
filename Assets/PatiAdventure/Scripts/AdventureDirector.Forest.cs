using UnityEngine;
namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private void BuildSunnyForest()
        {
            var grass=TexturedSurface(new Color(.32f,.43f,.24f),0,32);var leaves=TexturedSurface(new Color(.18f,.3f,.16f),0,2);
            var pale=TexturedSurface(new Color(.4f,.46f,.31f),2,6);var wood=TexturedSurface(new Color(.36f,.24f,.15f),1,3);
            var stone=TexturedSurface(new Color(.43f,.44f,.4f),2,3);var water=ColorMaterial(new Color(.22f,.4f,.46f));
            var gold=ColorMaterial(new Color(.88f,.67f,.25f));var pink=TexturedSurface(new Color(.57f,.29f,.2f),2,2);
            var white=ColorMaterial(new Color(.82f,.79f,.7f));var dark=ColorMaterial(new Color(.1f,.1f,.095f));
            var cone=NaturalGeometry.Cone();meshes.Add(cone);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.008f;RenderSettings.fogColor=new Color(.6f,.68f,.73f);
            water.SetFloat("_Smoothness",.55f);
            Shape("SunnyForest Meadow",PrimitiveType.Cube,new Vector3(0,-.55f,28),new Vector3(40,1,80),grass);
            // A broad, continuous route with optional side paths, not random scattering.
            for(int i=0;i<20;i++)
            {
                float z=-5+i*3.5f;float x=Mathf.Sin(i*.5f)*1.3f;
                WeatheredStone("Trail stone",new Vector3(x,.02f,z),new Vector3(1.5f,.13f,1.1f),stone,100+i,false);
                if(i%3==0)Shape("Trail guide flower",PrimitiveType.Sphere,new Vector3(3,.22f,z),new Vector3(.35f,.3f,.35f),gold,false);
            }
            for(int side=-1;side<=1;side+=2)
                for(int i=0;i<8;i++)
                {
                    float x=side*(14+i%2*2),z=-4+i*9;
                    var ridge=NaturalGeometry.Rock(20+i,true);meshes.Add(ridge);MeshObject("Forest ridge",ridge,new Vector3(x+side*5,-.15f,z),new Vector3(7,5+i%3*2,6),pale,true);
                    Shape("Tree trunk",PrimitiveType.Cylinder,new Vector3(x,1.9f,z),new Vector3(.6f,1.9f,.6f),wood);
                    if(i%3==0)
                    { for(int crown=0;crown<3;crown++)MeshObject("Pine foliage",cone,new Vector3(x,2.5f+crown*1.2f,z),new Vector3(2.4f-crown*.5f,3-crown*.3f,2.4f-crown*.5f),leaves,false); }
                    else
                    {
                        for(int branch=0;branch<3;branch++)
                        {
                            float offset=(branch-1)*1.2f;
                            Shape("Tree branch",PrimitiveType.Cylinder,new Vector3(x+offset*.4f,3.1f,z),new Vector3(.2f,1.1f,.2f),wood,false).transform.rotation=Quaternion.Euler(0,0,-offset*35);
                            WeatheredStone("Leaf cluster",new Vector3(x+offset,4.7f+branch*.35f,z+(branch%2)*.6f),new Vector3(1.7f,2.4f,1.6f),leaves,80+branch+i,false);
                        }
                    }
                    WeatheredStone("Undergrowth",new Vector3(side*9,.5f,z+3),new Vector3(1.2f,1.2f,1),leaves,40+i,false);
                    Shape("Flower stem",PrimitiveType.Cylinder,new Vector3(side*5,.2f,z+1),new Vector3(.06f,.2f,.06f),leaves,false);
                    Shape("Flower petals",PrimitiveType.Sphere,new Vector3(side*5,.45f,z+1),new Vector3(.4f,.15f,.4f),i%2==0?pink:gold,false);
                }
            Cabin(new Vector3(-5,0,-3),wood,stone,dark);
            Cabin(new Vector3(9,0,42),wood,stone,dark);
            // Life Tree is visibly dim at the beginning; all lights are material colours.
            Shape("Fading Life Tree",PrimitiveType.Cylinder,new Vector3(-10,2.5f,4),new Vector3(1.5f,2.5f,1.5f),wood);
            WeatheredStone("Ancient crown",new Vector3(-10,6,4),new Vector3(3.5f,5,3),leaves,91,false);
            Shape("Dim light seed",PrimitiveType.Sphere,new Vector3(-10,4,1.5f),Vector3.one*.6f,pale,false);
            Animal("Ada the Owl",new Vector3(2,1,2),wood,white,dark,AdventureActor.Role.Guide);
            // Bridge deck is above the water trigger; the meadow offers a safe bypass.
            Shape("Little pond",PrimitiveType.Sphere,new Vector3(0,.02f,15),new Vector3(12,.08f,8),water,false);
            var shoreRock=NaturalGeometry.Rock(312);meshes.Add(shoreRock);
            for(int rock=0;rock<24;rock++)
            {
                float angle=rock*Mathf.PI*2/24;
                if(Mathf.Abs(Mathf.Cos(angle))<.28f)continue;
                MeshObject("Pond shore rock",shoreRock,new Vector3(Mathf.Cos(angle)*6.2f,.15f,15+Mathf.Sin(angle)*4.1f),new Vector3(.55f,.55f,.45f),stone,false);
            }
            var pond=new GameObject("Pond safety trigger");pond.transform.SetParent(world,false);pond.transform.position=new Vector3(0,.15f,15);
            var pondCollider=pond.AddComponent<BoxCollider>();pondCollider.size=new Vector3(11.5f,.3f,7.5f);pondCollider.isTrigger=true;
            var pondActor=pond.AddComponent<AdventureActor>();pondActor.role=AdventureActor.Role.Hazard;pondActor.director=this;pondActor.actorId="pond";
            for(int i=0;i<12;i++)Shape("Bridge plank",PrimitiveType.Cube,new Vector3(0,.55f,10.7f+i*.78f),new Vector3(2.8f,.3f,.72f),wood);
            Shape("Bridge entry low step",PrimitiveType.Cube,new Vector3(0,.06f,8.7f),new Vector3(3,.32f,1.3f),wood);
            Shape("Bridge entry high step",PrimitiveType.Cube,new Vector3(0,.25f,9.7f),new Vector3(3,.4f,1.4f),wood);
            Shape("Bridge exit high step",PrimitiveType.Cube,new Vector3(0,.25f,20),new Vector3(3,.4f,1.4f),wood);
            Shape("Bridge exit low step",PrimitiveType.Cube,new Vector3(0,.06f,21),new Vector3(3,.32f,1.3f),wood);
            for(int side=-1;side<=1;side+=2)
            {
                Shape("Bridge rail",PrimitiveType.Cube,new Vector3(side*1.5f,1.1f,15),new Vector3(.15f,.15f,10),wood);
                for(int i=0;i<4;i++)Shape("Bridge post",PrimitiveType.Cylinder,new Vector3(side*1.5f,.7f,11+i*2.6f),new Vector3(.16f,.7f,.16f),wood);
            }
            // A cave alcove has an actual walkable opening between two pillars.
            WeatheredStone("Cave left rock",new Vector3(-12,1.3f,29),new Vector3(1.5f,3,3),stone,52,true);
            WeatheredStone("Cave right rock",new Vector3(-6,1.3f,29),new Vector3(1.5f,3,3),stone,63,true);
            WeatheredStone("Cave roof",new Vector3(-9,3,29),new Vector3(4.5f,1.4f,3),stone,74,true);
            Shape("Cave back",PrimitiveType.Cube,new Vector3(-9,1.3f,32),new Vector3(8.5f,2.7f,.7f),stone);
            for(int i=0;i<4;i++)Shape("Cave trail",PrimitiveType.Cube,new Vector3(-2-i*1.8f,.02f,26),new Vector3(1.2f,.12f,1),stone,false);
            var chest=Shape("Hidden treasure chest",PrimitiveType.Cube,new Vector3(-9,.45f,30),new Vector3(1.3f,.85f,.9f),wood,false);Actor(chest,AdventureActor.Role.Chest);
            Shape("Chest latch",PrimitiveType.Cube,new Vector3(-9,.5f,29.5f),new Vector3(.2f,.3f,.1f),gold,false);
            for(int i=0;i<3;i++)
            {
                float y=.25f+i*.35f;Vector3 p=new Vector3(5+i*1.6f,y,33+i*2);
                Shape("Mushroom stalk",PrimitiveType.Cylinder,p,new Vector3(.55f,y+.25f,.55f),white);
                Shape("Mushroom platform",PrimitiveType.Cylinder,new Vector3(p.x,y*2+.3f,p.z),new Vector3(2.6f,.15f,2.6f),pink);
            }
            var moving=Shape("Slow moving platform",PrimitiveType.Cube,new Vector3(-4,.8f,39),new Vector3(3,.4f,3),wood);
            moving.AddComponent<MovingPlatform>().travel=new Vector3(4,0,0);
            Vector3[] positions={new Vector3(0,1,4),new Vector3(-3,1,8),new Vector3(0,1.7f,15),new Vector3(-9,1.1f,28),new Vector3(6.6f,2,35),new Vector3(-4,1.8f,39),new Vector3(3,1,45),new Vector3(0,1,53)};
            sharedStar=CreateStar();meshes.Add(sharedStar);
            for(int i=0;i<positions.Length;i++)
            {
                var star=new GameObject("Forest star "+i);star.transform.SetParent(world,false);star.transform.position=positions[i];
                star.AddComponent<MeshFilter>().sharedMesh=sharedStar;star.AddComponent<MeshRenderer>().sharedMaterial=gold;Actor(star,AdventureActor.Role.Star);
            }
            for(int i=0;i<2;i++)
            {
                var checkpoint=Shape("Safe checkpoint",PrimitiveType.Cylinder,new Vector3(0,.04f,22+i*22),new Vector3(1.6f,.1f,1.6f),water,false);Actor(checkpoint,AdventureActor.Role.Checkpoint);
            }
            int friendCount=stage==1?3:1;
            for(int i=0;i<friendCount;i++)Animal("Mino's lost friend",new Vector3(7-i*5, .85f,25+i*9),white,pink,dark,AdventureActor.Role.Friend);
            int beaconCount=stage==2?3:1;
            for(int i=0;i<beaconCount;i++)
            {
                var beacon=Shape("Light beacon",PrimitiveType.Cylinder,new Vector3(-3+i*3,.8f,48+i*2),new Vector3(.9f,.8f,.9f),stone,false);Actor(beacon,AdventureActor.Role.Beacon);
                Shape("Beacon lantern",PrimitiveType.Sphere,new Vector3(-3+i*3,1.8f,48+i*2),new Vector3(.65f,.65f,.65f),pale,false);
            }
            // Gentle guardians only appear after training in the second and third stages.
            if(stage>0)for(int i=0;i<3;i++)Animal("Confused forest guardian",new Vector3(i%2==0?4:-4,.8f,30+i*9),pink,white,dark,AdventureActor.Role.Creature);
            Shape("Gateway left pillar",PrimitiveType.Cylinder,new Vector3(-2,1.8f,59),new Vector3(.8f,1.8f,.8f),wood);
            Shape("Gateway right pillar",PrimitiveType.Cylinder,new Vector3(2,1.8f,59),new Vector3(.8f,1.8f,.8f),wood);
            Shape("Gateway arch",PrimitiveType.Cube,new Vector3(0,3.5f,59),new Vector3(5,.7f,.7f),leaves);
            var gate=Shape("Golden adventure gate",PrimitiveType.Cube,new Vector3(0,1.5f,59),new Vector3(2.4f,2.7f,.4f),gold,false);Actor(gate,AdventureActor.Role.Gate);
            gate.GetComponent<Renderer>().enabled=false;
            for(int i=0;i<48;i++)
            {
                float z=-5+i*1.5f,x=(i%2==0?-1:1)*(4.5f+Mathf.PerlinNoise(i*.7f,4)*4);
                Shape("Wild grass",PrimitiveType.Cube,new Vector3(x,.16f,z),new Vector3(.07f,.36f,.3f),leaves,false).transform.rotation=Quaternion.Euler(0,i*37,12);
            }
            fox.SetCheckpoint(new Vector3(0,.2f,-3));fox.Respawn();var camera=fox.cameraTransform?fox.cameraTransform.GetComponent<Camera>():null;if(camera)camera.backgroundColor=new Color(.6f,.72f,.8f);
        }
        private void Cabin(Vector3 origin,Material wood,Material roof,Material dark)
        {
            Shape("Cosy cabin",PrimitiveType.Cube,origin+Vector3.up,new Vector3(3.5f,2,3),wood);
            for(int side=-1;side<=1;side+=2)Shape("Pitched cabin roof",PrimitiveType.Cube,origin+new Vector3(side*.9f,2.6f,0),new Vector3(2.25f,.22f,3.8f),roof).transform.rotation=Quaternion.Euler(0,0,-side*25);
            Shape("Cabin door",PrimitiveType.Cube,origin+new Vector3(0,.65f,-1.52f),new Vector3(.9f,1.3f,.1f),dark,false);
            if(portalAdventure)DressLivingCabin(origin,wood,roof,dark);
        }
        private Material TexturedSurface(Color color,int kind,float tiling)
        {
            var material=ColorMaterial(color);material.SetFloat("_Smoothness",.08f);
            var texture=NaturalGeometry.Surface(kind);textures.Add(texture);material.SetTexture("_BaseMap",texture);material.SetTextureScale("_BaseMap",Vector2.one*tiling);return material;
        }
        private GameObject MeshObject(string name,Mesh mesh,Vector3 position,Vector3 scale,Material material,bool solid)
        {
            var obj=new GameObject(name);obj.transform.SetParent(world,false);obj.transform.position=position;obj.transform.localScale=scale;
            obj.AddComponent<MeshFilter>().sharedMesh=mesh;obj.AddComponent<MeshRenderer>().sharedMaterial=material;
            if(solid)obj.AddComponent<MeshCollider>().sharedMesh=mesh;return obj;
        }
        private GameObject WeatheredStone(string name,Vector3 position,Vector3 scale,Material material,int seed,bool solid)
        { var mesh=NaturalGeometry.Rock(seed);meshes.Add(mesh);return MeshObject(name,mesh,position,scale,material,solid); }
        private void RefineFoxVisual()
        {
            if(!fox)return;var visual=fox.transform.Find("Fox Visual Pose");if(!visual)return;
            if(visual.Find("Fox Rig"))return;
            foreach(Transform part in visual)
            {
                bool cream=part.name=="Muzzle"||part.name=="Tail tip"||part.name=="Chest fur",dark=part.name=="Eye"||part.name=="Nose"||part.name=="Foot";
                var renderer=part.GetComponent<Renderer>();if(renderer){var properties=new MaterialPropertyBlock();properties.SetColor("_BaseColor",dark?new Color(.055f,.05f,.045f):cream?new Color(.82f,.77f,.64f):new Color(.7f,.29f,.11f));properties.SetFloat("_Smoothness",.12f);renderer.SetPropertyBlock(properties);}
                if(part.name=="Head")part.localScale=new Vector3(.67f,.56f,.65f);
                if(part.name=="Eye")part.localScale=Vector3.one*.085f;
                if(part.name=="Muzzle"){part.localScale=new Vector3(.35f,.24f,.48f);part.localPosition=new Vector3(0,1.12f,.63f);}
                if(part.name=="Nose")part.localPosition=new Vector3(0,1.17f,.84f);
                if(part.name=="Body")part.localScale=new Vector3(.65f,.8f,1);
                if(portalAdventure)
                {
                    if(part.name=="Body")part.localScale=new Vector3(.7f,.65f,1.35f);
                    if(part.name=="Head")part.localPosition=new Vector3(0,1.02f,.55f);
                    if(part.name=="Ear")part.localPosition=new Vector3(Mathf.Sign(part.localPosition.x)*.25f,1.28f,.52f);
                    if(part.name=="Eye")part.localPosition=new Vector3(Mathf.Sign(part.localPosition.x)*.22f,1.09f,.85f);
                    if(part.name=="Muzzle")part.localPosition=new Vector3(0,.91f,.92f);
                    if(part.name=="Nose")part.localPosition=new Vector3(0,.96f,1.14f);
                }
            }
        }
        private void Animal(string name,Vector3 position,Material body,Material accent,Material eyes,AdventureActor.Role role)
        {
            var animal=Shape(name,PrimitiveType.Sphere,position,new Vector3(1,1.2f,1),body,false);Actor(animal,role,stage);
            if(role==AdventureActor.Role.Guide||role==AdventureActor.Role.Friend)
            {
                var marker=Shape("Friendship marker",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.32f,accent,false);
                marker.transform.SetParent(animal.transform,false);marker.transform.localPosition=Vector3.up*1.25f;
            }
            for(int s=-1;s<=1;s+=2)
            {
                var ear=Shape("Animal ear",PrimitiveType.Sphere,Vector3.zero,new Vector3(.2f,.45f,.2f),accent,false);ear.transform.SetParent(animal.transform,false);ear.transform.localPosition=new Vector3(s*.3f,.55f,0);
                var eye=Shape("Animal eye",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.14f,eyes,false);eye.transform.SetParent(animal.transform,false);eye.transform.localPosition=new Vector3(s*.2f,.15f,-.45f);
            }
        }
    }
}
