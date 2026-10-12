# Basamak teması, canlı hareket ve kapsamlı oynanış kontrolü

12 Ekim 2026.

Yeni sahne: `Assets/PatiAdventure/Generated/PatiAdventure_Terrain_Ready.unity`.

## Hareket ve temas düzeltmeleri

- Küçük aşağı basamaklarda zemin temasını koruyan sınırlı ground snap eklendi. Zıplama sırasında uygulanmaz; duvar ve yüksek engeller üzerinden taşımaz.
- Dört ayak, gerçek model taban ölçüsünden hesaplanan temas noktalarıyla zemine yerleştirilir. Basılı ayak taşıyıcı yüzeye bağlanır; hareketli platformla birlikte taşınır. Gövde eğimi göğüs çevresinde döner ve ön/arka destek yüksekliğine uyum sağlar.
- Kapsül bir sonraki basamağa oturmuşken orta ışının önceki basamağı görmesiyle oluşan yanlış gövde yüksekliği giderildi.
- Hareketli platform ve terasın çarpışma yüzeyi, görünen 7 cm yüksekliğindeki döşemeyle eşitlendi.
- Platform yıldızı, taşıyıcısına bağlandı; sallanma hareketi artık yerel konum üzerinden uygulanır.
- Buz gölü, görünen buz geometrisiyle aynı yürünebilir yüzeye kavuştu. Kapı önü zemin/kar adaları ve kapı sütunlarına uygun fizik yüzeyleri eklendi; geçiş koridorları açıktır.
- Düşmanlar zemini takip eder, büyük engellerin içine yürümeyi engelleyen sorgular kullanır ve başka yükseklikteki oyuncuyu yerden vurmaz.
- Muhafız adı düşmanla hareket eder; buzul muhafızının adı can çizgilerinin üzerine binmez ve yenilince düşmanla birlikte gizlenir.

## Görsel değişiklikler

Ağaç ve orman kapısı tepelerinde ayrı yapraklar, damar dokusu ve renk farkları kullanılır. Pati oranları, dört parmak ve domuzun çift tırnakları düzenlendi; yüz/göğüs kürk çıkıntıları küçültüldü. Siyah ve krem kürke yüzey dokusu eklendi, parlaklık azaltıldı. Eklemli modelde aşırı zıplama/iniş esnemesi azaltıldı; doğal hayvan düşmanlara göz kırpma eklendi. Modeller prosedürel ve stilizedir; fotogerçekçi kürk simülasyonu veya profesyonel sanat varlığı eklenmedi.

## Test kapsamı

Kontroller köy ve üç portal dünyası, ortak hareket sistemi ve eski ilk orman akışını kapsar. Otomatik oynanış, normal dokunmatik hareket/zıplama ve saldırı komutlarıyla yıldız toplar ve düşmanları yener; görev tamamlamak için yıldız/düşman sayaçlarına doğrudan yazmaz. Merdiven temas testi, düşman müdahalesi olmadan altı yürüyüş/koşu rotasını ve basamak üstünde duran dört ayağı ayrıca ölçer. Fiziksel tablet, gerçek çoklu dokunma ve 36 eski hikâye etabının tamamı bu bilgisayar testlerinin kapsamında değildir.

## Doğrulanan sonuçlar

- Grafik cihazı açık [34 PlayMode testi](TestResults/TerrainPlayMode.xml) ve [9 EditMode testi](TestResults/TerrainEditMode.xml) geçti; başarısız veya atlanmış test yoktur.
- Üç bölgenin her birinde 8 yıldız/4 düşman tamamlandı; köye dönünce üç yaşam tohumu geri geldi. [Oynanış ve pati ölçüm kayıtları](TestResults/TerrainContactAndPlaythrough.txt).
- Merdivende yürüyüş/koşu, çapraz çıkış, rampada yürüyüş, eğimde dört pati teması, zıplama, çarpışma, düşme/kontrol noktası, hareketli platform, saldırı, kapı geçişi, kayıt ve duraklatma kontrolleri geçti.
- [Sahne aktarımı](TestResults/TerrainTransfer.txt): 80 referans çözülür; eksik referans ve yinelenen proje GUID'si yoktur.

Oyun çalışırken alınan görüntüler: [orman basamakları](TerrainStairForest.png), [kanyon basamakları](TerrainStairCanyon.png), [buzul basamakları](TerrainStairGlacier.png), [yapraklı orman girişi](TerrainForestGateway.png), [orman çevresi](TerrainForestOverview.png).

## Tablet çıktısı

[PatiAdventure_Terrain_Ready.apk](../Builds/Android/PatiAdventure_Terrain_Ready.apk) başarıyla üretildi; boyutu 36.721.897 bayttır. ARM64 / IL2CPP, sürüm kodu 5, minimum Android API 25 (Android 7.1), hedef API 36. Paket kimliği `com.UnityTechnologies.com.unity.template.urpblank`; APK v2 imzası doğrulandı ve sertifika önceki sürümle aynıdır. [Doğrulama kaydı](TestResults/TerrainAndroidBuild.txt).

APK SHA256: `8A80331D44F5FCE214632F92290CCF3FE1849EF00B7CBA1718BA2D497BE0E13D`.

Dosyayı tablete kopyalayın, dosya yöneticisinden açın, istenirse bu dosya yöneticisine uygulama yükleme izni verin ve mevcut oyunun üzerine güncelleyin. Fiziksel tablette kurulum, çoklu dokunma, performans ve pil tüketimi bu bilgisayarda doğrulanmadı.
