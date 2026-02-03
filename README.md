# SecureMailApp – Secure Full-Stack Email System

---

## 🇹🇷 Türkçe

### Projenin Amacı
Bu proje, Kriptografiye Giriş dersi kapsamında geliştirilmiş tam kapsamlı (full-stack) güvenli bir mesajlaşma uygulamasıdır.  
Amaç, modern yazılım geliştirme teknolojileri ile kriptografik teknikleri birleştirerek güvenli bir e-posta sistemi oluşturmaktır.

Proje ile aşağıdaki güvenlik hedefleri sağlanmaktadır:

- Gizlilik (Confidentiality)
- Bütünlük (Integrity)
- Kimlik Doğrulama (Authentication)
- İnkar Edilemezlik (Non-repudiation)

---

### Kullanılan Teknolojiler

Bu proje tamamen modern .NET teknolojileri kullanılarak geliştirilmiştir:

- **ASP.NET Core MVC** – Web uygulama altyapısı  
- **Entity Framework Core** – ORM ve veritabanı yönetimi  
- **SQLite** – Hafif ve taşınabilir veritabanı  
- **C# ve .NET 8** – Uygulama geliştirme dili  
- **System.Security.Cryptography** – Kriptografik işlemler  
- HTML / CSS / Razor Views – Kullanıcı arayüzü  

Bu sayede proje, hem güvenlik odaklı hem de tam bir full-stack uygulama olarak tasarlanmıştır.

---

### Proje Ne Sağlar?

Uygulama aşağıdaki özellikleri içerir:

- Kullanıcı kayıt ve giriş sistemi  
- Güvenli parola saklama  
- Kullanıcılar arası şifreli mesajlaşma  
- Dijital imza ile mesaj doğrulama  
- Güvenli veritabanı depolama  
- Modern ve kullanıcı dostu arayüz  

---

### Kullanılan Kriptografik Yöntemler

- **Parola Güvenliği:** Salt + SHA-256 ile parola hashleme  
- **Gizlilik:** AES-256 (CBC) ile mesaj şifreleme  
- **Anahtar Paylaşımı:** RSA-2048 ile AES anahtarının şifrelenmesi  
- **Bütünlük:** SHA-256 hash doğrulaması  
- **Kimlik Doğrulama:** RSA dijital imzalar  

---

### Sistem Nasıl Çalışır?

#### 1) Kullanıcı Kaydı ve Giriş
- Kullanıcı kayıt olurken parola düz metin olarak saklanmaz  
- Rastgele bir salt üretilir  
- Parola şu yöntemle hashlenir:  
  `hash = SHA256(salt || password)`  
- Veritabanına yalnızca hash ve salt kaydedilir  
- Aynı zamanda kullanıcı için RSA anahtar çifti oluşturulur  
- Kullanıcı giriş yaparken hash doğrulaması gerçekleştirilir  

#### 2) Mesaj Gönderme
- Kullanıcı arayüzü üzerinden mesaj yazılır  
- Mesaj içeriği AES-256 ile şifrelenir  
- AES anahtarı, alıcının RSA public key’i ile şifrelenir  
- Mesajın hash değeri hesaplanır  
- Hash, gönderenin private key’i ile imzalanır  
- Tüm veriler veritabanına şifreli olarak kaydedilir  

#### 3) Mesaj Alma
- Alıcı gelen kutusunu açar  
- Sistem RSA private key ile AES anahtarını çözer  
- AES anahtarı ile mesaj içeriği çözülür  
- Dijital imza doğrulanır  
- Hash kontrolü yapılarak mesajın değiştirilmediği onaylanır  

#### 4) Veritabanı Güvenliği
- Veritabanında hiçbir zaman düz metin mesaj saklanmaz  
- Şifrelenmiş BLOB veriler tutulur  
- Veritabanı ele geçirilse bile içerikler okunamaz  

---

### Projenin Kapsamı

Bu proje:

- Modern bir web uygulamasıdır  
- Güvenli yazılım geliştirme prensiplerini uygular  
- Kriptografi kavramlarının pratik uygulamasını gösterir  
- Full-stack geliştirme süreçlerini içerir  

---

## 🇬🇧 English

### Project Objective
This project is a full-stack secure messaging application developed for the Introduction to Cryptography course.  
The aim is to combine modern software development technologies with cryptographic techniques to build a secure email system.

The project ensures the following security goals:

- Confidentiality  
- Integrity  
- Authentication  
- Non-repudiation  

---

### Technologies Used

This project is built using modern .NET technologies:

- **ASP.NET Core MVC** – Web application framework  
- **Entity Framework Core** – ORM and database management  
- **SQLite** – Lightweight and portable database  
- **C# and .NET 8** – Application development language  
- **System.Security.Cryptography** – Cryptographic operations  
- HTML / CSS / Razor Views – User interface  

The application is designed as a complete and functional full-stack system.

---

### What the Project Provides

The system includes:

- User registration and login  
- Secure password storage  
- Encrypted messaging between users  
- Digital signature verification  
- Secure database storage  
- Modern and user-friendly interface  

---

### Cryptographic Methods Used

- **Password Security:** Salt + SHA-256 hashing  
- **Confidentiality:** AES-256 (CBC) encryption  
- **Key Exchange:** RSA-2048 encryption of AES key  
- **Integrity:** SHA-256 hash verification  
- **Authentication:** RSA digital signatures  

---

### How the System Works

#### 1) User Registration and Login
- Passwords are never stored in plaintext  
- A random salt is generated for each user  
- Passwords are hashed as:  
  `hash = SHA256(salt || password)`  
- Only the hash and salt are stored  
- An RSA key pair is generated for every user  
- Login is verified using hash comparison  

#### 2) Sending a Message
- The user composes a message through the web interface  
- Message content is encrypted using AES-256  
- The AES key is encrypted with the recipient’s RSA public key  
- A SHA-256 hash of the message is computed  
- The hash is signed with the sender’s private key  
- All values are stored securely in the database  

#### 3) Receiving a Message
- The recipient opens the inbox  
- The AES key is decrypted using RSA private key  
- The message is decrypted using the AES key  
- The digital signature is verified  
- Hash verification confirms integrity  

#### 4) Database Security
- No plaintext messages are stored  
- All sensitive fields are stored as encrypted BLOBs  
- Even if the database is compromised, messages remain unreadable  

---

### Project Scope

This project demonstrates:

- A complete full-stack web application  
- Practical use of cryptography in software  
- Secure software development principles  
- Integration of backend, frontend, and security mechanisms   
