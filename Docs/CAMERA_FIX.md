# Kahverengi ekran düzeltmesi

Köydeki dağ halkası başlangıç noktasına fazla yakındı. Kamera çarpışma sistemi kamerayı yaklaşık 0,4 metreye çekiyor, görüntünün büyük bölümünü tilkinin turuncu/kahverengi gövdesi kaplıyordu. Hata Unity Play modunda gerçek takip kamerasıyla yeniden üretildi.

Dağların merkezden uzaklığı harita genişliğinin 0,57 katından 0,75 katına çıkarıldı. Köy başlangıcı açık meydanın önüne taşındı. Harita oluşturma ve ışınlanma sonunda fizik konumları eşitlenir ve takip kamerası yeniden konumlanır. Eski sahneler korunur; kod düzeltmesi Play sırasında eski portal sahnelerine de uygulanır.

Yeni sahne: `Assets/PatiAdventure/Generated/PatiAdventure_Living_Fixed.unity`. Play'i durdurun, Assets > Refresh seçin, Project aramasına `Living_Fixed` yazın, sahneyi çift tıklayıp Play'e basın. Dil seçimi ardından Keşfe başla düğmesini kullanın.

Tablet güncellemesi: `Builds/Android/PatiAdventure_Living_Fixed.apk`, versionCode 3. Önceki uygulamanın üzerine kurarak ilerlemeyi koruyabilirsiniz. Unity'den APK oluştururken Scene List'te yalnızca yeni sahneyi etkinleştirin.

Android ARM64 / IL2CPP derlemesi başarılıdır. APK 35.005.277 bayt, minimum API 25, hedef API 36; APK Signature Scheme v2 doğrulaması geçti. SHA256: `BED672962A61AB555F9C76A7D429CA3E4F469878BE423611B180395D0F252D6A`.

Görüntü kontrolü, sahnenin kendi mobil URP profili ve gerçek takip kamerası ile Play modunda çalışır. Dil ekranında ve dört haritanın girişinde kamera mesafesini, görüntüde farklı renklerin bulunmasını denetler; görüntüleri kaydeder. [Düzeltilmiş köy görüntüsü](CameraFixedVillage.png). Fiziksel tablet görüntüsü ve performansı bilgisayardaki bu kontrolün kapsamına girmez.

Yeni sahnede grafik cihazıyla çalıştırılan 21 PlayMode testinin 20'si ilk koşuda geçti; kamera, hareket, portal, dövüş ve model kontrolleri başarılıydı. Eski ormanın yıldız toplama testi oyuncuyu platformun dışında başlattığı için başarısız oldu. Test, temas alanının dışında ama platform üzerinde başlayacak şekilde düzeltildi ve aynı gerçek yürüyüş/toplama/kayıt kontrolünün tekrar koşusu geçti. Sonuçlar: `TestResults/LivingFixedGraphics.xml` (ilk koşu, 20 başarılı / 1 başarısız), `TestResults/ForestPickupGraphicsRetry.xml` (düzeltilmiş test, 1 başarılı). Ayrı başlangıç görüntü kontrolü `TestResults/CameraRendering.xml` dosyasındadır. Android derleme sonucu `TestResults/LivingFixedAndroidBuild.txt` dosyasındadır. Test düzeltmesi oyun kodunu değiştirmediği için yeni APK aynı doğrulanmış kamera düzeltmesini içerir.
