# 🕸️ Sosyal Ağ Analizi ve Görselleştirme Aracı (SNA Tool)

**Ders:** Yazılım Laboratuvarı I  
**Tarih:** Ocak 2026  

### 👥 Ekip Üyeleri
* **[Denizhan Çil]** - [231307104]
* **[Meliha Damla Coşkun]** - [231307113]

---

## 1. Giriş ve Problemin Tanımı

### 1.1. Problemin Tanımı
Günümüzde sosyal medya, iletişim ağları ve biyolojik sistemler gibi birçok yapı, karmaşık ağlar (network) olarak modellenebilmektedir. Bu ağların büyüklüğü ve karmaşıklığı arttıkça; kişiler arasındaki en kısa yolların bulunması, etkili kişilerin (fenomenlerin) tespiti ve topluluk yapılarının analizi klasik yöntemlerle imkansız hale gelmektedir. Bu proje, bu karmaşık verileri **Çizge Teorisi (Graph Theory)** prensipleriyle modelleyerek analiz etmeyi hedefler.

### 1.2. Projenin Amacı
Bu projenin temel amacı; kullanıcıların ve etkileşimlerin bulunduğu bir sosyal ağı simüle eden, Nesne Yönelimli Programlama (OOP) prensiplerine uygun, etkileşimli bir masaüstü uygulaması geliştirmektir.

Uygulama şu temel işlevleri yerine getirmektedir:
* Sosyal ağ verilerinin **görselleştirilmesi** ve sürükle-bırak ile yönetimi.
* **Yol Bulma Algoritmaları** (Dijkstra, A*) ile ilişkilerin analizi.
* **Tarama Algoritmaları** (BFS, DFS) ile ağın keşfi.
* **Renklendirme Algoritmaları** (Welsh-Powell) ile ayrık toplulukların görsel ayrımı.
* Kullanıcı özelliklerine (Aktiflik, Etkileşim vb.) dayalı **Dinamik Ağırlık Hesaplaması**.

## 3. Projede Gerçeklenen Algoritmalar

Bu bölümde; `AlgorithmService.cs` ve `WelshPowell.cs` sınıflarında kodlanan algoritmaların teorik altyapısı, projedeki uygulama mantığı, karmaşıklık analizleri ve akış diyagramları detaylandırılmıştır.

### 3.1. Breadth-First Search (BFS) - Genişlik Öncelikli Arama

#### 📚 Literatür ve Tanım
BFS, 1959 yılında **Edward F. Moore** tarafından labirentlerde en kısa yolu bulmak için geliştirilmiş temel bir çizge tarama algoritmasıdır. Algoritma, başlangıç düğümünden başlayarak önce tüm komşularını, sonra o komşuların komşularını ziyaret ederek "halka halka" genişler.

#### ⚙️ Projedeki Çalışma Mantığı
Projede BFS, iki farklı amaçla kullanılmıştır:
1.  **Ağ Tarama:** Bir kullanıcıdan başlayarak erişilebilen tüm kullanıcı ağını keşfetmek (`FindShortestPath` metodunun temelinde ve tarama fonksiyonunda).
2.  **Veri Yapısı:** `Queue` (FIFO - İlk Giren İlk Çıkar) yapısı kullanılarak düğümler sırayla işlenir.

#### 🧮 Karmaşıklık Analizi
* **Zaman Karmaşıklığı:** $O(V + E)$ (V: Düğüm, E: Kenar sayısı).
* **Alan Karmaşıklığı:** $O(V)$ (Kuyrukta tutulan düğümler).

#### 🔄 Akış Diyagramı (Mermaid)

```mermaid
flowchart TD
    Start([Başla]) --> Init[Queue Oluştur ve Başlangıcı Ekle]
    Init --> CheckQ{Kuyruk Boş mu?}
    CheckQ -- Evet --> End([Bitir])
    CheckQ -- Hayır --> Dequeue[Düğümü Çıkar]
    Dequeue --> GetNeigh[Komşuları Getir]
    GetNeigh --> CheckVis{Ziyaret Edildi mi?}
    CheckVis -- Hayır --> Enqueue[Ziyaret Et ve Kuyruğa Ekle]
    Enqueue --> CheckQ
    CheckVis -- Evet --> CheckQ
