# Tablet kontrolleri, kamera ve geçiş düzeltmeleri

12 Ekim 2026. Sahne: `Assets/PatiAdventure/Generated/PatiAdventure_Terrain_Ready.unity`.

## Yapılan değişiklikler

- Joystick, koşma/zıplama düğmesi ve kamera sürüklemesi uygulama odağı kaybolunca veya uygulama arka plana geçince parmak sahipliğini sıfırlar. Yeni dokunuş yeniden kullanılabilir; hareket/koşma basılı kalmaz.
- Kilitli joystick dokunuşu parmak sahipliğini almaz. Joystick, koşma, zıplama ve kamera ayrı parmaklarını korur; başka parmağın bırakılması etkin kontrolü kapatmaz.
- Kamera sürükleme miktarı Canvas ölçeğine bölünür; aynı ekran oranında daha yüksek çözünürlükteki sürükleme gereksiz hızlanmaz.
- Kamera, ışınlanma/yeniden doğma sırasında duvar kontrolünü anında uygular. Duraklamada biriken bakış, devam edilince kamerayı sıçratmaz. Engel kalkınca uzaklık yumuşak açılır.
- Dönüş kapısına geçiş beklemesi dolmadan girilirse içeride bekleyen oyuncu için yeniden denenir. Mevcut 1,5 saniyelik sınır ve duraklatma kontrolleri korunur.
- Isırık, kuyruk saldırısı ve düşman saldırısı, karakterlerin göğüsleri arasındaki katı yüzeyleri kontrol eder. Oyuncu/düşman gövdeleri ve tetikleyiciler engel sayılmaz. Saldırı hazırlığı sırasında araya giren duvar da vuruşu durdurur.
- Muhafızın geri itilmesi, kendi gövde yarıçapıyla en yakın engelin önünde durur; uygun zemin yoksa geri itiş uygulanmaz. Hareket ve geri itiş aynı zemin/engel sorgusunu kullanır.

## Doğrulama kapsamı

Yeni kontroller PlayMode testlerinde ayrı parmak kimlikleri, uygulama odak/arka plan mesajları ve Canvas ölçeğiyle çalıştırılır. Kapı testi, gerçek CharacterController hareketi ve fizik tetikleyicileriyle üç bölgenin dönüş geçişini kontrol eder. Kamera testi duvar, oyuncu gövdesi ve tetikleyici yüzeyleri ayırır. Dövüş testi katı engel, saldırı hazırlığı sırasında engel eklenmesi, engelin kaldırılması ve muhafız geri itişini gerçek cooldown ile kontrol eder.

İlk dar kapsamlı turda 11 testten 10'u geçti. Kamera testinin bulduğu aynı kare duraklatma hatası, yalnız `deltaTime` yerine ayrıca `timeScale` kontrol edilerek düzeltildi. [İlk teşhis kaydı](TestResults/MobileDiagnosticPlayMode.xml) son kaynak için geçerli değildir.

İlk tam turda 48 testten 47'si geçti. Büyük muhafızın geri itiş sorgusunun zeminle başlangıç çakışması, sorgu merkezi destek düzleminin üzerinde tutulup eğim normali dikkate alınarak düzeltildi. Gerçek gövde yüksekliği ve duvar yarıçapı korunur. Ardından [üç dövüş regresyonu](TestResults/MobileCombatFocusedPlayMode.xml) geçti. [İlk tam tur teşhisi](TestResults/MobileCombatDiagnosticPlayMode.xml) bu düzeltmeden önceki kaynaktır.

Bunlar Editor üzerindeki otomatik kontrollerdir. Fiziksel tabletin dokunmatik sürücüsü, pil tüketimi ve gerçek cihaz performansı ayrıca ölçülmelidir. Görseller önceki turdaki doğal yüzey/yaprak/pati düzenlemelerini içerir ve stilizedir.

ADB ile bağlı cihaz listesi kontrol edildi; bu oturumda bağlı/yetkili Android cihaz bulunmadı.

## Son kaynak sonucu

Grafik cihazı açık [48 PlayMode testi](TestResults/MobilePlayMode.xml) geçti; başarısız veya atlanan test yoktur. Üç bölgede normal hareket/zıplama/saldırı girdileriyle 24 yıldız ve 12 düşman tamamlandı; köye dönünce üç yaşam tohumu geri geldi. Merdiven/eğim, hareketli platform, kayıt ve duraklatma regresyonları da geçti. [Pati teması ve oynanış kaydı](TestResults/MobileContactAndPlaythrough.txt).

Son kaynakla [9 EditMode testi](TestResults/MobileEditMode.xml) de geçti. Toplam **57 ayrı test başarılıdır**; başarısız veya atlanmış test yoktur.

[Kaynak/sahne karşılaştırması](TestResults/MobileSourceTransfer.txt): 43 C# dosyası izole test kopyasıyla aynıdır. Bu tur kaydedilmiş sahne verisini değiştirmez; test sahnesi ve projedeki Terrain_Ready sahnesinin SHA256 değeri aynıdır. Yeni test GUID'leri tekildir.

## Tablet paketi

[PatiAdventure_Mobile_Ready.apk](../Builds/Android/PatiAdventure_Mobile_Ready.apk) başarıyla üretildi: 36.724.201 bayt, ARM64 / IL2CPP, sürüm kodu 6, minimum Android API 25 (Android 7.1), hedef API 36. APK v2 imzası doğrulandı. Paket kimliği `com.UnityTechnologies.com.unity.template.urpblank`; sertifika önceki sürümle eşleşir. [Build/imza kaydı](TestResults/MobileAndroidBuild.txt).

APK SHA256: `0E22F294762B082CE9E3A236A9F07C0DB50706450BDC65F6E7DBFA51520F31B1`.

APK'yı tablete kopyalayın ve dosya yöneticisinden açın. İstenirse bu dosya yöneticisine yükleme izni verin; mevcut oyunun üzerine güncelleyin. Varsayılan build sahnesi Terrain_Ready, proje Android sürüm kodu 6'dır.
