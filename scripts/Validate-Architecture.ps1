# Architecture Validation Script
# Validates DDD + Clean Architecture conventions for .NET projects

param(
    [string]$ProjectPath = ".",
    [switch]$Fix = $false,
    [switch]$Verbose = $false
)

class ValidationRule {
    [string]$Name
    [string]$Category
    [string]$Description
    [string]$Severity
    [scriptblock]$Check
    [scriptblock]$Fix
    
    ValidationRule([string]$name, [string]$category, [string]$description, [string]$severity, [scriptblock]$check, [scriptblock]$fix) {
        $this.Name = $name
        $this.Category = $category
        $this.Description = $description
        $this.Severity = $severity
        $this.Check = $check
        $this.Fix = $fix
    }
}

class ValidationResult {
    [string]$Rule
    [string]$File
    [string]$Message
    [string]$Severity
    [bool]$CanFix
    
    ValidationResult([string]$rule, [string]$file, [string]$message, [string]$severity, [bool]$canFix) {
        $this.Rule = $rule
        $this.File = $file
        $this.Message = $message
        $this.Severity = $severity
        $this.CanFix = $canFix
    }
}

$global:ValidationResults = @()

function Add-ValidationResult {
    param(
        [string]$Rule,
        [string]$File,
        [string]$Message,
        [string]$Severity,
        [bool]$CanFix = $false
    )
    
    $result = [ValidationResult]::new($Rule, $File, $Message, $Severity, $CanFix)
    $global:ValidationResults += $result
    
    if ($Verbose) {
        $color = switch ($Severity) {
            "Error" { "Red" }
            "Warning" { "Yellow" }
            "Info" { "Cyan" }
            default { "White" }
        }
        Write-Host "[$Severity] $Rule - ${File}: $Message" -ForegroundColor $color
    }
}

# ================================
# CLEAN ARCHITECTURE RULES
# ================================

$rules = @()

# Rule: Domain Layer Naming Convention
$rules += [ValidationRule]::new(
    "CA001",
    "Clean Architecture",
    "Domain projects must be named *.Domain",
    "Error",
    {
        param($projectPath)
        $domainProjects = Get-ChildItem -Path $projectPath -Recurse -Name "*.Domain.csproj"
        foreach ($project in $domainProjects) {
            $projectDir = Split-Path $project -Parent
            if (-not ($projectDir -match ".*\.Domain$")) {
                Add-ValidationResult "CA001" $project "Domain project directory should end with .Domain" "Error" $true
            }
        }
    },
    { }
)

# Rule: Application Layer Dependencies
$rules += [ValidationRule]::new(
    "CA002",
    "Clean Architecture",
    "Application layer should only reference Domain layer",
    "Error",
    {
        param($projectPath)
        $appProjects = Get-ChildItem -Path $projectPath -Recurse -Name "*Application*.csproj"
        foreach ($project in $appProjects) {
            $content = Get-Content (Join-Path $projectPath $project) -Raw
            if ($content -match '<ProjectReference.*Infrastructure.*/>') {
                Add-ValidationResult "CA002" $project "Application layer should not reference Infrastructure layer" "Error" $false
            }
            if ($content -match '<ProjectReference.*Api.*/>') {
                Add-ValidationResult "CA002" $project "Application layer should not reference API layer" "Error" $false
            }
        }
    },
    { }
)

# Rule: Domain Layer Dependencies
$rules += [ValidationRule]::new(
    "CA003",
    "Clean Architecture",
    "Domain layer should have no external dependencies except primitives",
    "Error",
    {
        param($projectPath)
        $domainProjects = Get-ChildItem -Path $projectPath -Recurse -Name "*Domain*.csproj"
        foreach ($project in $domainProjects) {
            $content = Get-Content (Join-Path $projectPath $project) -Raw
            if ($content -match '<ProjectReference.*/>') {
                Add-ValidationResult "CA003" $project "Domain layer should not reference other projects" "Error" $false
            }
            # Check for external packages that violate domain purity
            $invalidPackages = @("Microsoft.EntityFrameworkCore", "Newtonsoft.Json", "System.Data.SqlClient")
            foreach ($package in $invalidPackages) {
                if ($content -match "<PackageReference.*$package.*/>") {
                    Add-ValidationResult "CA003" $project "Domain layer should not reference $package" "Error" $false
                }
            }
        }
    },
    { }
)

# ================================
# DDD RULES
# ================================

# Rule: Entity Base Class
$rules += [ValidationRule]::new(
    "DDD001",
    "Domain Driven Design",
    "All entities should inherit from BaseEntity",
    "Warning",
    {
        param($projectPath)
        $entityFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs" | Where-Object { $_ -match "Entities[\\/].*\.cs$" }
        foreach ($file in $entityFiles) {
            $content = Get-Content (Join-Path $projectPath $file) -Raw
            if ($content -match "class\s+\w+\s*(?::\s*(?!BaseEntity))") {
                $className = [regex]::Match($content, "class\s+(\w+)").Groups[1].Value
                if ($className -ne "BaseEntity") {
                    Add-ValidationResult "DDD001" $file "Entity $className should inherit from BaseEntity" "Warning" $true
                }
            }
        }
    },
    { }
)

# Rule: Value Object Naming
$rules += [ValidationRule]::new(
    "DDD002",
    "Domain Driven Design",
    "Value objects should be in ValueObjects folder and inherit from ValueObject",
    "Error",
    {
        param($projectPath)
        $voFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs" | Where-Object { $_ -match "ValueObjects[\\/].*\.cs$" }
        foreach ($file in $voFiles) {
            $content = Get-Content (Join-Path $projectPath $file) -Raw
            if (-not ($content -match ":\s*ValueObject")) {
                Add-ValidationResult "DDD002" $file "Value object should inherit from ValueObject base class" "Error" $true
            }
        }
    },
    { }
)

# Rule: Domain Events
$rules += [ValidationRule]::new(
    "DDD003",
    "Domain Driven Design",
    "Domain events should implement IDomainEvent",
    "Warning",
    {
        param($projectPath)
        $eventFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*Event.cs"
        foreach ($file in $eventFiles) {
            $content = Get-Content (Join-Path $projectPath $file) -Raw
            if (-not ($content -match ":\s*IDomainEvent")) {
                Add-ValidationResult "DDD003" $file "Domain event should implement IDomainEvent" "Warning" $true
            }
        }
    },
    { }
)

# ================================
# FOLDER STRUCTURE RULES
# ================================

# Rule: Clean Architecture Folder Structure
$rules += [ValidationRule]::new(
    "FS001",
    "Folder Structure",
    "Projects should follow Clean Architecture folder structure",
    "Error",
    {
        param($projectPath)
        $requiredFolders = @{
            "Domain" = @("Entities", "ValueObjects", "Common", "Events")
            "Application" = @("Common", "Features", "Commands", "Queries")
            "Infrastructure" = @("Data", "Repositories", "Services")
            "Api" = @("Controllers")
        }
        
        foreach ($layer in $requiredFolders.Keys) {
            $layerProjects = Get-ChildItem -Path $projectPath -Recurse -Directory -Name "*$layer*"
            foreach ($project in $layerProjects) {
                $projectPath = Join-Path $projectPath $project
                foreach ($folder in $requiredFolders[$layer]) {
                    $folderPath = Join-Path $projectPath $folder
                    if (-not (Test-Path $folderPath)) {
                        Add-ValidationResult "FS001" $project "Missing required folder: $folder" "Error" $true
                    }
                }
            }
        }
    },
    { }
)

# ================================
# NAMING CONVENTION RULES
# ================================

# Rule: File Naming Convention
$rules += [ValidationRule]::new(
    "NC001",
    "Naming Convention",
    "C# files should use PascalCase naming",
    "Warning",
    {
        param($projectPath)
        $csFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs"
        foreach ($file in $csFiles) {
            $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file)
            if ($fileName -cnotmatch "^[A-Z][a-zA-Z0-9]*$") {
                Add-ValidationResult "NC001" $file "File name should be PascalCase" "Warning" $true
            }
        }
    },
    { }
)

# Rule: Interface Naming
$rules += [ValidationRule]::new(
    "NC002",
    "Naming Convention",
    "Interfaces should start with 'I' prefix",
    "Error",
    {
        param($projectPath)
        $csFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs"
        foreach ($file in $csFiles) {
            $content = Get-Content (Join-Path $projectPath $file) -Raw
            $interfaces = [regex]::Matches($content, "interface\s+(\w+)")
            foreach ($match in $interfaces) {
                $interfaceName = $match.Groups[1].Value
                if (-not $interfaceName.StartsWith("I")) {
                    Add-ValidationResult "NC002" $file "Interface $interfaceName should start with 'I'" "Error" $true
                }
            }
        }
    },
    { }
)

# ================================
# CQRS RULES  
# ================================

# Rule: Command/Query Segregation
$rules += [ValidationRule]::new(
    "CQRS001",
    "CQRS",
    "Commands and Queries should be in separate folders",
    "Warning",
    {
        param($projectPath)
        $featuresFolders = Get-ChildItem -Path $projectPath -Recurse -Directory -Name "Features"
        foreach ($folder in $featuresFolders) {
            $featuresPath = Join-Path $projectPath $folder
            $subfolders = Get-ChildItem -Path $featuresPath -Directory
            foreach ($subfolder in $subfolders) {
                $commandsPath = Join-Path $subfolder.FullName "Commands"
                $queriesPath = Join-Path $subfolder.FullName "Queries"
                
                if (-not (Test-Path $commandsPath) -and -not (Test-Path $queriesPath)) {
                    Add-ValidationResult "CQRS001" $subfolder.Name "Feature should have Commands or Queries folder" "Warning" $true
                }
            }
        }
    },
    { }
)

# Rule: Command/Query Naming
$rules += [ValidationRule]::new(
    "CQRS002",
    "CQRS",
    "Commands should end with 'Command', Queries with 'Query'",
    "Error",
    {
        param($projectPath)
        $commandFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs" | Where-Object { $_ -match "Commands[\\/]" }
        foreach ($file in $commandFiles) {
            $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file)
            if (-not $fileName.EndsWith("Command") -and -not $fileName.EndsWith("Handler")) {
                Add-ValidationResult "CQRS002" $file "Command file should end with 'Command'" "Error" $true
            }
        }
        
        $queryFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs" | Where-Object { $_ -match "Queries[\\/]" }
        foreach ($file in $queryFiles) {
            $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file)
            if (-not $fileName.EndsWith("Query") -and -not $fileName.EndsWith("Handler")) {
                Add-ValidationResult "CQRS002" $file "Query file should end with 'Query'" "Error" $true
            }
        }
    },
    { }
)

# ================================
# SECURITY RULES
# ================================

# Rule: No Hardcoded Secrets
$rules += [ValidationRule]::new(
    "SEC001",
    "Security",
    "No hardcoded secrets or connection strings",
    "Error",
    {
        param($projectPath)
        $csFiles = Get-ChildItem -Path $projectPath -Recurse -Name "*.cs"
        $secretPatterns = @(
            "password\s*=\s*[""'].+[""']",
            "connectionstring\s*=\s*[""'].+[""']",
            "apikey\s*=\s*[""'].+[""']",
            "secret\s*=\s*[""'].+[""']"
        )
        
        foreach ($file in $csFiles) {
            $content = Get-Content (Join-Path $projectPath $file) -Raw
            foreach ($pattern in $secretPatterns) {
                if ($content -match $pattern) {
                    Add-ValidationResult "SEC001" $file "Potential hardcoded secret found" "Error" $false
                }
            }
        }
    },
    { }
)

# ================================
# MAIN VALIDATION FUNCTION
# ================================

function Invoke-ArchitectureValidation {
    param(
        [string]$ProjectPath,
        [bool]$Fix = $false
    )
    
    Write-Host "🏗️  Architecture Validation Starting..." -ForegroundColor Cyan
    Write-Host "📁 Project Path: $ProjectPath" -ForegroundColor Gray
    Write-Host "🔧 Fix Mode: $Fix" -ForegroundColor Gray
    Write-Host ""
    
    $global:ValidationResults = @()
    
    foreach ($rule in $rules) {
        if ($Verbose) {
            Write-Host "Running rule: $($rule.Name) - $($rule.Description)" -ForegroundColor Gray
        }
        
        try {
            & $rule.Check $ProjectPath
        }
        catch {
            Write-Host "Error running rule $($rule.Name): $_" -ForegroundColor Red
        }
    }
    
    # Group results by severity
    $errors = $global:ValidationResults | Where-Object { $_.Severity -eq "Error" }
    $warnings = $global:ValidationResults | Where-Object { $_.Severity -eq "Warning" }
    $infos = $global:ValidationResults | Where-Object { $_.Severity -eq "Info" }
    
    # Display summary
    Write-Host ""
    Write-Host "📊 VALIDATION SUMMARY" -ForegroundColor Cyan
    Write-Host "===================" -ForegroundColor Cyan
    Write-Host "❌ Errors: $($errors.Count)" -ForegroundColor Red
    Write-Host "⚠️  Warnings: $($warnings.Count)" -ForegroundColor Yellow  
    Write-Host "ℹ️  Info: $($infos.Count)" -ForegroundColor Blue
    Write-Host ""
    
    # Display detailed results
    if ($errors.Count -gt 0) {
        Write-Host "❌ ERRORS:" -ForegroundColor Red
        foreach ($error in $errors) {
            Write-Host "  [$($error.Rule)] $($error.File): $($error.Message)" -ForegroundColor Red
        }
        Write-Host ""
    }
    
    if ($warnings.Count -gt 0) {
        Write-Host "⚠️  WARNINGS:" -ForegroundColor Yellow
        foreach ($warning in $warnings) {
            Write-Host "  [$($warning.Rule)] $($warning.File): $($warning.Message)" -ForegroundColor Yellow
        }
        Write-Host ""
    }
    
    # Exit code based on errors
    if ($errors.Count -gt 0) {
        Write-Host "💥 Validation failed with $($errors.Count) error(s)" -ForegroundColor Red
        exit 1
    }
    elseif ($warnings.Count -gt 0) {
        Write-Host "⚠️  Validation completed with $($warnings.Count) warning(s)" -ForegroundColor Yellow
        exit 0
    }
    else {
        Write-Host "✅ All validations passed!" -ForegroundColor Green
        exit 0
    }
}

# ================================
# EXECUTION
# ================================

Invoke-ArchitectureValidation -ProjectPath $ProjectPath -Fix $Fix
