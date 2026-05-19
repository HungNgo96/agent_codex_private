---
name: backend-patterns-dotnet
description: >-
  Backend architecture patterns for .NET applications including repository pattern, service layer,
  caching strategies, authentication, rate limiting, background jobs, and logging. Use when
  implementing ASP.NET Core services, designing data access layers, or building API middleware.
---

# Backend Development Patterns (.NET)

Backend architecture patterns and best practices for scalable server-side applications using .NET.

## When to Activate

- Designing REST APIs with ASP.NET Core
- Implementing repository, service, or controller layers
- Optimizing database queries (N+1, indexing, connection pooling)
- Adding caching (IMemoryCache, Redis)
- Setting up background jobs or async processing
- Structuring error handling and validation for APIs
- Building middleware (auth, logging, rate limiting)

## Repository Pattern with EF Core

### Base Repository Interface

```csharp
public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}
```

### EF Core Implementation

```csharp
public sealed class EfRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public EfRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _context.Set<T>()
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}
```

### Specification Pattern for Complex Queries

```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}

public class UserByEmailSpec : ISpecification<User>
{
    public Expression<Func<User, bool>> Criteria => u => u.Email == Email;
    public List<Expression<Func<User, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<User, object>>? OrderBy => null;
    public Expression<Func<User, object>>? OrderByDescending => u => u.CreatedAt;
    public int Take => 1;
    public int Skip => 0;
    public bool IsPagingEnabled => false;

    public UserByEmailSpec(string email) => Email = email;
    public string Email { get; }
}
```

## Service Layer Pattern

```csharp
public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<UserDto>> CreateAsync(
        CreateUserRequest request,
        CancellationToken ct = default)
    {
        if (await _repository.ExistsAsync(u => u.Email == request.Email, ct))
            return Result<UserDto>.Failure("Email already exists");

        var user = User.Create(request.Name, request.Email);
        await _repository.AddAsync(user, ct);

        _logger.LogInformation("User created: {UserId}", user.Id);

        return Result<UserDto>.Success(user.ToDto());
    }
}
```

## Result Pattern

```csharp
public readonly struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public readonly struct Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }

    private Result(T? value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value, true, null);
    public static Result<T> Failure(string error) => new(default, false, error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);
}
```

## Error Handling Patterns

### Global Exception Handler

```csharp
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound,
                new ApiError("not_found", ex.Message)),
            ValidationException ex => (StatusCodes.Status422UnprocessableEntity,
                new ApiError("validation_error", ex.Message, ex.Errors)),
            _ => (StatusCodes.Status500InternalServerError,
                new ApiError("internal_error", "An unexpected error occurred"))
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}

// Registration in Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

### Retry with Polly

```csharp
// Add Polly
// dotnet add package Microsoft.Extensions.Http.Polly

builder.Services.AddHttpClient<IExternalApi, ExternalApi>()
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Or with resilience pipeline
builder.Services.AddHttpClient<IExternalApi, ExternalApi>()
    .AddResiliencePipeline("default", pipeline =>
    {
        pipeline.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2),
            BackoffType = DelayBackoffType.Exponential
        });
        pipeline.AddTimeout(TimeSpan.FromSeconds(30));
    });
```

## Caching Strategies

### IMemoryCache

```csharp
public interface ICachedRepository<T>
{
    Task<T?> GetOrCreateAsync(string key, Func<Task<T?>> factory, TimeSpan expiration);
    void Invalidate(string key);
}

public sealed class CachedUserRepository : ICachedRepository<User>
{
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;

    public CachedUserRepository(IUserRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<User?> GetOrCreateAsync(
        string key,
        Func<Task<User?>> factory,
        TimeSpan expiration)
    {
        if (_cache.TryGetValue(key, out User? cached))
            return cached;

        var user = await factory();
        if (user is not null)
        {
            _cache.Set(key, user, expiration);
        }
        return user;
    }

    public void Invalidate(string key) => _cache.Remove(key);
}
```

### Response Caching

```csharp
// Program.cs
builder.Services.AddResponseCaching();

// Endpoint
app.MapGet("/api/users", async (IUserRepository repository) =>
{
    var users = await repository.GetAllAsync();
    return Results.Ok(users);
})
.CacheOutput("users")
.WithTags("Users");

// Or with explicit cache headers
app.MapGet("/api/products", async (IProductRepository repository) =>
{
    Response.Headers.Append("Cache-Control", "public, max-age=300");
    return Results.Ok(await repository.GetAllAsync());
});
```

## Authentication & Authorization

### JWT Configuration

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .AddPolicy("ResourceOwner", p => p.RequireAssertion(context =>
        context.Resource is HttpContext httpContext &&
        httpContext.User.HasClaim("userId", httpContext.Request.RouteValues["id"]?.ToString())));
```

### Claims Transformation

```csharp
public class CustomClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        
        // Add custom claims from database
        var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            identity.AddClaim(new Claim("permissions", "read,write"));
        }

        return Task.FromResult(principal);
    }
}
```

## Rate Limiting

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress!.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ApiError("rate_limit", "Too many requests. Please try again later."),
            cancellationToken);
    };
});

app.UseRateLimiter();

// Usage
app.MapGet("/api/data", () => Results.Ok("data"))
    .RequireRateLimiting("fixed");
```

## Background Jobs & Queues

### BackgroundService Pattern

```csharp
public class EmailConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailConsumerService> _logger;

    public EmailConsumerService(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email consumer started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var queue = scope.ServiceProvider.GetRequiredService<IEmailQueue>();
                
                var email = await queue.DequeueAsync(stoppingToken);
                if (email is not null)
                {
                    await ProcessEmailAsync(email, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ProcessEmailAsync(EmailMessage email, CancellationToken ct)
    {
        // Process email...
    }
}
```

## Logging & Monitoring

### Structured Logging

```csharp
public interface IAppLogger
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
}

public sealed class AppLogger<T> : IAppLogger
{
    private readonly ILogger<T> _logger;

    public AppLogger(ILogger<T> logger) => _logger = logger;

    public void LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);

    public void LogError(Exception ex, string message, params object[] args)
        => _logger.LogError(ex, message, args);
}

// Usage with correlation ID
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
        
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path.Value ?? string.Empty
        }))
        {
            await _next(context);
        }
    }
}
```

## Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, "redis")
    .AddCheck<ExternalApiHealthCheck>("external_api");

app.MapHealthChecks("/health");

// Custom health check
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _client;

    public ExternalApiHealthCheck(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("ExternalApi");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("API returned unhealthy status");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("API is unreachable", ex);
        }
    }
}
```

**Remember**: Backend patterns enable scalable, maintainable server-side applications. Choose patterns that fit your complexity level.
