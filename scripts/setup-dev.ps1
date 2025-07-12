#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Setup development environment for DDD Clean Architecture project
.DESCRIPTION
    This script sets up the development environment with all necessary tools and configurations
.PARAMETER InstallTools
    Install development tools (dotnet-format, dotnet-ef, etc.)
.PARAMETER SetupGitHooks
    Setup Git hooks for pre-commit validation
.PARAMETER UpdatePackages
    Update all NuGet packages to latest versions
.PARAMETER Force
    Force reinstallation of tools
.PARAMETER SetupInfrastructure
    Setup Docker infrastructure services
#>

param(
    [Parameter()]
    [switch]$InstallTools = $false,
    
    [Parameter()]
    [switch]$SetupGitHooks = $false,
    
    [Parameter()]
    [switch]$UpdatePackages = $false,
    
    [Parameter()]
    [switch]$Force = $false,
    
    [Parameter()]
    [switch]$SetupInfrastructure = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RootDir = Split-Path -Parent $ScriptDir

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "🔧 ========================================" -ForegroundColor Cyan
    Write-Host "🔧 $Message" -ForegroundColor Cyan  
    Write-Host "🔧 ========================================" -ForegroundColor Cyan
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

function Test-DotNetTool {
    param([string]$ToolName)
    
    try {
        $result = dotnet tool list --global | Select-String $ToolName
        return $result -ne $null
    }
    catch {
        return $false
    }
}

function Install-DotNetTools {
    Write-Header "Installing .NET Tools"
    
    $tools = @(
        @{Name = "dotnet-format"; Package = "dotnet-format"; Description = "Code formatter" },
        @{Name = "dotnet-ef"; Package = "dotnet-ef"; Description = "Entity Framework Core tools" },
        @{Name = "dotnet-outdated"; Package = "dotnet-outdated-tool"; Description = "Package update checker" },
        @{Name = "dotnet-reportgenerator"; Package = "dotnet-reportgenerator-globaltool"; Description = "Coverage report generator" },
        @{Name = "dotnet-stryker"; Package = "dotnet-stryker"; Description = "Mutation testing" }
    )
    
    foreach ($tool in $tools) {
        Write-Step "Checking $($tool.Description)..."
        
        if (Test-DotNetTool $tool.Name) {
            if ($Force) {
                Write-Step "Reinstalling $($tool.Name)..."
                dotnet tool uninstall $tool.Package --global
                dotnet tool install $tool.Package --global
            } else {
                Write-Success "$($tool.Name) is already installed"
            }
        } else {
            Write-Step "Installing $($tool.Name)..."
            dotnet tool install $tool.Package --global
        }
    }
    
    Write-Success "All .NET tools installed successfully"
}

function Setup-GitHooks {
    Write-Header "Setting up Git Hooks"
    
    $gitHooksDir = Join-Path $RootDir ".git/hooks"
    
    if (-not (Test-Path $gitHooksDir)) {
        Write-Warning "Git repository not found. Initialize git first."
        return
    }
    
    # Pre-commit hook
    $preCommitHook = @'
#!/bin/sh
# Pre-commit hook for DDD Clean Architecture validation

echo "🔍 Running pre-commit validation..."

# Run architecture validation
pwsh -File ./scripts/Validate-Architecture.ps1 -ProjectPath . -Verbose

if [ $? -ne 0 ]; then
    echo "❌ Architecture validation failed!"
    exit 1
fi

# Run code formatting check
dotnet format --verify-no-changes --verbosity minimal

if [ $? -ne 0 ]; then
    echo "❌ Code formatting issues found!"
    echo "💡 Run 'dotnet format' to fix formatting issues"
    exit 1
fi

echo "✅ Pre-commit validation passed!"
'@
    
    $preCommitPath = Join-Path $gitHooksDir "pre-commit"
    Write-Step "Creating pre-commit hook..."
    Set-Content -Path $preCommitPath -Value $preCommitHook -Encoding UTF8
    
    # Make executable on Unix systems
    if ($IsLinux -or $IsMacOS) {
        chmod +x $preCommitPath
    }
    
    # Pre-push hook
    $prePushHook = @'
#!/bin/sh
# Pre-push hook for comprehensive testing

echo "🧪 Running pre-push validation..."

# Run full build and test
pwsh -File ./scripts/build.ps1 -Target All -Configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Build or tests failed!"
    exit 1
fi

echo "✅ Pre-push validation passed!"
'@
    
    $prePushPath = Join-Path $gitHooksDir "pre-push"
    Write-Step "Creating pre-push hook..."
    Set-Content -Path $prePushPath -Value $prePushHook -Encoding UTF8
    
    # Make executable on Unix systems
    if ($IsLinux -or $IsMacOS) {
        chmod +x $prePushPath
    }
    
    Write-Success "Git hooks configured successfully"
}

function Update-Packages {
    Write-Header "Updating NuGet Packages"
    
    if (-not (Test-DotNetTool "dotnet-outdated")) {
        Write-Warning "dotnet-outdated tool not found. Installing..."
        dotnet tool install dotnet-outdated-tool --global
    }
    
    Write-Step "Checking for outdated packages..."
    dotnet outdated $RootDir
    
    Write-Step "Updating packages..."
    dotnet outdated $RootDir --upgrade
    
    Write-Success "Package update completed"
}

function Setup-Infrastructure {
    Write-Header "Setting up Infrastructure Services"
    
    # Check if Docker is running
    try {
        docker info | Out-Null
        Write-Success "Docker is running"
    } catch {
        Write-Error "Docker is not running. Please start Docker and try again."
        exit 1
    }
    
    # Check if .NET 8 is installed
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -match "^8\.") {
        Write-Success ".NET 8 SDK is installed: $dotnetVersion"
    } else {
        Write-Error ".NET 8 SDK is not installed. Please install .NET 8 SDK and try again."
        exit 1
    }
    
    # Create .env file from example if it doesn't exist
    $envFile = Join-Path $RootDir ".env"
    $envExample = Join-Path $RootDir ".env.example"
    
    if (-not (Test-Path $envFile) -and (Test-Path $envExample)) {
        Write-Step "Creating .env file from example..."
        Copy-Item $envExample $envFile
        Write-Success ".env file created. Please update the values as needed."
    }
    
    # Start infrastructure services
    $dockerCompose = Join-Path $RootDir "docker-compose.dev.yml"
    if (Test-Path $dockerCompose) {
        Write-Step "Starting infrastructure services (PostgreSQL, Redis, RabbitMQ, MongoDB)..."
        docker-compose -f $dockerCompose up -d
        
        Write-Step "Waiting for services to be ready..."
        Start-Sleep -Seconds 10
        
        Write-Success "Infrastructure services started"
    } else {
        Write-Warning "docker-compose.dev.yml not found. Skipping infrastructure setup."
    }
}

function Setup-VSCodeSettings {
    Write-Header "Setting up VS Code Configuration"
    
    $vscodeDir = Join-Path $RootDir ".vscode"
    if (-not (Test-Path $vscodeDir)) {
        New-Item -Path $vscodeDir -ItemType Directory -Force | Out-Null
    }
    
    # Settings.json
    $settings = @{
        "dotnet.defaultSolution" = "Licensify.sln"
        "omnisharp.enableEditorConfigSupport" = $true
        "omnisharp.enableRoslynAnalyzers" = $true
        "editor.formatOnSave" = $true
        "editor.formatOnPaste" = $true
        "editor.codeActionsOnSave" = @{
            "source.fixAll" = $true
            "source.organizeImports" = $true
        }
        "files.exclude" = @{
            "**/bin" = $true
            "**/obj" = $true
            "**/.vs" = $true
            "**/TestResults" = $true
        }
        "dotnet.completion.showCompletionItemsFromUnimportedNamespaces" = $true
        "dotnet.codeLens.enableReferencesCodeLens" = $true
        "dotnet.unitTests.runSettingsPath" = "./runsettings.xml"
    }
    
    $settingsPath = Join-Path $vscodeDir "settings.json"
    $settings | ConvertTo-Json -Depth 10 | Set-Content -Path $settingsPath -Encoding UTF8
    
    # Extensions.json
    $extensions = @{
        "recommendations" = @(
            "ms-dotnettools.csharp",
            "ms-dotnettools.vscode-dotnet-runtime",
            "editorconfig.editorconfig",
            "formulahendry.dotnet-test-explorer",
            "patcx.vscode-nuget-gallery",
            "kreativ-software.csharpextensions",
            "jchannon.csharpextensions",
            "ms-vscode.powershell"
        )
    }
    
    $extensionsPath = Join-Path $vscodeDir "extensions.json"
    $extensions | ConvertTo-Json -Depth 10 | Set-Content -Path $extensionsPath -Encoding UTF8
    
    # Tasks.json
    $tasks = @{
        "version" = "2.0.0"
        "tasks" = @(
            @{
                "label" = "build"
                "command" = "dotnet"
                "type" = "process"
                "args" = @("build")
                "problemMatcher" = "`$msCompile"
                "group" = @{
                    "kind" = "build"
                    "isDefault" = $true
                }
            },
            @{
                "label" = "test"
                "command" = "dotnet"
                "type" = "process"
                "args" = @("test")
                "problemMatcher" = "`$msCompile"
                "group" = "test"
            },
            @{
                "label" = "validate-architecture"
                "command" = "pwsh"
                "type" = "process"
                "args" = @("-File", "./scripts/Validate-Architecture.ps1", "-ProjectPath", ".", "-Verbose")
                "problemMatcher" = []
                "group" = "test"
            },
            @{
                "label" = "format-code"
                "command" = "dotnet"
                "type" = "process"
                "args" = @("format")
                "problemMatcher" = []
                "group" = "build"
            }
        )
    }
    
    $tasksPath = Join-Path $vscodeDir "tasks.json"
    $tasks | ConvertTo-Json -Depth 10 | Set-Content -Path $tasksPath -Encoding UTF8
    
    Write-Success "VS Code configuration completed"
}

function Show-Summary {
    Write-Header "Setup Summary"
    
    Write-Host "🎉 Development environment setup completed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📚 What's been configured:" -ForegroundColor Cyan
    Write-Host "  • .NET development tools" -ForegroundColor White
    Write-Host "  • Git hooks for validation" -ForegroundColor White
    Write-Host "  • VS Code settings and tasks" -ForegroundColor White
    Write-Host "  • Code quality analyzers" -ForegroundColor White
    Write-Host ""
    Write-Host "🚀 Next steps:" -ForegroundColor Cyan
    Write-Host "  • Run: ./scripts/build.ps1 -Target All" -ForegroundColor White
    Write-Host "  • Run: ./scripts/Validate-Architecture.ps1 -ProjectPath ." -ForegroundColor White
    Write-Host "  • Open project in VS Code" -ForegroundColor White
    Write-Host ""
    
    if ($SetupInfrastructure) {
        Write-Host "📊 Service URLs:" -ForegroundColor Cyan
        Write-Host "  • Auth Service: http://localhost:5001" -ForegroundColor White
        Write-Host "  • Product Service: http://localhost:5002" -ForegroundColor White
        Write-Host "  • PostgreSQL: localhost:5432" -ForegroundColor White
        Write-Host "  • Redis: localhost:6379" -ForegroundColor White
        Write-Host "  • RabbitMQ Management: http://localhost:15672 (admin/admin123)" -ForegroundColor White
        Write-Host ""
    }
}

# Main execution
try {
    Write-Header "DDD Clean Architecture Development Setup"
    
    if ($InstallTools) {
        Install-DotNetTools
    }
    
    if ($SetupGitHooks) {
        Setup-GitHooks
    }
    
    if ($UpdatePackages) {
        Update-Packages
    }
    
    if ($SetupInfrastructure) {
        Setup-Infrastructure
    }
    
    Setup-VSCodeSettings
    
    Show-Summary
}
catch {
    Write-Error "Setup failed: $_"
    exit 1
}
