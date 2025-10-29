# 🔐 Authentication API

**Base Path:** `/api/auth`  
**Controller:** `AuthController`

Manages user authentication, registration, email verification, password recovery, and token refresh.

---

## 📋 Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/register` | Register new user | ❌ |
| POST | `/login` | Login user | ❌ |
| GET | `/verify-email/code` | Send verification code | ❌ |
| POST | `/verify-email/confirm` | Confirm email | ❌ |
| GET | `/forgot-password/code` | Send reset code | ❌ |
| POST | `/forgot-password/verify` | Verify reset code | ❌ |
| POST | `/forgot-password/reset` | Reset password | ❌ |
| POST | `/google` | Google OAuth login | ❌ |
| POST | `/refresh` | Refresh access token | ❌ |

---

## Register

### `POST /api/auth/register`

Creates a new user account and sends an email verification code.

**Request Body:**

```json
{
  "username": "kevin123",
  "email": "kevin@example.com",
  "password": "StrongP@ssw0rd!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "userId": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
    "username": "kevin123",
    "email": "kevin@example.com",
    "isEmailVerified": false
  },
  "message": "Registration successful. Please check your email for verification code."
}
```

**Error (400 Bad Request):**

```json
{
  "message": "Email already registered",
  "statusCode": 400
}
```

**Notes:**
- Password must be at least 8 characters
- Email verification code is valid for 10 minutes
- You must verify your email before logging in

---

## Login

### `POST /api/auth/login`

Authenticates a user and returns JWT tokens.

**Request Body:**

```json
{
  "email": "kevin@example.com",
  "password": "StrongP@ssw0rd!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
    "username": "kevin123",
    "email": "kevin@example.com",
    "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64_encoded_refresh_token"
  },
  "message": "Login successful"
}
```

**Error (401 Unauthorized):**

```json
{
  "message": "Email not verified. Please verify your email before logging in.",
  "statusCode": 401
}
```

**Token Details:**
- **Access Token**: Valid for 15 minutes
- **Refresh Token**: Valid for 7 days
- Use access token in `Authorization: Bearer {token}` header

---

## Verify Email

### Send Code: `GET /api/auth/verify-email/code`

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| email | string | Yes | User's email address |

**Example:** `GET /api/auth/verify-email/code?email=kevin@example.com`

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Verification code sent to email"
}
```

### Confirm Code: `POST /api/auth/verify-email/confirm`

**Request Body:**

```json
{
  "email": "kevin@example.com",
  "code": "123456"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Email successfully verified. You can now log in"
}
```

---

## Forgot Password

### 1. Send Reset Code: `GET /api/auth/forgot-password/code`

**Example:** `GET /api/auth/forgot-password/code?email=kevin@example.com`

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Verification code sent to email"
}
```

### 2. Verify Code: `POST /api/auth/forgot-password/verify`

**Request Body:**

```json
{
  "email": "kevin@example.com",
  "code": "123456"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": true,
  "message": "Code verified successfully"
}
```

### 3. Reset Password: `POST /api/auth/forgot-password/reset`

**Request Body:**

```json
{
  "email": "kevin@example.com",
  "resetCode": "123456",
  "newPassword": "NewStrongP@ssw0rd!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Password successfully reset"
}
```

---

## Google OAuth

### `POST /api/auth/google`

Authenticates or registers a user via Google OAuth.

**Request Body:**

```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU5M..."
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": "guid",
    "username": "johnsmith123",
    "email": "john@gmail.com",
    "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64_encoded_token"
  },
  "message": "Google authentication successful"
}
```

**Notes:**
- If the email doesn't exist, a new account is created automatically
- Google accounts are automatically verified
- No email confirmation required

---

## Refresh Token

### `POST /api/auth/refresh`

Refreshes an expired access token.

**Request Body:**

```json
{
  "refreshToken": "base64_encoded_refresh_token"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "kevin123",
    "email": "kevin@example.com",
    "token": "new_access_token",
    "refreshToken": "new_refresh_token"
  },
  "message": "Tokens refreshed successfully"
}
```

**Security:**
- Each refresh token can only be used once
- After use, a new refresh token is issued
- Old refresh token is marked as used
- Prevents token replay attacks

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request |
| 401 | Unauthorized |
| 404 | Not Found |
| 500 | Server Error |
