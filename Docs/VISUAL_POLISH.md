# Karakter ve çevre ayrıntıları

12 Ekim 2026.

Unity'de `Assets/PatiAdventure/Generated/PatiAdventure_Visual_Polish.unity` sahnesini açıp Play'e basın. Project aramasında `Visual_Polish` yazabilirsiniz. Bu sahne yeni karakter meshlerini içerir; mevcut portal sahneleri de Play sırasında yeni çevre dekorlarını oluşturur.

## Görsel değişiklikler

- Tilki ve çakalın üst/alt bacaklarında eklemlere bağlanan, incelen özel meshler kullanılır. Mevcut yürüyüş, koşu, zıplama ve saldırı hareketleri aynı eklemlerle çalışır.
- Yanak ve göğüste kürk çıkıntıları, kuyrukta incelen beyaz uç ve küçük kürk parçaları bulunur.
- Yaban domuzunun gözlerine göz bebeği ve kaş; sırtına eğimli kıl çıkıntıları eklendi.
- Köy meydanından üç portala uzanan, zemine yakın taş döşemeler eklendi.
- Köy ve ormanda eğrelti kümeleri, mantarlar ve çakıl grupları bulunur.
- Kanyonda küçük aşınmış kaya katmanları ve kuru bitki kümeleri; buzulda sivri kristal kümeleri ve kar tabanları bulunur.

Yeni dekorlar çarpışma bileşeni taşımaz. Küçük parçalar malzemeye göre en fazla beş ortak meshte birleştirilir. Eğrelti yaprağı beş köşeli düşük poligonlu bir meshtir. Ortak meshler dünya değişiminde mevcut mesh yaşam döngüsüyle temizlenir.

Karakterler prosedürel modellerdir. Bu güncelleme profesyonel bir karakter rig'i veya nihai sanat varlıkları eklemez. Tablet performansı fiziksel cihazda ölçülmedi; önceki APK bu güncellemeyi içermez.

## Doğrulama

Editor ve Android C# derleme kontrolleri geçti. Unity'nin grafik cihazı açık PlayMode çalıştırmasında **21 test geçti, 0 başarısız, 0 atlanan**. Bunlar gerçek takip kamerasının köy ve üç dünyadaki giriş görüntüsünü, hareketi, portal geçişini, dövüşü, eklem hareketlerini ve platform dekorlarının taşınmasını kapsar. Sonuç: [VisualPolishPlayMode.xml](TestResults/VisualPolishPlayMode.xml).

Yeni sahne ayrı doğrulama kopyasında üretildi. Gerçek Unity önizlemeleri incelendi: [tilki](PolishedFox.png), [köy](PolishedVillage.png), [orman](PolishedForest.png), [kanyon](PolishedCanyon.png), [buzul](PolishedGlacier.png) ve [yaban domuzu](PolishedBoar.png). Bunlar Editor görüntüleridir.

Aktarılan sahnenin 52 GUID referansı doğrulandı: 49 proje varlığı, bir URP scripti ve iki Unity yerleşik kaynağı. Çözümlenmeyen referans veya yinelenen proje GUID'si yoktur. Kanıt: [VisualPolishTransfer.txt](TestResults/VisualPolishTransfer.txt).

Önizleme üretimini tekrarlamak için mevcut doğrulama kopyasında `Tools/Run-UnityValidation.ps1 -Mode PolishedPreview` kullanılır. Bu mod benzersiz bir portal sahnesi üretip dört bölgeyi ve karakterleri çizer.
