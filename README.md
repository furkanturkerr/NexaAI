<div align="center">

# ✨ NexaAI

### Gerçek Zamanlı AI Sohbet ve Sesli Etkileşim Platformu

<br/>

<img src="https://img.shields.io/badge/.NET-ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
<img src="https://img.shields.io/badge/Clean_Architecture-111827?style=for-the-badge" />
<img src="https://img.shields.io/badge/CQRS-MediatR-2563EB?style=for-the-badge" />
<img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" />

<br/>

<img src="https://img.shields.io/badge/OpenAI-AI-412991?style=for-the-badge&logo=openai&logoColor=white" />
<img src="https://img.shields.io/badge/SignalR-Real_Time-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
<img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
<img src="https://img.shields.io/badge/Identity-User_Management-7B2CBF?style=for-the-badge&logo=dotnet&logoColor=white" />

<br/>

<img src="https://img.shields.io/badge/Speech_to_Text-OpenAI-0F766E?style=for-the-badge" />
<img src="https://img.shields.io/badge/Text_to_Speech-OpenAI-B45309?style=for-the-badge" />
<img src="https://img.shields.io/badge/Google-OAuth-4285F4?style=for-the-badge&logo=google&logoColor=white" />

</div>

<br/>

---

## ✨ Proje Hakkında

**NexaAI**, kullanıcıların yapay zekâ ile gerçek zamanlı olarak sohbet edebildiği, konuşma geçmişini saklayabildiği ve sesli etkileşim kurabildiği modern bir AI asistan platformudur.

Proje yalnızca bir AI servisine soru gönderip cevap alan basit bir chatbot olarak geliştirilmedi. Kullanıcı kimlik doğrulamasından konuşma oturumlarının veritabanında saklanmasına, gerçek zamanlı response streaming yapısından Speech-to-Text ve Text-to-Speech entegrasyonlarına kadar birbirine bağlı bir sohbet deneyimi oluşturuldu.

<div align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/nexaAI.gif" alt="NexaAI Demo" width="100%">
</div>

Uygulamada;

- kullanıcı kayıt ve giriş işlemleri,
- Google ile giriş,
- conversation bazlı sohbet yapısı,
- konuşma geçmişinin PostgreSQL üzerinde saklanması,
- OpenAI entegrasyonu,
- SignalR ile gerçek zamanlı cevap akışı,
- mikrofon ile mesaj oluşturma,
- AI yanıtlarını sesli dinleme

tek bir yapı içerisinde birlikte çalışmaktadır.

---

## 🏗️ Mimari

Proje **Clean Architecture** yaklaşımıyla katmanlara ayrılmıştır.

```text
NexaAI
│
├── NexaAI.Domain
├── NexaAI.Application
├── NexaAI.Infrastructure
├── NexaAI.WebApi
└── NexaAI.WebUI
```

### Katmanların Sorumlulukları

**Domain**
- Entity yapıları
- Conversation ve Message modelleri
- Domain enum'ları

**Application**
- CQRS
- Commands / Queries / Handlers
- MediatR
- Repository interface'leri
- AI servis abstraction'ları
- STT / TTS servis interface'leri
- Realtime servis abstraction'ı
- Authentication servis interface'leri

**Infrastructure**
- Entity Framework Core
- PostgreSQL
- Repository implementasyonları
- ASP.NET Core Identity
- JWT üretimi
- OpenAI entegrasyonu
- Speech-to-Text servisi
- Text-to-Speech servisi
- Google Authentication işlemleri

**Web API**
- RESTful endpointler
- JWT Bearer Authentication
- Authorization
- SignalR Hub
- Realtime SignalR servisi
- MediatR request yönlendirmeleri

**WebUI**
- ASP.NET Core MVC
- Razor Views
- Cookie Authentication
- HttpClient ile API tüketimi
- SignalR JavaScript Client
- MediaRecorder
- Speech ve TTS kullanıcı arayüzü

Application katmanı dış teknolojilerin implementasyonlarına doğrudan bağımlı değildir. Dış servisler ve framework bağımlılıkları interface'ler üzerinden soyutlanmıştır.

---

## ⚡ Gerçek Zamanlı AI Sohbet

Kullanıcı bir mesaj gönderdiğinde AI cevabının tamamlanması beklenmeden cevap parçalar halinde alınır.

```text
Kullanıcı
   ↓
WebUI
   ↓
Web API
   ↓
CreateMessageCommand
   ↓
MediatR
   ↓
CreateMessageCommandHandler
   ↓
OpenAI Streaming
   ↓
IRealtimeService
   ↓
SignalR
   ↓
Browser
```

Kullanıcı mesajı arayüzde anında gösterilir.

Streaming sırasında gelen parçalar tek tek veritabanına kaydedilmez. AI cevabı tamamlandığında bütün response tek bir **Assistant Message** olarak saklanır.

---

## 🧠 Conversation Memory

NexaAI'de sohbet hafızasının kaynağı uygulamanın kendi veritabanıdır.

Her konuşma ayrı bir `Conversation` kaydıdır ve ilgili mesajlar `ConversationId` üzerinden bu konuşmaya bağlanır.

```text
Conversation
   │
   ├── User Message
   ├── Assistant Message
   ├── User Message
   └── Assistant Message
```

Yeni bir mesaj geldiğinde ilgili conversation içerisindeki geçmiş mesajlar tarih sırasına göre alınır ve yeni kullanıcı mesajıyla birlikte AI servisine gönderilir.

```text
Yeni Mesaj
    ↓
ConversationId
    ↓
Geçmiş Mesajları Getir
    ↓
User + Assistant Mesajları
    ↓
OpenAI
    ↓
Yeni AI Cevabı
```

Bu yapı sayesinde kullanıcı önceki konuşmaya referans veren sorular sorabilir ve AI conversation bağlamını koruyarak cevap verebilir.

---

## 💬 Conversation ve Message Yapısı

Her sohbet ayrı bir `Conversation` kaydıdır.

```text
Conversation
│
├── Id
├── UserId
├── Title
├── CreatedAt
├── UpdatedAt
├── IsDeleted
└── Messages
```

Her kullanıcı ve AI mesajı ise ayrı bir `Message` kaydı olarak tutulur.

```text
Message
│
├── Id
├── ConversationId
├── Content
├── Role
└── CreatedAt
```

Mesajın tarafı `MessageRole` ile belirlenir.

```text
User      → 1
Assistant → 2
```

`ConversationId`, aynı sohbet içerisindeki soru ve cevapların ortak oturum kimliği olarak kullanılır.

---

## 🎙️ Speech-to-Text

Kullanıcılar mesajlarını klavyeden yazmak yerine mikrofon kullanarak da oluşturabilir.

Ses kaydı browser tarafında **MediaRecorder API** ile alınır.

```text
Kullanıcı
   ↓
Mikrofon
   ↓
MediaRecorder
   ↓
Audio Blob
   ↓
WebUI
   ↓
Web API
   ↓
ISTTService
   ↓
OpenAI Speech-to-Text
   ↓
Metin
   ↓
Mesaj Alanı
```

Kullanıcı mikrofon butonuna bastığında kayıt başlar. Tekrar bastığında kayıt durdurulur ve ses backend'e gönderilir.

OpenAI tarafından oluşturulan transkripsiyon sonucu mesaj alanına aktarılır. Mesaj kullanıcı onayı olmadan otomatik olarak gönderilmez.

---

## 🔊 Text-to-Speech

AI mesajlarının altında sesli dinleme butonu bulunur.

Kullanıcı bu butona bastığında mesajın metin içeriği backend üzerinden OpenAI TTS servisine gönderilir.

```text
AI Mesajı
    ↓
Sesli Dinle
    ↓
WebUI
    ↓
Web API
    ↓
ITTSService
    ↓
OpenAI Text-to-Speech
    ↓
MP3
    ↓
Browser Audio
```

Oluşturulan ses browser üzerinde oynatılır.

Kullanıcı sesi başlatabilir, durdurabilir ve farklı bir AI mesajını dinlemeye geçebilir.

---

## 🔐 Authentication & Authorization

NexaAI'de WebUI ve Web API tarafında birbirine bağlı iki authentication sistemi kullanılmaktadır.

```text
Kullanıcı
   ↓
WebUI Login
   ↓
Web API
   ↓
ASP.NET Core Identity
   ↓
JWT Access Token
   ↓
WebUI Cookie Authentication
```

Kullanıcı giriş yaptığında API tarafından oluşturulan JWT, WebUI tarafındaki authenticated principal içerisinde `access_token` claim'i olarak saklanır.

Korunan API isteklerinde token:

```text
Authorization: Bearer <token>
```

şeklinde Web API'ye gönderilir.

API tarafında kullanıcı kimliği JWT içerisindeki claim'lerden okunur.

```text
JWT
 ↓
JwtBearer
 ↓
HttpContext.User
 ↓
Claims
 ↓
NameIdentifier
```

Conversation ve Message işlemlerinde istemciden gönderilen kullanıcı kimliğine güvenilmez; authenticated kullanıcının kimliği token üzerinden belirlenir.

---

## 🔵 Google Authentication

Standart e-posta ve parola ile girişin yanında Google hesabı ile authentication desteği de bulunmaktadır.

```text
Google
   ↓
Google OAuth
   ↓
WebUI
   ↓
Web API
   ↓
IGoogleAuthService
   ↓
Identity
   ↓
JWT
```

Google authentication işlemlerinde Application katmanı framework implementasyonuna doğrudan bağımlı değildir.

---

## 🗑️ Soft Delete

Conversation silme işleminde sohbet geçmişi fiziksel olarak veritabanından kaldırılmaz.

Conversation kaydı:

```text
IsDeleted = true
```

olarak işaretlenir.

Normal conversation sorgularında silinmiş kayıtlar filtrelenir ve geçmiş mesajlar veritabanında korunmaya devam eder.

---

## 📝 Markdown Yanıt Desteği

AI yanıtları Markdown formatında üretilebilir.

WebUI tarafında **Markdig** kullanılarak Markdown içeriği HTML'e dönüştürülür.

Bu sayede;

- başlıklar,
- listeler,
- kalın ve eğik metinler,
- inline code,
- kod blokları,
- tablolar,
- blockquote alanları

daha okunabilir şekilde gösterilir.

Raw HTML işleme kapalı tutularak AI çıktısının doğrudan HTML çalıştırmasının önüne geçilir.

---

## 🧩 Kullanılan Yaklaşımlar

- Clean Architecture
- CQRS
- MediatR
- Repository Pattern
- Dependency Injection
- Interface Abstraction
- ASP.NET Core Identity
- JWT Authentication
- Cookie Authentication
- Claims-Based Authentication
- SignalR
- Async / Await
- RESTful API
- Global Exception Middleware
- Soft Delete
- Conversation-Based Logging
- Streaming Response
- Speech-to-Text
- Text-to-Speech

---

## 🛠️ Kullanılan Teknolojiler

### Backend

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Bearer Authentication
- Cookie Authentication
- MediatR
- SignalR
- HttpClient

### AI & Voice

- OpenAI API
- OpenAI Chat
- OpenAI Speech-to-Text
- OpenAI Text-to-Speech
- MediaRecorder API

### Frontend

- Razor Views
- ViewComponents
- HTML5
- CSS3
- JavaScript
- Bootstrap
- Bootstrap Icons
- Markdig
- Responsive Design
- Dark / Light Theme

### Development

- JetBrains Rider
- Swagger / OpenAPI
- DBeaver
- PostgreSQL
- Docker
- Git
- GitHub

---

## 🔄 Genel Uygulama Akışı

```text
                    Kullanıcı
                       │
                       ↓
                 ASP.NET MVC
                       │
             ┌─────────┴─────────┐
             │                   │
             ↓                   ↓
        HTTP / REST           SignalR
             │                   ↑
             ↓                   │
           Web API ──────────────┘
             │
             ↓
           MediatR
             │
             ↓
          Handler
        ┌────┼──────────────┐
        │    │              │
        ↓    ↓              ↓
 PostgreSQL OpenAI     Realtime Service
               │
        ┌──────┼──────┐
        ↓      ↓      ↓
       Chat    STT    TTS
```

---

## 📸 Proje Görselleri
<p align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/login.png" />
</p>
<p align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/register.png" />
</p>
<p align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/nexa-1.png" />
</p>
<p align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/nexa-2.png" />
</p>
<p align="center">
  <img src="https://github.com/furkanturkerr/NexaAI/blob/main/NexaAI.WebUI/wwwroot/images/nexa-3.png" />
</p>

---

<div align="center">

### ✨ NexaAI

**Real-Time AI Assistant & Voice Interface**

</div>
