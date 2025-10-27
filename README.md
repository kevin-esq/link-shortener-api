# Link Shortener API 🔗

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE)

**Enterprise-grade URL shortening service** with advanced authentication, role-based access control, and comprehensive admin capabilities. Built as a production-ready SaaS platform with **ASP.NET Core 8**, **Entity Framework Core**, **Google OAuth**, and **JWT authentication**.

## ✨ Features

### Core Functionality
- 🔗 **URL Shortening** - Generate unique 7-character codes for any URL
- ↗️ **Automatic Redirection** - Fast 302 redirects to original URLs
- 📊 **Click Tracking** - Monitor access patterns with IP and User Agent
- 🗄️ **Database Persistence** - SQL Server with EF Core

### Authentication & Security
- 🔐 **JWT Authentication** - RSA-signed tokens with 15-minute expiry
- 🔄 **Refresh Tokens** - Secure 7-day tokens with rotation
- 🌐 **Google OAuth 2.0** - Seamless social login integration
- ✉️ **Email Verification** - Required before first login
- 🔑 **Password Reset** - Secure code-based recovery flow
- 🛡️ **Multi-Provider Support** - Local and OAuth accounts

### User Management & Roles
- 👥 **Role-Based Access Control** - User, Premium, Admin roles
- 🚫 **Account Moderation** - Suspend/ban capabilities
- 📝 **User States** - Active, Suspended, Banned, PendingVerification
- 🔍 **Session Tracking** - Monitor active sessions per user
- 🌍 **Device Tracking** - IP, User Agent, location logging

### Admin Panel
- 📊 **User Management** - View, suspend, ban users
- 🔧 **Role Management** - Dynamically assign/revoke roles
- 💻 **Session Control** - End sessions remotely
- 🎫 **Token Revocation** - Force re-login by revoking refresh tokens
- 📈 **Metrics & Analytics** - User stats, login history, link counts

### API Standards
- ✅ **Standardized Responses** - Consistent `{ success, data, message }` format
- 🚨 **Global Error Handling** - Centralized exception middleware
- 📖 **OpenAPI/Swagger** - Complete interactive documentation
- 🔒 **Authorization Attributes** - `[RequireRole]` for endpoint protection

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB works fine for development)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/kevin-esq/link-shortener-api.git
   cd link-shortener-api
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Update configuration**

   Edit `LinkShortener.Api/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LinkShortenerDb;Trusted_Connection=True;..."
     },
     "Jwt": {
       "Issuer": "LinkShortener",
       "Audience": "LinkShortenerClient",
       "KeysFolder": "../LinkShortener.Application/Abstractions/Security/Keys"
     },
     "Email": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromAddress": "your-email@gmail.com"
     },
     "Google": {
       "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
     }
   }
   ```

4. **Generate RSA Keys** (for JWT signing)

   ```bash
   dotnet run --project KeyGenerator
   ```

   Keys will be generated in `LinkShortener.Application/Abstractions/Security/Keys/`

5. **Apply database migrations**

   ```bash
   dotnet ef database update --project LinkShortener.Infrastructure --startup-project LinkShortener.Api
   ```

   This will create:
   - `Users` table with roles, status, and OAuth support
   - `Links` table for shortened URLs
   - `LinkAccesses` table for click tracking
   - `RefreshTokens` table for token management
   - `Sessions` table for session tracking

6. **Run the API**

   ```bash
   dotnet run --project LinkShortener.Api
   ```

   🎉 API available at:
   - **Swagger UI**: `https://localhost:7092/swagger`
   - **API Base**: `https://localhost:7092/api`

### Optional: Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable **Google+ API**
4. Create **OAuth 2.0 Client ID** (Web application)
5. Add authorized origins: `http://localhost:3000`
6. Copy the Client ID to `appsettings.json`

See [`QUICK_START_GOOGLE_OAUTH.md`](QUICK_START_GOOGLE_OAUTH.md) for detailed instructions.

## 📡 API Endpoints

### Authentication Endpoints

#### Register
**POST** `/api/auth/register`

```json
{
  "username": "john123",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

Response includes verification email. Email must be verified before login.

#### Login
**POST** `/api/auth/login`

```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

Returns `accessToken` (15 min) and `refreshToken` (7 days).

#### Google OAuth
**POST** `/api/auth/google`

```json
{
  "idToken": "google_id_token_from_frontend"
}
```

Automatically creates account if email doesn't exist.

#### Refresh Token
**POST** `/api/auth/refresh`

```json
{
  "refreshToken": "your_refresh_token"
}
```

Returns new `accessToken` and `refreshToken`.

#### Verify Email
**POST** `/api/auth/verify-email/confirm`

```json
{
  "email": "john@example.com",
  "code": "123456"
}
```

---

### URL Shortening Endpoints

#### Shorten URL
**POST** `/api/shorten` 🔒 *Requires Authentication*

```json
{
  "url": "https://example.com/very/long/url"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "shortUrl": "https://localhost:7092/s/ABC1234",
    "originalUrl": "https://example.com/very/long/url",
    "code": "ABC1234"
  }
}
```

#### Redirect
**GET** `/s/{code}`

Redirects to original URL (302) or returns 404.

#### Get URL Info
**GET** `/api/info/{code}`

Returns URL details without authentication.

---

### Admin Endpoints 🔒 *Admin Role Required*

All admin endpoints require `Authorization: Bearer {admin_token}`.

#### User Management
- **GET** `/api/admin/users?page=1&pageSize=50` - List all users
- **GET** `/api/admin/users/{userId}` - Get user details + metrics
- **POST** `/api/admin/users/{userId}/suspend` - Suspend user
- **POST** `/api/admin/users/{userId}/ban` - Ban user (closes all sessions)
- **POST** `/api/admin/users/{userId}/unsuspend` - Reactivate user

#### Role Management
- **POST** `/api/admin/users/{userId}/roles/{role}` - Add role (Admin, Premium, User)
- **DELETE** `/api/admin/users/{userId}/roles/{role}` - Remove role

#### Session Management
- **GET** `/api/admin/users/{userId}/sessions` - View all user sessions
- **DELETE** `/api/admin/users/{userId}/sessions/{sessionId}` - End specific session
- **DELETE** `/api/admin/users/{userId}/sessions` - End ALL user sessions

#### Token Management
- **POST** `/api/admin/users/{userId}/revoke-tokens` - Revoke all refresh tokens

---

## 🏗️ Project Structure

```
LinkShortener.Api/              # API layer (Controllers, Middleware)
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   ├── UrlController.cs        # URL shortening endpoints
│   └── AdminController.cs      # Admin management endpoints
├── Middleware/
│   └── GlobalExceptionMiddleware.cs
└── Attributes/
    └── RequireRoleAttribute.cs # Role-based authorization

LinkShortener.Application/      # Business logic (CQRS with MediatR)
├── Features/
│   ├── Auth/                   # Auth commands/handlers/DTOs
│   ├── Url/                    # URL commands/handlers
│   └── Admin/                  # Admin commands/handlers
├── Common/
│   └── Models/                 # ApiResponse, ErrorDetails
└── Abstractions/               # Interfaces

LinkShortener.Domain/           # Domain entities and events
├── Entities/
│   ├── User.cs                 # Multi-provider, roles, status
│   ├── Link.cs                 # Shortened links
│   ├── RefreshToken.cs         # Token management
│   └── Session.cs              # Session tracking
└── Events/

LinkShortener.Infrastructure/   # Data access and external services
├── Repositories/               # EF Core repositories
├── Services/                   # JWT, Email, Google OAuth
└── ApplicationDbContext.cs
```

## 🔐 Security Features

- **RSA-Signed JWT** - Asymmetric key cryptography
- **Token Rotation** - New refresh token on each use
- **One-Time Tokens** - Prevents replay attacks
- **Session Tracking** - Monitor device and location
- **Account States** - Suspend/ban capabilities
- **Role-Based Access** - Fine-grained permissions
- **Email Verification** - Required before login
- **Password Hashing** - SHA-256 (consider upgrading to bcrypt)

## 🧪 Testing

Run all tests:

```bash
dotnet test
```

## 📖 Documentation

- [`QUICK_START_GOOGLE_OAUTH.md`](QUICK_START_GOOGLE_OAUTH.md) - Google OAuth setup
- [`API_RESPONSE_MODELS.md`](API_RESPONSE_MODELS.md) - Response format guide
- **Swagger UI** - Interactive API docs at `/swagger`

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8
- **ORM:** Entity Framework Core 9
- **Database:** SQL Server
- **Authentication:** JWT + Google OAuth 2.0
- **Architecture:** Clean Architecture + CQRS
- **Patterns:** Repository, Mediator (MediatR), Domain Events

## 🚀 Deployment

### Production Checklist

- [ ] Update `appsettings.Production.json` with production config
- [ ] Set strong `Jwt:SecretKey` and rotate RSA keys
- [ ] Configure HTTPS and proper CORS policies
- [ ] Enable rate limiting on endpoints
- [ ] Set up database backups
- [ ] Configure logging (Serilog, Application Insights)
- [ ] Update Google OAuth redirect URLs
- [ ] Review and harden security headers

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the **GNU General Public License v3.0** - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Kevin** - [@kevin-esq](https://github.com/kevin-esq)

---

⭐ **Star this repo** if you find it useful!
