# 🏗️ Licensify Architecture Guide - Clean Architecture + DDD

## 📋 Tổng quan kiến trúc

Licensify được xây dựng theo **Clean Architecture** kết hợp với **Domain Driven Design (DDD)** để đảm bảo:
- ✅ **Separation of Concerns** - Tách biệt rõ ràng các layer
- ✅ **Testability** - Dễ dàng unit test
- ✅ **Maintainability** - Dễ bảo trì và mở rộng
- ✅ **Independence** - Không phụ thuộc vào framework hay database cụ thể

## 🎯 Kiến trúc tổng thể

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLEAN ARCHITECTURE LAYERS                    │
├─────────────────────────────────────────────────────────────────┤
│  🌐 Presentation Layer (API Controllers)                       │
│      ├── Controllers/                                          │
│      ├── DTOs/ (Request/Response)                              │
│      └── Middleware/                                           │
├─────────────────────────────────────────────────────────────────┤
│  📋 Application Layer (Use Cases / Business Logic)             │
│      ├── Services/ (Application Services)                      │
│      ├── Interfaces/ (Service Contracts)                       │
│      ├── Commands/ (CQRS Commands)                             │
│      ├── Queries/ (CQRS Queries)                               │
│      └── Validators/ (FluentValidation)                        │
├─────────────────────────────────────────────────────────────────┤
│  🏛️ Domain Layer (Business Logic Core)                         │
│      ├── Entities/ (Domain Models)                             │
│      ├── ValueObjects/ (Value Objects)                         │
│      ├── Interfaces/ (Repository Contracts)                    │
│      ├── Events/ (Domain Events)                               │
│      └── Exceptions/ (Domain Exceptions)                       │
├─────────────────────────────────────────────────────────────────┤
│  💾 Infrastructure Layer (External Dependencies)               │
│      ├── Persistence/ (EF Core, Repositories)                  │
│      ├── ExternalServices/ (Redis, RabbitMQ)                   │
│      ├── Configuration/ (Settings, Extensions)                 │
│      └── Migrations/ (Database Migrations)                     │
└─────────────────────────────────────────────────────────────────┘
```

## 📁 Structure theo từng Service

### Auth Service Structure
```
services/auth-service/
├── src/
│   ├── AuthService.Api/                    # 🌐 Presentation Layer
│   │   ├── Controllers/
│   │   │   └── AuthController.cs
│   │   ├── DTOs/
│   │   │   ├── Requests/
│   │   │   └── Responses/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── AuthService.Application/            # 📋 Application Layer
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── ITokenService.cs
│   │   │   └── TokenService.cs
│   │   ├── Commands/
│   │   │   ├── LoginCommand.cs
│   │   │   ├── RefreshTokenCommand.cs
│   │   │   └── RegisterUserCommand.cs
│   │   ├── Queries/
│   │   │   ├── GetUserQuery.cs
│   │   │   └── GetUsersQuery.cs
│   │   ├── Validators/
│   │   │   ├── LoginCommandValidator.cs
│   │   │   └── RegisterUserCommandValidator.cs
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs
│   │       └── IRefreshTokenRepository.cs
│   │
│   ├── AuthService.Domain/                 # 🏛️ Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   └── RefreshToken.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   └── Password.cs
│   │   ├── Events/
│   │   │   ├── UserLoggedInEvent.cs
│   │   │   └── UserRegisteredEvent.cs
│   │   ├── Interfaces/
│   │   │   ├── IUserRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Exceptions/
│   │       ├── InvalidCredentialsException.cs
│   │       └── UserNotFoundException.cs
│   │
│   └── AuthService.Infrastructure/         # 💾 Infrastructure Layer
│       ├── Persistence/
│       │   ├── AuthDbContext.cs
│       │   ├── Repositories/
│       │   │   ├── UserRepository.cs
│       │   │   └── RefreshTokenRepository.cs
│       │   ├── Configurations/
│       │   │   ├── UserConfiguration.cs
│       │   │   └── RefreshTokenConfiguration.cs
│       │   └── Migrations/
│       ├── ExternalServices/
│       │   ├── RedisService.cs
│       │   └── EmailService.cs
│       └── Configuration/
│           ├── ServiceCollectionExtensions.cs
│           └── DatabaseExtensions.cs
│
└── tests/
    ├── AuthService.UnitTests/
    ├── AuthService.IntegrationTests/
    └── AuthService.ArchitectureTests/
```

## 🔧 Implementation Rules

### 1. Dependency Rules
```
Presentation → Application → Domain ← Infrastructure
```
- **Domain** không phụ thuộc vào layer nào khác
- **Application** chỉ phụ thuộc vào Domain
- **Infrastructure** implement interfaces từ Domain
- **Presentation** phụ thuộc vào Application

### 2. Entity Rules (Domain Layer)
```csharp
// ✅ ĐÚNG - Rich Domain Model
public class User : BaseEntity
{
    private User() { } // Private constructor for EF
    
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    
    // Factory method
    public static User Create(Email email, Password password, UserRole role)
    {
        var user = new User
        {
            Id = UserId.NewId(),
            Email = email,
            Password = password,
            Role = role,
            IsActive = true
        };
        
        // Domain event
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email));
        return user;
    }
    
    // Business logic
    public void ChangePassword(Password newPassword)
    {
        if (!IsActive)
            throw new InactiveUserException();
            
        Password = newPassword;
        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }
}

// ❌ SAI - Anemic Domain Model
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    // Chỉ có properties, không có business logic
}
```

### 3. Value Object Rules
```csharp
// ✅ Email Value Object
public class Email : ValueObject
{
    public string Value { get; private set; }
    
    private Email(string value)
    {
        Value = value;
    }
    
    public static Email Create(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be empty");
            
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");
            
        return new Email(email.ToLowerInvariant());
    }
    
    private static bool IsValidEmail(string email)
    {
        // Email validation logic
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### 4. Repository Pattern
```csharp
// Domain Interface
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User?> GetByEmailAsync(Email email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
}

// Infrastructure Implementation
public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    
    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    // Implementation...
}
```

### 5. CQRS Pattern
```csharp
// Command (Write Operations)
public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null || !user.VerifyPassword(request.Password))
            throw new InvalidCredentialsException();
            
        var token = await _tokenService.GenerateTokenAsync(user);
        return new LoginResponse(token, user.Email.Value);
    }
}

// Query (Read Operations)
public class GetUserQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(request.UserId);
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new UserNotFoundException();
            
        return UserDto.FromDomain(user);
    }
}
```

### 6. Application Service Pattern
```csharp
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginCommand command);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand command);
    Task LogoutAsync(LogoutCommand command);
}

public class AuthService : IAuthService
{
    private readonly IMediator _mediator;
    
    public AuthService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<LoginResponse> LoginAsync(LoginCommand command)
    {
        return await _mediator.Send(command);
    }
}
```

## 🚀 Migration Plan

### Step 1: Restructure Auth Service
```bash
# 1. Tạo các project mới
dotnet new classlib -n AuthService.Domain
dotnet new classlib -n AuthService.Application  
dotnet new classlib -n AuthService.Infrastructure
dotnet new web -n AuthService.Api

# 2. Add references
dotnet add AuthService.Application/AuthService.Application.csproj reference AuthService.Domain/AuthService.Domain.csproj
dotnet add AuthService.Infrastructure/AuthService.Infrastructure.csproj reference AuthService.Domain/AuthService.Domain.csproj
dotnet add AuthService.Api/AuthService.Api.csproj reference AuthService.Application/AuthService.Application.csproj
dotnet add AuthService.Api/AuthService.Api.csproj reference AuthService.Infrastructure/AuthService.Infrastructure.csproj

# 3. Add to solution
dotnet sln add AuthService.Domain/AuthService.Domain.csproj
dotnet sln add AuthService.Application/AuthService.Application.csproj
dotnet sln add AuthService.Infrastructure/AuthService.Infrastructure.csproj
dotnet sln add AuthService.Api/AuthService.Api.csproj
```

### Step 2: Move existing code
1. **Domain Layer**: Move entities, value objects
2. **Application Layer**: Move services, interfaces
3. **Infrastructure Layer**: Move DbContext, repositories
4. **API Layer**: Move controllers, DTOs

### Step 3: Apply patterns
1. **Implement CQRS** với MediatR
2. **Add FluentValidation** cho commands
3. **Implement Domain Events**
4. **Add Unit of Work pattern**

## 📋 Benefits của kiến trúc này

### For Junior Developers:
- ✅ **Clear structure** - Biết rõ code đặt ở đâu
- ✅ **Separation** - Mỗi layer có trách nhiệm riêng
- ✅ **Testable** - Dễ dàng viết unit test
- ✅ **Reusable** - Logic có thể tái sử dụng

### For Mid-level Developers:
- ✅ **Maintainable** - Dễ bảo trì và refactor
- ✅ **Extensible** - Dễ thêm feature mới
- ✅ **Best practices** - Theo chuẩn industry
- ✅ **Performance** - Optimize được từng layer

### For Senior Developers:
- ✅ **Scalable** - Scale được từng component
- ✅ **Technology independent** - Đổi tech stack dễ dàng
- ✅ **Domain-focused** - Tập trung vào business logic
- ✅ **Event-driven** - Support microservices communication

## 🔄 Next Actions

1. **Restructure Auth Service** theo Clean Architecture
2. **Apply CQRS pattern** với MediatR
3. **Implement Domain Events** 
4. **Add comprehensive testing**
5. **Apply same pattern cho Product/License/Audit services**

---

**💡 Lưu ý**: Việc refactor này sẽ mất 1-2 ngày nhưng sẽ tạo foundation vững chắc cho toàn bộ team development!
