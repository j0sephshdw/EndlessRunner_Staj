# 🏃‍♂️-Endless Runner 3D

Bu proje Unity tabanlı bir 3D sonsuz koşu oyunudur. Oyuncu, dinamik olarak üretilen üç şeritli bir yolda ilerlerken engellerin üzerinden zıplamalı, yüksek bariyerlerin altından yuvarlanarak geçmeli ve altınları toplayarak en yüksek skoru elde etmelidir.

## 🏗️ Proje Mimarisi

Proje, Unity'nin motor yapısına uygun olarak Bileşen Tabanlı bir mimari ile inşa edilmiştir. Spagetti koddan kaçınmak ve modülerliği sağlamak için sistemler birbirlerinden izole edilmiştir:

* **`PlayerController.cs`:** Karakterin fiziksel hareketleri (şerit değiştirme, zıplama, takla atma), animasyon-fizik senkronizasyonu (çarpışma kutusu yönetimi), dokunulmazlık hesaplamaları ve çarpışma algılamalarını yönetir.
* **`UIManager.cs`:** Oyunun arayüzünü (Can sistemi, Skor, Altın sayacı ve Game Over ekranı) merkezi olarak kontrol eder.
* **`TileObstacleSpawner.cs`:** Sonsuz yol hissini vermek için Z ekseninde obje üretimi yapar. Engellerin ve toplanabilir öğelerin üst üste binmesini engelleyen mesafe filtreleme algoritmalarına sahiptir.

## 🧩 Kullanılan Tasarım Kalıpları

Projede bellek yönetimi ve kodun sürdürülebilirliği açısından aşağıdaki mühendislik yaklaşımları kullanılmıştır:

* **Singleton Pattern:** `UIManager` sınıfı, bellekte sadece bir örneği bulunacak şekilde yapılandırılmıştır. Bu sayede `PlayerController` veya diğer sınıflar, `GetComponent` gibi maliyetli fonksiyonları çağırmadan skor ve can arayüzlerine doğrudan erişebilir.
* **Dynamic Generation & Memory Management :** Nesne üretiminde oluşabilecek bellek sızıntılarını önlemek için, yollara dizilen altın ve engeller bir List havuzunda tutulmuş ve `OnDestroy` metodolojisi ile yol modülü (Tile) oyuncunun arkasında kalıp silindiğinde, üzerindeki tüm çocuk nesnelerin de bellekten güvenle temizlenmesi güvence altına alınmıştır.

## 🚀 Derleme ve Test Etme

Projeyi yerel ortamınızda derlemek ve test etmek için aşağıdaki adımları izleyebilirsiniz:

1. **Gereksinimler:** Proje **Unity 6** (veya güncel Unity 2022+ sürümleri) ile geliştirilmiştir. 
2. **Kurulum:** Repoyu bilgisayarınıza klonlayın (`git clone <repo-url>`).
3. **Projeyi Açma:** Unity Hub üzerinden `Add -> Add project from disk` seçeneği ile klonladığınız klasörü seçin.
4. **Sahne Ayarı:** Proje açıldığında `Assets/Scenes` klasörü altındaki ana oyun sahnesini (`MainLevel`) çift tıklayarak açın.
5. **Oynatma:** Editörün üst kısmındaki `Play` (▶) butonuna basarak simülasyonu başlatın.

### 🎮 Kontroller
* **Sağ/Sol Yön Tuşları (veya A/D):** Şerit değiştirme
* **Yukarı Yön Tuşu (veya W / Space):** Zıplama (Normal engelleri aşmak için)
* **Aşağı Yön Tuşu (veya S):** Yuvarlanma/Takla atma (Yüksek engellerin altından geçmek için)
