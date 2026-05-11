# Carbon Emissions App

A .NET backend project based on Clean Architecture principles.

## Getting Started

### Requirements

- .NET 10 SDK
- PostgreSQL
- Docker (optional)

### Installation

```bash
# Restore dependencies
dotnet restore

# Database migration
dotnet ef database update --project src/IzTek.Carbon.Footprint.Persistence --startup-project src/IzTek.Carbon.Footprint.Api

# Run
dotnet run --project src/IzTek.Carbon.Footprint.Api
```

### Running with Docker

```bash
docker-compose up -d
```

## Project Structure

```
src/
├── IzTek.Carbon.Footprint.Api/           # API layer (Controllers, Middleware)
├── IzTek.Carbon.Footprint.Application/   # Application layer (CQRS, Validators)
├── IzTek.Carbon.Footprint.Domain/        # Domain layer (Entities, Events)
├── IzTek.Carbon.Footprint.Infrastructure/# Infrastructure layer (External Services)
└── IzTek.Carbon.Footprint.Persistence/   # Database layer (EF Core, Migrations)
```

## Architecture Rules

### Clean Architecture Layers

| Layer | Responsibility | Dependency |
|-------|---------------|------------|
| **Domain** | Entity, Value Object, Domain Event | No dependencies |
| **Application** | Use Case, CQRS, Validation, DTO | Domain only |
| **Infrastructure** | External Service, Email, SMS | Application |
| **Persistence** | DbContext, Repository, Migration | Application |
| **Api** | Controller, Middleware, Filter | All layers |

### Dependency Rule

```
Api → Application → Domain
      ↓
Infrastructure / Persistence
```

> ⚠️ **Important**: Inner layers must never depend on outer layers!

## REST API Standards

### HTTP Methods

| Method | Usage | Example |
|--------|-------|---------|
| `GET` | Read data | `GET /api/products` |
| `POST` | Create new record | `POST /api/products` |
| `PUT` | Full update | `PUT /api/products/{id}` |
| `PATCH` | Partial update | `PATCH /api/products/{id}` |
| `DELETE` | Delete | `DELETE /api/products/{id}` |

### HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| `200 OK` | Success | GET, PUT, PATCH |
| `201 Created` | Created | POST |
| `204 No Content` | No content | DELETE |
| `400 Bad Request` | Invalid request | Validation error |
| `401 Unauthorized` | Not authenticated | Missing/invalid token |
| `403 Forbidden` | Unauthorized | Insufficient permissions |
| `404 Not Found` | Not found | Record missing |
| `409 Conflict` | Conflict | Duplicate record |
| `422 Unprocessable Entity` | Unprocessable | Business rule violation |
| `500 Internal Server Error` | Server error | Unexpected error |

### URL Structure

```
✅ Correct:
GET    /api/v1/products
GET    /api/v1/products/{id}
GET    /api/v1/products/{id}/reviews
POST   /api/v1/products
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}

❌ Incorrect:
GET    /api/v1/getProducts
GET    /api/v1/product/{id}
POST   /api/v1/createProduct
DELETE /api/v1/deleteProduct/{id}
```

### Response Format

**Success Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Product Name"
  }
}
```

**List Response (Pagination):**
```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
  }
}
```

**Error Response:**
```json
{
  "success": false,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Name field is required",
      "field": "name"
    }
  ]
}
```

## .NET Code Standards

### Naming Conventions

| Type | Rule | Example |
|------|------|---------|
| Class | PascalCase | `ProductService` |
| Interface | I + PascalCase | `IProductService` |
| Method | PascalCase | `GetProductById` |
| Property | PascalCase | `ProductName` |
| Private Field | _camelCase | `_productRepository` |
| Parameter | camelCase | `productId` |
| Constant | PascalCase | `MaxRetryCount` |
| Async Method | PascalCase + Async | `GetProductByIdAsync` |

### File Organization

```csharp
// 1. Using statements (alphabetical order)
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using IzTek.Carbon.Footprint.Application;

// 2. Namespace
namespace IzTek.Carbon.Footprint.Api.Controllers;

// 3. Class
public class ProductsController : BaseController
{
    // 4. Private fields
    private readonly IProductService _productService;

    // 5. Constructor
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // 6. Public methods
    // 7. Private methods
}
```

### CQRS Pattern

**Command (Write operations):**
```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<int>>;

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Business logic
    }
}
```

**Query (Read operations):**
```csharp
// Query
public record GetProductByIdQuery(int Id) : IRequest<Result<ProductDto>>;

// Handler
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        // Read logic
    }
}
```

### Validation

FluentValidation is used:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must be at most 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
```

### Exception Handling

Custom exception classes are used:

```csharp
// Usage
throw new NotFoundException("Product", productId);
throw new BadRequestException("Invalid request");
throw new ConflictException("This product already exists");
throw new ForbiddenException("You do not have permission for this action");
```

## Development Guide

### Adding a New Entity

1. Add an entity class under **Domain/Entities/**:
```csharp
public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
```

2. Add EF configuration under **Persistence/Configurations/**:
```csharp
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}
```

3. Add CQRS command/query under **Application/Features/**

4. Add a controller under **Api/Controllers/**

### Creating a Migration

```bash
# Add migration
dotnet ef migrations add AddCategoryTable \
  --project src/IzTek.Carbon.Footprint.Persistence \
  --startup-project src/IzTek.Carbon.Footprint.Api

# Apply migration
dotnet ef database update \
  --project src/IzTek.Carbon.Footprint.Persistence \
  --startup-project src/IzTek.Carbon.Footprint.Api
```

### Writing Tests

```csharp
public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 100);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
    }
}
```

## API Documentation

While the application is running: `https://localhost:5001/scalar/v1`

## Configuration

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mydb;Username=postgres;Password=postgres"
  },
  "OpenTelemetry": {
    "Endpoint": "http://localhost:4317",
    "ServiceName": "IzTek.Carbon.Footprint.Api"
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "IzTek",
    "Audience": "IzTek.Api",
    "ExpirationInMinutes": 60
  }
}
```

## CI/CD

| Trigger | Action |
|---------|--------|
| Every commit | Build + Test |
| Push to main branch | Docker image → `latest` |
| Tag creation (`v1.0.0`) | Docker image → `v1.0.0` + `latest` |

### Creating a Release

```bash
# Create tag with semantic versioning
git tag v1.0.0
git push origin v1.0.0
```

## Useful Commands

```bash
# Build
dotnet build

# Test
dotnet test

# Format check
dotnet format --verify-no-changes

# Code analysis
dotnet build /p:TreatWarningsAsErrors=true
```
