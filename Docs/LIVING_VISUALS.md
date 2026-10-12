# Canlı karakter ve çevre güncellemesi

Güncel kamera düzeltmesi: `Assets/PatiAdventure/Generated/PatiAdventure_Living_Fixed.unity` ve `Builds/Android/PatiAdventure_Living_Fixed.apk`. Köydeki kahverengi görüntü sorununun nedeni ve kontrolü [CAMERA_FIX.md](CAMERA_FIX.md) dosyasındadır. Aşağıdaki bölüm önceki görsel sürümün geliştirme kaydıdır.

Bu sürümün sahnesi `Assets/PatiAdventure/Generated/PatiAdventure_Living_Ready.unity` dosyasıdır. Önce Play'i durdurun, Assets > Refresh seçin, bu sahneyi açıp yeniden Play'e basın. Project aramasında `Living_Ready` yazarak kolayca bulabilirsiniz. Eski sahneler korunur. Portal ilerlemesi aynı kayıt anahtarını kullanır.

## Tilki

Yeni model, UV koordinatları ve yumuşak yüzey normalleri olan özel meshlerden oluşur. Uzun ve daralan gövde, boyun, yanak kürkü, sivri kulak ve iç kulak, daralan burun, amber iris/pupil, alt çene, küçük dişler, dört eklemli bacak, pati/parmaklar ve üç eklemli kuyruk bulunur. İnce kürk çizgileri içeren yerel doku kullanılır. Modelin görsel hiyerarşisi `Fox Visual Pose/Fox Rig` altındadır; CharacterController ve oynanış kökü ayrıdır.

Yürüyüşte çapraz bacaklar sırayla sallanır; koşuşta adım genliği artar. Zıplarken bacaklar toparlanır. Beklerken nefes, baş/kulak hareketi ve göz kırpma vardır. J/ISIR çeneyi açıp kapatır ve başı ileri uzatır. K/KUYRUK gövdeyi bir tam tur döndürür; kuyruk eklemleri dönüşe eşlik eder. Saldırı sonunda poz dinlenme durumuna döner. Eski geometrik tilki sahnelerinde eski görsel hareket desteği korunur.

## Düşmanlar

- Orman: kahverengi yaban domuzu; burun, burun delikleri, küçük gözler, kulaklar, fildişi çıkıntılar, sırt kılları ve dört ayak.
- Kanyon: kum rengi çakal; daralan burun, sivri kulak, dört bacak ve kuyruk.
- Buzul: buz muhafızı; gövde/göğüs plakası, yüz/gözler, omuzlar, kollar, yumruklar, bacaklar ve buz tacı.

Muhafızlar daha büyük görünür. Düşmanların yürüyüş, saldırıya hazırlanma, darbe sırasında eğilme ve kısa parlama hareketleri vardır. Hasar veren nesneler artık yüzü ve uzuvları olan karakterlerdir. Çevre bitkileri dekoratiftir; oyuncuya hasar vermez.

## Çevre

Toprak dokusu daha geniş ölçekte ve daha düşük kontrastta üretilir. Taşlar yuvarlatılmış, ağaç gövdeleri daralan ve hafif eğrilen yüzeylerden oluşur. Dallar, kökler ve çok parçalı yaprak kümeleri eklenir; yapraklar hafifçe rüzgârda sallanır. Papatyalar, eğri çimen yaprakları, kaktüsler ve kar birikintileri ortamları ayırır. Hareketli platformda ahşap döşeme, alt destekler ve bağlantı başları vardır; bu parçalar platformla birlikte hareket eder. Terasta taş döşeme bulunur.

Gündüz gökyüzü ile gökyüzü/ufuk/zemin için ayrı ortam ışığı kullanılır. Portallar, yıldızlar, kayıt, kontrol ve görev kimlikleri korunur.

Modeller bu proje için kodla üretilmiştir; fotoğraf gerçekçiliği veya profesyonel dışarıdan alınmış rig/animasyon paketi iddia edilmez. Gerçek tablet performansı ayrıca ölçülmelidir. Mevcut tabletteki APK bu dosya değişikliklerini otomatik almaz; yeni sürümün APK'sı kurulmalıdır.

## Doğrulama

Unity'de 20 PlayMode testi geçti. Yeni testler çenenin açılıp kapanmasını, kuyruk dönüşünün tamamlanıp dinlenme pozuna dönmesini, bölgelere özgü düşman anatomisini ve platform döşemesinin hareketli desteği takip etmesini kontrol eder. Önceki hareket/kamera/portal/kayıt testleri de geçti. İlk çalıştırmada fark edilen darbe efektinin MaterialPropertyBlock başlatma hatası Awake'e taşınarak düzeltildi. Mesh adının Unity tarafından asset dosya adına dönüşmesi nedeniyle fazla dar olan test kontrolü, gövde geometrisini ve normal verisini kontrol edecek şekilde düzeltildi.

Editor ve Android tanımlarıyla C# derleme kontrolü geçti. Sahnedeki 43 GUID referansı çözüldü; tek paket referansı URP UniversalAdditionalCameraData scriptidir. Yinelenen asset GUID bulunmadı. Sonuç: `Docs/TestResults/LivingPlayMode.xml` ve `LivingSceneValidation.txt`.

İlk Editor kontrolünde 8 test geçti; sahne üretim testi asset işlemleri nedeniyle 180 saniyelik sınırı aştı. Üretici asset işlemlerini toplu gerçekleştirecek şekilde düzenlendi ve tekrar çalıştırılan 9 Editor testinin tamamı geçti. Toplam 29 Unity testi başarılıdır. EditMode sonucu `Docs/TestResults/LivingEditMode.xml` dosyasındadır. Gerçek Unity görüntüleri: [Tilki](LivingFox.png), [Yaban domuzu](LivingBoar.png), [Çakal](LivingJackal.png), [Buz muhafızı](LivingIceGuardian.png), [Orman](LivingForest.png).

## Android APK

Yeni dosya: `Builds/Android/PatiAdventure_Living.apk`. Android ARM64 / IL2CPP derlemesi başarıyla tamamlandı. Dosya 34.929.301 bayttır (yaklaşık 35 MB), uygulama adı PatiAdventure, versionCode 2, minimum API 25, hedef API 36'dır. APK Signature Scheme v2 doğrulaması geçti. SHA256: `5149F4ED6FC6E42F88F03CA5907392A198E56E2A6AF68B71F1873362EA0F5E85`.

Dosyayı tablete kopyalayıp açarak kurun/güncelleyin. Unity'den yeniden derleyecekseniz Scene List'te `PatiAdventure_Living_Ready` sahnesini seçin. APK'nın fiziksel tablette kurulumu, performansı ve TTS seslendirmesi bu bilgisayardan doğrulanmadı. Mevcut APK'yı yeniden derlemek eski görselleri gösterebilir; sahne seçimini kontrol edin.
