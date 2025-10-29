# 📊 Analytics API

**Base Path:** `/api/analytics`  
**Controller:** `AnalyticsController`

Provides detailed analytics and metrics for link tracking.

**Authentication:** Required for all endpoints  
**Headers:** `Authorization: Bearer {token}`

---

## 📋 Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/link/{linkId}` | Get link analytics | ✅ |
| GET | `/dashboard` | Get user dashboard | ✅ |
| GET | `/summary` | Get quick stats summary | ✅ |

---

## Get Link Analytics

### `GET /api/analytics/link/{linkId}`

Retrieves comprehensive analytics for a specific link.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| linkId | GUID | Yes | The ID of the link |

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| days | int | 30 | Number of days to analyze (1-365) |

**Example:** `GET /api/analytics/link/550e8400-e29b-41d4-a716-446655440000?days=7`

**Response (200 OK):**

```json
{
  "linkId": "550e8400-e29b-41d4-a716-446655440000",
  "shortCode": "aBc123",
  "totalClicks": 1250,
  "uniqueVisitors": 890,
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
  "clicksByCountry": [
    {
      "country": "United States",
      "countryCode": "US",
      "clicks": 450,
      "percentage": 36.0
    },
    {
      "country": "Mexico",
      "countryCode": "MX",
      "clicks": 200,
      "percentage": 16.0
    }
  ],
  "clicksByCity": [
    {
      "city": "New York",
      "country": "United States",
      "clicks": 180
    },
    {
      "city": "Los Angeles",
      "country": "United States",
      "clicks": 150
    }
  ],
  "clicksByDevice": [
    {
      "deviceType": "Mobile",
      "clicks": 650,
      "percentage": 52.0
    },
    {
      "deviceType": "Desktop",
      "clicks": 500,
      "percentage": 40.0
    },
    {
      "deviceType": "Tablet",
      "clicks": 100,
      "percentage": 8.0
    }
  ],
  "clicksByBrowser": [
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
    }
  ],
  "clicksByOS": [
    {
      "os": "Windows",
      "version": "11",
      "clicks": 450
    },
    {
      "os": "iOS",
      "version": "17.1",
      "clicks": 400
    }
  ],
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
    }
  ],
  "utmCampaigns": [
    {
      "source": "twitter",
      "medium": "social",
      "campaign": "launch2025",
      "clicks": 200
    },
    {
      "source": "newsletter",
      "medium": "email",
      "campaign": "weekly",
      "clicks": 150
    }
  ],
  "peakHours": [
    {
      "hour": 14,
      "clicks": 120
    },
    {
      "hour": 15,
      "clicks": 110
    }
  ]
}
```

**Error (404 Not Found):**

```json
{
  "message": "Link not found",
  "statusCode": 404
}
```

---

## Get User Dashboard

### `GET /api/analytics/dashboard`

Returns aggregated analytics across all user's links.

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| days | int | 30 | Number of days to analyze (1-365) |

**Example:** `GET /api/analytics/dashboard?days=7`

**Response (200 OK):**

```json
{
  "totalLinks": 45,
  "totalClicks": 5230,
  "uniqueVisitors": 3420,
  "clicksLast24Hours": 180,
  "clicksLast7Days": 1250,
  "clicksLast30Days": 5230,
  "averageClicksPerLink": 116.2,
  "topLinks": [
    {
      "linkId": "guid-1",
      "shortCode": "aBc123",
      "originalUrl": "https://example.com/page1",
      "clicks": 1250,
      "percentage": 23.9
    },
    {
      "linkId": "guid-2",
      "shortCode": "XyZ789",
      "originalUrl": "https://example.com/page2",
      "clicks": 890,
      "percentage": 17.0
    }
  ],
  "clickTrend": [
    {
      "date": "2025-10-23",
      "clicks": 150
    },
    {
      "date": "2025-10-24",
      "clicks": 180
    },
    {
      "date": "2025-10-25",
      "clicks": 165
    }
  ],
  "topCountries": [
    {
      "country": "United States",
      "clicks": 1890,
      "percentage": 36.1
    },
    {
      "country": "Mexico",
      "clicks": 780,
      "percentage": 14.9
    }
  ],
  "deviceBreakdown": [
    {
      "deviceType": "Mobile",
      "clicks": 2720,
      "percentage": 52.0
    },
    {
      "deviceType": "Desktop",
      "clicks": 2090,
      "percentage": 40.0
    },
    {
      "deviceType": "Tablet",
      "clicks": 420,
      "percentage": 8.0
    }
  ],
  "browserBreakdown": [
    {
      "browser": "Chrome",
      "clicks": 3347,
      "percentage": 64.0
    },
    {
      "browser": "Safari",
      "clicks": 1255,
      "percentage": 24.0
    }
  ],
  "topReferrers": [
    {
      "referrer": "twitter.com",
      "clicks": 1570
    },
    {
      "referrer": "facebook.com",
      "clicks": 1046
    },
    {
      "referrer": "direct",
      "clicks": 890
    }
  ],
  "recentActivity": [
    {
      "linkId": "guid",
      "shortCode": "aBc123",
      "timestamp": "2025-10-29T02:45:00Z",
      "country": "United States",
      "device": "Mobile"
    }
  ]
}
```

---

## Get Quick Summary

### `GET /api/analytics/summary`

Returns a lightweight summary of key metrics (last 7 days).

**Response (200 OK):**

```json
{
  "totalLinks": 45,
  "totalClicks": 1250,
  "clicksLast24Hours": 180,
  "clicksLast7Days": 1250,
  "topLink": {
    "linkId": "guid",
    "shortCode": "aBc123",
    "originalUrl": "https://example.com/page",
    "clicks": 450
  }
}
```

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request (invalid days parameter) |
| 401 | Unauthorized |
| 404 | Link not found |
| 500 | Server Error |

---

## Notes

### Data Aggregation

- Click data is aggregated in real-time
- Background services compute daily metrics
- Historical data is preserved indefinitely

### Performance

- Dashboard queries are optimized with caching
- Redis cache used for frequently accessed metrics
- Cache TTL: 5 minutes for dashboard data

### Timezone

- All timestamps are in UTC
- Date aggregations use UTC midnight as the boundary
