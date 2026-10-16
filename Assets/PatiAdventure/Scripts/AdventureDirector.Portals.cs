using UnityEngine;
namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private ExplorationProgress exploration=new ExplorationProgress();
        private int region;
        private float portalReadyAt;
        private Material burstMaterial;
        public int CurrentRegion=>region;
        public int ExplorationStars=>exploration.stars.Count;
        public int DefeatedEnemies=>exploration.enemies.Count;
        public void BuildRegionPreview(int id){if(!Application.isPlaying&&id>=0&&id<=3){region=id;BuildEditorPreview();}}
        private string RegionName(int id)=>id==0?L("Portal Köyü","Portal Village"):id==1?L("Fısıltı Ormanı","Whispering Forest"):id==2?L("Kızıl Kanyon","Red Canyon"):L("Buzul Zirveleri","Glacier Peaks");
        public bool TravelTo(int destination)
        {
            if(!portalAdventure||!IsPlaying||destination<0||destination>3||destination==region||Time.time<portalReadyAt)return false;
            SaveNow();region=destination;portalReadyAt=Time.time+1.5f;BuildWorld(false);
            notice=L("Yeni dünyayı keşfet! J: Isır / K: Kuyruk", "Explore! J: Bite / K: Tail spin");
            return true;
        }
        private bool CollectExploration(AdventureActor.Role role,string id)
        {
            if(role!=AdventureActor.Role.Star)return false;
            string key=region+":"+id;if(exploration.stars.Contains(key))return false;
            exploration.stars.Add(key);stars++;notice=L("Işık yıldızı buldun!", "You found a light star!");
            GetComponent<PatiSoundFeedback>()?.Play(role);SaveNow();return true;
        }
        public void EnemyDefeated(string id)
        {
            string key=region+":"+id;if(exploration.enemies.Contains(key))return;
            exploration.enemies.Add(key);notice=L("Harika saldırı! Gölge dağıldı.","Great attack! The shadow vanished.");
            GetComponent<PatiSoundFeedback>()?.Play(AdventureActor.Role.Star);SaveNow();
        }
        public void ShowAttackBurst(Vector3 center,bool spin)
        {
            var cue=CombatVisual(spin?"Tail sweep trail":"Bite crescent",center,0,spin?2.3f:1.0f,spin?.55f:.38f);
            if(cue){cue.arc=spin?360:110;cue.expansion=0;cue.transform.rotation=Quaternion.Euler(0,fox.transform.eulerAngles.y,0);}
        }
        private void BuildPortalWorld()
        {
            var grass=LandscapeSurface(region==2?new Color(.66f,.44f,.27f):region==3?new Color(.85f,.92f,.95f):new Color(.32f,.53f,.24f),region==2?1:region==3?2:0);
            var rock=TexturedSurface(region==2?new Color(.58f,.3f,.18f):region==3?new Color(.4f,.5f,.58f):new Color(.43f,.45f,.37f),3,2);
            var wood=TexturedSurface(new Color(.36f,.23f,.13f),1,3);
            var leaves=TexturedSurface(region==3?new Color(.23f,.4f,.4f):new Color(.18f,.38f,.19f),0,3);
            var gold=ColorMaterial(new Color(1,.77f,.22f));burstMaterial=gold;
            var pale=ColorMaterial(new Color(.9f,.9f,.8f));var dark=ColorMaterial(new Color(.07f,.09f,.13f));
            var blue=ColorMaterial(new Color(.17f,.58f,.68f));var purple=ColorMaterial(new Color(.46f,.27f,.63f));
            SetupLivingSky();
            var camera=fox.cameraTransform?fox.cameraTransform.GetComponent<Camera>():null;if(camera)camera.backgroundColor=RenderSettings.fogColor;
            float width=region==0?60:96;
            Shape("Exploration ground",PrimitiveType.Cube,new Vector3(0,-.55f,17),new Vector3(width,1,width),grass);
            BuildFoothillLandscape(width,grass);
            var ridge=NaturalGeometry.Ridge(83);meshes.Add(ridge);var boulder=NaturalGeometry.Rock(17);meshes.Add(boulder);var cone=NaturalGeometry.Cone();meshes.Add(cone);
            var ridge2=NaturalGeometry.Ridge(51);meshes.Add(ridge2);
            OrganicGeometry.SmoothNormals(ridge);OrganicGeometry.SmoothNormals(ridge2);
            PrepareLivingMeshes();
            for(int i=0;i<18;i++)
            {
                // Leave room behind the spawn for the seven-metre follow camera.
                // The old inner ring forced the camera into Pati's back in the village.
                float a=i*Mathf.PI*2/18;Vector3 p=new Vector3(Mathf.Sin(a)*(width*.75f),-.05f,17+Mathf.Cos(a)*(width*.75f));
                MeshObject("Distant mountain ridge",i%2==0?ridge:ridge2,p,new Vector3(9+i%3,region==3?12+i%3*3:6+i%4*2,6+i%3),rock,true).transform.rotation=Quaternion.Euler(0,i*43,0);
                if(region==3)MeshObject("Snow summit",ridge,p+Vector3.up*(9+i%3*2),new Vector3(3,4,3),pale,false);
            }
            if(region==0)
            {
                Cabin(new Vector3(-11,0,1),wood,rock,dark);Cabin(new Vector3(11,0,1),wood,rock,dark);
                Shape("Village plaza",PrimitiveType.Cylinder,new Vector3(0,-.03f,9),new Vector3(9,.05f,9),rock);
                LivingTree(new Vector3(0,0,19),wood,leaves,1.45f);
                Portal(VillageGatewayPosition(1),1,leaves,gold);Portal(VillageGatewayPosition(2),2,rock,gold);Portal(VillageGatewayPosition(3),3,blue,pale);
                WorldLabel(L("Üç dünya seni bekliyor!","Three worlds await!"),new Vector3(0,3,8),gold);
                BuildExpandedVillage(wood,rock,leaves,dark);
                for(int i=0;i<3;i++)
                {
                    bool complete=RegionComplete(i+1);
                    Shape("Restored light seed",PrimitiveType.Sphere,new Vector3((i-1)*2,3,18),Vector3.one*.65f,complete?gold:rock,false);
                }
            }
            else
            {
                Portal(new Vector3(0,0,-7),0,blue,pale);
                WorldLabel(RegionName(region),new Vector3(0,3,2),gold);
                if(region==1)
                {
                    for(int i=0;i<18;i++)
                    {
                        Vector3 p=new Vector3((i%2==0?-1:1)*(10+i%4*3),0,2+i/2*4);
                        LivingTree(p,wood,leaves,.9f+i%3*.12f);
                    }
                    Cabin(new Vector3(-18,0,28),wood,rock,dark);
                    LivingWater("Forest pond",new Vector3(17,-.01f,32),new Vector2(12,9),rock,leaves);
                    for(int i=0;i<9;i++)Shape("Woodland bridge plank",PrimitiveType.Cube,new Vector3(17,.15f,29+i*.8f),new Vector3(3,.3f,.72f),wood);
                }
                else if(region==2)
                {
                    for(int i=0;i<12;i++)
                    {
                        Vector3 p=new Vector3((i%2==0?-1:1)*(15+i%3*3),0,3+i/2*6);
                        MeshObject("Sandstone spire",ridge,p,new Vector3(3,8+i%4*2,3),rock,true);
                        for(int band=1;band<4;band++)MeshObject("Sandstone strata",boulder,p+Vector3.up*band*1.8f,new Vector3(3.2f,.35f,3.2f),grass,false);
                    }
                    LivingWater("Canyon oasis",new Vector3(-17,-.01f,25),new Vector2(10,8),rock,leaves);
                    for(int i=0;i<5;i++)Shape("Ancient arch block",PrimitiveType.Cube,new Vector3(-4+i*2,6,39),new Vector3(2.05f,1,2),rock);
                    for(int s=-1;s<=1;s+=2)Shape("Ancient arch pillar",PrimitiveType.Cube,new Vector3(s*5,2.5f,39),new Vector3(2,5,2),rock);
                }
                else
                {
                    var needles=LandscapeLookGeometry.PineBoughs();meshes.Add(needles);
                    for(int i=0;i<16;i++)
                    {
                        Vector3 p=new Vector3((i%2==0?-1:1)*(14+i%3*4),0,i/2*5);
                        LivingAlpineTree(p,needles,wood,leaves,pale,i);
                    }
                    for(int i=0;i<7;i++)Shape("Ice crystal",PrimitiveType.Cube,new Vector3(-18+i*.7f,1.3f,31),new Vector3(.6f,2.6f,.6f),blue).transform.rotation=Quaternion.Euler(0,i*37,15);
                    var lake=Shape("Frozen lake",PrimitiveType.Sphere,new Vector3(16,0,22),new Vector3(14,.1f,10),blue,false);
                    lake.AddComponent<MeshCollider>().sharedMesh=lake.GetComponent<MeshFilter>().sharedMesh;
                }
                // Optional wide stair route with .25 m rises, a moving platform and summit reward.
                for(int i=0;i<8;i++)Shape("Summit stair",PrimitiveType.Cube,new Vector3(-7,(-.05f+i*.25f)/2,26+i*1.25f),new Vector3(4,.1f+i*.25f,1.3f),rock);
                Shape("Summit terrace",PrimitiveType.Cube,new Vector3(-7,.95f,38),new Vector3(8,2.1f,6),rock);
                var moving=Shape("Exploration moving platform",PrimitiveType.Cube,new Vector3(7,.25f,31),new Vector3(3,.5f,3),wood);moving.AddComponent<MovingPlatform>().travel=new Vector3(4,0,0);
                BuildExpandedTrails(wood,rock,leaves,pale,gold);
                Vector3[] positions={new Vector3(-5,1,6),new Vector3(5,1,10),new Vector3(-12,1,18),new Vector3(12,1,26),new Vector3(-7,3,38),new Vector3(7,1.2f,31),new Vector3(0,1,42),new Vector3(19,1,35),new Vector3(-30,1,44),new Vector3(-30,1,56),new Vector3(0,1,58),new Vector3(30,1,56),new Vector3(30,1,44)};
                sharedStar=CreateStar();meshes.Add(sharedStar);
                for(int i=0;i<positions.Length;i++)
                {
                    var star=new GameObject("Exploration star "+i);star.transform.SetParent(world,false);star.transform.position=positions[i];star.AddComponent<MeshFilter>().sharedMesh=sharedStar;star.AddComponent<MeshRenderer>().sharedMaterial=gold;
                    if(i==5)star.transform.SetParent(moving.transform,true);
                    Actor(star,AdventureActor.Role.Star);var actor=star.GetComponent<AdventureActor>();actor.actorId="star"+i;
                    if(exploration.stars.Contains(region+":"+actor.actorId))actor.RestoreCollected();
                }
                var enemyBody=ColorMaterial(region==1?new Color(.29f,.18f,.115f):region==2?new Color(.49f,.42f,.29f):new Color(.24f,.54f,.7f));
                var fur=region==3?LivingSurface.Ground(1):LivingSurface.Fur();textures.Add(fur);enemyBody.SetTexture("_BaseMap",fur);enemyBody.SetFloat("_Smoothness",region==3?.5f:.2f);
                for(int i=0;i<4;i++)BuildEnemy(new Vector3(i%2==0?-4:5,.85f,12+i*8),i,enemyBody,pale,dark,boulder);
                DressPlatforms(wood,rock,gold);
            }
            // Decorative rocks and ground tufts occupy edges, leaving the central exploration area open.
            for(int i=0;i<32;i++)
            {
                float a=i*2.39996f,r=13+i%7;
                Vector3 p=new Vector3(Mathf.Sin(a)*r,.2f,17+Mathf.Cos(a)*r);
                if(i%4==0)MeshObject("Weathered boulder",livingStone,p,new Vector3(1.9f,1.3f,1.5f),rock,true);
            }
            LivingGroundDetails(grass,leaves,rock,pale,gold);
            DressRegionalLandscape(leaves,rock,pale);
            DressStorybookMeadow(leaves,gold);
            BuildRegionalMotes(pale,gold);
            stars=0;foreach(string key in exploration.stars)if(key.StartsWith(region+":"))stars++;
            fox.SetCheckpoint(new Vector3(0,.2f,region==0?4:-2));fox.Respawn();
            Physics.SyncTransforms();
            if(fox.cameraTransform)fox.cameraTransform.GetComponent<FollowCamera>()?.Snap();
        }
        private void BuildExpandedVillage(Material wood,Material rock,Material leaves,Material dark)
        {
            Cabin(new Vector3(-20,0,32),wood,rock,dark);
            Cabin(new Vector3(20,0,32),wood,rock,dark);
            for(int side=-1;side<=1;side+=2)
            {
                LivingTree(new Vector3(side*24,0,20),wood,leaves,1.15f);
                Shape("Village garden bench",PrimitiveType.Cube,new Vector3(side*16,.6f,37),new Vector3(3,.25f,1),wood);
                for(int leg=-1;leg<=1;leg+=2)Shape("Garden bench leg",PrimitiveType.Cube,new Vector3(side*16+leg,.25f,37),new Vector3(.2f,.5f,.7f),wood);
                Shape("Village garden path",PrimitiveType.Cube,new Vector3(side*16,-.035f,29),new Vector3(3,.035f,22),rock,false);
            }
            WorldLabel(L("Bahçe yolu","Garden walk"),new Vector3(0,2.5f,39),leaves);
        }
        private void BuildExpandedTrails(Material wood,Material rock,Material leaves,Material pale,Material gold)
        {
            // Flat, wide loops join the existing adventure to the new outer clearings.
            // Decoration stays outside the walking lanes and collectible approaches.
            var path=region==1?wood:region==3?pale:rock;
            for(int side=-1;side<=1;side+=2)
            {
                Shape("Outer exploration trail",PrimitiveType.Cube,new Vector3(side*30,-.025f,41),new Vector3(4,.03f,34),path,false);
                Shape("Clearing approach trail",PrimitiveType.Cube,new Vector3(side*21,-.025f,26),new Vector3(22,.03f,4),path,false);
                Shape("Trail rest bench",PrimitiveType.Cube,new Vector3(side*35,.55f,50),new Vector3(3,.2f,1),wood);
                for(int leg=-1;leg<=1;leg+=2)Shape("Trail bench leg",PrimitiveType.Cube,new Vector3(side*35+leg,.25f,50),new Vector3(.2f,.5f,.8f),wood);
                for(int i=0;i<5;i++)
                {
                    var p=new Vector3(side*39,0,25+i*7);
                    if(region==1)LivingTree(p,wood,leaves,1.05f+i%2*.2f);
                    else if(region==2)MeshObject("Outer canyon lookout rock",livingStone,p,new Vector3(3,3+i%2,2),rock,true);
                    else MeshObject("Outer glacier crystal",livingStone,p,new Vector3(1.5f,3+i%2,1.5f),pale,true);
                }
                WorldLabel(region==1?L("Ateşböceği açıklığı","Firefly clearing"):region==2?L("Kanyon seyir yolu","Canyon lookout trail"):L("Kristal vadisi","Crystal valley"),new Vector3(side*30,2.8f,39),gold);
            }
            Shape("Far meadow connecting trail",PrimitiveType.Cube,new Vector3(0,-.025f,58),new Vector3(64,.03f,4),path,false);
            WorldLabel(L("Keşif çemberi • Köye dönüş girişte","Exploration loop • Village portal at the entrance"),new Vector3(0,2.8f,61),gold);
        }
        private void BuildEnemy(Vector3 p,int index,Material body,Material eyes,Material dark,Mesh mesh)
        {
            string id="enemy"+index;if(exploration.enemies.Contains(region+":"+id))return;
            var root=new GameObject(index==3?"Shadow guardian":"Shadow prowler");root.transform.SetParent(world,false);root.transform.position=p;
            var monster=root.AddComponent<PortalEnemy>();monster.director=this;monster.enemyId=id;monster.kind=region;monster.health=index==3?6:2;
            var collider=root.AddComponent<SphereCollider>();collider.radius=index==3?1:.65f;collider.isTrigger=true;
            var rigidbody=root.AddComponent<Rigidbody>();rigidbody.isKinematic=true;rigidbody.useGravity=false;
            var hp=new GameObject("Enemy health");hp.transform.SetParent(root.transform,false);hp.transform.localPosition=Vector3.up*1.7f;
            monster.healthDisplay=hp.AddComponent<TextMesh>();monster.healthDisplay.fontSize=40;monster.healthDisplay.characterSize=.13f;monster.healthDisplay.anchor=TextAnchor.MiddleCenter;monster.healthDisplay.color=new Color(1,.8f,.3f);
            hp.AddComponent<PortalLabel>().cameraTransform=fox.cameraTransform;
            var iris=ColorMaterial(region==3?new Color(.22f,.92f,1):new Color(.85f,.46f,.1f));iris.SetFloat("_Smoothness",.65f);
            CharacterSculptor.BuildEnemy(root.transform,region,index==3,body,eyes,dark,iris,value=>meshes.Add(value));root.AddComponent<CreatureMotion>();
            if(region==3)monster.healthDisplay.transform.localPosition=Vector3.up*(index==3?2.7f:2.15f);
            if(index==3)WorldLabel(L("Gölge Muhafızı","Shadow Guardian"),p+Vector3.up*(region==3?3.15f:2.7f),eyes).SetParent(root.transform,true);
        }
        private void Portal(Vector3 p,int destination,Material frame,Material light)
        {
            int theme=destination==0?region:destination;
            var gate=new GameObject(theme==1?"Forest gateway":theme==2?"Sandstone gateway":"Glacier gateway");gate.transform.SetParent(world,false);gate.transform.position=p;
            if(region==0)gate.transform.rotation=Quaternion.LookRotation(new Vector3(p.x,0,p.z-9));
            BuildThemedGateway(gate.transform,theme);
            var trigger=gate.AddComponent<BoxCollider>();trigger.center=Vector3.up*1.4f;trigger.size=new Vector3(3.2f,2.8f,1.8f);trigger.isTrigger=true;
            var portal=gate.AddComponent<AdventurePortal>();portal.director=this;portal.destination=destination;
            WorldLabel(RegionName(destination),p+Vector3.up*6.2f,light);
        }
        private Transform WorldLabel(string text,Vector3 p,Material color)
        {
            var obj=new GameObject(text);obj.transform.SetParent(world,false);obj.transform.position=p;
            var label=obj.AddComponent<TextMesh>();label.text=text;label.characterSize=.14f;label.fontSize=45;label.anchor=TextAnchor.MiddleCenter;label.color=color.color;
            obj.AddComponent<PortalLabel>().cameraTransform=fox.cameraTransform;
            return obj.transform;
        }
        private bool RegionComplete(int id)
        {
            int s=0,e=0;foreach(string key in exploration.stars)if(key.StartsWith(id+":"))s++;foreach(string key in exploration.enemies)if(key.StartsWith(id+":"))e++;
            return s>=5&&e>=4;
        }
        private void DrawExplorationGUI()
        {
            if(mode==Mode.Story)
            {
                GUI.Box(new Rect(140,80,720,270),L("Pati ve Üç Işık Tohumu\n\nPortallardan istediğin dünyaya git.\nYıldızları bul, gölgeleri dağıt ve köyün ağacını aydınlat!","Pati and the Three Light Seeds\n\nTravel through any portal.\nFind stars, defeat shadows and restore the village tree!"),boxStyle);
                if(GUI.Button(new Rect(330,380,340,65),L("Keşfe başla!","Let's explore!"),buttonStyle))Begin();
                return;
            }
            if(!IsPlaying)return;
            int seeds=0;for(int i=1;i<=3;i++)if(RegionComplete(i))seeds++;
            int enemies=0;foreach(string key in exploration.enemies)if(key.StartsWith(region+":"))enemies++;
            string objective=region==0?L("Bir portal seç • Her dünyada 5 yıldız ve 4 gölge", "Choose a portal • 5 stars and 4 shadows per world"):(L("Yıldız ","Stars ")+stars+"/13   "+L("Tohum için 5 yeter • Gölge ","5 for a seed • Shadows ")+enemies+"/4");
            GUI.Box(new Rect(70,10,830,105),RegionName(region)+" • "+L("Işık tohumu ","Light seeds ")+seeds+"/3\n"+objective+"\n"+notice,boxStyle);
            if(region!=0&&GUI.Button(new Rect(15,125,180,45),L("Köye dön","Return to village"),buttonStyle))TravelTo(0);
            if(seeds==3&&region==0)GUI.Box(new Rect(250,135,500,80),L("Üç tohum kurtuldu! Yaşam Ağacı yeniden ışıldıyor!","All three seeds restored! The Tree of Life shines again!"),boxStyle);
        }
    }
}
