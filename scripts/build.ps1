#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Build and validate script for DDD Clean Architecture project
.DESCRIPTION
    This script runs compilation, tests, and architecture validation
.PARAMETER Target
    The build target: Build, Test, Validate, All
.PARAMETER Configuration
    Build configuration: Debug or Release
.PARAMETER Fix
    Automatically fix issues where possible
.PARAMETER Verbose
    Show detailed output
#>

param(
    [Parameter()]
    [ValidateSet("Build", "Test", "Validate", "All")]
    [string]$Target = "All",
    
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    
    [Parameter()]
    [switch]$Fix = $false,
    
    [Parameter()]
    [switch]$Verbose = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RootDir = Split-Path -Parent $ScriptDir

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "🏗️ ========================================" -ForegroundColor Cyan
    Write-Host "🏗️ $Message" -ForegroundColor Cyan  
    Write-Host "🏗️ ========================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Step {
    param([string]$Message)
    Write-Host "📋 $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Invoke-BuildStep {
    Write-Header "Building Solution"
    
    Write-Step "Restoring NuGet packages..."
    dotnet restore $RootDir --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Package restore failed"
        exit 1
    }
    Write-Success "Package restore completed"
    
    Write-Step "Building solution in $Configuration mode..."
    $buildArgs = @(
        "build"
        $RootDir
        "--configuration", $Configuration
        "--no-restore"
        "--verbosity", "minimal"
    )
    
    if ($Verbose) {
        $buildArgs[-1] = "normal"
    }
    
    dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed"
        exit 1
    }
    Write-Success "Build completed successfully"
}

function Invoke-TestStep {
    Write-Header "Running Tests"
    
    $testProjects = Get-ChildItem -Path $RootDir -Recurse -Name "*Test*.csproj"
    
    if ($testProjects.Count -eq 0) {
        Write-Warning "No test projects found"
        return
    }
    
    Write-Step "Running unit tests..."
    $testArgs = @(
        "test"
        $RootDir
        "--configuration", $Configuration
        "--no-build"
        "--verbosity", "minimal"
        "--collect", "XPlat Code Coverage"
        "--results-directory", "$RootDir/TestResults"
    )
    
    if ($Verbose) {
        $testArgs[5] = "normal"
    }
    
    dotnet @testArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit 1
    }
    Write-Success "All tests passed"
}

function Invoke-ValidationStep {
    Write-Header "Architecture Validation"
    
    $validationScript = Join-Path $ScriptDir "Validate-Architecture.ps1"
    
    if (-not (Test-Path $validationScript)) {
        Write-Error "Validation script not found: $validationScript"
        exit 1
    }
    
    Write-Step "Running architecture validation..."
    $validationArgs = @{
        ProjectPath = $RootDir
        Fix = $Fix
        Verbose = $Verbose
    }
    
    & $validationScript @validationArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Architecture validation failed"
        exit 1
    }
    Write-Success "Architecture validation passed"
}

function Invoke-CodeAnalysis {
    Write-Header "Code Analysis"
    
    Write-Step "Running code analysis..."
    
    # Run dotnet format for code formatting
    Write-Step "Checking code formatting..."
    $formatArgs = @(
        "format"
        $RootDir
        "--verify-no-changes"
        "--verbosity", "minimal"
    )
    
    if ($Fix) {
        $formatArgs = $formatArgs[0..1] + $formatArgs[3..4]  # Remove --verify-no-changes
        Write-Step "Fixing code formatting..."
    }
    
    dotnet @formatArgs
    if ($LASTEXITCODE -ne 0 -and -not $Fix) {
        Write-Error "Code formatting issues found. Run with -Fix to auto-fix."
        exit 1
    }
    
    if ($Fix) {
        Write-Success "Code formatting applied"
    } else {
        Write-Success "Code formatting validated"
    }
}

function Invoke-SecurityScan {
    Write-Header "Security Scan"
    
    Write-Step "Checking for known vulnerabilities..."
    dotnet list $RootDir package --vulnerable --include-transitive
    
    Write-Step "Checking for deprecated packages..."  
    dotnet list $RootDir package --deprecated
    
    Write-Success "Security scan completed"
}

function Show-Summary {
    Write-Header "Build Summary"
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "📊 Build completed at: $timestamp" -ForegroundColor Green
    Write-Host "📊 Configuration: $Configuration" -ForegroundColor Green
    Write-Host "📊 Target: $Target" -ForegroundColor Green
    
    if (Test-Path "$RootDir/TestResults") {
        $testResults = Get-ChildItem "$RootDir/TestResults" -Recurse -Name "*.xml" | Measure-Object
        Write-Host "📊 Test results: $($testResults.Count) files generated" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Success "✨ All operations completed successfully! ✨"
}

# Main execution
try {
    Write-Header "DDD Clean Architecture Build Pipeline"
    Write-Host "🎯 Target: $Target" -ForegroundColor Magenta
    Write-Host "⚙️ Configuration: $Configuration" -ForegroundColor Magenta
    Write-Host "🛠️ Fix Mode: $Fix" -ForegroundColor Magenta
    Write-Host "📝 Verbose: $Verbose" -ForegroundColor Magenta
    Write-Host "📁 Root Directory: $RootDir" -ForegroundColor Magenta
    
    switch ($Target) {
        "Build" {
            Invoke-BuildStep
        }
        "Test" {
            Invoke-BuildStep
            Invoke-TestStep
        }
        "Validate" {
            Invoke-ValidationStep
            Invoke-CodeAnalysis
        }
        "All" {
            Invoke-BuildStep
            Invoke-TestStep
            Invoke-ValidationStep  
            Invoke-CodeAnalysis
            Invoke-SecurityScan
        }
    }
    
    Show-Summary
}
catch {
    Write-Error "Build failed: $_"
    exit 1
}
