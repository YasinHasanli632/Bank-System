BANKSYSTEM – Bank Əməliyyatlarının İdarəetmə Sistemi
------------------------------------------------------

📌 Layihə haqqında
BankSystem – bank əməliyyatlarının təhlükəsiz, sabit, genişlənə bilən və yüksək performanslı şəkildə idarə edilməsi üçün hazırlanmış mikroservis əsaslı backend layihəsidir.

Sistem bank müştərilərinin qeydiyyatı, hesab yaradılması, balansların izlənməsi, tranzaksiyaların icrası, sənədlərin saxlanması və müştəri məlumatlarının sinxron idarə olunması kimi real biznes funksionallıqlarını təmin edir.

Layihə enterprise səviyyəli prinsiplərə əsaslanır və aşağıdakı məqsədlərə xidmət edir:
- Xidmətlərin bir-birindən asılılığını azaltmaq
- Yüksək performans və genişlənmə imkanı
- Aydın servis limitləri (bounded contexts)
- Təhlükəsiz və audit oluna bilən sistem qurmaq
- Event-driven kommunikasiya vasitəsilə zəif coupling

------------------------------------------------------

🔧 İstifadə olunan texnologiyalar
- **C# / .NET 8**
- **ASP.NET Web API**
- **Mikroservis Arxitekturası**
- **Clean Architecture** (API → Application → Domain → Infrastructure)
- **CQRS Pattern**
- **Entity Framework Core (SQL Server)**
- **RabbitMQ – Event-Driven kommunikasiya**
- **Redis Cache** – sürətli oxuma əməliyyatları üçün
- **Ocelot API Gateway** – mərkəzləşdirilmiş marşrutlaşdırma
- **Swagger / OpenAPI** – API sənədləşməsi
- **Unit və Integration Testlər** – sistemin stabilliyi üçün

------------------------------------------------------

🧩 Servislər və məsuliyyət bölgüsü

📌 **CustomerService**
- Müştərilərin yaradılması, məlumatların yenilənməsi
- Müştəriyə bağlı sənədlərin və hesabların birləşdirilməsi
- Event-lərə reaksiya: DocumentUploadedEvent, TransactionCreatedEvent

📌 **AccountService**
- Hesabların yaradılması və balans məlumatlarının saxlanması
- Balans yoxlaması (tranzaksiya öncəsi)
- Hesab → Tranzaksiya əlaqəsi

📌 **TransactionService**
- Tranzaksiyaların yaradılması
- Hesab üzərindən balans yoxlama
- Uğurlu əməliyyata görə event generasiyası
- Loqlama və audit məqsədilə məlumat saxlanması

📌 **DocumentService**
- Müştəri sənədlərinin saxlanması
- Fayl yükləmə, yeniləmə və silmə əməliyyatları

📌 **FileStorageService**
- Faylların fiziki yaddaşa yazılması
- Geri oxunması
- DocumentService ilə inteqrasiya

📌 **OcelotGateway**
- API marşrutlarının mərkəzləşdirilmiş şəkildə yönləndirilməsi
- Authorization və Rate-limit kimi funksiyalar üçün genişləndirilə bilər

------------------------------------------------------

🔄 Event Flow nümunələri

🟦 CUSTOMERPHOTOUPLOADED EVENT – Qısa izah
1. İstifadəçi sənəd yükləyir
2. DocumentService faylı FileStorageService-ə göndərir
3. Fayl uğurla saxlanandan sonra DocumentService → DB-də qeyd yaradır
4. DocumentService → RabbitMQ vasitəsilə CustomerPhotoUploadedEvent göndərir
5. CustomerService event-i qəbul edir və müştərinin sənəd siyahısını yeniləyir

👉 Bu yanaşma servislər arasındakı asılılığı azaldır.

🟥 TRANSACTIONCREATED EVENT – Qısa izah
1. İstifadəçi API Gateway-dən tranzaksiya göndərir
2. TransactionService → AccountService-dən balans istəyir
3. Balans uyğun olduqda tranzaksiya saxlanır
4. TransactionService → RabbitMQ üzərindən event göndərir
5. CustomerService bu event ilə müştəriyə aid əməliyyat tarixçəsini yeniləyir

👉 Bu “event-driven” yanaşma real bank sistemlərinə çox bənzəyir.

------------------------------------------------------

🗄 Məlumat bazası quruluşu

📌 **SQL Server** üzərində qurulub  
📌 Normalizasiya səviyyəsi:
- 1NF – Atomik dəyərlər
- 2NF – Composite key dependency yoxdur
- 3NF – Transitive dependency yoxdur

📌 Əsas relation-lar:
- **Customer (1) → (N) Account**
- **Customer (1) → (N) Document**
- **Account (1) → (N) Transaction**

Bu struktur həm performans, həm də təhlükəsizlik baxımından optimaldır.

------------------------------------------------------

🧪 Testlər

🔹 **Unit Testlər**
- Servislərin biznes məntiqi test olunur
- InMemoryDatabase istifadə edilir
- Repository mock-larından istifadə olunub

🔹 **Integration Testlər**
- Controller-lərin real davranışı test olunur
- API cavabları yoxlanır

Testlər sistemin stabilliyini və genişlənə bilən strukturunu təmin edir.

------------------------------------------------------

🚀 Quraşdırma və işə salma

⚙️ **1. Məlumat bazasını konfiqurasiya edin**
`appsettings.json` faylında connection string dəyişdirilir.

⚙️ **2. Migration tətbiq edin**
cd BankSystem
dotnet ef database update

⚙️ **3. Servisləri işə salın**
Hər servisin öz qovluğuna keçib:

cd Services/CustomerService.API
dotnet run

cd Services/AccountService.API
dotnet run

və s.

⚙️ **4. API Gateway-i işə salın**
cd OcelotGateway
dotnet run

Artıq bütün sistem gateway üzərindən əlçatan olacaq.

------------------------------------------------------

📌 Nəticə
BankSystem layihəsi .NET ekosistemində mikroservis arxitekturası, servis ayrılığı, event-driven kommunikasiya, cache mexanizmləri, API Gateway, test yazma və enterprise səviyyəli düşüncə tərzi üzrə geniş praktiki bacarıqlarımı nümayiş etdirir.

Bu sistem real bank proseslərinin backend tərəfində necə işlədiyini modelləşdirir və genişləndirməyə tam hazır strukturda hazırlanmışdır.
