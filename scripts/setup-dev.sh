#!/bin/bash

# Licensify Development Setup Script
# This script sets up the development environment for Licensify

echo "🚀 Setting up Licensify Development Environment..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check if .NET 8 is installed
if ! dotnet --version | grep -q "8."; then
    echo "❌ .NET 8 SDK is not installed. Please install .NET 8 SDK and try again."
    exit 1
fi

# Create .env file from example if it doesn't exist
if [ ! -f .env ]; then
    echo "📄 Creating .env file from example..."
    cp .env.example .env
    echo "✅ .env file created. Please update the values as needed."
fi

# Start infrastructure services
echo "🐳 Starting infrastructure services (PostgreSQL, Redis, RabbitMQ, MongoDB)..."
docker-compose -f docker-compose.dev.yml up -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 10

# Check if PostgreSQL is ready
echo "🔍 Checking PostgreSQL connection..."
timeout=30
while [ $timeout -gt 0 ]; do
    if docker exec licensify-postgres-1 2>/dev/null || docker exec $(docker-compose -f docker-compose.dev.yml ps -q postgres) pg_isready -U postgres; then
        echo "✅ PostgreSQL is ready"
        break
    fi
    sleep 1
    timeout=$((timeout-1))
done

if [ $timeout -eq 0 ]; then
    echo "❌ PostgreSQL failed to start"
    exit 1
fi

# Build shared library
echo "🔨 Building shared library..."
cd shared
dotnet build
cd ..

# Build and run Auth Service
echo "🔨 Building Auth Service..."
cd services/auth-service/src
dotnet restore
dotnet build

# Run database migrations
echo "📊 Running database migrations..."
dotnet ef database update || echo "⚠️ EF Core tools might not be installed. Run: dotnet tool install --global dotnet-ef"

cd ../../..

# Build Product Service
echo "🔨 Building Product Service..."
cd services/product-service/src
dotnet restore
dotnet build
cd ../../..

echo "✅ Development environment setup completed!"
echo ""
echo "🎯 Next steps:"
echo "1. Start Auth Service: cd services/auth-service/src && dotnet run"
echo "2. Start Product Service: cd services/product-service/src && dotnet run"
echo "3. Test APIs using the provided Postman collection or .http files"
echo ""
echo "📊 Service URLs:"
echo "- Auth Service: http://localhost:5001"
echo "- Product Service: http://localhost:5002"
echo "- PostgreSQL: localhost:5432"
echo "- Redis: localhost:6379"
echo "- RabbitMQ Management: http://localhost:15672 (admin/admin123)"
echo ""
echo "🔐 Default Admin Credentials:"
echo "- Email: admin@licensify.com"
echo "- Password: Admin123!"
