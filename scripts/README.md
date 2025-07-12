# 🛠️ Development Scripts

This folder contains PowerShell scripts to help set up, validate, and maintain the DDD Clean Architecture codebase.

## 📋 Available Scripts

### 🔧 setup-dev.ps1
Sets up the complete development environment with tools, configuration, and infrastructure.

```powershell
# Full setup with all features
.\setup-dev.ps1 -InstallTools -SetupGitHooks -SetupInfrastructure

# Install development tools only
.\setup-dev.ps1 -InstallTools

# Setup Git hooks for validation
.\setup-dev.ps1 -SetupGitHooks

# Update all NuGet packages
.\setup-dev.ps1 -UpdatePackages

# Force reinstall tools
.\setup-dev.ps1 -InstallTools -Force
```

**Features:**
- 🔨 Installs .NET development tools (dotnet-format, dotnet-ef, etc.)
- 🎣 Sets up Git hooks for pre-commit/pre-push validation
- 📦 Updates NuGet packages to latest versions
- 🐳 Configures Docker infrastructure services
- ⚙️ Creates VS Code settings and tasks
- 📝 Configures EditorConfig and analyzers

### 🏗️ build.ps1
Comprehensive build pipeline with validation, testing, and code analysis.

```powershell
# Run complete build pipeline
.\build.ps1 -Target All

# Build only
.\build.ps1 -Target Build

# Build and test
.\build.ps1 -Target Test

# Validation and code analysis only
.\build.ps1 -Target Validate

# Release build with auto-fix
.\build.ps1 -Target All -Configuration Release -Fix

# Verbose output
.\build.ps1 -Target All -Verbose
```

**Pipeline Steps:**
- 🔨 **Build**: Restore packages and compile solution
- 🧪 **Test**: Run unit tests with coverage
- ✅ **Validate**: Architecture and DDD validation
- 🎨 **Code Analysis**: Formatting and style checks
- 🔒 **Security**: Vulnerability and deprecation scan

### 🏛️ Validate-Architecture.ps1
Validates DDD and Clean Architecture conventions across the entire codebase.

```powershell
# Validate current project
.\Validate-Architecture.ps1 -ProjectPath .

# Auto-fix issues where possible
.\Validate-Architecture.ps1 -ProjectPath . -Fix

# Verbose output with detailed explanations
.\Validate-Architecture.ps1 -ProjectPath . -Verbose

# Validate specific service
.\Validate-Architecture.ps1 -ProjectPath .\services\auth-service
```

**Validation Rules:**
- 🏗️ **Clean Architecture** (CA001-CA003): Layer separation, dependency flow
- 🎯 **DDD** (DDD001-DDD003): Entity validation, value objects, domain events
- 📁 **Folder Structure** (FS001): Project organization
- 🏷️ **Naming Conventions** (NC001-NC002): File and class naming
- ⚡ **CQRS** (CQRS001-CQRS002): Command/Query separation
- 🔒 **Security** (SEC001): Security best practices

## 🎯 Common Workflows

### 🆕 Setting Up New Development Environment

```powershell
# 1. Clone repository and navigate to project
git clone <repository-url>
cd Licensify

# 2. Run full setup
.\scripts\setup-dev.ps1 -InstallTools -SetupGitHooks -SetupInfrastructure

# 3. Validate everything is working
.\scripts\build.ps1 -Target All

# 4. Open in VS Code
code .
```

### 🔄 Daily Development Workflow

```powershell
# Before starting work - validate current state
.\scripts\Validate-Architecture.ps1 -ProjectPath .

# During development - quick build and test
.\scripts\build.ps1 -Target Test

# Before committing - full validation
.\scripts\build.ps1 -Target All -Fix

# Update packages weekly
.\scripts\setup-dev.ps1 -UpdatePackages
```

### 🐛 Troubleshooting Build Issues

```powershell
# 1. Check architecture compliance
.\scripts\Validate-Architecture.ps1 -ProjectPath . -Verbose

# 2. Fix formatting issues
.\scripts\build.ps1 -Target Validate -Fix

# 3. Clean build
dotnet clean
.\scripts\build.ps1 -Target Build

# 4. Update tools if needed
.\scripts\setup-dev.ps1 -InstallTools -Force
```

### 🔧 CI/CD Integration

```powershell
# In CI pipeline - strict validation
.\scripts\build.ps1 -Target All -Configuration Release

# In pre-commit hook (automatically set up)
.\scripts\Validate-Architecture.ps1 -ProjectPath . -Verbose
dotnet format --verify-no-changes

# In pre-push hook (automatically set up)  
.\scripts\build.ps1 -Target All -Configuration Release
```

## 📊 Code Quality Tools Integration

The scripts integrate with multiple code quality tools:

- **EditorConfig**: File formatting standards
- **StyleCop**: C# code style analysis  
- **Microsoft.CodeAnalysis.NetAnalyzers**: .NET best practices
- **dotnet-format**: Automatic code formatting
- **Custom Rulesets**: DDD and Clean Architecture specific rules

## 🎨 VS Code Integration

After running `setup-dev.ps1`, VS Code will be configured with:

- **Tasks**: Build, test, validate, format
- **Settings**: Auto-format on save, analyzer integration
- **Extensions**: Recommended C# and .NET extensions
- **Debugging**: Launch configurations for services

## 🎣 Git Hooks

Git hooks are automatically configured to run:

- **Pre-commit**: Architecture validation + code formatting
- **Pre-push**: Full build pipeline including tests

## 🔧 Tool Requirements

- **PowerShell 5.1+** or **PowerShell Core 6+**
- **.NET 8 SDK**
- **Docker** (for infrastructure setup)
- **Git** (for hooks setup)

## 📝 Configuration Files

The scripts create and maintain these configuration files:

- `.editorconfig` - File formatting standards
- `.globalconfig` - .NET code analysis rules
- `DDD.CleanArchitecture.ruleset` - StyleCop rules
- `Directory.Build.props` - MSBuild configuration
- `.vscode/settings.json` - VS Code settings
- `.vscode/tasks.json` - VS Code tasks
- `.vscode/extensions.json` - Recommended extensions

## 🎯 Best Practices

1. **Run validation before committing** - Catch issues early
2. **Use auto-fix for formatting** - Maintain consistency
3. **Keep tools updated** - Run setup script regularly
4. **Follow naming conventions** - Enable better validation
5. **Write tests** - Maintain quality standards
6. **Document changes** - Update architecture docs

## 🆘 Support

If you encounter issues:

1. Check script output for specific error messages
2. Ensure all prerequisites are installed
3. Try running with `-Verbose` for detailed information
4. Run with `-Force` to reinstall tools if needed
5. Check VS Code problems panel for analyzer warnings

---

💡 **Tip**: These scripts implement a "shift-left" approach to quality, catching issues early in the development process rather than in CI/CD or production.
