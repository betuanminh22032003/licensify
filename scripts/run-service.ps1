# ==========================================
# Licensify - Quick Start Individual Services
# ==========================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("gateway", "auth", "product", "all")]
    [string]$Service = "all"
)

$RootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

# Service definitions
$Services = @{
    "gateway" = @{ Name = "API Gateway"; Path = "$RootDir\APIGateway"; Port = 5047 }
    "auth" = @{ Name = "Auth Service"; Path = "$RootDir\services\auth-service\src\AuthService.Api"; Port = 29446 }
    "product" = @{ Name = "Product Service"; Path = "$RootDir\services\product-service\src"; Port = 27874 }
}

function Start-SingleService {
    param($ServiceConfig)
    
    if (Test-Path $ServiceConfig.Path) {
        Write-Host "Starting $($ServiceConfig.Name)..." -ForegroundColor Green
        Set-Location $ServiceConfig.Path
        dotnet run
    } else {
        Write-Host "Service not found at: $($ServiceConfig.Path)" -ForegroundColor Red
        exit 1
    }
}

if ($Service -eq "all") {
    Write-Host "Use: .\scripts\run-services.ps1 -Service <gateway|auth|product>" -ForegroundColor Yellow
    Write-Host "Or use: .\scripts\run-all-services.ps1 to start all services" -ForegroundColor Yellow
    exit 0
}

if ($Services.ContainsKey($Service)) {
    $ServiceConfig = $Services[$Service]
    Write-Host "Starting $($ServiceConfig.Name) on port $($ServiceConfig.Port)..." -ForegroundColor Cyan
    Start-SingleService $ServiceConfig
} else {
    Write-Host "Invalid service. Use: gateway, auth, or product" -ForegroundColor Red
    exit 1
}
