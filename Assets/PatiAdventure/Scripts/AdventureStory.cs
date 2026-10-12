using UnityEngine;
namespace PatiAdventure
{
    public static class AdventureStory
    {
        public static readonly string[] TurkishNames = { "Güneşli Orman", "Papatya Vadisi", "Yosunlu Mağaralar", "Şelale Geçidi", "Rüzgârlı Dağlar", "Kristal Zirveler", "Mantar Korusu", "Kumtaşı Kanyonu", "Bulut Adaları", "Unutulmuş Bahçe", "Gölge Kalesi", "Yaşam Ağacı" };
        public static readonly string[] EnglishNames = { "Sunny Forest", "Daisy Valley", "Mossy Caves", "Waterfall Pass", "Windy Mountains", "Crystal Peaks", "Mushroom Grove", "Sandstone Canyon", "Cloud Islands", "Forgotten Garden", "Shadow Castle", "Tree of Life" };
        public static readonly string[] TurkishStory = {
            "Pati sıcacık yuvasında uyandı. Bugün oyun günü olmalıydı! Ama Yaşam Ağacı'nın ışığı soluyordu. Baykuş Ada, kaybolan on iki ışık tohumunu bulmasını istedi. Önce biraz yürüyelim, koşalım ve zıplayalım!",
            "Papatyalar başlarını eğmişti. Tavşan Mino, sisin arkadaşlarını şaşırttığını anlattı. Pati çayır bekçilerini geçip ikinci ışık tohumunu bulacaktı.",
            "Mağarada her damla bir melodi çalıyordu. Uykulu taş yaratıkları yolu tutuyordu. Pati parlayan yıldızları izleyip mağaranın kalbine ilerledi.",
            "Şelalenin ardında unutulmuş bir patika vardı. Su damlası yaratıkları sıçrıyordu. Pati acele etmeden güvenli kayalara basmalıydı.",
            "Dağlarda rüzgâr şarkı söylüyordu. Bulut koyunları sisin içinde kaybolmuştu. Pati rüzgâr bekçilerini geçip yol gösterecekti.",
            "Kristaller artık parlamıyordu. Buz pofudukları ışığı saklıyordu. Pati sıcacık cesaretiyle zirveye tırmandı.",
            "Dev mantarların altında küçük bir köy uyuyordu. Zıpzıp mantarlar sis yüzünden huysuzlaşmıştı. Pati köye renklerini geri getirecekti.",
            "Kanyonun kayaları eski bir sırrı fısıldıyordu. Kum kabukluları tohum sandığını koruyordu. Pati yüksek geçitlerden ilerledi.",
            "Bulut adaları gökyüzünde yüzüyordu. Şaşkın gök kuşları yıldız yollarını kapatıyordu. Pati dikkatli sıçrayışlarla ışığı aradı.",
            "Unutulmuş bahçede kimse oyun oynamıyordu. Diken bekçileri yalnız kalmıştı. Pati bahçeye arkadaşlık ve ışık getirecekti.",
            "Gölge kalesinden üzgün bir ses geliyordu. Pati gölge pofuduklarını geçti. Devin korkutucu görünüşünün ardında bir sır vardı.",
            "Gölge devi Nox kötü değildi. Kaybolan dostunu ararken ışık tohumlarını toplamıştı. Pati ona el uzattı. Birlikte son ışığı Yaşam Ağacı'na götüreceklerdi." };
        public static readonly string[] EnglishStory = {
            "Pati woke in a cosy den. Today should have been a day for games! But the Tree of Life was fading. Ada the owl asked Pati to find twelve lost light seeds. First, let us practise walking, running and jumping!",
            "The daisies bowed their heads. Mino the rabbit said the mist had confused his friends. Pati would pass the meadow guardians and find the second light seed.",
            "Every drop played a melody in the cave. Sleepy stone creatures blocked the path. Pati followed the shining stars deeper inside.",
            "A forgotten trail hid behind the waterfall. Water creatures bounced across it. Pati would take time and land on safe rocks.",
            "The mountain wind was singing. Cloud sheep were lost in the mist. Pati would pass the wind guardians and guide them home.",
            "The crystals had stopped shining. Frost puffballs hid the light. Pati climbed the peak with a warm and brave heart.",
            "A little village slept beneath giant mushrooms. The mist had made the bouncing mushrooms grumpy. Pati would bring back the colours.",
            "The canyon rocks whispered an old secret. Sand shell creatures guarded a seed chest. Pati followed the high trails.",
            "Cloud islands floated in the sky. Confused sky birds blocked the star paths. Pati searched for light with careful jumps.",
            "Nobody played in the forgotten garden. The thorn guardians were lonely. Pati would bring friendship and light to the garden.",
            "A sad voice came from the shadow castle. Pati passed the shadow puffballs. A secret hid behind the giant's frightening appearance.",
            "Nox the shadow giant was not mean. He had gathered light seeds while searching for his lost friend. Pati offered a helping paw. Together, they would bring the last light to the Tree of Life." };
        public const string TurkishEnding = "On iki ışık tohumu dallarda parladı. Nox'un kayıp dostu, ağacın içinde uyuyan küçük ışık kuşuydu! Kuş uyandı, orman yeniden yeşerdi. Pati öğrendi: en büyük cesaret, anlamaya çalışmak ve yardım etmektir. Şimdi bütün dostlarıyla eve dönüp yeni bir güne merhaba diyebilirdi.";
        public const string EnglishEnding = "Twelve light seeds shone in the branches. Nox's lost friend was a little light bird sleeping inside the tree! The bird woke and the forest turned green again. Pati learned that the greatest courage is trying to understand and helping others. Now all the friends could return home and welcome a new day.";
        public static Color Ground(int chapter) => new[] { new Color(.3f,.68f,.22f), new Color(.5f,.75f,.3f), new Color(.2f,.4f,.3f), new Color(.25f,.6f,.5f), new Color(.4f,.6f,.3f), new Color(.65f,.82f,.9f), new Color(.4f,.6f,.3f), new Color(.8f,.6f,.35f), new Color(.65f,.8f,.95f), new Color(.3f,.6f,.4f), new Color(.3f,.25f,.45f), new Color(.4f,.8f,.35f) }[chapter];
    }
}
