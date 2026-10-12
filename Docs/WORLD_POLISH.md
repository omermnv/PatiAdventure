# Kapı çevreleri ve güncel tablet sürümü

12 Ekim 2026.

Yeni sahne: `Assets/PatiAdventure/Generated/PatiAdventure_World_Polish.unity`. Üç temalı köşe kapısı ve kesintisiz tilki kuyruğu bu sahnede korunur.

Kapı önlerinde artık dünyaya özgü zemin adaları vardır. Orman girişinde koyu toprak, yaprak örtüsü ve alçak bitkiler; kızıl kanyon girişinde kum/kızıl toprak, çakıllar ve dallı kaktüsler; buzul girişinde geniş kar örtüsü, küçük kar parçaları ve buz kristalleri bulunur. Küçük ayrıntılar malzemeye göre ortak meshlerde birleştirilir. Ortadaki yürüyüş koridorunda yeni çarpışma engelleri yoktur.

Doğrulama kopyaları Unity'nin temizleyebildiği `Temp` yerine Git dışında tutulan `.codex-validation` klasöründe hazırlanır. Eski doğrulama yollarını kullanan komutlar da desteklenir. `Tools/Prepare-UnityValidation.ps1 -Preview` ilk kopyada orman/portal sahnelerini ve grafik önizlemelerini hazırlar; `Tools/Run-UnityValidation.ps1 -Mode WorldAndroid` doğrulanmış son portal sahnesini Android APK'ya dönüştürür.

Gerçek Unity görüntüleri: [orman](WorldPolish_Gateway1.png), [kanyon](WorldPolish_Gateway2.png), [buzul](WorldPolish_Gateway3.png), [köy](WorldPolish_PortalWorld0.png).

Editor ve Android kod kontrolleri geçti. Grafik cihazı açık Unity çalıştırmasında **23 PlayMode testi geçti, 0 başarısız, 0 atlanan**. Kanıt: [WorldPolishPlayMode.xml](TestResults/WorldPolishPlayMode.xml). Aktarılan sahnenin 74 GUID referansı çözülür; yinelenen proje GUID'si yoktur. Kanıt: [WorldPolishTransfer.txt](TestResults/WorldPolishTransfer.txt).

Projenin varsayılan build listesinde bu yeni sahne tek etkin sahnedir. Android sürüm kodu 4 olarak güncellendi. Önceki sahneler korunur.

## Tablet APK'sı

`Builds/Android/PatiAdventure_World_Polish.apk` üretildi: 36.501.165 bayt, ARM64 / IL2CPP, sürüm kodu 4, minimum API 25, hedef API 36. APK Signature Scheme v2 doğrulaması başarılıdır. Uygulama kimliği ve imza sertifikası önceki `Living_Fixed` APK'sıyla aynıdır; mevcut uygulamanın üzerine kurulabilir. APK fiziksel tablette kurulup denenmedi.

APK'yı USB ile tablete kopyalayın. Tabletin Dosyalar uygulamasında dosyaya dokunup Yükle/Güncelle seçin. Gerekirse Dosyalar uygulamasına bilinmeyen uygulama yükleme izni verin.

SHA256: `C399484C27A4FC2077998AB61D50F770C6326EE4B045F193178E74F2F8689398`. İmza sertifikası SHA256: `8786d394f1dc85d044352302486a31c9c565f6004af581cbfc735ed6c28d53ba`. Derleme kanıtı: [WorldPolishAndroidBuild.txt](TestResults/WorldPolishAndroidBuild.txt).
