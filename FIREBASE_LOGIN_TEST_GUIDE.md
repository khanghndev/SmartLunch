# Firebase Login Testing Guide

Hướng dẫn test Firebase Login endpoint cho SmartLunch Backend Service.

## Prerequisites

1. **Firebase Project đã được cấu hình:**
   - Project ID: `smartlunch-2b5a7`
   - Firebase Admin SDK config file: `smartlunch-2b5a7-firebase-adminsdk-fbsvc-93e6045e22.json`
   - File này phải nằm ở root directory của project

2. **Backend Service đang chạy:**
   - URL: `http://localhost:5001`
   - Firebase config đã được setup trong `appsettings.json`

## Cách 1: Sử dụng Test HTML Page (Khuyến nghị)

1. **Cập nhật Firebase Config trong `test-firebase-login.html`:**
   - Mở file `test-firebase-login.html`
   - Tìm phần `firebaseConfig` và cập nhật với config từ Firebase Console:

     ```javascript
     const firebaseConfig = {
         apiKey: "YOUR_API_KEY",
         authDomain: "smartlunch-2b5a7.firebaseapp.com",
         projectId: "smartlunch-2b5a7",
         // ... các config khác
     };
     ```

   - Lấy config từ: Firebase Console > Project Settings > General > Your apps > Web app

2. **Mở file HTML trong browser:**
   - Double-click vào `test-firebase-login.html` hoặc mở bằng browser
   - Chọn phương thức đăng nhập:
     - **Google Sign-In:** Click nút "Sign in with Google" (yêu cầu Google provider đã được enable trong Firebase Console)
     - **Email/Password:** Nhập email và password của user đã tạo trong Firebase Authentication, sau đó click "Sign In with Email/Password"
   - Copy Firebase ID Token được hiển thị

3. **Enable Google Sign-In trong Firebase Console (nếu chưa có):**
   - Vào Firebase Console > Authentication > Sign-in method
   - Click vào "Google" provider
   - Enable Google sign-in và cấu hình OAuth consent screen
   - Lưu lại cấu hình

4. **Thêm Authorized Domains (QUAN TRỌNG cho local testing):**
   - Vào Firebase Console > Authentication > Settings > Authorized domains
   - Click "Add domain"
   - Thêm các domain sau:
     - `127.0.0.1` (cho localhost testing)
     - `localhost` (cho localhost testing)
   - Lưu lại
   - **Lưu ý:** Nếu không thêm domain này, Google Sign-In sẽ không hoạt động và bạn sẽ thấy lỗi "The current domain is not authorized for OAuth operations"

5. **Sử dụng token trong API test:**
   - Mở file `SmartLunch-Backend-Service.http`
   - Tìm biến `@firebaseIdToken` ở đầu file
   - Paste token vào: `@firebaseIdToken = YOUR_TOKEN_HERE`
   - Chạy request "Firebase Login - Authenticate with Firebase ID token"

## Cách 2: Sử dụng Firebase Console

1. **Tạo test user trong Firebase Console:**
   - Vào Firebase Console: <https://console.firebase.google.com/>
   - Chọn project: `smartlunch-2b5a7`
   - Vào Authentication > Users
   - Click "Add user" để tạo user mới hoặc sử dụng user có sẵn

2. **Lấy ID Token bằng Firebase CLI:**

   ```bash
   # Install Firebase CLI
   npm install -g firebase-tools
   
   # Login
   firebase login
   
   # Use Firebase Auth REST API
   # POST to: https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY
   ```

## Cách 3: Sử dụng Postman/Insomnia

1. **Lấy API Key từ Firebase Console:**
   - Firebase Console > Project Settings > General > Web API Key

2. **Call Firebase Auth REST API:**

   ```
   POST https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY
   Content-Type: application/json
   
   {
     "email": "test@example.com",
     "password": "password123",
     "returnSecureToken": true
   }
   ```

3. **Copy `idToken` từ response và dùng trong API test**

## Cách 4: Sử dụng cURL

```bash
# Get Firebase ID Token
curl -X POST "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123",
    "returnSecureToken": true
  }'

# Use the idToken from response to test backend API
curl -X POST "http://localhost:5001/api/v1/auth/firebase-login" \
  -H "Content-Type: application/json" \
  -d '{
    "idToken": "YOUR_ID_TOKEN_HERE"
  }'
```

## Test Cases

### 1. Valid Firebase Login

```http
POST http://localhost:5001/api/v1/auth/firebase-login
Content-Type: application/json

{
  "idToken": "VALID_FIREBASE_ID_TOKEN"
}
```

**Expected Response:**

```json
{
  "success": true,
  "message": "Firebase login successful",
  "data": {
    "userId": "guid",
    "username": "user@example.com",
    "email": "user@example.com",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "refreshTokenExpiresAt": "2026-01-26T..."
  }
}
```

### 2. Invalid Token

```http
POST http://localhost:5001/api/v1/auth/firebase-login
Content-Type: application/json

{
  "idToken": "invalid-token"
}
```

**Expected Response:** 401 Unauthorized

### 3. Missing Token

```http
POST http://localhost:5001/api/v1/auth/firebase-login
Content-Type: application/json

{
  "idToken": ""
}
```

**Expected Response:** 400 Bad Request

## Troubleshooting

### Lỗi: "Firebase Auth is not initialized"

- Kiểm tra file `smartlunch-2b5a7-firebase-adminsdk-fbsvc-93e6045e22.json` có tồn tại không
- Kiểm tra path trong `appsettings.json` có đúng không
- Đảm bảo backend service có quyền đọc file JSON

### Lỗi: "Invalid Firebase ID token"

- Token đã hết hạn (Firebase ID tokens expire sau 1 giờ)
- Token không hợp lệ hoặc bị chỉnh sửa
- Token không thuộc project `smartlunch-2b5a7`

### Lỗi: "User account is inactive"

- User đã bị deactivate trong database
- Kiểm tra field `IsActive` trong bảng `users`

### Lỗi: "The current domain is not authorized for OAuth operations"

- Domain hiện tại (ví dụ: `127.0.0.1` hoặc `localhost`) chưa được thêm vào Authorized domains
- **Giải pháp:**
  1. Vào Firebase Console > Authentication > Settings > Authorized domains
  2. Click "Add domain"
  3. Thêm domain: `127.0.0.1` và `localhost`
  4. Lưu lại và refresh trang test
  5. Thử lại Google Sign-In

## Notes

- Firebase ID tokens có thời gian sống là 1 giờ
- Nếu token hết hạn, cần lấy token mới từ Firebase
- User sẽ được tự động tạo trong database nếu chưa tồn tại (dựa trên email)
- Provider sẽ được set là "firebase" cho các user login qua Firebase
