# 🚀 Licensify - Complete Setup Guide

Welcome to **Licensify**, a comprehensive microservices-based license management system built with .NET 8, PostgreSQL, Redis, and React.

## 📋 Prerequisites

### Required Software
- **Docker & Docker Compose** (for infrastructure)
- **.NET 8 SDK** (for building services)
- **Node.js 18+** (for frontend development)
- **Git** (for version control)

### Optional Tools
- **Visual Studio Code** with C# extension
- **Postman** or similar API testing tool
- **pgAdmin** or **Azure Data Studio** for database management

## 🎯 Quick Start

### 1. Clone and Setup

```bash
# Clone the repository
git clone <your-repo-url>
cd Licensify

# Copy environment variables
cp .env.example .env

# Make setup script executable (Linux/Mac)
chmod +x scripts/setup-dev.sh

# Run setup script
# For Windows:
scripts/setup-dev.bat

# For Linux/Mac:
./scripts/setup-dev.sh
```

### 2. Start Services Manually (Alternative)

```bash
# Start infrastructure
docker-compose -f docker-compose.dev.yml up -d

# Build shared library
cd shared && dotnet build && cd ..

# Start Auth Service (Terminal 1)
cd services/auth-service/src
dotnet run

# Start Product Service (Terminal 2)
cd services/product-service/src
dotnet run
```

### 3. Test the APIs

Open the `.http` files in `tests/` folder with VS Code REST Client extension:
- `tests/auth-service.http`
- `tests/product-service.http`

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Licensify System                        │
├─────────────────────────────────────────────────────────────┤
│  Frontend (React + NextUI)                                 │
├─────────────────────────────────────────────────────────────┤
│  API Gateway (Nginx) - Port 8080                           │
├─────────────────────────────────────────────────────────────┤
│  Microservices:                                            │
│  ├── Auth Service (Port 5001)                              │
│  ├── Product Service (Port 5002)                           │
│  ├── License Service (Port 5003) [TODO]                    │
│  └── Audit Service (Port 5004) [TODO]                      │
├─────────────────────────────────────────────────────────────┤
│  Infrastructure:                                           │
│  ├── PostgreSQL (Port 5432)                                │
│  ├── Redis (Port 6379)                                     │
│  ├── RabbitMQ (Port 5672, Management: 15672)               │
│  └── MongoDB (Port 27017)                                  │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Service Configuration

### Database Schema
- **PostgreSQL** stores users, products, licenses
- **Redis** caches JWT tokens and license validations
- **MongoDB** stores audit logs
- **RabbitMQ** handles event messaging between services

### Default Credentials
```
Admin User:
- Email: admin@licensify.com
- Password: Admin123!

Database:
- Host: localhost:5432
- Database: licensify
- Username: postgres
- Password: postgres123

RabbitMQ Management:
- URL: http://localhost:15672
- Username: admin
- Password: admin123
```

## 🧪 Testing

### API Testing
Use the provided `.http` files in the `tests/` directory:

1. **Auth Service**: Login, get user info, refresh tokens
2. **Product Service**: CRUD operations for products

### Unit Testing (TODO)
```bash
# Run unit tests for all services
dotnet test

# Run tests for specific service
cd services/auth-service/tests
dotnet test
```

## 🚀 Production Deployment

### Docker Compose (Full Stack)
```bash
# Build and start all services
docker-compose up -d

# Check service health
docker-compose ps
```

### Kubernetes (Recommended)
```bash
# Deploy to Kubernetes (TODO)
helm install licensify ./infra/helm/licensify
```

## 📁 Project Structure

```
Licensify/
├── services/
│   ├── auth-service/          # Authentication & user management
│   ├── product-service/       # Product CRUD operations
│   ├── license-service/       # License generation & validation [TODO]
│   └── audit-service/         # Audit logging [TODO]
├── shared/                    # Shared DTOs, middleware, extensions
├── frontend/admin-panel/      # React admin interface [TODO]
├── infra/                     # Infrastructure configs
├── tests/                     # API test files
├── scripts/                   # Setup and utility scripts
└── docs/                      # Documentation
```

## 🔐 Security Features

### Authentication & Authorization
- **JWT tokens** with refresh token rotation
- **Role-based access control** (Admin, Developer)
- **BCrypt password hashing**
- **Token blacklisting** via Redis

### API Security
- **CORS** configuration
- **Rate limiting** (TODO)
- **Request logging** middleware
- **Health check endpoints**

## 📊 Monitoring & Logging

### Logging
- **Serilog** with structured logging
- **File and console** outputs
- **Request/Response** logging middleware

### Health Checks
- `/health` endpoints for all services
- **Database connectivity** checks
- **Redis connectivity** checks

### Metrics (TODO)
- **Prometheus** metrics collection
- **Grafana** dashboards
- **Application performance** monitoring

## 🔄 Development Workflow

### Adding New Features
1. **Design** the API endpoints
2. **Update** shared DTOs if needed
3. **Implement** service logic
4. **Add** database migrations
5. **Write** unit tests
6. **Test** with `.http` files
7. **Update** documentation

### Database Migrations
```bash
# Add new migration
cd services/auth-service/src
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## 🐛 Troubleshooting

### Common Issues

**Services can't connect to database:**
```bash
# Check if PostgreSQL is running
docker-compose -f docker-compose.dev.yml ps postgres

# Check connection string in appsettings.json
```

**JWT token issues:**
```bash
# Verify JWT secret in configuration
# Check token expiration times
```

**Port conflicts:**
```bash
# Check if ports are already in use
netstat -an | grep 5001
# Change ports in appsettings.json if needed
```

## 📚 API Documentation

### Auth Service Endpoints
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Get current user
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - Logout user
- `POST /api/auth/revoke-all` - Revoke all tokens

### Product Service Endpoints
- `GET /api/products` - List products (with pagination)
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product (Admin only)

## 🔮 Roadmap

### Phase 1: Core Services ✅
- [x] Auth Service implementation
- [x] Product Service implementation
- [x] Shared library with DTOs
- [x] Database setup and migrations
- [x] Docker infrastructure

### Phase 2: License Management 🚧
- [ ] License Service implementation
- [ ] License key generation and validation
- [ ] Device/IP limiting
- [ ] License usage tracking

### Phase 3: Audit & Frontend 📋
- [ ] Audit Service implementation
- [ ] Event-driven architecture with RabbitMQ
- [ ] React admin panel
- [ ] User management interface

### Phase 4: DevOps & Production 🚀
- [ ] Kubernetes deployment
- [ ] CI/CD pipelines
- [ ] Monitoring and alerting
- [ ] Performance optimization

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**🎉 You're now ready to develop with Licensify!**

For questions or support, please check the documentation or create an issue.
