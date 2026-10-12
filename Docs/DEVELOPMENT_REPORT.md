# PatiAdventure geliştirme raporu

## 12 Ekim 2026 — gün ışığı, doğal yüzeyler ve çevre

Yeni etkin sahne `PatiAdventure_Daylight_Ready.unity`, Android sürüm kodu 7'dir. Gri ufuk giderildi; dört bölgenin güneş/ambient/sis paleti ayrıldı. Yeni çok zirveli dağlar, 256 RGB24 dikişsiz zeminler, gerçek mesh collider ile sürekli çevre arazisi, su dalgaları/kıyı sazları, cottage ayrıntıları, ayrı kar taşıyan çam dalları ve daha sakin tilki yüzü eklendi. Bacak/pati/rig ölçüleri korunur. Çimen/çiçek ayrıntıları ortak meshlerde birleştirildi; sahneye ait profilde düşük maliyetli yumuşak gölgeler açıldı. Kullanıcının ortak grafik/volume ayarları değiştirilmedi.

Son geçerli sonuçlar **51 ayrı PlayMode + 16 EditMode kontrolüdür**. Tam oynanış üç bölgede 24 yıldız/12 düşman ve köye dönüşü bitirdi; merdiven, kamera, dokunmatik, geçiş, kayıt ve yeni arazi kontrolleri doğrulandı. İlk tam turdaki 3 test sorunu renk hassasiyeti/test alanı izolasyonundan kaynaklandı; yalnız bu testler düzeltildikten sonra 5 hedefli kontrol geçti. Ham XML ve son sonuçların birleştirildiği özet raporda açıktır. 94 sahne referansı çözülür; eksik/yinelenen GUID yoktur. Üretilen ve aktarılan sahne aynıdır. [Görüntüler, kapsam ve kanıtlar](DAYLIGHT_WORLD.md).

`Builds/Android/PatiAdventure_Daylight_Ready.apk` başarıyla üretildi: 37.940.888 bayt, ARM64 / IL2CPP, versionCode 7, minimum API 25, hedef API 36. APK v2 imzası doğrulandı; paket/sertifika önceki sürümle aynı. Build helper, Android kalite düzeyini geçici olarak sahnenin gerçek profiline bağlar ve geri yükler; böylece yeni yumuşak gölge shaderları ayıklanmaz. Modeller stilizedir, fiziksel tablet performansı henüz ölçülmedi. [Build kaydı](TestResults/DaylightAndroidBuild.txt).

## 12 Ekim 2026 — tablet, kamera, portal ve dövüş kontrolü

İşlenen yeni sorunlar: uygulama odağı/arka plan geçişinde basılı kalan dokunmalar, ekran ölçeğiyle değişen kamera sürüklemesi, ışınlanmada kamera duvar kontrolünün gecikmesi, duraklatılan aynı karede kameranın dönmesi, kapı bekleme süresinde içeri girince tekrar denenmeyen geçiş, katı yüzeyin arkasından saldırı ve muhafızın geri itişle taşın içine girmesi. Düzeltmeler mevcut `PatiAdventure_Terrain_Ready.unity` sahnesinin runtime kaynaklarında uygulanır; kayıtlı sahne geometrisi değişmedi.

Son kaynakla **48 PlayMode + 9 EditMode testi geçti**; başarısız veya atlanan test yoktur. Üç bölgede toplam 24 yıldız/12 düşman ve köye dönüş yeniden normal hareket/zıplama/saldırıyla tamamlandı. Önceki merdiven/eğim/platform/kayıt kontrolleri de geçti. İlk turlarda bulunan aynı kare kamera ve büyük gövde zemin çakışması hataları düzeltildi; teşhis kayıtları son başarılı sonuçlardan ayrı tutulur. [Ayrıntılar ve test kanıtı](MOBILE_QA.md).

`Builds/Android/PatiAdventure_Mobile_Ready.apk` üretildi: 36.724.201 bayt, ARM64 / IL2CPP, versionCode 6, minimum API 25, hedef API 36. APK v2 imzası doğrulandı; önceki sürümün paket kimliği ve sertifikasıyla eşleşir. Android kaynak/yerel kod derlemesi başarılıdır. ADB cihaz listesinde bağlı/yetkili tablet yoktu; fiziksel cihaz performansı ve dokunmatik sürücü davranışı ölçülmedi. [Build kaydı](TestResults/MobileAndroidBuild.txt).

## 12 Ekim 2026 — basamak teması ve tam portal oynanışı

`PatiAdventure_Terrain_Ready.unity` yeni etkin build sahnesidir; Android sürüm kodu 5'tir. Pati tabanları gerçek model ölçüsüyle zemine yerleşir, basılı ayak taşıyıcı yüzeyi takip eder; gövde basamak/eğime uyum sağlar. Kapsül üst basamağa çıktığında orta zemin ışınının alt basamağı görmesiyle oluşan havada pati sorunu düzeltildi. Küçük aşağı basamaklarda zemin teması korunur. Platform döşemesi, buz gölü, kapı sütunları ve giriş zeminlerinin fizik yüzeyleri görünen geometriye uyarlandı; platform yıldızı ve muhafız başlığı taşıyıcılarını takip eder.

Yuvarlak ağaç tepeleri ayrı damarlı yapraklarla değiştirildi. Pati/parmak oranları, kürk parlaklığı ve çıkıntıları, yüz oranları ve hayvan esnemesi düzenlendi; düşmanlara göz kırpma eklendi. Modeller hâlâ prosedürel ve stilizedir.

Son kaynakla grafik cihazı açık **34 PlayMode ve 9 EditMode testi geçti**; başarısız/atlanmış test yoktur. Altı bölge merdiven yürüyüş/koşu rotasında dört pati teması ve ayrı eğim testi doğrulandı. Normal hareket/zıplama/saldırı girdileriyle üç bölgede toplam 24 yıldız ve 12 düşman tamamlandı, köye dönülünce üç yaşam tohumu restore edildi. 80 sahne referansı çözülür; yinelenen proje GUID'si yoktur. Fiziksel tablet ve eski 36 hikâye etabının tamamı test kapsamında değildir. Kanıtlar ve görüntüler: [TERRAIN_QA.md](TERRAIN_QA.md).

`Builds/Android/PatiAdventure_Terrain_Ready.apk` başarıyla üretildi: 36.721.897 bayt, ARM64 / IL2CPP, versionCode 5, minimum API 25, hedef API 36. APK v2 imzası doğrulandı; paket kimliği ve sertifika önceki sürümle aynıdır. Test edilen kaynak sahne ve projeye aktarılan sahnenin SHA256 değeri aynıdır. [Build ve imza doğrulaması](TestResults/TerrainAndroidBuild.txt).

## 12 Ekim 2026 — kapı çevreleri ve build hazırlığı

`PatiAdventure_World_Polish.unity` eklendi. Temalı girişlerin önleri artık ayrı zemin alanlarıyla dünyayı tanıtır: orman toprağı/yaprak örtüsü, kızıl kum/çakıl/kaktüsler ve geniş kar/küçük kristaller. Küçük ayrıntılar ortak meshlerde birleştirilir. Köy geçişleri ve kesintisiz kuyruk korunur. Varsayılan build listesinde bu sahne tek etkin sahne olarak seçildi; Android sürüm kodu 4 yapıldı.

Editor/Android kod kontrolleri ve grafik cihazı açık 23 PlayMode testi başarılıdır. Sahnenin 74 GUID referansı çözülür; yinelenen proje GUID'si yoktur. Export edilen sahne ile doğrulanan kaynak sahnenin SHA256 değeri aynıdır. Doğrulama kopyası Unity'nin temizleyebildiği `Temp` klasöründen `.codex-validation` klasörüne taşındı; ilk grafik açılışı takıldığı için ilk içe aktarım grafiksiz, önizleme sonraki aşamada yapılır. Ayrıntılar ve tablet çıktısı: [WORLD_POLISH.md](WORLD_POLISH.md).

`Builds/Android/PatiAdventure_World_Polish.apk` başarıyla üretildi: 36.501.165 bayt, ARM64 / IL2CPP, versionCode 4, minimum API 25, hedef API 36. APK v2 imza doğrulaması geçti; paket kimliği ve sertifika önceki APK ile aynıdır. Fiziksel tablette kurulup denenmedi. Sonuçlar `Docs/TestResults/WorldPolishAndroidBuild.txt` içindedir.

## 12 Ekim 2026 — bölgeye özgü girişler

`PatiAdventure_Biome_Gateways.unity` eklendi. Köyün üç ayrı köşesinde orman için köklü ağaç/sarmaşık girişi, kanyon için kızıl katmanlı taş kemer, buzul için kesintisiz kırıklı buz kemeri/kristal/sarkıt/kar girişi bulunur. Taş yollar ve kapı yönleri yeni konumlara uyarlandı. Geri dönüş kapıları da mevcut dünyanın temasını kullanır. Kesintisiz tilki kuyruğu dahildir; önceki sahneler korunur.

Editor/Android kod kontrolleri geçti. Son düzenlemelerle grafik cihazı açık 23 PlayMode testi başarılıdır; yeni test üç köşe girişinden gerçek CharacterController hareketiyle geçişi doğrular. Sahnenin 62 GUID referansı çözülür; yinelenen proje GUID'si yoktur. Gerçek Unity önizlemeleri incelendi. Görüntüler, kullanım ve test kanıtları: [BIOME_GATEWAYS.md](BIOME_GATEWAYS.md). Yeni APK üretilmedi.

## 12 Ekim 2026 — kesintisiz kuyruk düzeltmesi

Kullanıcının kuyrukta yuvarlak boğumlar gördüğü geri bildirimi üzerine ayrı kuyruk parçaları kaldırıldı. Üç kemikle bükülen tek SkinnedMeshRenderer, dolgun orta bölümden sivri beyaz uca incelen kesintisiz yüzey kullanır. Yeni sahne `PatiAdventure_Natural_Tail.unity`; önceki sahneler korunmuştur. Editor/Android kod kontrolleri ve grafik cihazı açık 22 PlayMode testi başarılıdır. Yeni test gerçek mesh deformasyonunu ve kesitlerin ayrılmamasını denetler. Ayrıntılar ve görüntüler: [NATURAL_TAIL.md](NATURAL_TAIL.md). APK üretilmedi.

## 12 Ekim 2026 — karakter ve çevre ayrıntıları

`PatiAdventure_Visual_Polish.unity` eklendi. Tilki/çakal bacakları incelen özel meshlerle yenilendi; yanak/göğüs kürkü ve incelen kuyruk ucu eklendi. Yaban domuzunun kaş, göz bebeği ve sırt kılları ayrıntılandırıldı. Köyde üç portala taş yollar, ormanda eğrelti/mantarlar, kanyonda küçük kaya katmanları ve kuru bitkiler, buzulda kristal kümeleri bulunur. Statik küçük dekorlar en fazla beş mesh grubuna birleştirilir; eğrelti yaprakları beş köşelidir.

Editor/Android kod kontrolleri ve Unity sahne üretimi başarılı. Grafik cihazı açık çalıştırmada 21 PlayMode testi geçti, 0 başarısız, 0 atlanan. Kamera giriş görüntüleri, hareket, portal/dövüş, karakter eklemleri ve platform dekorları doğrulandı. Sonuçlar `Docs/TestResults/VisualPolishPlayMode.xml` içinde. Görseller ve kullanım: [VISUAL_POLISH.md](VISUAL_POLISH.md). Bu güncelleme için yeni APK üretilmedi; fiziksel tablet performansı ölçülmedi.

Tarih: 11 Ekim 2026. Unity 6000.3.26f1, URP 17.3.0, Input System 1.20.0. Android önceliklidir.

## Tamamlanan geliştirmeler

- Runtime, Editor ve test kodları ayrı assembly tanımlarına taşındı.
- CharacterController hareketi: yumuşak hızlanma/yavaşlama, havada yön kontrolü, çift zıplama, zıplama tamponu ve kısa kenar toleransı, tavan/duvar çarpışması, platformla taşınma ve güvenli noktaya dönüş.
- Tilki görselinde zıplama/iniş sırasında esneme; açılışta 2,5 saniyelik uyanış pozu.
- Kamera: yumuşak takip, dokunmatik/fare dönüşü, yavaş otomatik hizalama, oyuncuyu dışlayan engel sorguları, hızlı içeri çekilme ve yavaş uzaklaşma. Köşelerde takip odağının duvar içine kayması da kontrol edilir.
- Canvas üzerinde çoklu dokunma destekleyen joystick, koşma, zıplama ve kamera sürükleme alanı; ekran güvenli alanı ve farklı oran desteği.
- İlk orman için yönü belli ana yol, tepeler, ağaçlar, çalılar, çiçekler, gölet, basamaklı ahşap köprü, mağara, gizli sandık/yıldızlar, mantar ve hareketli platformlar, iki kontrol noktası, kulübeler ve Ada/Mino.
- İlk etapta Ada ile konuşma, eğitim, beş yıldız, bir dost ve ilk fenerin tamamlanması kapıyı açar. Sonraki etaplar için üç dost / üç fener koşulları korunur.
- Yıldız, dost, fener, sandık ve rehber etkileşimleri benzersiz kimlikle kaydedilir; aynı nesne iki kez sayılmaz. Kontrol noktası ve eğitim durumu yüklenebilir. Önceki bölüm kaydı yeni kayıt biçimine uyarlanır.
- Duraklatma, devam, kayıt ve uygulama arka plana geçince güvenli duraklatma.
- Türkçe/İngilizce kısa yönergeler ve simgeler. Uzun hikâye metni isteğe bağlıdır. İngilizce 17 prototip ses kaydı; Türkçe için Android TTS ve kayıt ekleme altyapısı.
- Hikâye ve 12 bölge / 36 etap planı korunmuştur; bu oturumun kalite odağı ilk ormandır.

## Düzeltilen hatalar

1. Önceki `CS0433: MenuItem`: bağımsız kontrol komutu hem monolitik `Managed/UnityEditor.dll` hem modüler `UnityEditor.CoreModule.dll` yüklemişti. Projede iki MenuItem tanımı yoktu. Kontrol artık yalnızca modüler Unity Editor assembly'lerini kullanır.
2. Windows komut satırı uzunluk sınırı: C# referansları response dosyasına yazılır.
3. Test assembly'lerinde otomatik TestAssemblies referansları ile elle girilen TestRunner referanslarının çakışması kaldırıldı.
4. Unity'nin adsız başlangıç sahnesinde additive üretim engeli: doğrulama kopyasında başlangıç sahnesi ayrı dosyaya kaydedilir. Kullanıcının adsız sahnesi üretici tarafından kaydedilmez; önce kaydetmesi açıkça istenir.
5. Joystick verisini her kare sıfırlayan eski IMGUI kontrolü yeni sahnelerde kullanılmaz. Yeni kontroller pointer kimliklerini takip eder ve kilitlenince bırakır.
6. Görev sayacı dost ve fener sayılarını karıştırmaz. Görev kapısı gerçek kayıtlı hedefleri doğrular.
7. Runtime dünya, yönetici nesnenin altında oluşturulur; additive sahnede nesneler yanlış sahneye gitmez.
8. Yıldız mesh'i karşıt normal iptali oluşmayan, kalınlığı olan ayrı ön/arka yüzlere dönüştürüldü.
9. Referansı eksik eski HUD/yıldız scriptlerinde null kontrolleri; Android TTS kapanışında bekleyen başlatma isteği kontrolü eklendi.
10. Tam eğitim testi köprü girişinin fazla yüksek olduğunu gösterdi. Köprü giriş/çıkışına düşük ve ardışık basamaklar eklendi; çocuk düz yol üzerinden yürüyüp koşarak eğitimi tamamlayabilir.
11. Doğrulama kopyasından aktarımda asset klasörünün adına güvenmek yetersizdi: önceki başarısız üretimler de klasör oluşturmuştu. Doğru klasörler sahnelerdeki GUID bağımlılıklarından belirlendi ve eklendi. İki aktarılan sahnenin doğrudan asset/script GUID'leri özgün çalışma alanında tamamen çözümlenir; kanıt `TransferValidation.txt` dosyasındadır.
12. İlk grafik önizlemede aynı shader kullanan nesneler tek renkte çizildi. Isıtılmış render istekleri ve SRP Batcher kapalı profil ile renkler doğru çizildi. Ayrı sahne profilinde SRP Batcher kapatıldı ve profil Editor Play'de de uygulanır; çıkışta önceki profil geri yüklenir. Mevcut proje profilleri değiştirilmez. GPU instancing açık kalır.

## Test ve doğrulama

Gerçek Unity süreçleri, açık kullanıcı projesinin yerine `Temp/UnityValidation-*` altındaki ayrı kopyada çalıştırıldı. Kopyanın company/product adları ayrıdır; otomatik testler kullanıcının oyun kayıtlarını kullanmaz.

| Kontrol | Sonuç |
| --- | --- |
| Unity Editor derlemesi | Başarılı, 6000.3.26f1 |
| Sahne üretimi ve eksik script taraması | Başarılı: 16 oynanış nesnesi, 0 eksik script |
| EditMode testleri | 9 geçti, 0 başarısız |
| PlayMode testleri | Son çalıştırmada 12 geçti, 0 başarısız |
| Unity Android oyuncu scriptleri | 28 assembly üretildi; APK üretilmedi |
| Bağımsız C# Editor / Android kod yolları | Başarılı |

EditMode testleri: görev koşulları, tekrar sayılmama, kayıt serileştirme, bozuk kayıttan güvenli dönüş, kayıt sınırları, 12 bölgelik iki dilli içerik ve sahnelerin benzersiz yollarda üretilmesi.

PlayMode testleri: hızlanma/yavaşlama, çift zıplama ve üçüncü zıplamanın engellenmesi, duvar çarpışması, düşme/kontrol noktası, platformda taşınma, kamera duvarı, dil ekranı ve Canvas kurulumu, CharacterController ile gerçek yıldız tetiklenmesi/kayıt, görev kapısı/etap ilerleme ve duraklatma. Uyanıştan eğitim sonuna kadar tam akış ile sahne yeniden yüklenince kayıtlı yıldızların gizlenmesi de gerçek PlayMode testlerinde geçmiştir. Toplam **21 otomatik test geçti**.

NUnit kanıtları: `Docs/TestResults/EditMode.xml`, `PlayMode.xml`; sahne taraması `SceneValidation.txt`. Tam geçici süreç günlükleri `Temp/LatestUnityValidation.txt` dosyasının gösterdiği kopyada bulunur. Temp temizlenirse doğrulama kopyası tekrar oluşturulmalıdır.

## Mobil performans

Mevcut PC/Mobile URP asset'lerine yazılmadı. Yeni sahneye özel mobil profil oluşturuldu: HDR kapalı, render scale 0.8, MSAA 1, 25 m gölge mesafesi, tek kaskad, 512 ana gölge çözünürlüğü ve ek ışıklar kapalı. Tek gerçek zamanlı güneş ışığı kullanılır. Runtime materyaller paylaşılır ve instancing açıktır; yıldızlar aynı mesh'i kullanır. Kamera sorguları tekrar kullanılan NonAlloc tamponlarıyla çalışır. Duran toplama nesnelerine gereksiz Rigidbody eklenmez.

Hedef 30 FPS'dir. **Gerçek orta segment Android cihazında FPS, ısınma, pil ve bellek ölçümü yapılmadı.**

Ormanın grafik önizlemesi `Docs/ForestOverview.png` dosyasındadır. Grafik cihazı açık ayrı Unity batch sürecinde üretildi ve görsel olarak incelendi. Bu bir Editor genel görünümüdür; cihaz performans ölçümü değildir. SRP Batcher renk doğruluğu için kapalıdır; bunun CPU etkisi Android profillemesinde ölçülmelidir. Önceki aktarım klasörleri silinmedi; kullanılan tam varlıklar `VerifiedFirstForestAssets` ve `VerifiedReadyForestAssets` içindedir.

## Test edilemeyenler ve bilinen sınırlar

- APK kurulumu, Android gerçek çoklu dokunma, çentikli cihaz ekranları ve cihaz TTS ses paketleri fiziksel cihazda doğrulanmadı.
- Türkçe kayıtlar henüz yoktur. Editör'de Türkçe ses çalmaz; Android cihazda uygun Türkçe TTS paketi gerekir. İngilizce kayıtlar sistem sesi prototipidir.
- iOS derlemesi ve iOS sistem seslendirmesi uygulanmadı/test edilmedi.
- Düşmanlar ve karakter geometrik prototiptir. Tam iskelet animasyonu, karakter sesleri, müzik ve nihai sanat varlıkları yoktur.
- Sonraki 11 bölge temel prosedürel etaplar olarak korunur; ilk ormanın ayrıntı düzeyinde üretilmedi. 36 etabın tamamında insan oynanış testi yapılmadı.
- Kayıt yereldir; bulut kaydı veya cihazlar arası taşıma yoktur.
- Hareketli platform taşıması konum/rotasyon takibine dayanır; çok hızlı veya dönen platformlar ayrıca test edilmelidir.
- Dokunmatik hareket kontrolleri Canvas'tadır; hikâye/görev/duraklatma ekranları halen IMGUI'dir.

## Unity'de yapılacak işlemler

1. Unity'ye geçin, derlemenin bitmesini bekleyin; gerekirse **Assets > Refresh** seçin.
2. `Assets/PatiAdventure/Generated/SunnyForest_Natural.unity` sahnesini açın, Play'e basın. Önceki `SunnyForest_FirstPlayable` ve `SunnyForest_Ready` sahneleri korunmuştur.
3. Dil seçin, Pati'nin uyanışından sonra yürüyün, koşun ve üç kez zıplayın. Klavye: WASD, Shift, Space; çift zıplama için havada tekrar Space. Kamera: sağ fare sürükleme.
4. Ada'ya yaklaşın; köprüden geçip yıldızları toplayın, Mino'ya yardım edin, feneri ve kapıyı deneyin. Mağara sandığını ve hareketli platformu keşfedin.
5. II / Escape ile duraklatın. Play'i kapatıp açın; **Kaldığım yerden** ile kontrol noktası ve toplama kaydını kontrol edin.
6. Android Build Profiles Scene List'e oynanabilir sahneyi ekleyin. Fiziksel cihazda en az joystick + kamera + zıplama, Türkçe anlatım, köprü, çarpışmalar ve 30 FPS hedefini değerlendirin.

Yeni sahne üretimi: `PatiAdventure > Create SunnyForest` / `Create Story Adventure`. Üretici açık sahneyi korur; benzersiz dosya yolları kullanır. Adsız açık sahneyi önce kendiniz kaydedin.

## Dosyalar ve proje güvenliği

Değiştirilen kodlar: `FoxController.cs`, `FollowCamera.cs`, `AdventureDirector.cs`, `AdventureActor.cs`, `StoryNarrator.cs`, `ForestHUD.cs`, `StarCollectible.cs`, `SunnyForestBuilder.cs`.

Eklenen sistemler: `MovingPlatform.cs`, `FoxVisualFeedback.cs`, `MobileControls.cs`, `PatiLocalization.cs`, `AdventureSave.cs`, `PatiSoundFeedback.cs`, `MobileQualityPolicy.cs`, `AdventureDirector.Forest.cs`, `Editor/ProjectValidation.cs`, runtime/editor/test asmdef dosyaları ve EditMode/PlayMode testleri.

Eklenen içerik: ilk oynanabilir sahne ve ona ait materyal/mesh/mobil profil asset'leri; eğitim ve Ada için dört ek İngilizce WAV. README, STORY ve bu rapor güncellendi; Tools altında doğrulama komutları bulunur. Meta dosyaları korundu/eklendi.

Oturum başında bulunan `ProjectSettings/ShaderGraphSettings.asset` değişikliği ve `.vsconfig` korundu. `Assets/Scenes/SampleScene.unity` değiştirilmedi. Library, Temp, Logs ve UserSettings Git dışında tutulur. Ücretli asset/servis veya yeni paket kullanılmadı. **Bu oturumda commit ve GitHub push yapılmadı.** Private repository oluşturma işi önceki oturumdan hâlâ tamamlanmamıştır.

## Sonraki öneriler

### Son görünüm ve menü güncellemesi

Kullanıcı geri bildirimi üzerine duraklatma menüsünün sağ üstüne belirgin X eklendi; X, Devam ve Esc aynı güvenli devam işlemini kullanır. Menü başlığı düğmeyle üst üste gelmez. Oyun sırasında Unity Rendering Debugger'ın Display Stats ve kalıcı debug panelleri devre dışı bırakılır.

Yeni `NaturalGeometry.cs` ile düzensiz kaya/tepe mesh'leri, çam silüetleri ve dallı ağaçlar oluşturulur. Zemin, kaya, gövde ve yapraklarda düşük çözünürlüklü prosedürel yüzey dokuları ve mat materyaller kullanılır. Pembe çatılar eğimli taş renkli çatılara dönüştürüldü; gölet ovalleştirildi ve kıyı taşları eklendi. Sis, soluk doğal renkler, yabani otlar ve daha küçük tilki kafa/göz oranları ile oyuncak görünümü azaltıldı. Kapının opak sarı yüzeyi kaldırıldı; çerçevesinden ilerisi görünür.

Bu, daha doğal bir stilize görünümdür; fotogerçekçi model veya taranmış doku değildir. Önceki sahnelere yazılmadan `SunnyForest_Natural` üretildi ve bağımlılık klasörü sahnedeki GUID'lerden belirlenerek taşındı. Editor/Android C# kod kontrolleri ve gerçek Unity sahne üretimi geçti. Yeni kapanış işlemi ve debug panelinin kapalı kalması dahil 12 PlayMode testi tekrar geçti. Yeni grafik önizleme `Docs/ForestNaturalOverview.png` dosyasına kaydedildi ve görsel olarak incelendi. Görünüm değişiklikleri fiziksel Android cihazda test edilmedi.

1. Orta segment Android cihazda oynanış, TTS ve performans ölçümü.
2. Türkçe kayıtlar ve çocuklarla kısa yönerge/ikon değerlendirmesi.
3. Hikâye ve duraklatma ekranlarının Canvas'a taşınması, ikonların görsel varlıklarla değiştirilmesi.
4. Tilki iskeleti, yürüyüş/koşu animasyonları ve çevre sesleri.
5. İkinci bölgeyi de ilk orman gibi özel bir rota, bulmaca ve yeni bir mekanikle geliştirme.
# Portal keşfi ve dövüş güncellemesi

## Canlı model güncellemesi

Son görsel istek için `PatiAdventure_Living_Ready.unity` eklendi. Yeni özel meshler, tilkinin eklemli bacakları/çenesi/kuyruğu, göz kırpma ve bekleme hareketleri; yaban domuzu, çakal ve buz muhafızı modelleri; düşman hareket/darbe tepkisi; dallı/rüzgârlı ağaçlar, bitki örtüsü ve platform döşemeleri hazırlandı. Önceki sahneler korunuyor. Açık projedeki kullanıcıya ait grafik/Android ayarı değişikliklerine dokunulmadı. Ayrıntılar ve gerçek Unity görüntüleri `Docs/LIVING_VISUALS.md` dosyasındadır.

20 PlayMode testi geçti. İlk kontrolde görülen darbe efektinin başlatılma hatası giderildi. Editor üretim testi süre sınırına takıldığı için asset üretimi batch işlemleriyle hızlandırıldı; tekrar çalıştırılan 9 Editor testinin tamamı geçti. Toplam 29 Unity testi başarılıdır. Kodlar Editor ve Android tanımlarıyla derlendi. Son sahnenin 43 referansı kontrol edildi, proje referansları çözüldü, tek dış referans URP paketine ait.

`Builds/Android/PatiAdventure_Living.apk` başarıyla üretildi: ARM64 / IL2CPP, 34.929.301 bayt, versionCode 2, minimum API 25, hedef API 36. APK Signature Scheme v2 doğrulandı. APK fiziksel tablette denenmedi. BuildResults toplam veri boyutu yerine gerçek APK dosya boyutunu raporlayacak şekilde düzeltildi. Sonuçlar `Docs/TestResults/LivingAndroidBuild.txt` içindedir. Commit/push yapılmadı.

Son kullanıcı isteği için ayrı `PatiAdventure_Portals_Ready.unity` sahnesi eklendi. Eski orman ve hikâye sahneleri korunuyor. Portal köyünden Fısıltı Ormanı, Kızıl Kanyon ve Buzul Zirveleri'ne istenen sırada gidilebilir; dönüş portalı ve köye dönüş düğmesi bulunur. Her dünyada 8 yıldız, 3 küçük gölge ve 6 canlı bir muhafız vardır. 5 yıldız ve 4 yenilmiş gölge bir ışık tohumu kazandırır; üç tohum köydeki Yaşam Ağacı'nı aydınlatır.

Tilkinin J/ISIR saldırısı öne 2 hasar, K/KUYRUK saldırısı çevresine 1 hasar verir. Saldırı bekleme süreleri, düşmanların takip alanı, saldırı hazırlığı, can göstergesi, geri itilme ve yenilme efekti uygulanmıştır. Dört ayaklı model oranları, sivri kulaklar, göğüs kürkü, yürüyüş ve kuyruk/saldırı hareketleri eklendi. Dünya renkleri, bitkileri, taş yapıları, kar/buz/vaha ve dağ biçimleri farklıdır. Yeni sahne kendi tam çözünürlüklü, 2x MSAA ve 1024 gölge haritalı URP profilini kullanır.

`Pati.Exploration.v1` ayrı kayıt anahtarı eski hikâye kaydını korur. Yıldız ve düşman kimlikleri dünya numarasıyla saklanır. Yeni İngilizce portal giriş ses kaydı eklendi; Türkçe Android anlatımı cihazın TTS paketine bağlıdır. Türkçe Editor ses kaydı henüz yoktur.

Unity'de portal sahnesinin eksik script sayısı 0, köydeki portal sayısı 3 olarak doğrulandı. 16 PlayMode testi (12 mevcut + 4 yeni entegrasyon testi) geçti: fiziksel portal geçişi ve üç dünya, ısırmayla yenilme/kaydetme, arkadaki düşmana kuyruk saldırısı/bekleme/duraklatma, fiziksel yıldız toplama ve iki dokunmatik saldırı düğmesi. Editor ve Android tanımlarıyla ayrı C# derleme kontrolleri geçti. İlk bağımsız kontroldeki HashSet kütüphane çakışması, küçük düşman listesi için List kullanılarak giderildi. Unity önizlemesinde kaydedilmiş köy nesnelerinin yeniden oluşturulan dünyaların üstünde kalması düzeltildi; bu düzeltme Editor önizlemesi içindir.

Son görsel düzenlemelerden sonra 16 PlayMode testi yeniden geçti; 9 EditMode testi de geçti. Toplam 25 test başarılıdır. Sonuçlar `Docs/TestResults/PortalPlayMode.xml`, `PortalEditMode.xml` ve `PortalSceneValidation.txt` içindedir. Ready sahnesinin 32 GUID referansı kontrol edildi; tüm proje referansları çözüldü, tek dış referans URP paketinin UniversalAdditionalCameraData scriptidir. Yinelenen asset GUID bulunmadı. Gerçek Unity görüntüleri `Docs/PortalVillage.png`, `PortalForest.png`, `PortalCanyon.png`, `PortalGlacier.png` ve `PortalFox.png` olarak kaydedildi.

Ayrıntılı oynanış ve kontrol açıklaması `Docs/PORTAL_ADVENTURE.md` dosyasındadır. APK ve gerçek telefon performans testi yapılmadı. Modeller ve hareketler prosedüreldir; profesyonel modellenmiş ve riglenmiş bir oyunun görsel kalitesine ulaşıldığı iddia edilmiyor. Commit veya push yapılmadı.

