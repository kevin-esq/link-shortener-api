# 📊 Metrics API

**Base Path:** `/api/metrics`  
**Controller:** `MetricsController`

Provides detailed click event statistics and real-time metrics for links.

**Authentication:** Required  
**Headers:** `Authorization: Bearer {token}`

---

## 📋 Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/link/{linkId}/stats` | Get detailed link statistics |
| GET | `/link/{linkId}/recent-clicks` | Get recent click events |

---

## Get Link Statistics

### `GET /api/metrics/link/{linkId}/stats`

Retrieves comprehensive statistics for a specific link within a date range.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| linkId | GUID | Yes | The link's unique ID |

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| fromDate | DateTime | No | Start date (UTC). Default: 30 days ago |
| toDate | DateTime | No | End date (UTC). Default: now |

**Example:**
```
GET /api/metrics/link/550e8400-e29b-41d4-a716-446655440000/stats?fromDate=2025-10-01T00:00:00Z&toDate=2025-10-29T23:59:59Z
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "linkId": "550e8400-e29b-41d4-a716-446655440000",
    "shortCode": "aBc123",
    "originalUrl": "https://example.com/page",
    "dateRange": {
      "from": "2025-10-01T00:00:00Z",
      "to": "2025-10-29T23:59:59Z"
    },
    "totalClicks": 1250,
    "uniqueVisitors": 890,
    "averageLatencyMs": 145,
    "clicksByDate": [
      {
        "date": "2025-10-28",
        "clicks": 45,
        "uniqueVisitors": 35
      },
      {
        "date": "2025-10-29",
        "clicks": 67,
        "uniqueVisitors": 52
      }
    ],
    "geographicDistribution": {
      "countries": [
        {
          "country": "United States",
          "countryCode": "US",
          "clicks": 450,
          "percentage": 36.0,
          "cities": [
            {
              "city": "New York",
              "clicks": 180,
              "latitude": 40.7128,
              "longitude": -74.0060
            },
            {
              "city": "Los Angeles",
              "clicks": 150,
              "latitude": 34.0522,
              "longitude": -118.2437
            }
          ]
        },
        {
          "country": "Mexico",
          "countryCode": "MX",
          "clicks": 200,
          "percentage": 16.0,
          "cities": [
            {
              "city": "Mexico City",
              "clicks": 120
            }
          ]
        }
      ]
    },
    "deviceAnalytics": {
      "deviceTypes": [
        {
          "type": "Mobile",
          "clicks": 650,
          "percentage": 52.0
        },
        {
          "type": "Desktop",
          "clicks": 500,
          "percentage": 40.0
        },
        {
          "type": "Tablet",
          "clicks": 100,
          "percentage": 8.0
        }
      ],
      "operatingSystems": [
        {
          "os": "Windows",
          "version": "11",
          "clicks": 450,
          "percentage": 36.0
        },
        {
          "os": "iOS",
          "version": "17.1",
          "clicks": 400,
          "percentage": 32.0
        },
        {
          "os": "Android",
          "version": "14",
          "clicks": 250,
          "percentage": 20.0
        }
      ],
      "browsers": [
        {
          "browser": "Chrome",
          "version": "118.0",
          "clicks": 800,
          "percentage": 64.0
        },
        {
          "browser": "Safari",
          "version": "17.0",
          "clicks": 300,
          "percentage": 24.0
        },
        {
          "browser": "Firefox",
          "version": "119.0",
          "clicks": 150,
          "percentage": 12.0
        }
      ]
    },
    "referrerAnalytics": {
      "topReferrers": [
        {
          "referrer": "twitter.com",
          "clicks": 450,
          "percentage": 36.0
        },
        {
          "referrer": "facebook.com",
          "clicks": 300,
          "percentage": 24.0
        },
        {
          "referrer": "direct",
          "clicks": 250,
          "percentage": 20.0
        },
        {
          "referrer": "linkedin.com",
          "clicks": 150,
          "percentage": 12.0
        }
      ]
    },
    "utmTracking": {
      "campaigns": [
        {
          "source": "twitter",
          "medium": "social",
          "campaign": "launch2025",
          "content": "post1",
          "term": "software",
          "clicks": 200
        },
        {
          "source": "newsletter",
          "medium": "email",
          "campaign": "weekly",
          "content": null,
          "term": null,
          "clicks": 150
        }
      ]
    },
    "temporalAnalytics": {
      "peakHours": [
        {
          "hour": 14,
          "hourLabel": "2:00 PM",
          "clicks": 120
        },
        {
          "hour": 15,
          "hourLabel": "3:00 PM",
          "clicks": 110
        },
        {
          "hour": 10,
          "hourLabel": "10:00 AM",
          "clicks": 95
        }
      ],
      "peakDays": [
        {
          "dayOfWeek": "Tuesday",
          "clicks": 220
        },
        {
          "dayOfWeek": "Wednesday",
          "clicks": 210
        }
      ]
    },
    "performanceMetrics": {
      "averageLatencyMs": 145,
      "medianLatencyMs": 130,
      "p95LatencyMs": 250,
      "p99LatencyMs": 450
    },
    "securityMetrics": {
      "phishingDetections": 0,
      "malwareDetections": 0,
      "blockedClicks": 0
    }
  }
}
```

---

## Get Recent Clicks

### `GET /api/metrics/link/{linkId}/recent-clicks`

Retrieves the most recent click events for a link.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| linkId | GUID | Yes | The link's unique ID |

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| limit | int | 100 | Number of recent clicks (max: 500) |

**Example:** `GET /api/metrics/link/550e8400-e29b-41d4-a716-446655440000/recent-clicks?limit=50`

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "id": "guid-1",
      "linkId": "550e8400-e29b-41d4-a716-446655440000",
      "shortCode": "aBc123",
      "timestamp": "2025-10-29T02:45:30Z",
      "ipAddress": "192.168.1.100",
      "country": "United States",
      "city": "New York",
      "latitude": 40.7128,
      "longitude": -74.0060,
      "deviceType": "Mobile",
      "os": "iOS",
      "osVersion": "17.1",
      "browser": "Safari",
      "browserVersion": "17.0",
      "userAgent": "Mozilla/5.0 (iPhone; CPU iPhone OS 17_1 like Mac OS X)...",
      "referrer": "twitter.com",
      "utmSource": "twitter",
      "utmMedium": "social",
      "utmCampaign": "launch2025",
      "utmContent": "post1",
      "utmTerm": null,
      "acceptLanguage": "en-US,en;q=0.9",
      "status": "Redirected",
      "latencyMs": 145,
      "phishingDetected": false,
      "malwareDetected": false
    },
    {
      "id": "guid-2",
      "linkId": "550e8400-e29b-41d4-a716-446655440000",
      "shortCode": "aBc123",
      "timestamp": "2025-10-29T02:40:15Z",
      "ipAddress": "10.0.0.50",
      "country": "Mexico",
      "city": "Mexico City",
      "latitude": 19.4326,
      "longitude": -99.1332,
      "deviceType": "Desktop",
      "os": "Windows",
      "osVersion": "11",
      "browser": "Chrome",
      "browserVersion": "118.0",
      "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)...",
      "referrer": "direct",
      "utmSource": null,
      "utmMedium": null,
      "utmCampaign": null,
      "utmContent": null,
      "utmTerm": null,
      "acceptLanguage": "es-MX,es;q=0.9",
      "status": "Redirected",
      "latencyMs": 132,
      "phishingDetected": false,
      "malwareDetected": false
    }
  ]
}
```

---

## Click Event Fields

### Geographic Data

- **country**: Country name
- **city**: City name
- **latitude**: Geographic latitude
- **longitude**: Geographic longitude

### Device Information

- **deviceType**: Mobile, Desktop, Tablet, Bot
- **os**: Operating system (Windows, iOS, Android, macOS, Linux)
- **osVersion**: OS version
- **browser**: Browser name (Chrome, Safari, Firefox, Edge)
- **browserVersion**: Browser version
- **userAgent**: Full user agent string

### Campaign Tracking (UTM)

- **utmSource**: Traffic source (twitter, google, newsletter)
- **utmMedium**: Marketing medium (social, email, cpc)
- **utmCampaign**: Campaign name
- **utmContent**: Ad content identifier
- **utmTerm**: Paid search keywords

### Performance & Security

- **latencyMs**: Response time in milliseconds
- **phishingDetected**: Security flag
- **malwareDetected**: Security flag
- **status**: Redirected, Blocked, Error

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request (invalid date range or limit) |
| 401 | Unauthorized |
| 404 | Link not found |
| 500 | Server Error |

---

## Notes

### Real-time Data

- Click events are recorded in real-time
- Statistics are computed on-the-fly
- No caching for recent clicks endpoint

### Performance

- Stats endpoint may cache results for 5 minutes
- Large date ranges may take longer to compute
- Recommended to limit queries to 90 days or less

### Privacy

- IP addresses are hashed for privacy
- Geographic data is city-level (not street address)
- Compliant with GDPR and privacy regulations
