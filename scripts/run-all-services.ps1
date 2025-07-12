# ==========================================
# Licensify - Run All Services
# ==========================================
# Simple script to run dotnet run for all services
# ==========================================

param(
    [switch]$WithDependencies,
    [switch]$SkipBuild
)

Write-Host "Starting Licensify Services..." -ForegroundColor Cyan

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RootDir = Split-Path -Parent $ScriptDir

Write-Host "Root Directory: $RootDir" -ForegroundColor Blue

# Define services to run
$Services = @(
    @{ Name = "API Gateway"; Path = "$RootDir\APIGateway"; Port = 5047 },
    @{ Name = "Auth Service"; Path = "$RootDir\services\auth-service\src\AuthService.Api"; Port = 29446 },
    @{ Name = "Product Service"; Path = "$RootDir\services\product-service\src"; Port = 27874 }
)

# Start dependencies if requested
if ($WithDependencies) {
    Write-Host ""
    Write-Host "Starting dependencies with Docker..." -ForegroundColor Yellow
    Set-Location $RootDir
    try {
        docker-compose -f docker-compose.dev.yml up -d
        Write-Host "Dependencies started" -ForegroundColor Green
        Start-Sleep -Seconds 5
    } catch {
        Write-Host "Failed to start dependencies" -ForegroundColor Red
    }
}

# Build services if not skipped
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building services..." -ForegroundColor Yellow
    foreach ($service in $Services) {
        if (Test-Path $service.Path) {
            Write-Host "Building $($service.Name)..." -ForegroundColor Gray
            Set-Location $service.Path
            dotnet build --configuration Release --verbosity quiet
            if ($LASTEXITCODE -eq 0) {
                Write-Host "$($service.Name) built successfully" -ForegroundColor Green
            } else {
                Write-Host "$($service.Name) build failed" -ForegroundColor Red
            }
        }
    }
}

# Start all services
Write-Host ""
Write-Host "Starting services..." -ForegroundColor Yellow

foreach ($service in $Services) {
    if (Test-Path $service.Path) {
        Write-Host "Starting $($service.Name) on port $($service.Port)..." -ForegroundColor Cyan
        
        # Create window title
        $WindowTitle = "$($service.Name) - Port $($service.Port)"
        
        # Start service in new PowerShell window
        Start-Process powershell.exe -ArgumentList @(
            "-NoExit",
            "-Command",
            "Set-Location '$($service.Path)'; `$Host.UI.RawUI.WindowTitle = '$WindowTitle'; Write-Host 'Starting $($service.Name)...' -ForegroundColor Green; dotnet run"
        ) -WindowStyle Normal
        
        Write-Host "$($service.Name) started in new window" -ForegroundColor Green
        Start-Sleep -Seconds 2
    } else {
        Write-Host "$($service.Name) not found at: $($service.Path)" -ForegroundColor Yellow
    }
}

# Summary
Write-Host ""
Write-Host "Services Summary:" -ForegroundColor Cyan
Write-Host "API Gateway:     http://localhost:5047/swagger" -ForegroundColor Magenta
Write-Host "Auth Service:    http://localhost:29446/swagger" -ForegroundColor Blue
Write-Host "Product Service: http://localhost:27874/swagger" -ForegroundColor Green

Write-Host ""
Write-Host "Test API Gateway routing:" -ForegroundColor Blue
Write-Host "  curl http://localhost:5047/api/auth/health" -ForegroundColor Gray
Write-Host "  curl http://localhost:5047/api/products" -ForegroundColor Gray

Write-Host ""
Write-Host "All services started! Check individual windows for logs." -ForegroundColor Green
Write-Host "Use Ctrl+C in each window to stop services" -ForegroundColor Yellow
