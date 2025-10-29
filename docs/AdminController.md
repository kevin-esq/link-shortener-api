# 🛠️ Admin Management API

**Base Path:** `/api/admin`  
**Controller:** `AdminController`

Admin-only endpoints for user management, role assignment, and session control.

**Authentication:** Required - Admin role only  
**Headers:** `Authorization: Bearer {token}`  
**Required Role:** `Admin`

---

## 📋 Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users` | Get all users with pagination |
| GET | `/users/{userId}` | Get user details |
| POST | `/users/{userId}/suspend` | Suspend user account |
| POST | `/users/{userId}/ban` | Ban user permanently |
| POST | `/users/{userId}/unsuspend` | Unsuspend user account |
| POST | `/users/{userId}/roles/{role}` | Add role to user |
| DELETE | `/users/{userId}/roles/{role}` | Remove role from user |
| GET | `/users/{userId}/sessions` | Get user sessions |
| DELETE | `/users/{userId}/sessions/{sessionId}` | End specific session |
| DELETE | `/users/{userId}/sessions` | End all user sessions |
| POST | `/users/{userId}/revoke-tokens` | Revoke all refresh tokens |

---

## Get All Users

### `GET /api/admin/users`

Retrieves paginated list of all users.

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 50 | Items per page (max: 100) |

**Example:** `GET /api/admin/users?page=1&pageSize=50`

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "userId": "guid-1",
      "username": "john123",
      "email": "john@example.com",
      "isEmailVerified": true,
      "isSuspended": false,
      "isBanned": false,
      "roles": ["User"],
      "createdAt": "2025-10-15T10:30:00Z",
      "lastLoginAt": "2025-10-29T02:00:00Z"
    }
  ]
}
```

---

## Get User Detail

### `GET /api/admin/users/{userId}`

Retrieves detailed information about a specific user.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "john123",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isEmailVerified": true,
    "isSuspended": false,
    "isBanned": false,
    "suspensionReason": null,
    "banReason": null,
    "roles": ["User"],
    "totalLinks": 25,
    "totalClicks": 1250,
    "activeSessions": 2,
    "createdAt": "2025-10-15T10:30:00Z",
    "lastLoginAt": "2025-10-29T02:00:00Z",
    "googleAuthLinked": false
  }
}
```

**Error (404 Not Found):**

```json
{
  "message": "User not found",
  "statusCode": 404
}
```

---

## Suspend User

### `POST /api/admin/users/{userId}/suspend`

Temporarily suspends a user account.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Request Body:**

```json
{
  "reason": "Violation of terms of service"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "User suspended successfully"
}
```

---

## Ban User

### `POST /api/admin/users/{userId}/ban`

Permanently bans a user account.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Request Body:**

```json
{
  "reason": "Repeated violations"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "User banned successfully"
}
```

---

## Unsuspend User

### `POST /api/admin/users/{userId}/unsuspend`

Reactivates a suspended user account.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Response (200 OK):**

```json
{
  "success": true,
  "message": "User unsuspended successfully"
}
```

---

## Add Role

### `POST /api/admin/users/{userId}/roles/{role}`

Adds a role to a user.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |
| role | string | Yes | Role name (Admin, Moderator, User) |

**Example:** `POST /api/admin/users/guid-123/roles/Moderator`

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Role Moderator added successfully"
}
```

---

## Remove Role

### `DELETE /api/admin/users/{userId}/roles/{role}`

Removes a role from a user.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |
| role | string | Yes | Role name (Admin, Moderator, User) |

**Example:** `DELETE /api/admin/users/guid-123/roles/Moderator`

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Role Moderator removed successfully"
}
```

---

## Get User Sessions

### `GET /api/admin/users/{userId}/sessions`

Retrieves all active sessions for a user.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "sessionId": "guid-1",
      "userId": "guid",
      "ipAddress": "192.168.1.1",
      "userAgent": "Mozilla/5.0...",
      "device": "Desktop",
      "browser": "Chrome 118",
      "os": "Windows 11",
      "country": "United States",
      "city": "New York",
      "createdAt": "2025-10-29T01:00:00Z",
      "lastActivityAt": "2025-10-29T02:00:00Z",
      "isActive": true
    }
  ]
}
```

---

## End User Session

### `DELETE /api/admin/users/{userId}/sessions/{sessionId}`

Terminates a specific user session.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |
| sessionId | GUID | Yes | Session ID to terminate |

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Session ended successfully"
}
```

---

## End All User Sessions

### `DELETE /api/admin/users/{userId}/sessions`

Terminates all sessions for a user (forces re-login).

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Response (200 OK):**

```json
{
  "success": true,
  "message": "All sessions ended successfully"
}
```

---

## Revoke All Tokens

### `POST /api/admin/users/{userId}/revoke-tokens`

Revokes all refresh tokens for a user.

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | GUID | Yes | User's unique ID |

**Response (200 OK):**

```json
{
  "success": true,
  "message": "All refresh tokens revoked successfully"
}
```

---

## Available Roles

| Role | Description |
|------|-------------|
| User | Standard user (default) |
| Moderator | Can manage users |
| Admin | Full system access |

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 401 | Unauthorized (not admin) |
| 404 | User not found |
| 500 | Server Error |

---

## Security Notes

- All actions are logged in the audit trail
- Admin actions are immutable and traceable
- Banned users cannot be unbanned (permanent)
- Suspended users can be reactivated
