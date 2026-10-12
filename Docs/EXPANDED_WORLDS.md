# Genişletilmiş keşif alanları

13 Ekim 2026.

Güncel sahne: [PatiAdventure_Expanded_Ready.unity](../Assets/PatiAdventure/Generated/PatiAdventure_Expanded_Ready.unity). Project aramasına `Expanded_Ready` yazıp açın. Build listesinde tek etkin sahnedir.

Köyün düz ana zemini 44 × 44 metreden 60 × 60 metreye çıktı (alan yaklaşık %86 arttı). Orman, kanyon ve buzulun düz ana zeminleri 64 × 64 metreden 96 × 96 metreye çıktı (alan %125 arttı). Çevredeki yürünebilir yamaçlar da bu ölçülere göre büyür.

Köyün kuzeyinde iki ev, bahçe yolları, ağaçlar ve banklar var. Üç macera bölgesinde dört metre genişliğinde dış keşif yolları, iki dinlenme noktası ve kuzeyde bağlantı yolu bulunuyor. Ormanda ağaç açıklıkları, kanyonda seyir kayaları, buzulda kristal vadisi rotayı belirginleştiriyor. Süslemeler yeni yürüyüş yollarının dışında kalıyor.

Her bölgeye beş isteğe bağlı yıldız eklendi: toplam 39 yıldız ve mevcut 12 düşman. Eski `star0`–`star7` kimlikleri korunur; yeni yıldızlar `star8`–`star12` kimliklerini kullanır. Eski kayıtlar ve ışık tohumu için gereken beş yıldız/dört düşman koşulu korunur.

## Kontroller

Gerçek Unity sahne üretimi, Editor ve Android C# derleme kontrolleri geçti. Toplam **68 farklı başarılı kontrol**:

- [PlayMode](TestResults/ExpandedPlayMode.xml): 51 geçti, grafik cihazı gerektiren bir kontrol atlandı.
- [Grafik cihazıyla başlangıç kontrolü](TestResults/ExpandedStartupRendering.xml): atlanan kontrol ayrıca çalıştırıldı ve geçti. Böylece 52 farklı PlayMode kontrolü doğrulandı.
- [EditMode](TestResults/ExpandedEditMode.xml): 16 geçti, hata ve atlanan kontrol yok.

`ExpandedLoopsSupportWalkingAndAllBonusPickups` üç bölgede dış yolları gerçek hareket girdileriyle dolaşır; zemin temasını ve yeni yıldızların kaydedilmesini kontrol eder. Tam macera testi 39 yıldız, 12 düşman ve köye dönüşü kapsar. Arazi sınırı testi yeni boyutlara göre güncellendi.

Gerçek oyun kamerası görüntüleri: [köy](ExpandedRuntimeWorld0.png), [orman](ExpandedRuntimeWorld1.png), [kanyon](ExpandedRuntimeWorld2.png), [buzul](ExpandedRuntimeWorld3.png).

Yeni tablet paketi: [PatiAdventure_Expanded_Ready.apk](../Builds/Android/PatiAdventure_Expanded_Ready.apk), ARM64 / IL2CPP, versionCode **8**. [Derleme ve imza raporu](TestResults/ExpandedAndroidBuild.txt). APK v2 imzası doğrulandı ve önceki sürümle aynı sertifikayı kullanıyor. Mevcut oyunun üzerine kurulabilir. APK dosyaları proje kuralı gereği Git dışında tutulur; yerel `Builds/Android` klasöründedir. Fiziksel tablet kurulumu ve performansı ölçülmedi.
