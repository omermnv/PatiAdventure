using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private Mesh livingStone,livingTrunk,livingRound,livingGrass,livingLeaf,livingCanopy,livingCrown;
        private Material canopyHighlights;
        private Texture2D leafVeins;
        private readonly Dictionary<Material,Material> leafMaterials=new Dictionary<Material,Material>();
        private void PrepareLivingMeshes()
        {
            leafMaterials.Clear();leafVeins=null;
            livingStone=OrganicGeometry.Stone(7);livingTrunk=OrganicGeometry.Bark(5);livingRound=OrganicGeometry.Rounded();
            livingGrass=new Mesh{name="Bent grass blade",vertices=new[]{new Vector3(-.045f,0,0),new Vector3(.045f,0,0),new Vector3(.025f,.23f,.03f),new Vector3(-.025f,.23f,.03f),new Vector3(.04f,.48f,.09f)},triangles=new[]{0,3,1,1,3,2,3,4,2,1,3,0,2,3,1,2,4,3},uv=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up,new Vector2(.5f,1)}};
            var grassNormals=new Vector3[livingGrass.vertexCount];for(int i=0;i<grassNormals.Length;i++)grassNormals[i]=Vector3.up;livingGrass.normals=grassNormals;
            livingLeaf=new Mesh{name="Folded fern leaflet",vertices=new[]{new Vector3(0,0,-.5f),new Vector3(-.5f,0,0),new Vector3(0,.15f,0),new Vector3(.5f,0,0),new Vector3(0,0,.5f)},triangles=new[]{0,1,2,0,2,3,1,4,2,2,4,3,2,1,0,3,2,0,2,4,1,3,4,2},uv=new[]{Vector2.zero,Vector2.left,Vector2.one*.5f,Vector2.right,Vector2.up}};
            livingLeaf.normals=new[]{Vector3.up,Vector3.up,Vector3.up,Vector3.up,Vector3.up};livingLeaf.RecalculateBounds();
            livingCanopy=NaturalGeometry.LeafyCanopy(31);canopyHighlights=ColorMaterial(new Color(.29f,.45f,.16f));canopyHighlights.SetFloat("_Smoothness",.06f);meshes.Add(livingCanopy);
            livingCrown=NaturalGeometry.LeafyCanopy(53,140);meshes.Add(livingCrown);
            meshes.Add(livingStone);meshes.Add(livingTrunk);meshes.Add(livingRound);meshes.Add(livingGrass);meshes.Add(livingLeaf);
        }
        private Material LandscapeSurface(Color color,int kind)
        {var material=ColorMaterial(color);var texture=LivingSurface.Ground(kind);textures.Add(texture);material.SetTexture("_BaseMap",texture);material.SetTextureScale("_BaseMap",Vector2.one*12);material.SetFloat("_Smoothness",region==3?.28f:.12f);return material;}
        private void SetupLivingSky()
        {
            ApplyRegionalAtmosphere();
        }
        private void WoodLink(Vector3 a,Vector3 b,float radius,Material wood)
        {
            var direction=b-a;var beam=MeshObject("Natural branch",livingTrunk,a,new Vector3(radius,radius,direction.magnitude),wood,false);
            beam.transform.rotation=Quaternion.LookRotation(direction,Mathf.Abs(Vector3.Dot(direction.normalized,Vector3.up))>.95f?Vector3.forward:Vector3.up);
        }
        private void LivingTree(Vector3 p,Material wood,Material foliage,float size)
        {
            var trunk=MeshObject("Bark trunk",livingTrunk,p,new Vector3(.32f*size,.32f*size,3.8f*size),wood,true);trunk.transform.rotation=Quaternion.Euler(-90,0,0);
            for(int side=-1;side<=1;side+=2)
            {
                WoodLink(p+Vector3.up*.5f,p+new Vector3(side*.85f,.07f,.3f)*size,.18f*size,wood);
                WoodLink(p+Vector3.up*(2.4f*size),p+new Vector3(side*1.25f,4,.3f)*size,.15f*size,wood);
            }
            var crown=new GameObject("Living tree canopy").transform;crown.SetParent(world,false);crown.position=p+Vector3.up*(4.1f*size);crown.gameObject.AddComponent<FoliageSway>().phase=p.x+p.z;
            var leaves=MeshObject("Individual tree leaves",livingCanopy,crown.position,Vector3.one*size,foliage,false);
            leaves.GetComponent<MeshRenderer>().sharedMaterials=new[]{LeafMaterial(foliage),LeafMaterial(canopyHighlights)};leaves.transform.rotation=Quaternion.Euler(0,p.x*13+p.z*7,0);leaves.transform.SetParent(crown,true);
        }
        private Material LeafMaterial(Material source)
        {
            if(leafMaterials.TryGetValue(source,out var cached))return cached;
            if(!leafVeins){leafVeins=LivingSurface.Leaf();textures.Add(leafVeins);}
            var material=new Material(source){name="Veined "+source.name};material.SetTexture("_BaseMap",leafVeins);material.SetTextureScale("_BaseMap",Vector2.one);material.SetFloat("_Smoothness",.08f);materials.Add(material);leafMaterials.Add(source,material);return material;
        }
        private void DressPlatforms(Material wood,Material rock,Material metal)
        {
            var platforms=new List<Transform>();foreach(Transform part in world)if(part.name=="Exploration moving platform"||part.name=="Summit terrace")platforms.Add(part);
            foreach(var platform in platforms)
            {
                bool moving=platform.name=="Exploration moving platform";float width=moving?3:8,depth=moving?3:6,top=platform.position.y+platform.localScale.y*.5f;
                int boards=moving?6:10;
                // The visible decking is 7 cm above the original slab; ground queries must hit it.
                var box=platform.GetComponent<BoxCollider>();if(box){float extra=.07f/platform.lossyScale.y;box.size+=Vector3.up*extra;box.center+=Vector3.up*(extra*.5f);}
                for(int i=0;i<boards;i++)
                {
                    float x=platform.position.x-width/2+width*(i+.5f)/boards;
                    var plank=Shape(moving?"Platform wooden plank":"Terrace stone paving",PrimitiveType.Cube,new Vector3(x,top+.035f,platform.position.z),new Vector3(width/boards-.035f,.07f,depth-.1f),moving?wood:rock,false);plank.transform.SetParent(platform,true);
                }
                if(moving)for(int side=-1;side<=1;side+=2)
                {
                    var beam=Shape("Platform underbeam",PrimitiveType.Cube,platform.position+new Vector3(side*.9f,-.12f,0),new Vector3(.16f,.18f,3.2f),wood,false);beam.transform.SetParent(platform,true);
                    for(int end=-1;end<=1;end+=2){var rivet=MeshObject("Iron platform rivet",livingRound,new Vector3(platform.position.x+side*1.15f,top+.09f,platform.position.z+end*1.15f),Vector3.one*.07f,metal,false);rivet.transform.SetParent(platform,true);}
                }
            }
        }
        private void LivingGroundDetails(Material ground,Material foliage,Material rock,Material cream,Material gold)
        {
            var greens=new DetailBatch();var petals=new DetailBatch();var centres=new DetailBatch();
            // A few shared meshes carry the richer meadow instead of hundreds of renderers.
            for(int i=0;i<130;i++)
            {
                float angle=i*2.39996f,r=9+i%11;
                Vector3 p=new Vector3(Mathf.Sin(angle)*r,-.035f,17+Mathf.Cos(angle)*r);
                if(region<2)
                {
                    for(int blade=0;blade<6;blade++)greens.Add(livingGrass,p+new Vector3(blade*.04f,0,0),Vector3.one*(.65f+i%3*.22f),Quaternion.Euler(0,i*23+blade*57,0));
                    if(i%5==0)
                    {
                        greens.Add(livingTrunk,p,new Vector3(.014f,.014f,.35f),Quaternion.Euler(-90,0,0));
                        for(int petal=0;petal<5;petal++){float a=petal*Mathf.PI*2/5;petals.Add(livingRound,p+new Vector3(Mathf.Cos(a)*.085f,.36f,Mathf.Sin(a)*.085f),new Vector3(.13f,.035f,.075f),Quaternion.Euler(0,-petal*72,0));}
                        centres.Add(livingRound,p+Vector3.up*.38f,new Vector3(.075f,.05f,.075f),Quaternion.identity);
                    }
                }
                else if(region==2&&i%8==0)
                {
                    greens.Add(livingRound,p+Vector3.up*.55f,new Vector3(.3f,1.1f,.3f),Quaternion.identity);
                    for(int side=-1;side<=1;side+=2)greens.Add(livingRound,p+new Vector3(side*.25f,.5f,0),new Vector3(.2f,.6f,.2f),Quaternion.identity);
                }
                else if(region==3&&i%8==0)petals.Add(livingRound,p,new Vector3(2,.5f,1.5f),Quaternion.identity);
            }
            greens.Bake(this,"Meadow blades and stems",foliage);petals.Bake(this,"Wildflower petals and snow",cream);centres.Bake(this,"Wildflower golden centres",gold);
        }

        private static Vector3 VillageGatewayPosition(int destination)
        {return destination==1?new Vector3(-15,0,29):destination==2?new Vector3(13,0,31):new Vector3(16,0,8);}

        private Transform GatewayPiece(Transform gate,string name,Mesh mesh,Vector3 local,Vector3 scale,Quaternion rotation,Material material,bool solid=false)
        {
            var piece=MeshObject(name,mesh,gate.TransformPoint(local),scale,material,solid).transform;
            piece.rotation=gate.rotation*rotation;piece.SetParent(gate,true);return piece;
        }
        private void GatewayBranch(Transform gate,Vector3 a,Vector3 b,float radius,Material material,string name)
        {
            var direction=b-a;
            GatewayPiece(gate,name,livingTrunk,a,new Vector3(radius,radius,direction.magnitude),Quaternion.LookRotation(direction,Mathf.Abs(Vector3.Dot(direction.normalized,Vector3.up))>.95f?Vector3.forward:Vector3.up),material);
        }
        private Mesh FrozenGatewayArch()
        {
            const int rings=25,sides=8;var vertices=new Vector3[rings*(sides+1)];var uv=new Vector2[vertices.Length];var triangles=new int[(rings-1)*sides*6];int index=0;
            for(int ring=0;ring<rings;ring++)for(int side=0;side<=sides;side++)
            {
                float a=ring*Mathf.PI/(rings-1),b=side*Mathf.PI*2/sides;
                float thickness=.43f+Mathf.Sin(ring*1.7f)*.045f;
                vertices[ring*(sides+1)+side]=new Vector3(Mathf.Cos(a)*(2.35f+Mathf.Cos(b)*thickness),2.5f+Mathf.Sin(a)*(2.25f+Mathf.Cos(b)*thickness),Mathf.Sin(b)*.68f);
                uv[ring*(sides+1)+side]=new Vector2(ring/(float)(rings-1),side/(float)sides);
            }
            for(int ring=0;ring<rings-1;ring++)for(int side=0;side<sides;side++)
            {
                int a=ring*(sides+1)+side,b=a+1,c=a+sides+1,d=c+1;
                triangles[index++]=a;triangles[index++]=c;triangles[index++]=b;triangles[index++]=b;triangles[index++]=c;triangles[index++]=d;
            }
            // Split faces for crisp planar ice facets, instead of rounding them like stones.
            var flat=new Vector3[triangles.Length];var flatUv=new Vector2[triangles.Length];var order=new int[triangles.Length];
            for(int i=0;i<triangles.Length;i++){flat[i]=vertices[triangles[i]];flatUv[i]=uv[triangles[i]];order[i]=i;}
            var mesh=new Mesh{name="Continuous fractured ice arch",vertices=flat,uv=flatUv,triangles=order};mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);return mesh;
        }
        private void BuildThemedGateway(Transform gate,int theme)
        {
            var identity=Quaternion.identity;
            DressGatewayApproach(gate,theme);
            if(theme==1)
            {
                var bark=TexturedSurface(new Color(.3f,.18f,.085f),1,3);
                var green=TexturedSurface(new Color(.16f,.36f,.095f),0,2);
                var fresh=ColorMaterial(new Color(.37f,.56f,.16f));
                fresh.SetFloat("_Smoothness",.08f);
                for(int s=-1;s<=1;s+=2)
                {
                    GatewayPiece(gate,"Rooted gateway trunk",livingTrunk,new Vector3(s*2.35f,0,0),new Vector3(.53f,.5f,4.5f),Quaternion.Euler(-90,0,-s*5),bark,true);
                    for(int root=0;root<3;root++)GatewayBranch(gate,new Vector3(s*2.35f,.5f,0),new Vector3(s*(2.6f+root*.45f),.04f,(root-1)*.85f),.22f,bark,"Gateway spreading root");
                    GatewayBranch(gate,new Vector3(s*2.15f,3.3f,0),new Vector3(s*1.4f,4.6f,.1f),.3f,bark,"Interwoven branch arch");
                    GatewayBranch(gate,new Vector3(s*1.4f,4.6f,.1f),new Vector3(0,5,.12f),.23f,bark,"Interwoven branch arch");
                    for(int leaf=0;leaf<6;leaf++)
                    {
                        var crown=GatewayPiece(gate,"Gateway leafy crown",livingCrown,new Vector3(s*(1.1f+leaf%3*.7f),4.6f+leaf%2*.7f,(leaf/3-.5f)*1.1f),new Vector3(.4f,.48f,.45f),Quaternion.Euler(0,leaf*43,leaf*7),leaf%3==0?fresh:green);
                        crown.GetComponent<MeshRenderer>().sharedMaterials=new[]{LeafMaterial(leaf%3==0?fresh:green),LeafMaterial(canopyHighlights)};
                    }
                    // Two extra living trees on each side make the opening feel like a forest edge.
                    for(int tree=0;tree<2;tree++)LivingTree(gate.TransformPoint(new Vector3(s*(4+tree*.8f),0,tree==0?1.6f:-1.4f)),bark,green,.8f+tree*.15f);
                    for(int vine=0;vine<9;vine++)
                    {
                        float y=.6f+vine*.4f,x=s*2.25f+Mathf.Sin(vine*.8f)*.12f;
                        GatewayBranch(gate,new Vector3(x,y,-.45f),new Vector3(s*2.25f+Mathf.Sin((vine+1)*.8f)*.12f,y+.4f,-.45f),.035f,green,"Climbing vine");
                        GatewayPiece(gate,"Vine leaf",livingLeaf,new Vector3(x+s*.14f,y+.12f,-.48f),new Vector3(.3f,.4f,.48f),Quaternion.Euler(60,0,s*35),fresh);
                    }
                    for(int fern=0;fern<7;fern++)GatewayPiece(gate,"Gateway fern",livingLeaf,new Vector3(s*(2.8f+fern%3*.2f),.2f,-1.1f),new Vector3(.28f,.4f,1.1f),Quaternion.Euler(-35,fern*51,0),fresh);
                }
            }
            else if(theme==2)
            {
                var red=TexturedSurface(new Color(.58f,.265f,.12f),3,2);
                var strata=TexturedSurface(new Color(.8f,.48f,.245f),3,2);
                var dry=ColorMaterial(new Color(.44f,.41f,.16f));
                for(int s=-1;s<=1;s+=2)for(int layer=0;layer<5;layer++)
                {
                    GatewayPiece(gate,"Eroded sandstone column",livingStone,new Vector3(s*(2.35f+Mathf.Sin(layer*2.1f)*.08f),.3f+layer*.52f,0),new Vector3(1.5f,.8f,1.65f),Quaternion.Euler(0,s*layer*7,0),layer%2==0?red:strata,true);
                    if(layer<3)GatewayPiece(gate,"Canyon gateway buttress",livingStone,new Vector3(s*(3.5f+layer*.22f),.25f+layer*.38f,.5f),new Vector3(1.8f,.65f,1.9f),Quaternion.Euler(0,layer*25,0),red);
                }
                for(int block=0;block<=12;block++)
                {
                    float a=block*Mathf.PI/12;
                    GatewayPiece(gate,"Weathered sandstone arch",livingStone,new Vector3(Mathf.Cos(a)*2.35f,2.45f+Mathf.Sin(a)*2.25f,0),new Vector3(1.05f,.92f,1.6f),Quaternion.Euler(0,0,block*15-90),block%4==0?strata:red);
                }
                for(int s=-1;s<=1;s+=2)
                {
                    GatewayPiece(gate,"Sandstone outcrop",livingStone,new Vector3(s*4.5f,.65f,1.2f),new Vector3(2.2f,1.6f,2),Quaternion.Euler(0,s*30,0),red);
                    for(int blade=0;blade<8;blade++)GatewayPiece(gate,"Dry gateway scrub",livingGrass,new Vector3(s*3.8f,0,-1.3f),new Vector3(2,1.4f,2),Quaternion.Euler(0,blade*45,20),dry);
                }
            }
            else
            {
                var crystal=OrganicGeometry.Crystal();meshes.Add(crystal);
                var ice=ColorMaterial(new Color(.36f,.7f,.83f));ice.SetFloat("_Smoothness",.75f);
                var frost=ColorMaterial(new Color(.67f,.86f,.92f));frost.SetFloat("_Smoothness",.45f);
                var snow=ColorMaterial(new Color(.94f,.97f,1));
                GatewayPiece(gate,"Gateway snow clearing",livingStone,new Vector3(0,.015f,0),new Vector3(8.3f,.09f,5.5f),identity,snow,true);
                for(int s=-1;s<=1;s+=2)
                {
                    for(int shard=0;shard<4;shard++)
                    {
                        float height=4.7f-shard*.7f;
                        GatewayPiece(gate,"Glacier crystal pillar",crystal,new Vector3(s*(2.4f+shard*.47f),height*.5f,.2f+shard%2*.35f),new Vector3(1.05f,.85f,height),Quaternion.Euler(-90,shard*33,s*(8+shard*4)),shard%2==0?ice:frost,true);
                    }
                    GatewayPiece(gate,"Gateway snow drift",livingStone,new Vector3(s*3.2f,.2f,-.65f),new Vector3(2.6f,.6f,2.1f),identity,snow,true);
                }
                GatewayPiece(gate,"Continuous frozen arch",FrozenGatewayArch(),Vector3.zero,Vector3.one,identity,frost);
                for(int block=2;block<7;block++)
                {
                    float a=block*Mathf.PI/8;
                    GatewayPiece(gate,"Hanging gateway icicle",crystal,new Vector3(Mathf.Cos(a)*2.15f,2.65f+Mathf.Sin(a)*1.65f,-.35f),new Vector3(.2f,.23f,.65f),Quaternion.Euler(90,0,0),frost);
                }
                GatewayPiece(gate,"Snow arch cap",livingStone,new Vector3(0,4.9f,0),new Vector3(2.6f,.35f,1.6f),identity,snow);
            }
        }

        private void DressGatewayApproach(Transform gate,int theme)
        {
            var soil=ColorMaterial(theme==1?new Color(.24f,.3f,.13f):theme==2?new Color(.69f,.39f,.21f):new Color(.9f,.95f,.98f));
            var texture=LivingSurface.Ground(theme==1?0:theme==2?1:2);textures.Add(texture);soil.SetTexture("_BaseMap",texture);soil.SetTextureScale("_BaseMap",Vector2.one*4);soil.SetFloat("_Smoothness",.08f);
            GatewayPiece(gate,"Biome approach ground",livingStone,new Vector3(0,-.012f,-.7f),new Vector3(11,.08f,9.5f),Quaternion.identity,soil,true);
            var detail=new DetailBatch();var litter=new DetailBatch();
            var foliage=ColorMaterial(theme==1?new Color(.25f,.43f,.13f):theme==2?new Color(.3f,.4f,.22f):new Color(.4f,.68f,.8f));foliage.SetFloat("_Smoothness",theme==3?.55f:.08f);
            Mesh cactus=null,crystal=null;
            if(theme==2){cactus=OrganicGeometry.Sweep("Ribbed desert cactus",new[]{new Vector4(0,.17f,.17f,0),new Vector4(.3f,.18f,.18f,0),new Vector4(.85f,.16f,.16f,0),new Vector4(1.05f,.1f,.1f,0),new Vector4(1.1f,.003f,.003f,0)},10);meshes.Add(cactus);}
            if(theme==3){crystal=OrganicGeometry.Crystal();meshes.Add(crystal);}
            for(int i=0;i<28;i++)
            {
                int s=i%2==0?-1:1;var local=new Vector3(s*(2.8f+i%4*.55f),.08f,-4+i/4*.85f);
                var p=gate.TransformPoint(local);var rotation=gate.rotation*Quaternion.Euler(0,i*47,0);
                if(theme==1)
                {
                    for(int leaf=0;leaf<5;leaf++)detail.Add(livingLeaf,p+Vector3.up*(.1f+leaf%2*.09f),new Vector3(.33f,.5f,.85f),rotation*Quaternion.Euler(-25,leaf*72,0));
                    litter.Add(livingLeaf,p,new Vector3(.2f,.04f,.35f),rotation);
                }
                else if(theme==2)
                {
                    if(i%7==0)
                    {
                        detail.Add(cactus,p,new Vector3(.8f,.8f,1.15f),rotation*Quaternion.Euler(-90,0,0));
                        detail.Add(cactus,p+gate.right*s*.19f+Vector3.up*.35f,new Vector3(.55f,.55f,.65f),rotation*Quaternion.Euler(-80,0,s*25));
                    }
                    litter.Add(livingStone,p,new Vector3(.32f,.18f,.4f),rotation);
                }
                else
                {
                    if(i%4==0)detail.Add(crystal,p+Vector3.up*.32f,new Vector3(.22f,.25f,.75f),rotation*Quaternion.Euler(-85,0,s*12));
                    litter.Add(livingStone,p,new Vector3(.7f,.14f,.6f),rotation);
                }
            }
            detail.Bake(this,theme==1?"Forest approach undergrowth":theme==2?"Canyon approach cacti":"Glacier approach shards",foliage);
            litter.Bake(this,theme==1?"Forest approach leaf litter":theme==2?"Canyon approach gravel":"Glacier approach snow patches",soil);
        }

        // Bake static details by material: richer silhouettes without a renderer per leaf or pebble.
        private sealed class DetailBatch
        {
            private readonly List<CombineInstance> pieces=new List<CombineInstance>();
            public void Add(Mesh mesh,Vector3 position,Vector3 scale,Quaternion rotation)
            {pieces.Add(new CombineInstance{mesh=mesh,transform=Matrix4x4.TRS(position,rotation,scale)});}
            public void Bake(AdventureDirector owner,string name,Material material)
            {
                if(pieces.Count==0)return;
                int vertices=0;foreach(var piece in pieces)vertices+=piece.mesh.vertexCount;
                var mesh=new Mesh{name=name,indexFormat=vertices>65535?IndexFormat.UInt32:IndexFormat.UInt16};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();owner.meshes.Add(mesh);
                owner.MeshObject(name,mesh,Vector3.zero,Vector3.one,material,false);
            }
        }
        private void DressRegionalLandscape(Material foliage,Material rock,Material cream)
        {
            var stones=new DetailBatch();var greens=new DetailBatch();var stems=new DetailBatch();var accents=new DetailBatch();var lights=new DetailBatch();
            var accent=ColorMaterial(region==2?new Color(.73f,.43f,.24f):region==3?new Color(.37f,.72f,.82f):new Color(.65f,.28f,.18f));
            Mesh crystal=null;
            if(region==3){crystal=OrganicGeometry.Crystal();meshes.Add(crystal);accent.SetFloat("_Smoothness",.65f);}
            if(region==0)
            {
                // Flush paving preserves the existing walking surface and portal trigger openings.
                Vector3[] ends={VillageGatewayPosition(1),VillageGatewayPosition(2),VillageGatewayPosition(3)};
                foreach(var end in ends)for(int i=0;i<18;i++)
                {
                    var p=Vector3.Lerp(new Vector3(0,0,9),end,i/17f);p.y=.025f;
                    for(int side=-1;side<=1;side+=2)stones.Add(livingStone,p+new Vector3(side*.42f,0,Mathf.Sin(i*2.1f)*.08f),new Vector3(.82f,.11f,.68f),Quaternion.Euler(0,i*37,0));
                }
            }
            for(int cluster=0;cluster<24;cluster++)
            {
                float side=cluster%2==0?-1:1;
                var p=new Vector3(side*(10+cluster%4*2.6f),0,3+cluster/2*3);
                for(int pebble=0;pebble<4;pebble++)stones.Add(livingStone,p+new Vector3(Mathf.Sin(pebble*2.4f)*.7f,.06f,Mathf.Cos(pebble*2.4f)*.7f),new Vector3(.34f+pebble*.06f,.18f,.3f),Quaternion.Euler(0,cluster*31+pebble*53,0));
                if(region<2)
                {
                    // Seven arching fronds, built from paired tapered leaflets.
                    for(int frond=0;frond<7;frond++)for(int leaf=0;leaf<5;leaf++)
                    {
                        float a=frond*Mathf.PI*2/7,t=(leaf+1)/6f;
                        var centre=p+new Vector3(Mathf.Sin(a)*t*.75f,Mathf.Sin(t*Mathf.PI)*.55f+.08f,Mathf.Cos(a)*t*.75f);
                        for(int s=-1;s<=1;s+=2)greens.Add(livingLeaf,centre+new Vector3(Mathf.Cos(a)*s*.09f,0,-Mathf.Sin(a)*s*.09f),new Vector3(.11f,.12f,.31f*(1-t)+.08f),Quaternion.Euler(-20,frond*360f/7+s*55,0));
                    }
                    if(cluster%3==0)for(int mushroom=0;mushroom<3;mushroom++)
                    {
                        var m=p+new Vector3(.8f+mushroom*.22f,0,.2f);float height=.2f+mushroom*.08f;
                        stems.Add(livingRound,m+Vector3.up*height*.5f,new Vector3(.08f,height,.08f),Quaternion.identity);
                        accents.Add(livingRound,m+Vector3.up*height,new Vector3(.32f,.15f,.3f),Quaternion.identity);
                        lights.Add(livingRound,m+new Vector3(.05f,height+.07f,.02f),Vector3.one*.045f,Quaternion.identity);
                    }
                }
                else if(region==2)
                {
                    // Small eroded strata stay beside the route, beneath the large canyon spires.
                    if(cluster%3==0)for(int layer=0;layer<4;layer++)accents.Add(livingStone,p+new Vector3(.12f*layer,.16f+layer*.25f,0),new Vector3(1.7f-layer*.22f,.46f,1.25f-layer*.12f),Quaternion.Euler(0,cluster*17,0));
                    for(int blade=0;blade<7;blade++)greens.Add(livingGrass,p+new Vector3(.5f,0,.5f),new Vector3(1.8f,1.2f,1.8f),Quaternion.Euler(0,blade*360f/7,25));
                }
                else if(cluster%2==0)
                {
                    for(int shard=0;shard<5;shard++)
                    {
                        float height=.65f+shard%3*.4f;
                        accents.Add(crystal,p+new Vector3(Mathf.Sin(shard*2.4f)*.4f,height*.5f,Mathf.Cos(shard*2.4f)*.4f),new Vector3(.32f,.32f,height),Quaternion.Euler(-90+shard*4,cluster*29,shard*7-14));
                    }
                    lights.Add(livingStone,p+Vector3.up*.04f,new Vector3(2.1f,.25f,1.6f),Quaternion.identity);
                }
            }
            stones.Bake(this,"Regional paving and pebbles",rock);greens.Bake(this,"Regional fern and scrub foliage",foliage);
            stems.Bake(this,"Woodland mushroom stems",cream);accents.Bake(this,"Regional landscape accents",accent);lights.Bake(this,"Regional pale details",cream);
        }
    }
}
