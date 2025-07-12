# Clean Architecture Setup Script for Licensify
# Run this script to migrate to Clean Architecture structure

Write-Host "🏗️ Setting up Clean Architecture for Licensify..." -ForegroundColor Green

# Function to create directory if not exists
function New-DirectoryIfNotExists {
    param($Path)
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
        Write-Host "✅ Created: $Path" -ForegroundColor Green
    } else {
        Write-Host "📁 Exists: $Path" -ForegroundColor Yellow
    }
}

# Function to move file if exists
function Move-FileIfExists {
    param($Source, $Destination)
    if (Test-Path $Source) {
        Move-Item $Source $Destination -Force
        Write-Host "📦 Moved: $Source → $Destination" -ForegroundColor Blue
    } else {
        Write-Host "⚠️ Not found: $Source" -ForegroundColor Yellow
    }
}

Write-Host "🔧 Phase 1: Creating Clean Architecture Structure" -ForegroundColor Cyan

# Create Clean Architecture folders for AuthService
$authServicePath = "services\auth-service\src"

# Application Layer folders
New-DirectoryIfNotExists "$authServicePath\AuthService.Application\Commands\Auth\Handlers"
New-DirectoryIfNotExists "$authServicePath\AuthService.Application\Queries\Users\Handlers"
New-DirectoryIfNotExists "$authServicePath\AuthService.Application\Validators"
New-DirectoryIfNotExists "$authServicePath\AuthService.Application\Services"
New-DirectoryIfNotExists "$authServicePath\AuthService.Application\DTOs"

# Infrastructure Layer folders  
New-DirectoryIfNotExists "$authServicePath\AuthService.Infrastructure\Persistence\Configurations"
New-DirectoryIfNotExists "$authServicePath\AuthService.Infrastructure\Persistence\Repositories"
New-DirectoryIfNotExists "$authServicePath\AuthService.Infrastructure\ExternalServices"
New-DirectoryIfNotExists "$authServicePath\AuthService.Infrastructure\Configuration"

# API Layer folders
New-DirectoryIfNotExists "$authServicePath\AuthService.Api\Controllers"
New-DirectoryIfNotExists "$authServicePath\AuthService.Api\Middleware"
New-DirectoryIfNotExists "$authServicePath\AuthService.Api\Extensions"

Write-Host "🔧 Phase 2: Creating Project Templates" -ForegroundColor Cyan

# Add projects to solution
Write-Host "📋 Adding projects to solution..." -ForegroundColor Blue
$originalLocation = Get-Location
Set-Location "services\auth-service"

try {
    dotnet sln ..\..\Licensify.sln add src\AuthService.Domain\AuthService.Domain.csproj
    dotnet sln ..\..\Licensify.sln add src\AuthService.Application\AuthService.Application.csproj  
    dotnet sln ..\..\Licensify.sln add src\AuthService.Infrastructure\AuthService.Infrastructure.csproj
    dotnet sln ..\..\Licensify.sln add src\AuthService.Api\AuthService.Api.csproj
} catch {
    Write-Host "⚠️ Some projects might already be in solution" -ForegroundColor Yellow
}

Write-Host "🔧 Phase 3: Apply same pattern to ProductService" -ForegroundColor Cyan

# Create structure for ProductService
$productServicePath = "services\product-service\src"

New-DirectoryIfNotExists "$productServicePath\ProductService.Domain\Entities"
New-DirectoryIfNotExists "$productServicePath\ProductService.Domain\ValueObjects"
New-DirectoryIfNotExists "$productServicePath\ProductService.Domain\Events"
New-DirectoryIfNotExists "$productServicePath\ProductService.Domain\Interfaces"

New-DirectoryIfNotExists "$productServicePath\ProductService.Application\Commands\Products\Handlers"
New-DirectoryIfNotExists "$productServicePath\ProductService.Application\Queries\Products\Handlers"
New-DirectoryIfNotExists "$productServicePath\ProductService.Application\Validators"

New-DirectoryIfNotExists "$productServicePath\ProductService.Infrastructure\Persistence"
New-DirectoryIfNotExists "$productServicePath\ProductService.Api\Controllers"

Write-Host "🔧 Phase 4: Setting up Testing Structure" -ForegroundColor Cyan

# Create test projects
New-DirectoryIfNotExists "services\auth-service\tests\AuthService.UnitTests"
New-DirectoryIfNotExists "services\auth-service\tests\AuthService.IntegrationTests"
New-DirectoryIfNotExists "services\auth-service\tests\AuthService.ArchitectureTests"

New-DirectoryIfNotExists "services\product-service\tests\ProductService.UnitTests"
New-DirectoryIfNotExists "services\product-service\tests\ProductService.IntegrationTests"

Write-Host "✅ Clean Architecture setup completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Review ARCHITECTURE_GUIDE.md for detailed guidelines" -ForegroundColor White
Write-Host "2. Study the Domain layer implementation in AuthService.Domain" -ForegroundColor White
Write-Host "3. Complete Application layer commands and queries" -ForegroundColor White
Write-Host "4. Migrate existing logic to proper layers" -ForegroundColor White
Write-Host "5. Add comprehensive unit tests" -ForegroundColor White
Write-Host ""
Write-Host "🎯 Goal: Everyone codes consistently following Clean Architecture!" -ForegroundColor Green

Set-Location $originalLocation
