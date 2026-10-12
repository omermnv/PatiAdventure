# Check-in doğrulaması — 12 Ekim 2026

Unity 6000.3.26f1 ile ayrı doğrulama kopyasında çalıştırıldı.

- Editor C# derlemesi: başarılı (`Tools/Validate-Code.ps1`).
- Android C# derlemesi: başarılı (`Tools/Validate-Code.ps1 -Android`).
- Unity script derlemesi: başarılı.
- PlayMode: 51/51 başarılı; hata ve atlanan test yok. Grafik aygıtıyla başlangıç kamerası kontrolü ve üç bölgenin oynanış turu dahildir. [XML](CheckinPlayMode.xml).
- EditMode: 16/16 başarılı; hata ve atlanan test yok. [XML](CheckinEditMode.xml).

Bu turda APK yeniden üretilmedi ve fiziksel cihaz testi yapılmadı. Mevcut APK derleme raporu [DaylightAndroidBuild.txt](DaylightAndroidBuild.txt) içindedir.

Sandbox içindeki ilk Unity denemesi lisansa erişemedi. Kullanıcının lisansına erişebilen ortamda ilk EditMode denemesi assembly yükleme aşamasında durdurulup yeniden çalıştırıldı. Yukarıdaki sonuçlar tamamlanan son test turlarına aittir.
