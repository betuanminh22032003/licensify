@echo off
REM Licensify Development Setup Script for Windows
REM This script sets up the development environment for Licensify

echo 🚀 Setting up Licensify Development Environment...

REM Check if Docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker and try again.
    exit /b 1
)

REM Check if .NET 8 is installed
dotnet --version | findstr "8." >nul
if %errorlevel% neq 0 (
    echo ❌ .NET 8 SDK is not installed. Please install .NET 8 SDK and try again.
    exit /b 1
)

REM Create .env file from example if it doesn't exist
if not exist .env (
    echo 📄 Creating .env file from example...
    copy .env.example .env
    echo ✅ .env file created. Please update the values as needed.
)

REM Start infrastructure services
echo 🐳 Starting infrastructure services (PostgreSQL, Redis, RabbitMQ, MongoDB)...
docker-compose -f docker-compose.dev.yml up -d

REM Wait for services to be ready
echo ⏳ Waiting for services to be ready...
timeout /t 10 /nobreak >nul

echo ✅ Infrastructure services started

REM Build shared library
echo 🔨 Building shared library...
cd shared
dotnet build
cd ..

REM Build Auth Service
echo 🔨 Building Auth Service...
cd services\auth-service\src
dotnet restore
dotnet build

REM Install EF Core tools if not already installed
dotnet tool list -g | findstr "dotnet-ef" >nul
if %errorlevel% neq 0 (
    echo 🔧 Installing EF Core tools...
    dotnet tool install --global dotnet-ef
)

REM Run database migrations
echo 📊 Running database migrations...
dotnet ef database update

cd ..\..\..

REM Build Product Service
echo 🔨 Building Product Service...
cd services\product-service\src
dotnet restore
dotnet build
cd ..\..\..

echo ✅ Development environment setup completed!
echo.
echo 🎯 Next steps:
echo 1. Start Auth Service: cd services\auth-service\src ^&^& dotnet run
echo 2. Start Product Service: cd services\product-service\src ^&^& dotnet run
echo 3. Test APIs using the provided Postman collection or .http files
echo.
echo 📊 Service URLs:
echo - Auth Service: http://localhost:5001
echo - Product Service: http://localhost:5002
echo - PostgreSQL: localhost:5432
echo - Redis: localhost:6379
echo - RabbitMQ Management: http://localhost:15672 (admin/admin123)
echo.
echo 🔐 Default Admin Credentials:
echo - Email: admin@licensify.com
echo - Password: Admin123!

pause
