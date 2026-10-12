# Bölgeye özgü köy girişleri

12 Ekim 2026.

Yeni sahne: `Assets/PatiAdventure/Generated/PatiAdventure_Biome_Gateways.unity`. Unity Project aramasına `Biome_Gateways` yazıp sahneyi açın ve Play'e basın. Kesintisiz kuyruk modeli de bu sahnede bulunur.

Üç giriş köyün farklı köşelerinde, yüzleri meydana dönük olarak yer alır. Meydandaki taş yollar girişlerin yeni konumlarına uzanır.

- Fısıltı Ormanı: köklü iki ağaç gövdesi, birbirine uzanan dallar, yaprak tepeliği, tırmanan sarmaşıklar, eğreltiler ve çevrede dört ek ağaç.
- Kızıl Kanyon: sıcak kızıl/kum renkli katmanlar, aşınmış taş kemer, kaya payandaları ve kuru bitkiler.
- Buzul Zirveleri: sivri buz sütunları, kristal kemer, sarkıtlar ve kemer üstünde/zeminde kar örtüsü.

Geri dönüş girişleri de bulundukları dünyanın temasını kullanır. Dekoratif kemer parçaları çarpışmasızdır; çevredeki ağaçlar geçiş koridorunun yanlarındadır. Kapı tetikleyicisinin merkezi yerden 1,4 metre yukarıda, boyutları 3,2 × 2,8 × 1,8 metredir. Köyün başlangıç konumu ve kamera boşluğu korunur.

Modeller prosedürel, stilize doğal girişlerdir. Yeni APK üretilmedi; fiziksel cihaz performansı ölçülmedi.

Gerçek Unity görüntüleri: [orman girişi](ForestGateway.png), [kanyon girişi](CanyonGateway.png), [buzul girişi](GlacierGateway.png) ve [köyün genel görünümü](ThemedGatewayVillage.png).

## Doğrulama

Editor ve Android kod kontrolleri geçti. Son buz kemeri düzenlemesinden sonra grafik cihazı açık Unity çalıştırmasında **23 PlayMode testi geçti, 0 başarısız, 0 atlanan**. Yeni test, CharacterController'ı her girişin önünden yürütür ve doğru dünyaya geçtiğini denetler. Kamera başlangıçları, portal/dövüş, karakter/kuyruk deformasyonu ve hareket testleri de başarılıdır. Kanıt: [BiomeGatewayPlayMode.xml](TestResults/BiomeGatewayPlayMode.xml).

Aktarılan sahnenin 62 GUID referansı doğrulandı; çözümlenemeyen varlık ve yinelenen proje GUID'si yoktur. Kanıt: [BiomeGatewayTransfer.txt](TestResults/BiomeGatewayTransfer.txt).
