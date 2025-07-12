param(
    [string]$Target = "Build",
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot

Write-Host "🏗️ Building Licensify Solution" -ForegroundColor Cyan
Write-Host "📁 Root Directory: $RootDir" -ForegroundColor Gray
Write-Host "🎯 Target: $Target" -ForegroundColor Gray
Write-Host "⚙️ Configuration: $Configuration" -ForegroundColor Gray
Write-Host ""

try {
    Write-Host "📋 Restoring NuGet packages..." -ForegroundColor Blue
    dotnet restore $RootDir --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Package restore failed" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Package restore completed" -ForegroundColor Green
    
    Write-Host "📋 Building solution..." -ForegroundColor Blue
    dotnet build $RootDir --configuration $Configuration --no-restore --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Build completed successfully" -ForegroundColor Green
    
    if ($Target -eq "Test" -or $Target -eq "All") {
        Write-Host "📋 Running tests..." -ForegroundColor Blue
        dotnet test $RootDir --configuration $Configuration --no-build --verbosity minimal
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Tests failed" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "✅ Tests completed successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "🎉 Build pipeline completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
