# Gün ışığı ve çevre görsel turu

12 Ekim 2026. Yeni sahne: `Assets/PatiAdventure/Generated/PatiAdventure_Daylight_Ready.unity`.

## Görsel değişiklikler

- Köy ve orman sabah ışığı, kanyon sıcak kum/toz renkleri, buzul soğuk mavi ışık kullanır. Gökyüzünün alt yarısı uzak sisle eşleştirilir; eski gri ufuk giderildi.
- Düşük kalite filtreli yumuşak gölgeler, sahnenin kendi URP profiline eklendi. Kullanıcının ortak render/volume varlıkları değiştirilmedi. Önizleme ve Android derlemesi sahnenin gerçek profilini kullanır; derleme öncesi Android kalite düzeyine bu profil atanarak gölge shaderlarının ayıklanması önlenir. Bu değişiklik yalnız izole doğrulama kopyasında geçicidir.
- Yuvarlak dağlar, dört asimetrik zirve ve aşınma çizgileri olan kapalı meshlerle değiştirildi. Paylaşılan her mesh 1.010 vertex ve 2.016 üçgendir.
- Kare zeminin çevresine sürekli, eğimli ve aynı meshle çarpışan arazi eklendi. Eski yürüyüş alanının içindeki zemin yüksekliği korunur; çevrede görünen arazi de yürünebilir.
- Çim/kum/kar, ince ayrıntılı 256×256 RGB24 dikişsiz dokular kullanır; mipmap ve trilinear filtreleme açıktır.
- Gölet ve vahada düzensiz kıyı, yavaş akan dalga dokusu, kıyı taşları ve sazlıklar bulunur. Su hareketi ortak materyali kopyalamadan MaterialPropertyBlock kullanır.
- Evler pencere/çerçeve, ahşap çıta, kapı kolu ve baca ayrıntıları aldı.
- Buzul çamları, konik hacimler yerine kar taşıyan ayrı dallı yüzeyler kullanır.
- Çimen/çiçek ayrıntıları arttırıldı ve malzemeye göre üç ortak meshte birleştirildi. Bu ayrıntılar geçiş veya dövüşü engelleyen collider taşımaz.
- Tilkinin gözleri, kaşları, burun profili, ağız çizgisi ve kavisli kulakları düzenlendi. Bacak/pati ölçüleri, eklem hiyerarşisi ve kesintisiz kuyruk korunur.

## Gerçek Unity görüntüleri

[Tilki yakın çekim](DaylightPortalFox.png), [kuyruk](DaylightFoxTailProfile.png), [köy](DaylightPortalWorld0.png), [orman](DaylightPortalWorld1.png), [kanyon](DaylightPortalWorld2.png), [buzul](DaylightPortalWorld3.png).

[Orman girişi](DaylightGateway1.png), [kanyon girişi](DaylightGateway2.png), [buzul girişi](DaylightGateway3.png).

Oyun çalışırken: [orman kamerası](DaylightRuntimeForest.png), [buzul kamerası](DaylightRuntimeGlacier.png), [orman basamak teması](DaylightStairForest.png), [kanyon basamak teması](DaylightStairCanyon.png), [buzul basamak teması](DaylightStairGlacier.png). Yukarıdaki yakın çekimler Editor sahne önizlemesidir; bu görüntüler runtime poz/ayak yerleşimini gösterir.

## Doğrulama

[16 EditMode testi](TestResults/DaylightEditMode.xml) geçti: mevcut kayıt/üretim kontrolleri ve yeni dağ collider yüzeyleri, kapalı kenarlar, geçerli normaller, farklı seed ve üç biyom doku tekrar kenarı kontrolleri dahildir.

Son geçerli sonuçlar toplam **67 ayrı başarılı kontroldür**: 51 PlayMode ve 16 EditMode. [PlayMode sonuç özeti](TestResults/DaylightPlayModeSummary.txt). Üç bölgede 24 yıldız/12 düşman ve köye dönüş normal hareket/zıplama/saldırı girdileriyle tamamlandı; [pati ve oynanış kayıtları](TestResults/DaylightContactAndPlaythrough.txt). Yeni arazi, eski kare sınırının ötesinde gerçek CharacterController hareketiyle kontrol edildi.

İlk [tam PlayMode turunda](TestResults/DaylightCorePlayMode.xml) 48/51 geçti. İki test materyal renkleri için fazla kesin eşitlik kullanıyordu; fark görsel düzeyin altındaydı, uygun 0,0001 toleransı eklendi. Dövüş testinin eski boş alanına genişleyen arazi ulaştığı için test alanı dışarı taşındı. Yalnız testler değiştirildi; gameplay kaynağı ve üretilen sahne aynı kaldı. Ardından [beş hedefli test](TestResults/DaylightFocusedPlayMode.xml) geçti. Özet bu son sonuçları esas alır; ham XML raporları değiştirilmedi.

[Sahne aktarımı](TestResults/DaylightTransfer.txt): 94 GUID referansı çözülür; eksik referans ve yinelenen proje GUID'si yoktur. Üretilen sahne ile projeye aktarılan sahnenin SHA256 değeri aynıdır.

Modeller prosedürel ve stilizedir. Fiziksel tablet performansı, pil tüketimi ve gerçek dokunmatik sürücü davranışı bu bilgisayarda doğrulanmadı. Eski 36 hikâye etabının tamamı bu görsel turun kapsamına girmez; köy ve üç portal bölgesi hedeflenir.

## Tablet çıktısı

[PatiAdventure_Daylight_Ready.apk](../Builds/Android/PatiAdventure_Daylight_Ready.apk) üretildi ve imzası doğrulandı. Boyut 37.940.888 bayt; ARM64 / IL2CPP, versionCode 7, minimum Android API 25 (Android 7.1), hedef API 36. Paket `com.UnityTechnologies.com.unity.template.urpblank` ve sertifika önceki APK ile aynıdır. [Build/imza kaydı](TestResults/DaylightAndroidBuild.txt), [50 C# dosyası kaynak karşılaştırması](TestResults/DaylightSourceManifest.txt).

APK SHA256: `5B49CC3A680771D5CACEAB68A1D99243E2492E11DC25C38E0ABBE0E88488614F`.

APK'yı tablete kopyalayın ve dosya yöneticisinden açın. İstenirse bu dosya yöneticisine yükleme izni verin; mevcut oyunun üzerine güncelleyin. Unity'de `Daylight_Ready` sahnesini açın. Önceki sahne ve APK'lar korunur.
