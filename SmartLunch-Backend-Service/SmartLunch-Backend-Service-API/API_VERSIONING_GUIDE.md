# Hướng Dẫn API Versioning

## Tổng Quan

API Versioning (Phiên bản hóa API) là một kỹ thuật quan trọng cho phép bạn quản lý các thay đổi trong API mà không làm gián đoạn các client đang sử dụng phiên bản cũ. Khi bạn cần thay đổi cấu trúc API, bạn có thể tạo một phiên bản mới thay vì phá vỡ các client hiện có.

## Cách Hoạt Động

### 1. Cấu Hình API Versioning

Trong `Program.cs`, chúng ta đã cấu hình API versioning với các tùy chọn sau:

```csharp
builder.Services.AddApiVersioning(options =>
{
    // Phiên bản mặc định là 1.0
    options.DefaultApiVersion = new ApiVersion(1, 0);
    
    // Nếu không chỉ định phiên bản, sử dụng phiên bản mặc định
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Báo cáo các phiên bản API có sẵn trong response headers
    options.ReportApiVersions = true;
    
    // Hỗ trợ nhiều cách chỉ định phiên bản
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),  // Qua query string
        new HeaderApiVersionReader("X-API-Version"),    // Qua HTTP header
        new UrlSegmentApiVersionReader()                 // Qua URL path
    );
});
```

### 2. Các Cách Chỉ Định Phiên Bản API

Có 3 cách để client chỉ định phiên bản API:

#### 1. Qua URL Path (Khuyến nghị)
```
GET /api/v1/auth/login
GET /api/v2/auth/login
```

#### 2. Qua Query String
```
GET /api/auth/login?api-version=1.0
GET /api/auth/login?api-version=2.0
```

#### 3. Qua HTTP Header
```
GET /api/auth/login
Headers:
  X-API-Version: 1.0
```

### 3. Sử Dụng Trong Controller

Để đánh dấu một controller với phiên bản cụ thể:

```csharp
[ApiController]
[ApiVersion("1.0")]  // Chỉ định phiên bản
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    // ...
}
```

**Lưu ý:** `{version:apiVersion}` trong route sẽ được thay thế bằng phiên bản thực tế (ví dụ: v1, v2).

### 4. Hỗ Trợ Nhiều Phiên Bản

Bạn có thể hỗ trợ nhiều phiên bản trong cùng một controller:

```csharp
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]  // Hỗ trợ cả 2 phiên bản
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [MapToApiVersion("1.0")]  // Chỉ áp dụng cho v1
    public async Task<ActionResult> LoginV1(LoginRequest request)
    {
        // Logic cho v1
    }

    [HttpPost("login")]
    [MapToApiVersion("2.0")]  // Chỉ áp dụng cho v2
    public async Task<ActionResult> LoginV2(LoginRequestV2 request)
    {
        // Logic mới cho v2
    }
}
```

### 5. Tạo Controller Mới Cho Phiên Bản Mới

Cách tốt nhất là tạo controller riêng cho mỗi phiên bản:

```csharp
// AuthController.cs - Phiên bản 1.0
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    // Logic cho v1
}

// AuthV2Controller.cs - Phiên bản 2.0
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthV2Controller : ControllerBase
{
    // Logic mới cho v2
}
```

### 6. Đánh Dấu Phiên Bản Đã Lỗi Thời (Deprecated)

Khi một phiên bản sắp bị loại bỏ:

```csharp
[ApiController]
[ApiVersion("1.0", Deprecated = true)]  // Đánh dấu là deprecated
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    // ...
}
```

## Swagger Integration

Swagger đã được cấu hình để hiển thị tất cả các phiên bản API. Khi truy cập `/swagger`, bạn sẽ thấy:

- Dropdown để chọn phiên bản (V1, V2, ...)
- Mỗi phiên bản có Swagger document riêng
- Các endpoint được nhóm theo phiên bản

## Ví Dụ Sử Dụng

### Ví dụ 1: Gọi API v1 qua URL
```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"user","password":"pass"}'
```

### Ví dụ 2: Gọi API v1 qua Query String
```bash
curl -X POST "http://localhost:5001/api/auth/login?api-version=1.0" \
  -H "Content-Type: application/json" \
  -d '{"username":"user","password":"pass"}'
```

### Ví dụ 3: Gọi API v1 qua Header
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -H "X-API-Version: 1.0" \
  -d '{"username":"user","password":"pass"}'
```

## Best Practices

1. **Sử dụng URL Path cho versioning** - Đây là cách rõ ràng và dễ hiểu nhất
2. **Giữ backward compatibility** - Khi có thể, giữ các endpoint cũ hoạt động
3. **Đánh dấu deprecated versions** - Thông báo cho client biết phiên bản nào sắp bị loại bỏ
4. **Document changes** - Ghi rõ những thay đổi giữa các phiên bản
5. **Có kế hoạch deprecation** - Cho client thời gian để migrate trước khi loại bỏ phiên bản cũ

## Response Headers

Khi gọi API, response sẽ chứa header `api-supported-versions` và `api-deprecated-versions`:

```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
```

## Tóm Tắt

- ✅ API versioning cho phép quản lý nhiều phiên bản API cùng lúc
- ✅ Hỗ trợ 3 cách chỉ định phiên bản: URL, Query String, Header
- ✅ Swagger tự động hiển thị tất cả các phiên bản
- ✅ Có thể đánh dấu phiên bản là deprecated
- ✅ Giúp duy trì backward compatibility khi API thay đổi
