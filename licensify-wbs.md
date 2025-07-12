# 📦 Project WBS – License Management System (Full Plan)

## ✅ 1. Requirements & Architecture
- [ ] 1.1 Xác định use cases: login, license, product CRUD
- [ ] 1.2 Thiết kế schema PostgreSQL cho Auth, Product, License
- [ ] 1.3 Redis model cache key/license TTL
- [ ] 1.4 Thiết kế giao tiếp REST + sự kiện RabbitMQ
- [ ] 1.5 Chốt công nghệ và thư viện dùng

---

## 🔧 2. Backend Microservices (Auth / Product / License)

### 2.1 Auth Service
- [ ] 2.1.1 `POST /login` (JWT return)
- [ ] 2.1.2 `GET /me` trả info user
- [ ] 2.1.3 `POST /users` (admin tạo user)
- [ ] 2.1.4 Middleware Auth & Role check
- [ ] 2.1.5 Dockerfile, healthcheck endpoint

### 2.2 Product Service
- [ ] 2.2.1 Model Product (Id, Name, Version)
- [ ] 2.2.2 CRUD API: create/update/delete/list
- [ ] 2.2.3 Pagination, filter by name
- [ ] 2.2.4 Tích hợp PostgreSQL + EF Core
- [ ] 2.2.5 Dockerfile + unit test basic

### 2.3 License Service
- [ ] 2.3.1 Generate License Key (Hash based)
- [ ] 2.3.2 Validate License Key (TTL, device limit, IP)
- [ ] 2.3.3 Redis cache license key lookup
- [ ] 2.3.4 RabbitMQ publish event "LicenseCreated"
- [ ] 2.3.5 Log hoạt động tạo / kiểm tra license

### 2.4 Audit Service *(Optional)*
- [ ] 2.4.1 Ghi log hoạt động quan trọng (login, tạo key)
- [ ] 2.4.2 Lưu trữ MongoDB hoặc Elasticsearch

---

## 💻 3. Frontend Admin Panel (React + MUI)
- [ ] 3.1 Dựng base Vite project + Tailwind/MUI
- [ ] 3.2 Trang login (gọi API, lưu JWT, redirect)
- [ ] 3.3 Trang quản lý product: table, edit, tạo mới
- [ ] 3.4 Trang tạo license key, validate key
- [ ] 3.5 AuditLog (nếu có): bảng log hoạt động
- [ ] 3.6 Route Guard, Layout (Header + Sidebar)

---

## 🐳 4. DevOps – Docker, K8s, CI/CD
- [ ] 4.1 Dockerfile riêng cho mỗi service
- [ ] 4.2 docker-compose để dev nhanh local
- [ ] 4.3 Helm chart base deploy từng service
- [ ] 4.4 Tạo secrets, configMap, persistentVolume
- [ ] 4.5 GitHub Actions: build & push Docker image
- [ ] 4.6 GitHub Actions: deploy Helm chart vào K8s

---

## 📈 5. Monitoring, Logging, Testing
- [ ] 5.1 Tích hợp Serilog (log file + console)
- [ ] 5.2 Prometheus metrics + Grafana dashboard
- [ ] 5.3 Healthcheck endpoint cho từng service
- [ ] 5.4 Unit test (xUnit + Moq)
- [ ] 5.5 Postman Collection + file test `.http`
- [ ] 5.6 README.md hướng dẫn build, test, deploy

---

## 🏁 Tổng kết sau hoàn thành
- [ ] Viết changelog / release note v1.0
- [ ] Checklist pre-release: test CI/CD, test deploy sạch
- [ ] Chuẩn bị bản SaaS / self-host package đầu tiên

