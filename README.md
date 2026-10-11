# PatiAdventure

4–7 yaş çocuklar için sevimli tilkili 3D platform macerası.

Unity **6000.3.26f1 (6.3 LTS)**, URP **17.3.0**, Input System **1.20.0**.

## Başlatma

1. Unity Hub ile bu klasörü açın ve script derlemesinin tamamlanmasını bekleyin.
2. `PatiAdventure > Create SunnyForest` menüsünü çalıştırın.
3. Oluşturulan sahneyi Project penceresinden açın ve Play'e basın.

Üretici mevcut sahneleri değiştirmez; SunnyForest zaten varsa yeni numaralı sahne oluşturur. Açık sahneniz korunur. Üretilen dosyalar `Assets/PatiAdventure/Generated` altında tutulur.

## Kontroller

WASD / ok tuşları: yürüme. Shift: koşma. Space: zıplama.
Android: sol alandaki yön kontrolü, sağdaki KOŞ ve ZIPLA düğmeleri. Çoklu dokunma desteklenir. Dokunmatik kontroller Editör'de fareyle de denenebilir.

Beş yıldızı toplayın. Düşünce başlangıca dönersiniz; toplanan yıldızlar korunur. Can kaybı ve zaman sınırı yoktur. Tilki prototipi geometrik parçalardan oluşur; lisanslı dış model gerektirmez.

## Android

Unity Hub'dan Android Build Support, SDK/NDK ve OpenJDK modüllerini kurun. Build Profiles'ta Android'e geçin, SunnyForest sahnesini Scene List'e ekleyin. Yatay ekran yönünü seçin. Gerçek cihazda dokunma, performans ve ekran güvenli alanlarını kontrol edin.

## Git

Assets (meta dosyaları dahil), Packages ve ProjectSettings sürümlenir. Library, Temp, Logs, UserSettings ve derleme çıktıları hariç tutulur. GitHub hedefi `omermnv/PatiAdventure` Private repository'dir. Repository henüz oluşturulmamıştır; push yalnızca kullanıcı onayından sonra yapılacaktır.
