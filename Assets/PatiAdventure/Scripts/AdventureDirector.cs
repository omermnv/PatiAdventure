using System.Collections.Generic;
using UnityEngine;
namespace PatiAdventure
{
    [RequireComponent(typeof(StoryNarrator))]
    public sealed partial class AdventureDirector : MonoBehaviour
    {
        public FoxController fox;
        public Material surfaceMaterial;
        public bool portalAdventure;
        [SerializeField] private Transform previewWorld;
        public IReadOnlyList<Material> GeneratedMaterials => materials;
        public IReadOnlyList<Mesh> GeneratedMeshes => meshes;
        public IReadOnlyList<Texture2D> GeneratedTextures => textures;
        public void BuildEditorPreview() { if(previewWorld&&previewWorld!=world)Release(previewWorld.gameObject);previewWorld=null;RefineFoxVisual();BuildWorld(true); previewWorld=world; }
        public bool IsPlaying => !paused && (mode == Mode.Play || mode == Mode.Training);
        public bool IsPaused => paused;
        public int StarCount => stars;
        public int LevelIndex => chapter * 3 + stage;
        public bool TrainingComplete => save.trainingComplete;
        public void ChooseLanguage(string value) { if(mode==Mode.Language)SelectLanguage(value); }
        public void StartAdventure() { if(mode==Mode.Story)Begin(); }
        public void ResumeSaved() { if(mode==Mode.Story)ContinueSaved(); }
        public void ContinueAdventure() { if(mode==Mode.Complete)Next(); }
        private enum Mode { Language, Story, Waking, Training, Play, Complete, Ending }
        private Mode mode;
        private Mode languageReturn = Mode.Story;
        private string language = "tr", notice = "";
        private int chapter, stage, stars, trainingStep, helped;
        private float distanceBaseline, runBaseline, hurtUntil;
        private int jumpBaseline;
        private Transform world;
        private StoryNarrator narrator;
        private Transform foxVisual;
        private float wakeStarted;
        private bool paused, sessionActive;
        private float previousTimeScale=1;
        private AdventureSaveData save = new AdventureSaveData();
        private int actorSequence;
        private GUIStyle boxStyle, buttonStyle, titleStyle;
        private Mesh sharedStar;
        private bool showStoryText;
        private readonly List<Material> materials = new List<Material>();
        private readonly List<Mesh> meshes = new List<Mesh>();
        private readonly List<Texture2D> textures = new List<Texture2D>();
        private bool English => language == "en";
        private string L(string tr, string en) => English ? en : tr;
        private void Start()
        {
            narrator = GetComponent<StoryNarrator>();
            if(!fox || !surfaceMaterial) { Debug.LogError("AdventureDirector: Fox ve Surface Material bağlantıları gerekli.",this); enabled=false; return; }
            fox.InputLocked = true;
            Application.targetFrameRate=30;
            foxVisual=fox.transform.Find("Fox Visual Pose");
            if(!foxVisual)
            {
                foxVisual = new GameObject("Fox Visual Pose").transform; foxVisual.SetParent(fox.transform, false);
                var parts = new List<Transform>(); foreach (Transform part in fox.transform) if (part != foxVisual) parts.Add(part);
                foreach (var part in parts) part.SetParent(foxVisual, false);
            }
            foxVisual.localRotation = Quaternion.Euler(0,0,75); foxVisual.localPosition = Vector3.up*.2f;
            RefineFoxVisual();
            language = PlayerPrefs.GetString("Pati.Language", "tr");
            PatiLocalization.SetLanguage(language);
            if(portalAdventure)exploration=ExplorationProgress.Load();
            if(previewWorld) {previewWorld.gameObject.SetActive(false);Destroy(previewWorld.gameObject);previewWorld=null;world=null;}
            materials.Clear();meshes.Clear();textures.Clear();
            BuildWorld(true); mode = Mode.Language;
        }
        private void Update()
        {
            if(UnityEngine.InputSystem.Keyboard.current!=null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
            if(paused)return;
            if (mode == Mode.Waking)
            {
                float fraction = Mathf.Clamp01((Time.time - wakeStarted) / 2.5f);
                foxVisual.localRotation = Quaternion.Slerp(Quaternion.Euler(0,0,75), Quaternion.identity, fraction);
                foxVisual.localPosition = Vector3.up * (.2f * (1-fraction));
                if (fraction >= 1) StartTraining();
                return;
            }
            if (mode != Mode.Training) return;
            if (trainingStep == 0 && fox.WalkedDistance - distanceBaseline >= 5) { trainingStep = 1; runBaseline = fox.RanDistance; narrator.Speak(language,"practiceRun",L("Şimdi koşalım! Turuncu koş düğmesini tut.","Now let us run! Hold the orange run button.")); }
            if (trainingStep == 1 && fox.RanDistance - runBaseline >= 6) { trainingStep = 2; jumpBaseline = fox.JumpCount; narrator.Speak(language,"practiceJump",L("Üç kez zıplayalım! Mavi zıpla düğmesine bas.","Let us jump three times! Press the blue jump button.")); }
            if (trainingStep == 2 && fox.JumpCount - jumpBaseline >= 3) { save.trainingComplete=true; SaveNow(); mode = Mode.Play; notice = L("★ Bul · Dostuna yardım et · Feneri yak", "★ Find · Help your friend · Light the beacon"); }
        }
        private void SelectLanguage(string choice)
        {
            language = choice; PlayerPrefs.SetString("Pati.Language", language); PlayerPrefs.Save();
            PatiLocalization.SetLanguage(language);
            mode = languageReturn; Narrate();
            if(portalAdventure)BuildWorld(false);
        }
        private void Narrate()
        {
            if(portalAdventure){narrator.Speak(language,"portalIntro",L("Günaydın Pati! Köyün Yaşam Ağacı ışığını kaybediyor. Üç portal seni farklı dünyalara götürecek. Yıldızları topla, gölgeleri dağıt ve üç ışık tohumunu eve getir!","Good morning Pati! The village Tree of Life is fading. Three portals lead to different worlds. Collect stars, defeat shadows and bring three light seeds home!"));return;}
            string text = mode == Mode.Ending ? (English ? AdventureStory.EnglishEnding : AdventureStory.TurkishEnding) : (English ? AdventureStory.EnglishStory[chapter] : AdventureStory.TurkishStory[chapter]);
            narrator.Speak(language, mode == Mode.Ending ? "ending" : "chapter" + chapter, text);
        }
        private void Begin()
        {
            narrator.Stop(); fox.InputLocked = false; notice = "";
            sessionActive=true;
            if(portalAdventure){foxVisual.localRotation=Quaternion.identity;foxVisual.localPosition=Vector3.zero;mode=Mode.Play;return;}
            if (chapter == 0 && stage == 0 && !save.trainingComplete)
            { fox.InputLocked = true; mode = Mode.Waking; wakeStarted = Time.time; }
            else { foxVisual.localRotation = Quaternion.identity; foxVisual.localPosition = Vector3.zero; mode = Mode.Play; }
        }
        private void StartTraining()
        { fox.InputLocked = false; mode = Mode.Training; trainingStep = 0; distanceBaseline = fox.WalkedDistance; runBaseline = fox.RanDistance; jumpBaseline = fox.JumpCount; narrator.Speak(language,"practiceWalk",L("Günaydın Pati! Sol yön düğmesini iterek biraz yürüyelim.","Good morning Pati! Push the left direction pad and walk a little.")); }
        public void Collect() { stars++; notice = L("Bir ışık yıldızı daha buldun!", "You found another light star!"); }
        public bool Interact(AdventureActor.Role role,string id)
        {
            if(!IsPlaying)return false;
            if(portalAdventure)return CollectExploration(role,id);
            bool added=false;
            switch(role)
            {
                case AdventureActor.Role.Star: added=AdventureSaveData.AddOnce(save.stars,id); if(added)Collect(); break;
                case AdventureActor.Role.Friend: added=AdventureSaveData.AddOnce(save.friends,id); if(added){ notice=L("♥ Dostun güvende!", "♥ Your friend is safe!"); } break;
                case AdventureActor.Role.Beacon:
                    if(stars<5){notice=L("★ Önce 5 yıldız bul", "★ Find 5 stars first");return false;}
                    added=AdventureSaveData.AddOnce(save.beacons,id); if(added){notice=L("☀ Fener yandı!", "☀ Beacon lit!");} break;
                case AdventureActor.Role.Chest: added=AdventureSaveData.AddOnce(save.chests,id); if(added)notice=L("♦ Gizli hazineyi buldun!", "♦ You found hidden treasure!"); break;
                case AdventureActor.Role.Guide: added=AdventureSaveData.AddOnce(save.guides,id); notice=L("Ada: ★ bul, Mino'yu kurtar, ☀ feneri yak!", "Ada: Find ★, rescue Mino, light the ☀ beacon!");if(added)narrator.Speak(language,"guideAda",L("Merhaba Pati! Beş yıldız bul, kaybolan Mino'yu kurtar ve ışık fenerini yak.","Hello Pati! Find five stars, rescue our lost friend Mino, and light the beacon.")); break;
            }
            helped=stage==1?save.friends.Count:save.beacons.Count;
            if(added) { GetComponent<PatiSoundFeedback>()?.Play(role); SaveNow(); }
            return added || role==AdventureActor.Role.Guide;
        }
        public void ReachCheckpoint(Vector3 point)
        { fox.SetCheckpoint(point); save.checkpoint=point; save.hasCheckpoint=true; SaveNow(); }
        public void SaveNow() { if(sessionActive){if(portalAdventure)exploration.Write();else AdventureSave.Write(save);} }
        public void TogglePause()
        {
            if(!paused&&!IsPlaying)return;
            paused=!paused; fox.InputLocked=paused;
            if(paused){previousTimeScale=Time.timeScale;Time.timeScale=0;SaveNow();}else ResumeGame();
        }
        public void ResumeGame()
        { paused=false;fox.InputLocked=false;Time.timeScale=previousTimeScale>0?previousTimeScale:1; }
        private void OnApplicationPause(bool value) { if(value){SaveNow();if(IsPlaying)TogglePause();} }
        public bool Help(bool beacon)
        {
            if (beacon && stars < 5) { notice = L("Feneri yakmak için beş yıldız bul.", "Find five stars to light the beacon."); return false; }
            helped++; notice = beacon ? L("Bir ışık feneri yandı!", "A light beacon is shining!") : L("Bir dostunu kurtardın!", "You rescued a friend!"); return true;
        }
        public void Bump()
        {
            if (Time.time < hurtUntil) return;
            hurtUntil = Time.time + 2; fox.Respawn(); notice = L("Sorun değil! Güvenli noktadan tekrar deneyelim.", "That is okay! Try again from your safe spot.");
        }
        public void TryFinish()
        {
            if (mode == Mode.Training) { notice = L("Önce küçük eğitimimizi bitirelim.", "Let us finish our little practice first."); return; }
            if(!save.CanFinish(chapter,stage)) {notice=L("★ Yıldız · ♥ Dost · ☀ Fener görevlerini bitir", "Finish ★ Star · ♥ Friend · ☀ Beacon goals");return;}
            mode = Mode.Complete; fox.InputLocked = true;
            int next = chapter * 3 + stage + 1;
            PlayerPrefs.SetInt("Pati.Progress", next); PlayerPrefs.Save();
            save=new AdventureSaveData {level=next,trainingComplete=true}; AdventureSave.Write(save); sessionActive=false;
        }
        private void Next()
        {
            if (++stage == 3)
            {
                stage = 0;
                if (++chapter == 12) { chapter = 11; mode = Mode.Ending; BuildFinale(); Narrate(); return; }
                BuildWorld(false); mode = Mode.Story; Narrate();
            }
            else { BuildWorld(false); Begin(); }
        }
        private void ContinueSaved()
        {
            if(portalAdventure){Begin();return;}
            save=AdventureSave.Load(); int progress = save.level;
            if (progress == 36) { chapter = 11; mode = Mode.Ending; BuildFinale(); Narrate(); return; }
            chapter = progress / 3; stage = progress % 3; BuildWorld(chapter == 0 && stage == 0); mode = Mode.Story; Narrate();
        }
        private void ClearWorld()
        {
            if (world) { world.gameObject.SetActive(false); Release(world.gameObject); }
            foreach (var material in materials) Release(material); materials.Clear();
            foreach (var mesh in meshes) Release(mesh); meshes.Clear();
            foreach(var texture in textures)Release(texture);textures.Clear();
            world = new GameObject("Active Adventure Region").transform;
            world.SetParent(transform,false);
            stars = helped = actorSequence = 0; notice = ""; hurtUntil = 0; sharedStar=null;
            if(save.level!=LevelIndex)save=new AdventureSaveData {level=LevelIndex,trainingComplete=true};
        }
        private static void Release(Object value) { if(Application.isPlaying)Destroy(value);else DestroyImmediate(value); }
        private Material ColorMaterial(Color color)
        {
            var material = new Material(surfaceMaterial) { color = color, enableInstancing=true }; materials.Add(material); return material;
        }
        private GameObject Shape(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, bool solid = true)
        {
            var obj = GameObject.CreatePrimitive(type); obj.name = name; obj.transform.SetParent(world, false);
            obj.transform.position = position; obj.transform.localScale = scale; obj.GetComponent<Renderer>().sharedMaterial = material;
            if (!solid) { obj.GetComponent<Collider>().enabled = false; Release(obj.GetComponent<Collider>()); }
            return obj;
        }
        private void Actor(GameObject obj, AdventureActor.Role role, int variant = 0)
        {
            var trigger = obj.AddComponent<SphereCollider>(); trigger.radius = role == AdventureActor.Role.Gate ? .9f : .65f; trigger.isTrigger = true;
            var actor = obj.AddComponent<AdventureActor>(); actor.role = role; actor.variant = variant; actor.director = this;
            actor.actorId=role+":"+actorSequence++;
            // The player's CharacterController generates trigger messages; static pickups need no Rigidbody.
            if(role==AdventureActor.Role.Creature){var body=obj.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;}
        }
        private void BuildWorld(bool training)
        {
            ClearWorld();
            if(portalAdventure){BuildPortalWorld();return;}
            if(chapter==0){BuildSunnyForest();RestoreWorld();return;}
            var ground = ColorMaterial(AdventureStory.Ground(chapter));
            var stone = ColorMaterial(Color.Lerp(AdventureStory.Ground(chapter), Color.gray, .5f));
            var foliage = ColorMaterial(new Color(.12f, .48f, .18f));
            var wood = ColorMaterial(new Color(.42f,.24f,.12f));
            var gold = ColorMaterial(new Color(1,.78f,.08f));
            var water = ColorMaterial(new Color(.15f,.65f,.88f));
            var creatureColor = ColorMaterial(Color.HSVToRGB((chapter * .073f + .6f) % 1, .55f, .8f));
            var white = ColorMaterial(Color.white); var black = ColorMaterial(new Color(.08f,.07f,.13f));
            float length = training ? 38 : 62 + stage * 10;
            bool islands = chapter == 3 || chapter == 8;
            if (!islands) Shape("Green meadow", PrimitiveType.Cube, new Vector3(0,-.6f,length/2-5), new Vector3(22,1,length+20), ground);
            else
            {
                for (int i = 0; i <= 10; i++) Shape("Safe island", PrimitiveType.Cube, new Vector3(Mathf.Sin(i)*1.5f,-.6f,-3+i*(length/10)), new Vector3(9,1,length/10-1), ground);
                if (chapter == 3) Shape("River", PrimitiveType.Cube, new Vector3(0,-3,length/2), new Vector3(24,.3f,length+20), water, false);
            }
            for (int i = 0; i < 24; i++)
            {
                float z = -9 + i * (length + 14) / 24; float side = i % 2 == 0 ? -1 : 1;
                Shape("Mountain", PrimitiveType.Sphere, new Vector3(side*(16+i%3),2+i%4,z), new Vector3(9,8+i%5*2,10), stone);
                Shape("Rock", PrimitiveType.Sphere, new Vector3(side*(7+i%3),.3f,z+2), new Vector3(1.5f,1.2f,1.3f), stone);
                if (chapter != 5 && chapter != 7 && chapter != 8)
                {
                    Shape("Tree trunk", PrimitiveType.Cylinder, new Vector3(side*9,1.2f,z), new Vector3(.5f,1.2f,.5f), wood);
                    Shape(chapter == 6 ? "Mushroom canopy" : "Leaf canopy", PrimitiveType.Sphere, new Vector3(side*9,2.8f,z), new Vector3(3,chapter == 6 ? 1 : 3,3), chapter == 6 ? creatureColor : foliage);
                }
                if (chapter == 5) Shape("Crystal", PrimitiveType.Cube, new Vector3(side*8,1,z), new Vector3(.7f,2.5f,.7f), water).transform.rotation = Quaternion.Euler(0,45,20);
                if (chapter == 2) Shape("Cave roof", PrimitiveType.Cube, new Vector3(0,8,z), new Vector3(24,1,5), stone);
            }
            if (training)
            {
                Shape("Den roof", PrimitiveType.Sphere, new Vector3(-4,1,-4), new Vector3(4,2.5f,4), wood);
                Shape("Den doorway", PrimitiveType.Cube, new Vector3(-4,.5f,-1.95f), new Vector3(1.2f,1,.15f), black, false);
            }
            for (int i = 0; i < 8; i++)
            {
                float z = 5 + i * (length - 10) / 8;
                float x = Mathf.Sin(i*1.7f + stage) * 3;
                float y = .3f + (i % 3) * .35f;
                Shape("Stepping platform", PrimitiveType.Cube, new Vector3(x,y,z), new Vector3(3,.6f,3), stone);
                if (i == 3 || i == 6)
                {
                    var checkpoint = Shape("Safe checkpoint", PrimitiveType.Cylinder, new Vector3(0,.02f,z-2), new Vector3(1.3f,.1f,1.3f), water, false);
                    Actor(checkpoint, AdventureActor.Role.Checkpoint);
                }
                var star = new GameObject("Light star"); star.transform.SetParent(world, false); star.transform.position = new Vector3(x,y+1.2f,z);
                if(!sharedStar){sharedStar=CreateStar();meshes.Add(sharedStar);} star.AddComponent<MeshFilter>().sharedMesh = sharedStar; star.AddComponent<MeshRenderer>().sharedMaterial = gold; Actor(star, AdventureActor.Role.Star);
                if (!training && i > 0 && i < 7)
                {
                    var creature = Shape("Region creature " + chapter, PrimitiveType.Sphere, new Vector3(i%2==0 ? -2 : 2,.7f,z+2.5f), new Vector3(1.1f,1.1f,1.1f), creatureColor, false);
                    Actor(creature, AdventureActor.Role.Creature, chapter);
                    for (int s = -1; s <= 1; s += 2)
                    {
                        var eye = Shape("Friendly eye", PrimitiveType.Sphere, Vector3.zero, Vector3.one*.22f, white, false);
                        eye.transform.SetParent(creature.transform, false); eye.transform.localPosition = new Vector3(s*.2f,.15f,-.43f);
                        var pupil = Shape("Pupil", PrimitiveType.Sphere, Vector3.zero, Vector3.one*.09f, black, false);
                        pupil.transform.SetParent(creature.transform, false); pupil.transform.localPosition = new Vector3(s*.2f,.15f,-.53f);
                    }
                    if (chapter % 3 == 1)
                    {
                        var ear = Shape("Creature crest", PrimitiveType.Cube, Vector3.zero, new Vector3(.3f,.55f,.3f), creatureColor, false);
                        ear.transform.SetParent(creature.transform, false); ear.transform.localPosition = Vector3.up*.65f;
                    }
                }
            }
            if (!training && !islands)
                for (int i = 0; i < stage + 2; i++)
                {
                    var hazard = Shape("Mist puddle", PrimitiveType.Cylinder, new Vector3(i%2==0 ? -3 : 3,.06f,15+i*15), new Vector3(2.3f,.08f,2.3f), creatureColor, false);
                    Actor(hazard, AdventureActor.Role.Hazard);
                }
            if (stage > 0)
                for (int i = 0; i < 3; i++)
                {
                    var friend = Shape(stage == 1 ? "Lost friend" : "Light beacon", stage == 1 ? PrimitiveType.Capsule : PrimitiveType.Cylinder,
                        new Vector3(i%2==0 ? 4 : -4, .8f, 10+i*(length-20)/3), new Vector3(.8f,1,.8f), stage == 1 ? white : gold, false);
                    Actor(friend, stage == 1 ? AdventureActor.Role.Friend : AdventureActor.Role.Beacon);
                }
            var gate = Shape("Light seed gate", PrimitiveType.Cube, new Vector3(0,1.6f,length-3), new Vector3(2,3,.45f), gold, false); Actor(gate, AdventureActor.Role.Gate);
            fox.SetCheckpoint(new Vector3(0,.2f,-3)); fox.Respawn();
            var activeCamera=fox.cameraTransform?fox.cameraTransform.GetComponent<Camera>():null;
            if(activeCamera)activeCamera.backgroundColor = chapter == 2 || chapter == 10 ? new Color(.2f,.22f,.35f) : new Color(.55f,.82f,1);
            RestoreWorld();
        }
        private void RestoreWorld()
        {
            stars=save.stars.Count; helped=stage==1?save.friends.Count:save.beacons.Count;
            foreach(var actor in world.GetComponentsInChildren<AdventureActor>(true))
            {
                bool collected=actor.role==AdventureActor.Role.Star?save.stars.Contains(actor.actorId):actor.role==AdventureActor.Role.Friend?save.friends.Contains(actor.actorId):actor.role==AdventureActor.Role.Beacon?save.beacons.Contains(actor.actorId):actor.role==AdventureActor.Role.Chest?save.chests.Contains(actor.actorId):actor.role==AdventureActor.Role.Guide&&save.guides.Contains(actor.actorId);
                if(collected)actor.RestoreCollected();
            }
            if(save.hasCheckpoint){fox.SetCheckpoint(save.checkpoint);fox.Respawn();}
        }
        private static Mesh CreateStar()
        {
            var vertices = new Vector3[22]; var triangles = new int[120];
            for(int side=0;side<2;side++)
            {
                vertices[side*11]=new Vector3(0,0,side==0?-.08f:.08f);
                for(int i=0;i<10;i++){float angle=(90+i*36)*Mathf.Deg2Rad,radius=i%2==0?.5f:.23f;vertices[side*11+i+1]=new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius,vertices[side*11].z);}
            }
            int t=0;
            for(int i=0;i<10;i++)
            {
                int a=i+1,b=(i+1)%10+1;
                triangles[t++]=0;triangles[t++]=b;triangles[t++]=a;
                triangles[t++]=11;triangles[t++]=a+11;triangles[t++]=b+11;
                triangles[t++]=a;triangles[t++]=b;triangles[t++]=a+11;
                triangles[t++]=b;triangles[t++]=b+11;triangles[t++]=a+11;
            }
            var mesh=new Mesh{name="Light Star",vertices=vertices,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        private void BuildFinale()
        {
            ClearWorld(); var green = ColorMaterial(new Color(.3f,.75f,.2f)); var brown = ColorMaterial(new Color(.5f,.28f,.1f)); var gold = ColorMaterial(new Color(1,.8f,.2f));
            Shape("Restored meadow", PrimitiveType.Cube, new Vector3(0,-.6f,5), new Vector3(28,1,30), green);
            Shape("Tree of Life trunk", PrimitiveType.Cylinder, new Vector3(0,3,9), new Vector3(2,3,2), brown);
            Shape("Tree of Life crown", PrimitiveType.Sphere, new Vector3(0,7,9), new Vector3(10,6,8), green);
            for (int i=0;i<12;i++) { float a=i*Mathf.PI/6; Shape("Restored light seed",PrimitiveType.Sphere,new Vector3(Mathf.Cos(a)*4,6+Mathf.Sin(a)*2,7),Vector3.one*.5f,gold,false); }
            Shape("Nox the gentle giant",PrimitiveType.Sphere,new Vector3(4,2,3),new Vector3(3,4,3),ColorMaterial(new Color(.5f,.4f,.7f)),false);
            Shape("Awakened light bird",PrimitiveType.Sphere,new Vector3(0,5,6),new Vector3(.8f,.5f,.5f),gold,false);
            fox.SetCheckpoint(new Vector3(0,.2f,-3)); fox.Respawn(); fox.InputLocked = true;
            foxVisual.localRotation = Quaternion.identity; foxVisual.localPosition = Vector3.zero;
        }
        private void OnGUI()
        {
            var old = GUI.matrix; var safe=Screen.safeArea;
            GUI.matrix = Matrix4x4.TRS(new Vector3(safe.xMin,Screen.height-safe.yMax,0), Quaternion.identity, new Vector3(safe.width/1000f,safe.height/650f,1));
            if(boxStyle==null){boxStyle=new GUIStyle(GUI.skin.box){fontSize=21,wordWrap=true,alignment=TextAnchor.MiddleCenter};buttonStyle=new GUIStyle(GUI.skin.button){fontSize=22};titleStyle=new GUIStyle(GUI.skin.label){fontSize=30,alignment=TextAnchor.MiddleCenter};}
            var box=boxStyle;var button=buttonStyle;
            if(paused)
            {
                GUI.Box(new Rect(200,100,600,300),"",box);
                GUI.Label(new Rect(260,135,440,55),L("Duraklatıldı", "Paused"),titleStyle);
                if(GUI.Button(new Rect(725,110,60,60),"X",button))ResumeGame();
                if(GUI.Button(new Rect(350,230,300,60),L("▶ Devam", "▶ Resume"),button))ResumeGame();
                if(GUI.Button(new Rect(350,310,300,60),L("Kaydet", "Save"),button)){SaveNow();notice=L("Kaydedildi", "Saved");}
                GUI.matrix=old;return;
            }
            if(IsPlaying&&GUI.Button(new Rect(920,10,65,65),"II",button))TogglePause();
            if(portalAdventure && mode!=Mode.Language){DrawExplorationGUI();GUI.matrix=old;return;}
            if (mode == Mode.Language)
            {
                GUI.Box(new Rect(170,100,660,340),"Pati Adventure\n\nDilini seç / Choose your language",box);
                if (GUI.Button(new Rect(230,280,240,65),"Türkçe",button)) SelectLanguage("tr");
                if (GUI.Button(new Rect(530,280,240,65),"English",button)) SelectLanguage("en");
            }
            else if (mode == Mode.Story || mode == Mode.Ending)
            {
                string title = mode == Mode.Ending ? L("Işık yeniden doğdu", "The light returns") : (English ? AdventureStory.EnglishNames[chapter] : AdventureStory.TurkishNames[chapter]);
                string story = mode == Mode.Ending ? (English ? AdventureStory.EnglishEnding : AdventureStory.TurkishEnding) : (English ? AdventureStory.EnglishStory[chapter] : AdventureStory.TurkishStory[chapter]);
                string summary=mode==Mode.Ending?L("♥ Dostluk kazandı!\n☀ Orman yeniden ışıldıyor.","♥ Friendship wins!\n☀ The forest shines again."):chapter==0?L("☀ Günaydın Pati!\nYaşam Ağacı'nın ışığı azalıyor.\n♥ Dostların yardımını bekliyor.","☀ Good morning Pati!\nThe Tree of Life is fading.\n♥ Your friends need your help."):L("★ Işığı bul · ♥ Dostlarını kurtar", "★ Find the light · ♥ Rescue your friends");
                GUI.Box(new Rect(90,45,820,330),title+"\n\n"+(showStoryText?story:summary)+"\n\n"+narrator.Status,box);
                if(GUI.Button(new Rect(90,5,210,35),showStoryText?L("Metni gizle","Hide text"):L("Hikâye metni","Story text"),button))showStoryText=!showStoryText;
                if (GUI.Button(new Rect(100,390,245,55),L("Tekrar dinle","Listen again"),button)) Narrate();
                if (GUI.Button(new Rect(365,390,245,55),L("Dil seçimi","Language"),button)) { narrator.Stop(); languageReturn = mode; mode = Mode.Language; }
                if (GUI.Button(new Rect(630,390,260,55), mode == Mode.Ending ? L("Yeniden oyna","Play again") : L("Maceraya başla","Start adventure"),button))
                {
                    if (mode == Mode.Ending) { chapter=stage=0; save=new AdventureSaveData(); PlayerPrefs.SetInt("Pati.Progress",0); PlayerPrefs.Save(); BuildWorld(true); mode=Mode.Story; Narrate(); }
                    else Begin();
                }
                if (mode == Mode.Story && chapter == 0 && stage == 0 && (PlayerPrefs.HasKey(AdventureSave.Key)||PlayerPrefs.GetInt("Pati.Progress",0)>0))
                    if (GUI.Button(new Rect(365,465,245,50),L("Kaldığım yerden","Continue saved"),button)) ContinueSaved();
            }
            else if (mode == Mode.Complete)
            {
                GUI.Box(new Rect(200,110,600,220),stage==2 ? L("Harika! Bir ışık tohumu kurtardın!","Wonderful! You rescued a light seed!") : L("Etap tamamlandı!","Stage complete!"),box);
                if (GUI.Button(new Rect(350,345,300,65),L("Devam et","Continue"),button)) Next();
            }
            else
            {
                string title = English ? AdventureStory.EnglishNames[chapter] : AdventureStory.TurkishNames[chapter];
                string objective = mode == Mode.Waking ? L("Günaydın Pati! Yeni bir gün başlıyor…", "Good morning Pati! A new day begins…") : mode == Mode.Training ? (trainingStep==0 ? L("Biraz yürü: WASD / ok tuşları veya sol yön alanı", "Walk a little: WASD / arrows or the left direction pad") : trainingStep==1 ? L("Şimdi koş: Shift veya KOŞ düğmesi", "Now run: Shift or the RUN button") : L("Üç kez zıpla: Space veya ZIPLA", "Jump three times: Space or JUMP")) : stage == 1 ? L("Beş yıldız + üç dost: ", "Five stars + three friends: ")+helped+" / 3" : stage == 2 ? L("Beş yıldız + üç fener: ", "Five stars + three beacons: ")+helped+" / 3" : L("Beş yıldız topla ve altın kapıya ulaş", "Collect five stars and reach the golden gate");
                if(chapter==0&&stage==0&&mode==Mode.Play)objective="★ "+stars+" / 5     ♥ "+save.friends.Count+" / 1     ☀ "+save.beacons.Count+" / 1     "+L("Ada'yı bul", "Find Ada");
                GUI.Box(new Rect(80,10,820,105),title+"  |  "+(chapter*3+stage+1)+" / 36  |  ★ "+stars+" / 5\n"+objective+"\n"+notice,box);
            }
            GUI.matrix = old;
        }
        private void OnDestroy() { if(!Application.isPlaying)return;SaveNow();if(paused)Time.timeScale=previousTimeScale;foreach (var material in materials) if(material) Destroy(material); foreach(var mesh in meshes) if(mesh) Destroy(mesh);foreach(var texture in textures)if(texture)Destroy(texture); }
    }
}
