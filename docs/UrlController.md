# 🔗 URL Shortening API

**Base Path:** `/api/url`  
**Controller:** `UrlController`

Manages URL shortening, redirection, link statistics, and QR code generation.

---

## 📋 Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/shorten` | Create short URL | ✅ |
| GET | `/s/{code}` | Redirect to original URL | ❌ |
| GET | `/info/{code}` | Get link details | ✅ |
| GET | `/my-links` | Get user's links | ✅ |
| DELETE | `/{code}` | Delete link | ✅ |
| GET | `/{code}/stats` | Get link statistics | ✅ |
| GET | `/{code}/qr` | Generate QR code | ✅ |

---

## Shorten URL

### `POST /api/url/shorten`

Creates a shortened URL for the authenticated user.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Request Body:**

```json
{
  "url": "https://www.example.com/very/long/link"
}
```

**Response (200 OK):**

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "shortCode": "aBc123",
  "originalUrl": "https://www.example.com/very/long/link",
  "shortUrl": "http://localhost:8080/s/aBc123",
  "createdAt": "2025-10-29T03:00:00Z"
}
```

**Error (400 Bad Request):**

```json
{
  "message": "URL is required.",
  "statusCode": 400
}
```

**Error (401 Unauthorized):**

```json
{
  "message": "Invalid user token.",
  "statusCode": 401
}
```

---

## Redirect to Original URL

### `GET /s/{code}`

Redirects to the original URL using the short code. This is a **public endpoint**.

**Authentication:** None (Public)

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| code | string | Yes | The 7-character short code |

**Example:** `GET /s/aBc123`

**Response (302 Found):**

Redirects to the original URL.

**Response (404 Not Found):**

```json
{
  "message": "Link not found",
  "statusCode": 404
}
```

**Analytics Tracking:**

Each access is recorded with:
- IP address
- User agent (browser, OS, device)
- Referrer URL
- UTM parameters (utm_source, utm_medium, utm_campaign, utm_content, utm_term)
- Geographic location (country, city, coordinates)
- Timestamp
- Latency

**Example with UTM parameters:**
```
GET /s/aBc123?utm_source=twitter&utm_medium=social&utm_campaign=launch
```

---

## Get Link Information

### `GET /api/url/info/{code}`

Retrieves detailed information about a shortened URL. Only accessible to the link owner.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| code | string | Yes | The short code |

**Response (200 OK):**

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "shortCode": "aBc123",
  "originalUrl": "https://www.example.com/very/long/link",
  "shortUrl": "http://localhost:8080/s/aBc123",
  "createdAt": "2025-10-29T03:00:00Z",
  "totalClicks": 150,
  "userId": "user-guid"
}
```

**Error (404 Not Found):**

```json
{
  "message": "Link not found or you don't have access",
  "statusCode": 404
}
```

---

## Get My Links

### `GET /api/url/my-links`

Retrieves all links created by the authenticated user with pagination and search.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page (max: 100) |
| search | string | null | Search in original URLs |
| orderBy | string | "createdAt" | Sort field (createdAt, totalClicks) |
| orderDirection | string | "desc" | Sort direction (asc, desc) |

**Example:**
```
GET /api/url/my-links?page=1&pageSize=20&search=example&orderBy=totalClicks&orderDirection=desc
```

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "shortCode": "aBc123",
      "originalUrl": "https://www.example.com/page",
      "shortUrl": "http://localhost:8080/s/aBc123",
      "createdAt": "2025-10-29T03:00:00Z",
      "totalClicks": 150
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

---

## Delete Link

### `DELETE /api/url/{code}`

Deletes a shortened URL. Only the link owner can delete it.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| code | string | Yes | The short code to delete |

**Response (204 No Content):**

No body. Link successfully deleted.

**Error (404 Not Found):**

```json
{
  "message": "Link not found or you don't have permission to delete it",
  "statusCode": 404
}
```

---

## Get Link Statistics

### `GET /api/url/{code}/stats`

Retrieves detailed statistics for a link.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| code | string | Yes | The short code |

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| days | int | 30 | Number of days to analyze |

**Example:** `GET /api/url/aBc123/stats?days=7`

**Response (200 OK):**

```json
{
  "linkId": "550e8400-e29b-41d4-a716-446655440000",
  "shortCode": "aBc123",
  "originalUrl": "https://www.example.com/page",
  "totalClicks": 1250,
  "clicksByDate": [
    {
      "date": "2025-10-28",
      "clicks": 45
    },
    {
      "date": "2025-10-29",
      "clicks": 67
    }
  ],
  "clicksByCountry": [
    {
      "country": "United States",
      "clicks": 450
    },
    {
      "country": "Mexico",
      "clicks": 200
    }
  ],
  "clicksByDevice": [
    {
      "device": "Mobile",
      "clicks": 650
    },
    {
      "device": "Desktop",
      "clicks": 500
    }
  ],
  "clicksByBrowser": [
    {
      "browser": "Chrome",
      "clicks": 800
    },
    {
      "browser": "Safari",
      "clicks": 300
    }
  ],
  "topReferrers": [
    {
      "referrer": "twitter.com",
      "clicks": 450
    },
    {
      "referrer": "facebook.com",
      "clicks": 300
    }
  ]
}
```

---

## Generate QR Code

### `GET /api/url/{code}/qr`

Generates a QR code image for the shortened URL.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| code | string | Yes | The short code |

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| size | int | 300 | QR code size in pixels (100-1000) |

**Example:** `GET /api/url/aBc123/qr?size=500`

**Response (200 OK):**

Returns a PNG image file.

**Content-Type:** `image/png`  
**Content-Disposition:** `attachment; filename="{code}_qr.png"`

**Error (404 Not Found):**

```json
{
  "message": "Link not found",
  "statusCode": 404
}
```

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 204 | No Content (delete successful) |
| 302 | Found (redirect) |
| 400 | Bad Request |
| 401 | Unauthorized |
| 404 | Not Found |
| 500 | Server Error |

---

## Notes

### Short Code Format

- 7 characters long
- Alphanumeric (a-z, A-Z, 0-9)
- Case-sensitive
- Example: `aBc123X`

### URL Validation

- Must be a valid HTTP or HTTPS URL
- Maximum length: 2048 characters
- Automatically validates URL format

### Analytics

- Click events are tracked in real-time
- Geographic data based on IP address
- Device detection from User-Agent header
- UTM parameters preserved from query string
