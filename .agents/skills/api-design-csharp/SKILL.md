---
name: api-design-csharp
description: >-
  REST API design patterns for .NET applications including resource naming, HTTP status codes, 
  pagination, filtering, error responses, versioning, and rate limiting. Use when designing 
  ASP.NET Core API endpoints, implementing controllers or minimal APIs, or reviewing API contracts.
---

# API Design Patterns (.NET)

Conventions and best practices for designing consistent, developer-friendly REST APIs in ASP.NET Core.

## When to Activate

- Designing new API endpoints in ASP.NET Core
- Implementing Minimal APIs or API Controllers
- Reviewing existing API contracts
- Adding pagination, filtering, or sorting
- Implementing error handling for APIs
- Planning API versioning strategy

## Resource Design

### URL Structure

```
# Resources are nouns, plural, lowercase, kebab-case
GET    /api/v1/users
GET    /api/v1/users/{id}
POST   /api/v1/users
PUT    /api/v1/users/{id}
PATCH  /api/v1/users/{id}
DELETE /api/v1/users/{id}

# Sub-resources for relationships
GET    /api/v1/users/{userId}/orders
POST   /api/v1/users/{userId}/orders

# Actions that don't map to CRUD
POST   /api/v1/orders/{orderId}/cancel
POST   /api/v1/auth/login
POST   /api/v1/auth/refresh
```

### Naming Rules

```
# GOOD
/api/v1/team-members              # kebab-case for multi-word resources
/api/v1/orders?status=active      # query params for filtering
/api/v1/users/{userId}/orders     # nested resources for ownership

# BAD
/api/v1/GetUsers                  # verb in URL
/api/v1/user                      # singular (use plural)
/api/v1/team_members              # snake_case in URLs
/api/v1/users/{userId}/GetOrders  # verb in nested resource
```

## HTTP Methods and Status Codes

### Method Semantics

| Method | Idempotent | Safe | Use For |
|--------|------------|------|---------|
| GET | Yes | Yes | Retrieve resources |
| POST | No | No | Create resources, trigger actions |
| PUT | Yes | No | Full replacement of a resource |
| PATCH | No | No | Partial update of a resource |
| DELETE | Yes | No | Remove a resource |

### Status Code Reference

```
# Success
200 OK                    — GET, PUT, PATCH (with response body)
201 Created               — POST (include Location header)
204 No Content            — DELETE, PUT (no response body)

# Client Errors
400 Bad Request           — Validation failure, malformed JSON
401 Unauthorized          — Missing or invalid authentication
403 Forbidden             — Authenticated but not authorized
404 Not Found             — Resource doesn't exist
409 Conflict              — Duplicate entry, state conflict
422 Unprocessable Entity  — Semantically invalid (valid JSON, bad data)
429 Too Many Requests     — Rate limit exceeded

# Server Errors
500 Internal Server Error — Unexpected failure (never expose details)
502 Bad Gateway            — Upstream service failed
503 Service Unavailable    — Temporary overload, include Retry-After
```

### Common Mistakes

```
# BAD: 200 for everything
{ "status": 200, "success": false, "error": "Not found" }

# GOOD: Use HTTP status codes semantically
HTTP/1.1 404 Not Found
{ "error": { "code": "not_found", "message": "User not found" } }

# BAD: 500 for validation errors
# GOOD: 400 or 422 with field-level details

# BAD: 200 for created resources
# GOOD: 201 with Location header
HTTP/1.1 201 Created
Location: /api/v1/users/abc-123
```

## Response Format

### Success Response

```csharp
public sealed record ApiResponse<T>(
    bool Success,
    T? Data = default,
    object? Meta = null);

// Usage in Minimal API
return Results.Ok(new ApiResponse<UserDto>(true, user));
```

### Collection Response (with Pagination)

```csharp
public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages);

public sealed record PaginationMeta(
    int Total,
    int Page,
    int PerPage,
    int TotalPages);

// JSON output
{
  "success": true,
  "data": [...],
  "meta": {
    "total": 142,
    "page": 1,
    "perPage": 20,
    "totalPages": 8
  }
}
```

### Error Response

```csharp
public sealed record FieldError(string Field, string Message, string Code);

public sealed record ApiError(
    string Code,
    string Message,
    IReadOnlyList<FieldError>? Details = null);

// JSON output
{
  "success": false,
  "error": {
    "code": "validation_error",
    "message": "Request validation failed",
    "details": [
      { "field": "email", "message": "Must be a valid email address", "code": "invalid_format" }
    ]
  }
}
```

## Pagination

### Offset-Based (Simple)

```
GET /api/v1/users?page=2&pageSize=20

// Implementation with EF Core
var skip = (page - 1) * pageSize;
var users = await dbContext.Users
    .OrderByDescending(u => u.CreatedAt)
    .Skip(skip)
    .Take(pageSize)
    .ToListAsync(cancellationToken);
```

**Pros:** Easy to implement, supports "jump to page N"
**Cons:** Slow on large offsets, inconsistent with concurrent inserts

### Cursor-Based (Scalable)

```
GET /api/v1/users?cursor={base64EncodedId}&limit=20

// Implementation
var query = dbContext.Users.AsQueryable();
if (!string.IsNullOrEmpty(cursor))
{
    var cursorId = DecodeCursor(cursor);
    query = query.Where(u => u.Id.CompareTo(cursorId) > 0);
}
var results = await query
    .OrderBy(u => u.Id)
    .Take(limit + 1)
    .ToListAsync(cancellationToken);
```

**Pros:** Consistent performance regardless of position
**Cons:** Cannot jump to arbitrary page

### When to Use Which

| Use Case | Pagination Type |
|----------|----------------|
| Admin dashboards, small datasets (<10K) | Offset |
| Infinite scroll, feeds, large datasets | Cursor |
| Public APIs | Cursor (default) |

## Filtering, Sorting, and Search

### Filtering

```
# Simple equality
GET /api/v1/orders?status=active&customerId=abc-123

# Comparison operators (use bracket notation)
GET /api/v1/products?price[gte]=10&price[lte]=100

# Multiple values (comma-separated)
GET /api/v1/products?category=electronics,clothing
```

### Sorting

```
# Single field (prefix - for descending)
GET /api/v1/products?sort=-createdAt

# Multiple fields (comma-separated)
GET /api/v1/products?sort=-featured,price,-createdAt
```

## Authentication and Authorization

### JWT Token Validation (.NET)

```csharp
// Token validation in Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Usage in Minimal API
app.MapGet("/api/users", async (HttpContext context) =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
        return Results.Unauthorized();
    
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // ...
}).RequireAuthorization();
```

### Authorization Policies

```csharp
// Define policies in Program.cs
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"))
    .AddPolicy("CanDelete", policy => 
        policy.RequireClaim("permission", "delete"));

// Usage in endpoint
app.MapDelete("/api/users/{id}", async (Guid id, IUserService service) =>
{
    await service.DeleteAsync(id);
    return Results.NoContent();
}).RequireAuthorization("CanDelete");
```

## Rate Limiting

### ASP.NET Core Rate Limiting

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Rate limit exceeded" }, cancellationToken: token);
    };
});

app.UseRateLimiter();
```

## Versioning

### URL Path Versioning (Recommended)

```csharp
// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Endpoints
app.MapGet("/api/v1/users", () => "v1");
app.MapGet("/api/v2/users", () => "v2");
```

### Versioning Strategy

```
1. Start with /api/v1/ — don't version until you need to
2. Maintain at most 2 active versions (current + previous)
3. Non-breaking changes don't need a new version:
   - Adding new fields to responses
   - Adding new optional query parameters
   - Adding new endpoints
4. Breaking changes require a new version:
   - Removing or renaming fields
   - Changing field types
   - Changing URL structure
```

## Implementation Patterns

### Minimal API Pattern

```csharp
var users = app.MapGroup("/api/v1/users")
    .WithTags("Users")
    .RequireAuthorization();

users.MapGet("/", async (
    IUserRepository repository,
    int page = 1,
    int pageSize = 20,
    CancellationToken ct = default) =>
{
    var (items, total) = await repository.GetPageAsync(page, pageSize, ct);
    return Results.Ok(new PaginatedResponse<UserDto>(
        items.Select(u => u.ToDto()),
        total,
        page,
        pageSize,
        (int)Math.Ceiling(total / (double)pageSize)));
})
.Produces<PaginatedResponse<UserDto>>()
.WithName("GetUsers");

users.MapPost("/", async (
    CreateUserRequest request,
    IUserService service,
    CancellationToken ct) =>
{
    var result = await service.CreateAsync(request, ct);
    return result.IsSuccess
        ? Results.Created($"/api/v1/users/{result.Value!.Id}", new ApiResponse(result.Value))
        : Results.BadRequest(new ApiError("create_failed", result.Error));
})
.Produces<ApiResponse<UserDto>>(StatusCodes.Status201Created)
.Produces<ApiError>(StatusCodes.Status400BadRequest);
```

### Validation with FluentValidation

```csharp
public class CreateUserRequest
{
    public required string Email { get; init; }
    public required string Name { get; init; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Name).MinimumLength(2).MaximumLength(100);
    }
}

// Usage in endpoint
public async Task<Result<UserDto>> Handle(CreateUserRequest request, CancellationToken ct)
{
    var validation = await _validator.ValidateAsync(request, ct);
    if (!validation.IsValid)
        return Result<UserDto>.Failure(
            validation.Errors.Select(e => new FieldError(e.PropertyName, e.ErrorMessage, e.ErrorCode)));
    
    // Business logic...
}
```

## API Design Checklist

Before shipping a new endpoint:

- [ ] Resource URL follows naming conventions (plural, kebab-case, no verbs)
- [ ] Correct HTTP method used
- [ ] Appropriate status codes returned (not 200 for everything)
- [ ] Input validated with FluentValidation or similar
- [ ] Error responses follow standard format with codes and messages
- [ ] Pagination implemented for list endpoints
- [ ] Authentication required (or explicitly marked as public)
- [ ] Authorization checked
- [ ] Response does not leak internal details
- [ ] OpenAPI documentation updated
