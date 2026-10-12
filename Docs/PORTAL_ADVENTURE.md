# Pati: portallar ve keşif

Yeni oynanış, eski 36 etaplık hikâye akışından ayrı bir sahnede çalışır. Eski sahneler ve eski hikâye kaydı korunur. Unity menüsünde `PatiAdventure > Create Portal Adventure` yeni ve benzersiz bir sahne üretir.

Hazır sürüm: `Assets/PatiAdventure/Generated/PatiAdventure_Portals_Ready.unity`. Play modunu durdurun, Assets > Refresh seçin, bu sahneyi açıp Play'e basın. Dil seçiminin ardından “Keşfe başla” ile portallara ulaşabilirsiniz.

Gerçek Unity önizlemeleri: [Köy](PortalVillage.png), [Orman](PortalForest.png), [Kanyon](PortalCanyon.png), [Buzul](PortalGlacier.png), [Tilki](PortalFox.png).

## Dünyalar

- Portal Köyü: evler, merkez meydanı, Yaşam Ağacı ve üç açık portal.
- Fısıltı Ormanı: ağaçlar, gölet, köprü, kulübe, basamaklı tepe ve hareketli platform.
- Kızıl Kanyon: katmanlı kaya kuleleri, vaha, antik taş kemer ve keşif yolları.
- Buzul Zirveleri: karla kaplı dağlar, çamlar, buz kristalleri ve donmuş göl.

Portaldan yürüyerek geçilir. Bölüm tamamlama ekranı veya kilitli dünya sırası yoktur. Dünyalardaki dönüş portalı ya da ekrandaki “Köye dön” düğmesi köye götürür. Kısa bir geçiş bekleme süresi, aynı portalın oyuncuyu hemen geri göndermesini engeller.

Her keşif dünyasında 8 yıldız, 3 küçük gölge ve 1 dayanıklı muhafız bulunur. En az 5 yıldız ve 4 gölge, o dünyanın ışık tohumunu kurtarır. Üç tohum köyün ağacını yeniden aydınlatır. Sonrasında keşif devam eder.

## Kontroller

WASD/oklar yürütür, Shift koşturur, Space zıplatır; havadayken bir kez daha zıplanabilir. Sağ fare düğmesiyle kamera döndürülür. J yakın ve öndeki düşmana ısırma saldırısı yapar (2 hasar); K çevredeki düşmanlara kuyruk dönüşü yapar (1 hasar). Küçük gölgelerin 2, muhafızın 6 canı vardır. Isırmanın beklemesi 0,42 saniye, kuyruk dönüşünün 0,85 saniyedir. Telefon arayüzünde ISIR ve KUYRUK düğmeleri vardır.

Canavarlar yavaş yaklaşır, vurmadan önce 0,8 saniyelik hazırlık hareketi yapar. Oyuncu vurulursa güvenli başlangıç noktasına döner; yıldızlarını kaybetmez. Yenilen canavarlar küçülerek kaybolur. Kan veya parçalanma yoktur.

## Görseller ve kapsam

Dünyaların renkleri, bitkileri, yapıları ve dağları farklıdır. Taş, toprak ve ahşap için üretilen dokular kullanılır. Yeni sahnenin URP profili tam çözünürlük, 2x MSAA ve 1024 gölge haritası kullanır; proje genelindeki grafik profilleri değiştirilmez. Android performansı cihazda ölçülmelidir.

Tilki dört ayaklı bir görünüme, sivri kulaklara, göğüs kürküne, yürüyüş ve saldırı hareketlerine sahiptir. Bunlar kodla üretilen basit modeller ve hareketlerdir; profesyonel rigli model, elle hazırlanmış çevre ve animasyon kalitesine henüz ulaşılmış değildir. Bu sürüm portal keşfi ve dövüş için oynanabilir bir temeldir.

TR/EN seçim korunur. İngilizce portal giriş anlatımı yerel ses kaydıdır. Türkçe giriş anlatımı Android cihazın TTS ses paketini kullanır; Editor için Türkçe ses kaydı henüz yoktur.

## Kayıt

`Pati.Exploration.v1` oyuncunun topladığı yıldızları ve yendiği gölgeleri dünya kimliğiyle ayrı kaydeder. Dünyaya tekrar girince bu nesneler yeniden belirmez. Oyun açılışta köyde başlar; dünyaların ilerlemesi korunur. Hikâye modu kayıtlarıyla karışmaz.

Doğrulama, açık kullanıcı projesini kapatmadan `Temp/UnityValidation-*` kopyasında yapılır. Commit veya push yapılmaz.
