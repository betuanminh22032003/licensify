# 📄 Software Requirements Specification (SRS)  
**Project Name**: Licensify  
**Purpose**: License Management System for SaaS or On-premise software distribution  
**Author**: Minh  

---

## 1. 🎯 **Overview**

Licensify is a backend-oriented, self-hostable microservice system for managing product licenses, authentication, auditing, and license validation. It targets internal SaaS use or resale alongside systems like `n8n`, enabling flexible licensing control with K8s-native deployment.

---

## 2. 📚 **Functional Requirements**

### 2.1 🧑‍💼 Authentication Service

- [ ] Login via email/password
- [ ] JWT token generation & refresh
- [ ] Role-based authorization (Admin, Developer)
- [ ] User creation by admin
- [ ] Fetch current user info

---

### 2.2 📦 Product Management

- [ ] Create, update, delete products
- [ ] Assign product name, version
- [ ] Filter/search products (by name, version)
- [ ] Pagination support

---

### 2.3 🔑 License Management

- [ ] Generate license keys based on:
  - Product ID
  - Expiry date (TTL)
  - Device limit / IP limit
- [ ] Validate license key:
  - Format
  - Expiration
  - Active device/IP constraint
- [ ] View license history / usage
- [ ] Revoke or expire license key

---

### 2.4 📊 Audit Logging (Optional)

- [ ] Track key events:
  - User login
  - License key generated
  - License validated
- [ ] Log to MongoDB or Elasticsearch
- [ ] View audit logs via UI (optional)

---

### 2.5 💽 Frontend Admin Panel

- [ ] Admin login with token persistence
- [ ] Dashboard with stats (Total Licenses, Active Users, etc.)
- [ ] Product management page (table CRUD)
- [ ] License creation and validation UI
- [ ] Optional: audit log viewer
- [ ] Sidebar + Header layout with routing

---

## 3. ⚙️ **Non-Functional Requirements**

| Category     | Requirements |
|--------------|--------------|
| **Performance** | License validation via Redis cache for <50ms latency |
| **Scalability** | Each microservice is stateless and horizontally scalable |
| **Security**    | JWT auth, rate limiting, role-based access |
| **Deployability** | Kubernetes-native, Helm chart support, CI/CD |
| **Portability** | Dockerized microservices, can run via docker-compose or K8s |
| **Logging** | Centralized via Serilog, optionally sent to Grafana |
| **Monitoring** | Prometheus metrics for each service, Grafana dashboards |

---

## 4. 🛠️ **Technology Stack**

| Component | Technology |
|----------|------------|
| Frontend | React + NextUI |
| Backend  | ASP.NET Core (.NET 8) |
| Database | PostgreSQL |
| Cache    | Redis |
| Messaging | RabbitMQ |
| Logging  | Serilog, optional MongoDB/Elasticsearch |
| DevOps   | Docker, Kubernetes, Helm, GitHub Actions |
| Monitoring | Prometheus + Grafana |

---

## 5. 🔐 User Roles & Permissions

| Role   | Permissions |
|--------|-------------|
| **Admin** | Full access to all features |
| **Developer** | Read-only or scoped product/license access |
| *(future)* | Scoped license generation for vendors |

---

## 6. 🧪 Testing Requirements

- Unit tests for API endpoints (xUnit + Moq)
- HTTP integration tests (`*.http`, Postman)
- Health check endpoints per service
- CI workflow to run tests on every push

