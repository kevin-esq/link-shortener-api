# Link Shortener API

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-6.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

A robust and efficient RESTful API for transforming long URLs into shorter, more manageable addresses. Built with ASP.NET Core and Entity Framework, this project provides a solid foundation for URL shortening services with clean architecture and comprehensive documentation.

## ✨ Features

- **URL Shortening**: Transform long URLs into short, memorable codes
- **Automatic Redirection**: Seamless redirect from short URLs to original destinations
- **Database Persistence**: Reliable storage using Entity Framework Core
- **RESTful API**: Clean, intuitive API endpoints
- **Swagger Documentation**: Interactive API documentation and testing
- **Error Handling**: Comprehensive error responses and validation

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB works fine for development)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/kevin-esq/link-shortener-api.git
   cd link-shortener-api
   ```

2. **Navigate to the project directory**

   ```bash
   cd link-shortener
   ```

3. **Restore dependencies**

   ```bash
   dotnet restore
   ```

4. **Set up the database**

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   > **💡 Tip**: If running from the solution root, use:
   >
   > ```bash
   > dotnet ef migrations add InitialCreate --project link-shortener --startup-project link-shortener
   > dotnet ef database update --project link-shortener --startup-project link-shortener
   > ```

5. **Run the application**

   ```bash
   dotnet run
   ```

The API will be available at:

- **Swagger UI**: `https://localhost:7092/swagger`
- **Base URL**: `https://localhost:7092/api`

## 📖 API Documentation

### Shorten a URL

Creates a short URL from a long URL.

**Endpoint:** `POST /api/shorten`

**Request Body:**

```json
{
  "Url": "https://www.example.com/very/long/url/with/many/parameters?param1=value1&param2=value2"
}
```

**Response:**

```json
{
  "shortUrl": "https://localhost:7092/s/abc1234",
  "originalUrl": "https://www.example.com/very/long/url/with/many/parameters?param1=value1&param2=value2",
  "code": "abc1234",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Status Codes:**

- `200 OK` - URL successfully shortened
- `400 Bad Request` - Invalid URL format
- `500 Internal Server Error` - Server error

### Redirect to Original URL

Redirects to the original URL using the short code.

**Endpoint:** `GET /s/{code}`

**Example:**

```http
GET /s/abc1234
```

**Response:**

- `302 Found` - Redirects to original URL
- `404 Not Found` - Short code not found

### Get URL Information

Retrieve information about a shortened URL without redirecting.

**Endpoint:** `GET /api/info/{code}`

**Example:**

```http
GET /api/info/abc1234
```

**Response:**

```json
{
  "shortUrl": "https://localhost:7092/s/abc1234",
  "originalUrl": "https://www.example.com/very/long/url/with/many/parameters?param1=value1&param2=value2",
  "code": "abc1234",
  "createdAt": "2024-01-15T10:30:00Z",
  "clickCount": 42
}
```

## 🔧 Configuration

### Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkShortenerDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### URL Generation

Customize URL generation settings:

```json
{
  "UrlShortener": {
    "BaseUrl": "https://yourdomain.com",
    "CodeLength": 7,
    "AllowCustomCodes": false
  }
}
```

## 🧪 Testing

Run the test suite:

```bash
dotnet test
```

For coverage reports:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🚢 Deployment

### Docker

1. Build the Docker image:

   ```bash
   docker build -t link-shortener-api .
   ```

2. Run the container:

   ```bash
   docker run -p 8080:80 link-shortener-api
   ```

### Cloud Deployment

This API is ready for deployment on:

- **Azure App Service**
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Heroku**

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines and our [Code of Conduct](CODE_OF_CONDUCT.md).

## 📋 Roadmap

- [ ] Rate limiting and abuse prevention
- [ ] Custom short codes
- [ ] Analytics and click tracking
- [ ] Bulk URL shortening
- [ ] URL expiration dates
- [ ] REST API versioning
- [ ] Caching layer (Redis)
- [ ] Admin dashboard

## 🐛 Known Issues

- URL validation could be more comprehensive
- Missing rate limiting (planned for v2.0)
- No URL expiration feature yet

## 📄 License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- Database management with [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- API documentation powered by [Swagger/OpenAPI](https://swagger.io/)

## 📞 Support

If you have any questions or need help:

- Open an [issue](https://github.com/kevin-esq/link-shortener-api/issues)
- Check the [documentation](https://github.com/kevin-esq/link-shortener-api/wiki)
- Contact: [your-email@example.com](mailto:your-email@example.com)

---

⭐ **Star this repository if you found it helpful!**
