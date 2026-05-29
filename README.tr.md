# Smart Quotation & Pricing Engine

![Backend CI](https://github.com/the-atasoy/PTN-SmartQuotationPricingEngine/actions/workflows/backend.yml/badge.svg)
![Frontend CI](https://github.com/the-atasoy/PTN-SmartQuotationPricingEngine/actions/workflows/frontend.yml/badge.svg)

*Bu dokümanı [İngilizce](README.md) okuyun*

Bu proje, donanım ürünleri (HMI, Led Panel, LCD) için geliştirilmiş bir teklif yönetim simülasyonudur. Sistem, kullanıcıların Next.js arayüzünden ürün seçip teklif almasını; yöneticilerin ise (Admin) bu talepleri bir Excel dosyası aracılığıyla fiyatlandırmasını ve müşteriye iletmesini sağlar. Teklif iletildiğinde, ürünlerin son fiyat ve tarih bilgileri otomatik olarak güncellenir ve tarihçe (history) olarak saklanır.

## 🔗 GitHub Bağlantısı
**Repository:** [https://github.com/the-atasoy/PTN-SmartQuotationPricingEngine](#)

---

## 🚀 Kullanılan Teknolojiler

- **Frontend:** Next.js (App Router), Tailwind CSS, TypeScript, `next-intl` (Çoklu dil desteği)
- **Backend:** .NET 10, Entity Framework Core (Code-First), MediatR (CQRS Pattern), DDD
- **Veritabanı:** PostgreSQL 16
- **Diğer Araçlar:** Docker & Docker Compose, Mailpit (Lokal SMTP Sunucusu), EPPlus (Excel İşlemleri)

---

## ⚙️ Kurulum Adımları ve Çalıştırma Komutları

Projeyi ayağa kaldırmanın en kolay yolu **Docker Compose** kullanmaktır.

### Seçenek 1: Docker Compose ile (Önerilen)

Sisteminizde Docker ve Docker Compose kurulu olmalıdır.

1. Projenin kök dizininde bir terminal açın.
2. Tüm servisleri (PostgreSQL, Mailpit, Backend API, Frontend) başlatmak için aşağıdaki komutu çalıştırın:
   ```bash
   docker-compose up --build
   ```
3. Uygulama ayağa kalktığında veritabanı migration'ları ve örnek veriler (Seed Data) **otomatik** olarak oluşturulacaktır.

**Erişim Adresleri:**
- **Frontend UI:** http://localhost:3000
- **Backend API:** http://localhost:5100
- **PostgreSQL:** localhost:5432
- **Mailpit Web UI (Mailleri Görmek İçin):** http://localhost:8025

### Seçenek 2: Lokal Geliştirme Ortamında (Manuel) Çalıştırma

Eğer projeyi Docker olmadan, geliştirme ortamında ayağa kaldırmak isterseniz:

1. **Altyapı Servislerini Başlatın (Sadece DB ve Mail):**
   ```bash
   docker-compose up postgres mailpit
   ```
2. **Backend API'yi Başlatın:**
   ```bash
   cd backend
   dotnet restore
   dotnet run --project src/API
   ```
   *(Backend `http://localhost:5100` adresinde çalışacaktır. İlk çalışmada veritabanı tabloları ve örnek veriler otomatik atılır.)*

3. **Frontend'i Başlatın:**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
   *(Frontend `http://localhost:3000` adresinde çalışacaktır.)*

### 🔑 Varsayılan Kullanıcı Bilgileri (Seed Data)

Veritabanı ayağa kalktığında otomatik olarak aşağıdaki kullanıcılar oluşturulur:

| Rol | E-Posta | Şifre |
|---|---|---|
| **Admin** | admin@piton.com.tr | Admin123! |
| **User** | user@piton.com.tr | User123! |

---

## 🗄️ Veritabanı Yapısı (PostgreSQL)

![DB ER Diagram](docs/images/db-er-diagram.png)

Proje **Code-First** yaklaşımıyla geliştirilmiştir. Tablo ve kolon isimlendirmelerinde standartlara uyulmuş ve veritabanına `snake_case` formatında yansıtılmıştır. Tablolar arası ilişkiler Foreign Key (`FK`) kısıtlamaları ile güven altına alınmıştır.

Tüm tablolarda `id` (PK, UUID), `created_at`, `created_by`, `updated_at`, `updated_by` ve `is_deleted` kolonlarını içeren bir **BaseEntity** yapısı kullanılmıştır (Soft Delete mantığı geçerlidir).

Ana tablolar ve görevleri şunlardır:

### 1. `products`
Donanım ürünlerinin (HMI, Led Panel, LCD) tanımlandığı tablodur.
- **Kritik Kolonlar:** `last_request_price` (Son teklif edilen fiyat) ve `last_request_date` (Son teklif tarihi) burada tutulur. Teklif iletildiğinde sistem tarafından **anlık olarak güncellenir.**
- **Kolonlar:** `id`, `name`, `base_price`, `last_request_price`, `last_request_currency`, `last_request_date`, `... (base alanlar)`

### 2. `requests`
Teklifin üst bilgilerinin (header) saklandığı tablodur.
- **Kolonlar:** `id`, `request_no` (Örn: RQ-20250515-001), `customer_id` (FK), `total_amount`, `currency`, `status` (0: Beklemede, 1: Gönderildi, 2: İptal).

### 3. `request_items`
Teklife ait ürün satırlarının (line items) tutulduğu detay tablosudur. Her bir satır ilgili teklife (`request_id`) ve ürüne (`product_id`) bağlıdır.
- **Kolonlar:** `id`, `request_id` (FK), `product_id` (FK), `quantity` (Miktar), `unit_price` (Birim Fiyat), `discount` (İndirim Tutarı), `line_total` (Satır Toplamı).

### 4. `customers`
Teklif verilen müşteri veya cari bilgilerin ürün ve taleplerden bağımsız olarak saklandığı tablodur.
- **Kolonlar:** `id`, `name`, `email`, `phone`.

### 5. `product_price_histories`
Ürünlerin geçmiş teklif hareketlerini ve fiyat değişimlerini loglamak (izlemek) için kullanılır. Teklif gönderildiği anda tetiklenerek bu tabloya yeni bir kayıt atılır.
- **Kolonlar:** `id`, `product_id` (FK), `request_id` (FK), `price` (O anki birim fiyat), `currency`.

### 6. `users`
Sisteme giriş yapan yetkili (Admin) ve normal (User) kullanıcıların saklandığı tablodur. Şifreler BCrypt algoritması ile hash'lenerek tutulur.
- **Kolonlar:** `id`, `email`, `password_hash`, `role`.

### 🔍 Veritabanı İndeksleri (Database Indexes)
Performansı artırmak ve veri bütünlüğünü sağlamak adına veritabanında aşağıdaki indekslemeler yapılmıştır:
- **`requests`:**
  - `request_no` (Unique Index, `is_deleted = false` filtresi ile)
  - `status` (Duruma göre hızlı filtreleme için)
  - `created_at` (Tarihe göre sıralama işlemleri için)
- **`customers`:**
  - `email` (Unique Index, `is_deleted = false` filtresi ile)
- **`users`:**
  - `email` (Unique Index, `is_deleted = false` filtresi ile)
- **`product_price_histories`:**
  - `product_id` ve `created_at` (Composite Index - ürünün fiyat geçmişini hızlı listelemek için)

---

## 🔄 CI/CD Akışı

Projede GitHub Actions kullanılarak Frontend ve Backend için ayrı CI workflow'ları hazırlanmıştır. `.github/workflows/` dizini altındaki dosyalar sayesinde her push işleminde:
1. Docker imajları derlenir.
2. (Frontend için) Lint kontrolleri ve prod build işlemleri konteyner içerisinde doğrulanır.
3. Mimari yapının (prod vs dev) stabilitesi güvence altına alınır.

---

## 📚 Dokümantasyon

Proje mimarisi, kurulum ve çeşitli bileşenler hakkında detaylı bilgi için lütfen dokümantasyonları inceleyin:

### Genel
- [Lokalde Çalıştırma (Running Locally)](docs/running-locally.md)
- [Dağıtım ve CI/CD (Deployment)](docs/deployment.md)

### Backend
- [Mimari (Architecture)](docs/backend/architecture.md)
- [API Uç Noktaları (API Endpoints)](docs/backend/api-endpoints.md)
- [Veritabanı (Database)](docs/backend/database.md)
- [E-Posta Servisleri (Email Services)](docs/backend/email.md)
- [Çoklu Dil Desteği (Localization)](docs/backend/localization.md)
- [Docker Yapılandırması (Docker Setup)](docs/backend/docker.md)

### Frontend
- [Mimari (Architecture)](docs/frontend/architecture.md)
- [Sayfalar ve Yönlendirme (Pages & Routing)](docs/frontend/pages.md)
- [Durum Yönetimi (State Management)](docs/frontend/state-management.md)
- [API İstemcisi (API Client)](docs/frontend/api-client.md)
- [Kimlik Doğrulama (Authentication)](docs/frontend/authentication.md)
- [Doğrulama (Validation)](docs/frontend/validation.md)
- [Çoklu Dil Desteği (Localization)](docs/frontend/localization.md)
- [Docker Yapılandırması (Docker Setup)](docs/frontend/docker.md)
