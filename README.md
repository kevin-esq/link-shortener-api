# Link Shortener API 🔗

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-97%20passing-success)]()

**Production-ready URL shortening platform** with advanced analytics, immutable audit trails, QR code generation, and comprehensive metrics aggregation.

## 🚀 Quick Stats

- 📊 **10 Database Tables** - Users, Links, ClickEvents, Metrics, Audit Logs
- 🎯 **6 Controllers** - Auth, URL, Analytics, Metrics, Audit, Admin
- ✅ **97 Tests Passing** - 100% success rate
- 🔐 **Enterprise Security** - RSA JWT, OAuth, RBAC, Session Management
- 📈 **Advanced Analytics** - UTM, Geo, Device, Daily Aggregations
- 📝 **Full Audit Trail** - Immutable compliance-ready logs
- 🔍 **Smart Search** - Full-text search with filters and sorting
- 📱 **QR Code Support** - Generate PNG QR codes on-demand

## ✨ Key Highlights

✅ **Clean Architecture** - CQRS + MediatR + Repository Pattern  
✅ **PostgreSQL + Redis** - Production database with optional caching  
✅ **Real-time Analytics** - Track every click with full context  
✅ **Background Jobs** - Auto-aggregate metrics hourly  
✅ **Admin Panel** - Complete user/session/role management  
✅ **Audit Trail** - WHO did WHAT, WHEN, WHERE, WHY  
✅ **OAuth Support** - Google OAuth 2.0 integration  
✅ **API Documentation** - Full Swagger/OpenAPI specs  

---

## ✨ Features

### 🎯 Core Functionality
- 🔗 **URL Shortening** - Generate unique 7-character codes
- ↗️ **Smart Redirection** - Fast 302 redirects with tracking
- 📊 **Advanced Click Tracking** - Complete UTM, geo, device analytics
- 🗄️ **PostgreSQL Database** - Production-ready with EF Core 9
- 🔍 **Search & Filters** - Full-text search with dynamic sorting
- 📱 **QR Code Generation** - PNG QR codes with custom sizes
- 🗑️ **Link Management** - Delete, list, paginate user links

### 📊 Advanced Analytics & Metrics

**ClickEvents Tracking** (Real-time)
- 🌍 **Geo Intelligence** - Country, city, lat/lon tracking
- 📱 **Device Analytics** - Type, OS, browser + versions
- 🎯 **UTM Campaign Tracking** - Full UTM parameter support
- 🔗 **Referrer Analysis** - Traffic source identification
- ⚡ **Performance Metrics** - Latency tracking (ms)
- 🛡️ **Security Monitoring** - Malware/phishing detection
- 🌐 **Accept-Language** - User language preferences

**Aggregated Metrics** (Daily rollups)
- 📈 **LinkMetrics** - Per-link daily aggregations
- 👤 **UserMetrics** - Per-user daily aggregations
- 🔄 **Auto-Aggregation** - Background service hourly processing
- 📊 **Dashboard APIs** - Ready-to-use analytics endpoints

**Performance**
- ⚡ **Redis Caching** - Optional for high-traffic scenarios
- 🚀 **Indexed Queries** - Optimized for speed
- 📉 **Efficient Rollups** - Reduce storage, increase query speed

### 🔐 Security & Authentication
- 🔐 **RSA-Signed JWT** - Asymmetric encryption (15min expiry)
- 🔄 **Refresh Token Rotation** - Secure 7-day tokens
- 🌐 **Google OAuth 2.0** - Social login integration
- ✉️ **Email Verification** - Required before first login
- 🔑 **Password Recovery** - Secure code-based reset
- 🛡️ **Multi-Provider** - Local + OAuth accounts
- 📝 **Immutable Audit Logs** - Complete action history
- 🔍 **Session Tracking** - Device and location logging

### 👥 User Management & RBAC
- 🎭 **Role-Based Access** - User, Premium, Admin roles
- 🚫 **Account Moderation** - Suspend/ban/unsuspend
- 📝 **User States** - Active, Suspended, Banned, Pending
- 🔍 **Multi-Session Support** - Concurrent device logins
- 🌍 **Device Intelligence** - IP, User Agent, geo tracking
- 🔐 **Remote Session Control** - End sessions from anywhere

### 🛡️ Audit Trail System
- 📝 **Immutable Logs** - Tamper-proof audit records
- 👤 **Actor Tracking** - Who, what, when, where, why
- 🎯 **Action Logging** - user.create, link.delete, etc.
- 🔍 **Advanced Filtering** - By actor, action, date range
- 📊 **Compliance Ready** - GDPR-friendly export
- 🔐 **Admin-Only Access** - Secure audit log viewing

### 🔧 Admin Panel
- 📊 **User Management** - List, suspend, ban, unsuspend
- 🎭 **Role Management** - Assign/revoke Admin/Premium
- 💻 **Session Control** - View and end user sessions
- 🎫 **Token Revocation** - Force user re-authentication
- 📈 **Audit Logs** - Complete action history with filters
- 📊 **System Metrics** - Platform-wide analytics

### API Standards
- ✅ **Standardized Responses** - Consistent `{ success, data, message }` format
- 🚨 **Global Error Handling** - Centralized exception middleware
- 📖 **OpenAPI/Swagger** - Complete interactive documentation
- 🔒 **Authorization Attributes** - `[RequireRole]` for endpoint protection

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) or Supabase account
- (Optional) [Redis](https://redis.io/download) for analytics caching

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
       "DefaultConnection": "Host=localhost;Database=LinkShortenerDb;Username=postgres;Password=yourpassword",
       "Redis": "localhost:6379" // Optional
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

   This creates 10 tables:
   - **Users** - User accounts with roles and OAuth
   - **UserRoles** - Role assignments with audit trail
   - **Links** - Shortened URLs
   - **LinkAccesses** - Basic click tracking
   - **ClickEvents** - Advanced analytics (UTM, geo, device)
   - **LinkMetrics** - Daily aggregated link metrics
   - **UserMetrics** - Daily aggregated user metrics
   - **AuditLogs** - Immutable audit trail
   - **RefreshTokens** - Token management
   - **Sessions** - Active session tracking

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

### 🔐 Authentication Endpoints

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

### 🔗 URL Management Endpoints

#### Shorten URL
**POST** `/api/url/shorten` 🔒 *Requires Authentication*

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

#### Redirect (Public)
**GET** `/s/{code}`

Redirects to original URL (302) and tracks:
- IP address, User Agent, Accept-Language
- UTM parameters (source, medium, campaign, content, term)
- Geo location (country, city, lat/lon)
- Device type, OS, Browser
- Referrer, latency, timestamp

#### Get Link Info
**GET** `/api/url/info/{code}` 🔒

Returns link details (owner only).

#### List My Links (with Search & Filters)
**GET** `/api/url/my-links` 🔒

**Query Params:**
- `search` - Search in code, longUrl, shortUrl
- `orderBy` - Sort by: `createdAt`, `clicks`, `code`, `url`
- `orderDirection` - `asc` or `desc`
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 20)

#### Get Link Statistics
**GET** `/api/url/{code}/stats?days=30` 🔒

Returns:
- Total clicks, unique visitors
- Clicks by day, country, device, browser
- Top referers
- Last accessed timestamp

#### Generate QR Code
**GET** `/api/url/{code}/qr?size=300` 🔒

Downloads PNG QR code image.

#### Delete Link
**DELETE** `/api/url/{code}` 🔒

Deletes link (owner only).

---

### 📊 Analytics & Metrics Endpoints

#### Get Link Analytics
**GET** `/api/analytics/link/{linkId}?days=30` 🔒

Detailed analytics for specific link.

#### Get User Dashboard
**GET** `/api/analytics/dashboard?days=30` 🔒

Aggregate metrics across all user links.

#### Get Quick Summary
**GET** `/api/analytics/summary` 🔒

Lightweight 7-day summary.

#### Get ClickEvents Stats
**GET** `/api/metrics/link/{linkId}/stats?fromDate=2025-10-01&toDate=2025-10-28` 🔒

Raw ClickEvents statistics.

#### Get Recent Clicks
**GET** `/api/metrics/link/{linkId}/recent-clicks?limit=100` 🔒

Last N clicks with full details.

---

### 📝 Audit Trail Endpoints

#### Get Audit Logs
**GET** `/api/audit/logs` 🔐 *Admin Only*

**Query Params:**
- `actorId` - Filter by user ID
- `action` - Filter by action (e.g., "user.create")
- `targetType` - Filter by target type (e.g., "user")
- `fromDate` - Start date
- `toDate` - End date
- `page`, `pageSize` - Pagination

#### Get Available Actions
**GET** `/api/audit/actions` 🔐 *Admin Only*

List all auditable actions.

---

### 🔧 Admin Endpoints 🔐 *Admin Role Required*

All admin endpoints require `Authorization: Bearer {admin_token}`.

#### User Management
- **GET** `/api/admin/users?page=1&pageSize=50` - List all users with search
- **GET** `/api/admin/users/{userId}` - User details + metrics
- **POST** `/api/admin/users/{userId}/suspend` - Suspend user
- **POST** `/api/admin/users/{userId}/ban` - Ban user (closes all sessions)
- **POST** `/api/admin/users/{userId}/unsuspend` - Reactivate user

#### Role Management
- **POST** `/api/admin/users/{userId}/roles/{role}` - Add role
- **DELETE** `/api/admin/users/{userId}/roles/{role}` - Remove role

#### Session Management
- **GET** `/api/admin/users/{userId}/sessions` - View all user sessions
- **DELETE** `/api/admin/users/{userId}/sessions/{sessionId}` - End session
- **DELETE** `/api/admin/users/{userId}/sessions` - End ALL sessions

#### Token Management
- **POST** `/api/admin/users/{userId}/revoke-tokens` - Revoke all tokens

---

## 🏗️ Project Structure

```
LinkShortener.Api/              # 🎯 API layer (Controllers, Middleware)
├── Controllers/
│   ├── AuthController.cs       # 🔐 Authentication (register, login, OAuth)
│   ├── UrlController.cs        # 🔗 URL management (shorten, list, delete, QR, stats)
│   ├── AnalyticsController.cs  # 📊 Advanced analytics dashboard
│   ├── MetricsController.cs    # 📈 ClickEvents metrics
│   ├── AuditController.cs      # 📝 Audit trail (Admin only)
│   └── AdminController.cs      # 🔧 Admin panel
├── Middleware/
│   ├── GlobalExceptionMiddleware.cs   # Error handling
│   └── AuditMiddleware.cs              # Auto-audit HTTP requests
└── Attributes/
    └── RequireRoleAttribute.cs         # RBAC

LinkShortener.Application/      # 🧠 Business logic (CQRS + MediatR)
├── Features/
│   ├── Auth/                   # Auth commands/handlers/DTOs
│   ├── Url/                    # URL commands/queries/handlers
│   ├── Analytics/              # Analytics queries/handlers
│   └── Admin/                  # Admin commands/handlers
├── Abstractions/
│   ├── IUrlRepository.cs       # Repository interfaces
│   ├── IAuditService.cs        # Audit service
│   ├── IClickEventService.cs   # Click tracking
│   ├── IQrCodeService.cs       # QR generation
│   └── ... (15+ interfaces)    # Clean dependencies
└── Common/                     # Shared models

LinkShortener.Domain/           # 🏛️ Domain entities (10 tables)
├── Entities/
│   ├── User.cs                 # Multi-provider auth
│   ├── Link.cs                 # Shortened URLs
│   ├── ClickEvent.cs           # Advanced click tracking
│   ├── LinkMetric.cs           # Daily link aggregates
│   ├── UserMetric.cs           # Daily user aggregates
│   ├── AuditLog.cs             # Immutable audit trail
│   ├── RefreshToken.cs         # JWT token management
│   └── Session.cs              # Multi-device sessions
└── Common/                     # Base entities, value objects

LinkShortener.Infrastructure/   # 🛠️ Data access & external services
├── Repositories/
│   ├── UrlRepository.cs        # Link CRUD + search/filter/sort
│   ├── LinkStatsRepository.cs  # Advanced stats queries
│   └── ... (8 repositories)
├── Services/
│   ├── AuditService.cs         # Audit trail logging
│   ├── ClickEventService.cs    # Click event tracking
│   ├── QrCodeService.cs        # QR code generation
│   ├── MetricsAggregationService.cs  # Background aggregation
│   ├── JwtService.cs           # JWT signing
│   ├── EmailService.cs         # SMTP email
│   └── ... (12+ services)
└── ApplicationDbContext.cs     # EF Core context (10 DbSets)

LinkShortener.Tests/            # ✅ 97 passing tests
├── UnitTests/
│   ├── Controllers/            # 85 tests (Auth, Url, Admin, Audit, Metrics)
│   ├── Features/               # 8 tests (Handlers, Queries)
│   └── Services/               # 4 tests (QR, analytics)
└── ... (integration tests ready)
```

## 🔐 Security & Compliance

**Authentication**
- 🔐 **RSA-256 JWT** - Asymmetric cryptography (15min expiry)
- 🔄 **Token Rotation** - New refresh token on each use
- 🚫 **One-Time Tokens** - Prevents replay attacks
- ✉️ **Email Verification** - Required before login
- 🔑 **Password Hashing** - SHA-256 + salt

**Authorization**
- 🎭 **Role-Based Access Control** - User/Premium/Admin
- 🔒 **Fine-Grained Permissions** - Endpoint-level authorization
- 📝 **Immutable Audit Logs** - WHO did WHAT, WHEN, WHERE, WHY

**Session Management**
- 💻 **Multi-Device Support** - Concurrent sessions
- 🌍 **Geo Tracking** - IP, location, user agent
- 🚫 **Remote Termination** - Admin can end sessions
- ⏰ **Auto-Expiry** - Refresh tokens expire after 7 days

**Compliance**
- 📝 **Audit Trail** - Complete action history
- 📄 **GDPR Ready** - Export and delete user data
- 🔒 **Data Encryption** - At rest and in transit
- 🛡️ **Account Moderation** - Suspend/ban capabilities

## 📊 Example Use Cases

**Marketing Campaigns**
- Track UTM parameters for campaign attribution
- Analyze traffic by source, medium, campaign
- Generate QR codes for print materials
- Monitor click-through rates by device/location

**SaaS Platform**
- Multi-tenant URL shortening
- User dashboards with analytics
- Admin panel for moderation
- Audit trail for compliance

**Analytics Platform**
- Real-time click tracking
- Daily metric aggregation
- Geographic and device intelligence
- Custom dashboards via API

**Enterprise**
- RBAC for team management
- Audit logs for compliance (SOC 2, GDPR)
- Session management across devices
- API for integration

## ✅ Testing

**97 Tests - 100% Passing**

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

**Test Coverage:**
- ✅ **85 Controller Tests** - Auth, Url, Admin, Audit, Metrics
- ✅ **8 Handler Tests** - CQRS command/query handlers
- ✅ **4 Service Tests** - QR generation, analytics

**Test Categories:**
- Unit tests with Moq for all controllers
- Handler tests for MediatR pipeline
- Service tests for business logic
- Integration tests ready (database, API)

**Quality:**
- Zero compilation errors
- Zero runtime errors
- All tests passing
- Clean, maintainable code

## 📊 Analytics Features

**Real-time ClickEvents Tracking**
- 🌍 Geographic: Country, city, coordinates
- 📱 Device: Mobile, Desktop, Tablet, Bot
- 🌐 Browser: Chrome, Safari, Firefox + versions
- 🔗 UTM: Full campaign parameter support
- ⏱️ Performance: Latency tracking
- 🛡️ Security: Malware/phishing detection

**Aggregated Metrics (Daily Rollups)**
- 📈 LinkMetrics - Per-link daily stats
- 👤 UserMetrics - Per-user daily stats
- 🔄 Auto-aggregation via background service
- 📊 Dashboard APIs with filters

**Performance Optimization**
- ⚡ Redis caching (optional)
- 🚀 Indexed database queries
- 📉 Efficient daily rollups

## 🛠️ Tech Stack

**Backend**
- 🎯 **ASP.NET Core 8** - Web framework
- 📊 **Entity Framework Core 9** - ORM
- 🐘 **PostgreSQL** - Primary database
- ⚡ **Redis** - Optional caching

**Architecture & Patterns**
- 🏛️ **Clean Architecture** - Layered design
- 🧠 **CQRS** - Command Query Responsibility Segregation
- 📦 **MediatR** - Mediator pattern for decoupling
- 📚 **Repository Pattern** - Data abstraction
- 🎭 **Domain Events** - Event-driven design

**Authentication & Security**
- 🔐 **JWT (RSA-256)** - Asymmetric token signing
- 🌐 **Google OAuth 2.0** - Social authentication
- 🔑 **SMTP Email** - Verification and recovery

**Analytics & Tracking**
- 📊 **UAParser** - User agent parsing
- 🌍 **GeoIP** - IP geolocation
- 📱 **QRCoder** - QR code generation

**Background Services**
- 🔄 **Hosted Services** - Metric aggregation
- ⏰ **Scheduled Jobs** - Daily rollups

**Testing**
- ✅ **xUnit** - Test framework
- 🎭 **Moq** - Mocking library
- 📊 **97 tests** - 100% passing

**Packages**
- MediatR, FluentValidation
- Npgsql, StackExchange.Redis
- UAParser, QRCoder
- Swashbuckle (Swagger/OpenAPI)

## 🚀 Deployment

### 🐳 Docker Deployment (Recommended)

**Quick Start with Docker Compose:**

```bash
# 1. Configure environment variables
cp .env.example .env
# Edit .env with your credentials

# 2. Run with Docker Compose
docker-compose up -d --build

# 3. Access the application
# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

**Configuration Files:**
- `Dockerfile` - Optimized multi-stage build
- `docker-compose.yml` - Complete stack (API + PostgreSQL + Redis)
- `.env.example` - Environment variables template
- `docker-entrypoint.sh` - Auto-generation of JWT keys

---

### Production Checklist

**Configuration**
- [ ] Update `.env` with production values (never commit `.env`)
- [ ] RSA keys auto-generated in Docker (or use `dotnet run --project KeyGenerator`)
- [ ] Set production database connection string in `.env`
- [ ] Configure Redis for caching (included in docker-compose)
- [ ] Update Google OAuth credentials in `.env`

**Security**
- [ ] Enable HTTPS and HSTS (use reverse proxy: Nginx/Traefik)
- [ ] Configure CORS policies
- [ ] Enable rate limiting
- [ ] Set up WAF (Web Application Firewall)
- [ ] Use Docker Secrets for production (instead of `.env`)
- [ ] Review security headers

**Database**
- [ ] Apply all migrations (`docker-compose exec api dotnet ef database update`)
- [ ] Set up automated backups (PostgreSQL volumes)
- [ ] Configure connection pooling
- [ ] Set up read replicas (optional)

**Monitoring**
- [ ] Configure logging (Serilog, Application Insights)
- [ ] Set up error tracking (Sentry, Raygun)
- [ ] Enable health checks (included in docker-compose)
- [ ] Configure alerts (email, Slack)

**Performance**
- [ ] Enable Redis caching (included in docker-compose)
- [ ] Configure CDN for static assets
- [ ] Set up load balancing
- [ ] Optimize database indexes

**Recommended Deployment Options**
- 🐳 **Docker Compose** - ⭐ Easiest (included, production-ready)
- ☁️ **Azure Container Instances** - Deploy docker-compose.yml directly
- 🚢 **AWS ECS/Fargate** - Container orchestration
- ☸️ **Kubernetes** - Enterprise scale
- 🌐 **Railway/Render** - Quick deployment with Docker support
- 🟢 **Supabase** - PostgreSQL hosting + Docker deployment

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
