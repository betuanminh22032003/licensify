# API Gateway - Licensify

API Gateway sử dụng Ocelot để route requests đến các microservices.

## Cấu hình đã bổ sung

### 1. **Routes cho tất cả services**
- **Auth Service**: `localhost:29446` - `/api/auth/*`
- **Product Service**: `localhost:27874` - `/api/products/*`
- **License Service**: `localhost:5002` - `/api/licenses/*` (placeholder)
- **Audit Service**: `localhost:5003` - `/api/audit/*` (placeholder)

### 2. **JWT Authentication**
- Cấu hình JWT Bearer token authentication
- Tích hợp với Auth Service
- Protected routes cho Product, License, và Audit services

### 3. **Rate Limiting**
- Giới hạn 100 requests/phút cho Product Service
- Giới hạn 50 requests/phút cho Audit Service
- Custom error messages khi vượt giới hạn

### 4. **CORS Configuration**
- Cho phép tất cả origins, methods, và headers
- Phù hợp cho development environment

### 5. **Security Features**
- JWT token validation
- Issuer và Audience validation
- Signature key validation

## Sử dụng

### Khởi chạy Gateway
```bash
cd APIGateway
dotnet run
```

Gateway sẽ chạy trên: `http://localhost:5047`

### Endpoints
- `GET/POST/PUT/DELETE /api/auth/*` → Auth Service
- `GET/POST/PUT/DELETE /api/products/*` → Product Service (yêu cầu JWT)
- `GET/POST/PUT/DELETE /api/licenses/*` → License Service (yêu cầu JWT)
- `GET/POST/PUT/DELETE /api/audit/*` → Audit Service (yêu cầu JWT)

### Authentication
Để truy cập protected endpoints, thêm JWT token vào header:
```
Authorization: Bearer <your-jwt-token>
```

## Cấu hình Production

Để triển khai production, cập nhật:

1. **ocelot.json**: Thay đổi `localhost` thành service URLs thực tế
2. **appsettings.json**: Cập nhật JWT secret key an toàn
3. **CORS**: Giới hạn allowed origins
4. **Rate Limiting**: Điều chỉnh giới hạn phù hợp
5. **HTTPS**: Bật HTTPS cho tất cả communications
