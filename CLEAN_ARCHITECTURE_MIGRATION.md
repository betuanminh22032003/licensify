# 🔧 Hướng dẫn Restructure Licensify - Clean Architecture

## 🎯 Current Status & Issues

### ❌ Vấn đề hiện tại
1. **Anemic Domain Model** - Entities chỉ có properties, không có business logic
2. **Mixed Responsibilities** - Controllers, Services, Models lẫn lộn trong 1 project
3. **Tight Coupling** - Hard to test và maintain
4. **No Clear Boundaries** - Không phân biệt rõ Infrastructure vs Domain vs Application

### ✅ Solution: Clean Architecture + DDD

## 🏗️ New Structure Implementation

### 1. Đã tạo Clean Architecture Layers

```
AuthService/
├── AuthService.Domain/          ✅ COMPLETED
│   ├── Common/
│   │   ├── BaseEntity.cs
│   │   ├── IDomainEvent.cs
│   │   └── ValueObject.cs
│   ├── Entities/
│   │   ├── User.cs             # Rich Domain Model
│   │   └── RefreshToken.cs
│   ├── ValueObjects/
│   │   ├── Email.cs            # Email validation
│   │   ├── UserId.cs           # Strongly typed ID
│   │   └── UserRole.cs         # Role validation
│   ├── Events/
│   │   └── UserEvents.cs       # Domain Events
│   ├── Interfaces/
│   │   └── IRepositories.cs    # Repository contracts
│   └── Exceptions/
│       └── DomainExceptions.cs # Business exceptions
│
├── AuthService.Application/     🚧 IN PROGRESS
│   ├── Commands/
│   │   └── Auth/
│   │       ├── LoginCommand.cs
│   │       ├── RefreshTokenCommand.cs
│   │       └── RegisterUserCommand.cs
│   ├── Queries/
│   │   └── Users/
│   │       ├── GetUserQuery.cs
│   │       └── GetUsersQuery.cs
│   ├── Handlers/
│   │   ├── CommandHandlers/
│   │   └── QueryHandlers/
│   ├── Validators/
│   │   ├── LoginCommandValidator.cs
│   │   └── RegisterUserCommandValidator.cs
│   ├── Services/
│   │   ├── ITokenService.cs
│   │   └── TokenService.cs
│   └── DTOs/
│       ├── UserDto.cs
│       └── AuthenticationResult.cs
│
├── AuthService.Infrastructure/  📋 TODO
│   ├── Persistence/
│   │   ├── AuthDbContext.cs
│   │   ├── Configurations/
│   │   └── Repositories/
│   ├── ExternalServices/
│   │   ├── RedisService.cs
│   │   └── EmailService.cs
│   └── Configuration/
│
└── AuthService.Api/            📋 TODO
    ├── Controllers/
    ├── Middleware/
    ├── Extensions/
    └── Program.cs
```

## 🔄 Migration Plan

### Phase 1: Complete Application Layer (Today)

```bash
# 1. Tạo Command Handlers
dotnet new class -n LoginCommandHandler -o Commands/Auth/Handlers
dotnet new class -n RefreshTokenCommandHandler -o Commands/Auth/Handlers

# 2. Tạo Query Handlers  
dotnet new class -n GetUserQueryHandler -o Queries/Users/Handlers

# 3. Tạo Validators
dotnet new class -n LoginCommandValidator -o Validators
```

### Phase 2: Infrastructure Layer (Tomorrow)

```bash
# 1. Migrate DbContext
mv ../src/Data/AuthDbContext.cs Infrastructure/Persistence/
mv ../src/Models/* Domain/Entities/

# 2. Create Repositories
# Move repository logic từ Services sang Repository pattern

# 3. Configure EF Core
# Setup proper entity configurations
```

### Phase 3: API Layer (Day 3)

```bash
# 1. Create thin controllers
# Controllers chỉ gọi MediatR commands/queries

# 2. Setup DI Container
# Configure all dependencies properly

# 3. Add Middleware
# Move middleware to proper layer
```

## 📝 Key Changes Made

### 1. Rich Domain Model
**Before (Anemic):**
```csharp
public class User 
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    // Chỉ có properties
}
```

**After (Rich Domain):**
```csharp
public class User : BaseEntity
{
    private User() { } // EF Constructor
    
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    
    // Factory method with business rules
    public static User Create(Email email, string password, ...)
    {
        // Validation logic
        // Business rules
        // Domain events
    }
    
    // Business methods
    public bool VerifyPassword(string password) { }
    public void ChangePassword(string newPassword) { }
}
```

### 2. Value Objects cho Type Safety
```csharp
// Before: Primitive obsession
string email = "invalid-email"; // Runtime error!

// After: Compile-time safety
Email email = Email.Create("test@example.com"); // Validated!
UserId userId = UserId.NewId(); // Strongly typed
```

### 3. CQRS với MediatR
```csharp
// Before: Fat controllers
[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    // 50 lines of logic in controller
}

// After: Thin controllers
[HttpPost("login")]
public async Task<IActionResult> Login(LoginCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### 4. Domain Events
```csharp
// Automatically publish events when domain changes
public static User Create(...)
{
    var user = new User { ... };
    user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email));
    return user;
}
```

## 🚀 Benefits for Team

### For Junior Developers:
- ✅ **Clear structure** - Biết đặt code ở đâu
- ✅ **Type safety** - Compile-time errors thay vì runtime
- ✅ **Guided development** - Framework guide cách code
- ✅ **Better testing** - Dễ mock và test

### For Mid-level Developers:
- ✅ **SOLID principles** - Dependency inversion, SRP
- ✅ **Design patterns** - Repository, CQRS, Domain Events
- ✅ **Clean separation** - Business logic tách khỏi infrastructure
- ✅ **Maintainable** - Easy to refactor và extend

### For Senior Developers:
- ✅ **Strategic design** - Bounded contexts, Aggregates
- ✅ **Event-driven** - Domain events for microservices
- ✅ **Technology agnostic** - Swap database/framework easily
- ✅ **Scalable architecture** - Each layer scales independently

## 📋 Next Actions for Team

### Immediate (Today):
1. **Review** architecture guide document
2. **Study** existing Domain layer implementation
3. **Complete** Application layer commands/queries
4. **Test** new structure với simple scenarios

### This Week:
1. **Migrate** existing AuthService logic
2. **Apply** same pattern cho ProductService
3. **Create** Infrastructure layer với proper repositories
4. **Setup** comprehensive testing

### Next Sprint:
1. **Implement** LicenseService với new architecture
2. **Add** AuditService với event-driven pattern
3. **Create** integration tests
4. **Document** team coding standards

## 🔍 Code Review Checklist

### Domain Layer ✅
- [ ] Entities have private setters
- [ ] Business logic in domain methods
- [ ] Value objects for primitives
- [ ] Domain events for state changes
- [ ] No infrastructure dependencies

### Application Layer 🚧
- [ ] Commands for write operations
- [ ] Queries for read operations
- [ ] Handlers are thin coordinators
- [ ] Validators for input validation
- [ ] DTOs for data transfer

### Infrastructure Layer 📋
- [ ] Repository implementations
- [ ] EF Core configurations
- [ ] External service integrations
- [ ] No business logic

### API Layer 📋
- [ ] Thin controllers
- [ ] MediatR integration
- [ ] Proper error handling
- [ ] Swagger documentation

---

**💡 Key Message cho Team:**

> "Chúng ta đang chuyển từ **Transaction Script** pattern (logic trong services) sang **Rich Domain Model** pattern (logic trong domain entities). Điều này giúp code dễ hiểu, test, và maintain hơn rất nhiều!"

**🎯 Goal:** Mỗi developer đều có thể contribute vào codebase một cách consistent và hiệu quả!
