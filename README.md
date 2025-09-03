# Link Shortener API

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

Simple and efficient RESTful API to shorten URLs and handle automatic redirection.
Built with **ASP.NET Core 8**, **Entity Framework Core**, and a clean, extensible architecture.

## Features

- **Shorten URLs**: Convert long links into short codes
- **Automatic redirection**: Access short codes and get redirected to the original URL
- **Database persistence**: Store links using EF Core + SQL Server
- **Swagger/OpenAPI**: Interactive documentation for testing endpoints
- **Caching**: In-memory caching for faster lookups

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

3. **Restore EF Core tools**

   This project uses `dotnet-ef` as a **local tool** (recommended). Run:

   ```bash
   dotnet tool restore
   ```

   > If you prefer to install it globally:
   >
   > ```bash
   > dotnet tool install --global dotnet-ef
   > ```

4. **Apply migrations & create database**

   Run from the repo root (where the `.sln` file is):

   ```bash
   dotnet ef migrations add InitialCreate --project LinkShortener.Infrastructure --startup-project LinkShortener.Api
   dotnet ef database update --project LinkShortener.Infrastructure --startup-project LinkShortener.Api
   ```

5. **Run the project**

   ```bash
   dotnet run --project LinkShortener.Api
   ```

By default the API will be available at:

- Swagger: `https://localhost:7092/swagger`
- API Base: `https://localhost:7092/api`

## API Endpoints

### 1. Shorten a URL

**POST** `/api/shorten`

Request:

```json
{
  "url": "https://example.com/very/long/link"
}
```

Response:

```json
{
  "shortUrl": "https://localhost:7092/s/abc123",
  "originalUrl": "https://example.com/very/long/link",
  "code": "abc123"
}
```

### 2. Redirect to Original URL

**GET** `/s/{code}`

Example:

```http
GET /s/abc123
```

Response:

- `302 Found` → Redirects to the original URL
- `404 Not Found` → Code not found

### 3. (Preview) Get URL info

**GET** `/api/info/{code}`

Response:

```json
{
  "shortUrl": "https://localhost:7092/s/abc123",
  "originalUrl": "https://example.com/very/long/link",
  "code": "abc123",
  "createdAt": "2024-08-30T10:30:00Z"
}
```

## Configuration

Update `appsettings.json` with your connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkShortenerDb;Trusted_Connection=True;"
  }
}
```

## Testing

Run all tests:

```bash
dotnet test
```

## Roadmap

- [ ] Add custom codes support
- [ ] Add click tracking
- [ ] Add expiration dates
- [ ] Redis cache layer

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file.
