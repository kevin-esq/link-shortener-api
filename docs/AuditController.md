# 📝 Audit Logs API

**Base Path:** `/api/audit`  
**Controller:** `AuditController`

Provides immutable audit trail for compliance and security monitoring.

**Authentication:** Required - Admin role only  
**Headers:** `Authorization: Bearer {token}`  
**Required Role:** `Admin`

---

## 📋 Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/logs` | Query audit logs with filters |
| GET | `/actions` | Get list of available audit actions |

---

## Query Audit Logs

### `GET /api/audit/logs`

Retrieves audit logs with advanced filtering and pagination.

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| actorId | GUID | No | Filter by user who performed action |
| action | string | No | Filter by action type |
| targetType | string | No | Filter by target entity type |
| fromDate | DateTime | No | Start date (UTC) |
| toDate | DateTime | No | End date (UTC) |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 50, max: 100) |

**Example:**
```
GET /api/audit/logs?actorId=guid-123&action=UserLogin&fromDate=2025-10-01&toDate=2025-10-29&page=1&pageSize=50
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "id": "guid-1",
      "timestamp": "2025-10-29T02:30:00Z",
      "actorId": "guid-user",
      "actorUsername": "john123",
      "actorEmail": "john@example.com",
      "action": "UserLogin",
      "targetType": "User",
      "targetId": "guid-user",
      "targetIdentifier": "john@example.com",
      "changes": {
        "lastLoginAt": "2025-10-29T02:30:00Z"
      },
      "metadata": {
        "ipAddress": "192.168.1.1",
        "userAgent": "Mozilla/5.0...",
        "requestId": "req-123"
      },
      "result": "Success",
      "reason": null
    },
    {
      "id": "guid-2",
      "timestamp": "2025-10-29T01:15:00Z",
      "actorId": "guid-admin",
      "actorUsername": "admin",
      "actorEmail": "admin@example.com",
      "action": "UserSuspended",
      "targetType": "User",
      "targetId": "guid-target",
      "targetIdentifier": "baduser@example.com",
      "changes": {
        "isSuspended": true,
        "suspensionReason": "Violation of terms"
      },
      "metadata": {
        "ipAddress": "10.0.0.1",
        "userAgent": "Mozilla/5.0...",
        "requestId": "req-456"
      },
      "result": "Success",
      "reason": "Violation of terms"
    },
    {
      "id": "guid-3",
      "timestamp": "2025-10-29T00:45:00Z",
      "actorId": "guid-user2",
      "actorUsername": "jane456",
      "actorEmail": "jane@example.com",
      "action": "LinkCreated",
      "targetType": "Link",
      "targetId": "guid-link",
      "targetIdentifier": "aBc123",
      "changes": {
        "originalUrl": "https://example.com/page",
        "shortCode": "aBc123"
      },
      "metadata": {
        "ipAddress": "192.168.1.5"
      },
      "result": "Success",
      "reason": null
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 50,
    "hasMore": true
  }
}
```

---

## Get Available Actions

### `GET /api/audit/actions`

Retrieves list of all audit action types.

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    { "name": "UserRegistered", "value": "UserRegistered" },
    { "name": "UserLogin", "value": "UserLogin" },
    { "name": "UserLogout", "value": "UserLogout" },
    { "name": "UserEmailVerified", "value": "UserEmailVerified" },
    { "name": "UserPasswordChanged", "value": "UserPasswordChanged" },
    { "name": "UserPasswordReset", "value": "UserPasswordReset" },
    { "name": "UserSuspended", "value": "UserSuspended" },
    { "name": "UserUnsuspended", "value": "UserUnsuspended" },
    { "name": "UserBanned", "value": "UserBanned" },
    { "name": "UserRoleAdded", "value": "UserRoleAdded" },
    { "name": "UserRoleRemoved", "value": "UserRoleRemoved" },
    { "name": "LinkCreated", "value": "LinkCreated" },
    { "name": "LinkDeleted", "value": "LinkDeleted" },
    { "name": "LinkAccessed", "value": "LinkAccessed" },
    { "name": "SessionStarted", "value": "SessionStarted" },
    { "name": "SessionEnded", "value": "SessionEnded" },
    { "name": "TokenRefreshed", "value": "TokenRefreshed" },
    { "name": "TokenRevoked", "value": "TokenRevoked" }
  ]
}
```

---

## Audit Action Types

### User Actions

| Action | Description |
|--------|-------------|
| UserRegistered | User creates new account |
| UserLogin | User logs in |
| UserLogout | User logs out |
| UserEmailVerified | User verifies email |
| UserPasswordChanged | User changes password |
| UserPasswordReset | User resets forgotten password |
| UserSuspended | Admin suspends user |
| UserUnsuspended | Admin reactivates user |
| UserBanned | Admin permanently bans user |
| UserRoleAdded | Admin adds role to user |
| UserRoleRemoved | Admin removes role from user |

### Link Actions

| Action | Description |
|--------|-------------|
| LinkCreated | User creates shortened link |
| LinkDeleted | User deletes link |
| LinkAccessed | Someone accesses shortened link |

### Session & Token Actions

| Action | Description |
|--------|-------------|
| SessionStarted | User starts new session |
| SessionEnded | Session is terminated |
| TokenRefreshed | Access token refreshed |
| TokenRevoked | Refresh token revoked |

---

## Target Types

| Type | Description |
|------|-------------|
| User | User account |
| Link | Shortened link |
| Session | User session |
| RefreshToken | Refresh token |
| Role | User role |

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request (invalid date range) |
| 401 | Unauthorized (not admin) |
| 500 | Server Error |

---

## Notes

### Immutability

- Audit logs are **immutable** and cannot be modified or deleted
- Each log entry is permanently stored
- Provides compliance trail for SOC 2, GDPR, HIPAA

### Data Retention

- Audit logs are retained indefinitely
- No automatic deletion
- Includes full context of every action

### Performance

- Indexed by timestamp, actorId, action, and targetType
- Efficient querying even with millions of records
- Pagination recommended for large result sets

### Privacy & Compliance

- Captures WHO did WHAT, WHEN, WHERE, and WHY
- IP addresses and user agents logged for security
- Satisfies audit requirements for regulated industries
