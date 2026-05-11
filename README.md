# IzTek.Carbon.Footprint

Clean Architecture prensiplerine dayalı .NET backend projesi.

## Başlangıç

### Gereksinimler

- .NET 10 SDK
- PostgreSQL
- Docker (opsiyonel)

### Kurulum

```bash
# Bağımlılıkları yükle
dotnet restore

# Veritabanı migration
dotnet ef database update --project src/IzTek.Carbon.Footprint.Persistence --startup-project src/IzTek.Carbon.Footprint.Api

# Çalıştır
dotnet run --project src/IzTek.Carbon.Footprint.Api
```

### Docker ile Çalıştırma

```bash
docker-compose up -d
```

## Proje Yapısı

```
src/
├── IzTek.Carbon.Footprint.Api/           # API katmanı (Controllers, Middleware)
├── IzTek.Carbon.Footprint.Application/   # Uygulama katmanı (CQRS, Validators)
├── IzTek.Carbon.Footprint.Domain/        # Domain katmanı (Entities, Events)
├── IzTek.Carbon.Footprint.Infrastructure/# Altyapı katmanı (External Services)
└── IzTek.Carbon.Footprint.Persistence/   # Veritabanı katmanı (EF Core, Migrations)
```

## Mimari Kurallar

### Clean Architecture Katmanları

| Katman | Sorumluluk | Bağımlılık |
|--------|------------|------------|
| **Domain** | Entity, Value Object, Domain Event | Hiçbir katmana bağımlı değil |
| **Application** | Use Case, CQRS, Validation, DTO | Sadece Domain'e bağımlı |
| **Infrastructure** | External Service, Email, SMS | Application'a bağımlı |
| **Persistence** | DbContext, Repository, Migration | Application'a bağımlı |
| **Api** | Controller, Middleware, Filter | Tüm katmanlara bağımlı |

### Bağımlılık Kuralı

```
Api → Application → Domain
      ↓
Infrastructure / Persistence
```

> ⚠️ **Önemli**: İç katmanlar dış katmanlara asla bağımlı olmamalı!

## REST API Standartları

### HTTP Metodları

| Metod | Kullanım | Örnek |
|-------|----------|-------|
| `GET` | Veri okuma | `GET /api/products` |
| `POST` | Yeni kayıt oluşturma | `POST /api/products` |
| `PUT` | Tam güncelleme | `PUT /api/products/{id}` |
| `PATCH` | Kısmi güncelleme | `PATCH /api/products/{id}` |
| `DELETE` | Silme | `DELETE /api/products/{id}` |

### HTTP Durum Kodları

| Kod | Anlam | Kullanım |
|-----|-------|----------|
| `200 OK` | Başarılı | GET, PUT, PATCH |
| `201 Created` | Oluşturuldu | POST |
| `204 No Content` | İçerik yok | DELETE |
| `400 Bad Request` | Geçersiz istek | Validation hatası |
| `401 Unauthorized` | Kimlik doğrulanmadı | Token yok/geçersiz |
| `403 Forbidden` | Yetkisiz | Yetki yok |
| `404 Not Found` | Bulunamadı | Kayıt yok |
| `409 Conflict` | Çakışma | Duplicate kayıt |
| `422 Unprocessable Entity` | İşlenemez | Business rule hatası |
| `500 Internal Server Error` | Sunucu hatası | Beklenmeyen hata |

### URL Yapısı

```
✅ Doğru:
GET    /api/v1/products
GET    /api/v1/products/{id}
GET    /api/v1/products/{id}/reviews
POST   /api/v1/products
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}

❌ Yanlış:
GET    /api/v1/getProducts
GET    /api/v1/product/{id}
POST   /api/v1/createProduct
DELETE /api/v1/deleteProduct/{id}
```

### Response Format

**Başarılı Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Product Name"
  }
}
```

**Liste Response (Pagination):**
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

**Hata Response:**
```json
{
  "success": false,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Name alanı zorunludur",
      "field": "name"
    }
  ]
}
```

## .NET Kod Standartları

### Naming Conventions

| Tür | Kural | Örnek |
|-----|-------|-------|
| Class | PascalCase | `ProductService` |
| Interface | I + PascalCase | `IProductService` |
| Method | PascalCase | `GetProductById` |
| Property | PascalCase | `ProductName` |
| Private Field | _camelCase | `_productRepository` |
| Parameter | camelCase | `productId` |
| Constant | PascalCase | `MaxRetryCount` |
| Async Method | PascalCase + Async | `GetProductByIdAsync` |

### Dosya Organizasyonu

```csharp
// 1. Using statements (alfabetik sıralı)
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

**Command (Yazma işlemleri):**
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

**Query (Okuma işlemleri):**
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

FluentValidation kullanılır:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır");
    }
}
```

### Exception Handling

Özel exception sınıfları kullanılır:

```csharp
// Kullanım
throw new NotFoundException("Product", productId);
throw new BadRequestException("Geçersiz istek");
throw new ConflictException("Bu ürün zaten mevcut");
throw new ForbiddenException("Bu işlem için yetkiniz yok");
```

## Geliştirme Rehberi

### Yeni Entity Ekleme

1. **Domain/Entities/** altına entity sınıfı ekleyin:
```csharp
public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
```

2. **Persistence/Configurations/** altına EF configuration ekleyin:
```csharp
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}
```

3. **Application/Features/** altına CQRS command/query ekleyin

4. **Api/Controllers/** altına controller ekleyin

### Migration Oluşturma

```bash
# Migration ekle
dotnet ef migrations add AddCategoryTable \
  --project src/IzTek.Carbon.Footprint.Persistence \
  --startup-project src/IzTek.Carbon.Footprint.Api

# Migration uygula
dotnet ef database update \
  --project src/IzTek.Carbon.Footprint.Persistence \
  --startup-project src/IzTek.Carbon.Footprint.Api
```

### Test Yazma

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

## API Dokümantasyonu

Uygulama çalışırken: `https://localhost:5001/scalar/v1`

## Konfigürasyon

`appsettings.json` dosyasında:

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

| Tetikleyici | İşlem |
|-------------|-------|
| Her commit | Build + Test |
| Main branch'e push | Docker image → `latest` |
| Tag oluşturma (`v1.0.0`) | Docker image → `v1.0.0` + `latest` |

### Release Oluşturma

```bash
# Semantic versioning ile tag oluştur
git tag v1.0.0
git push origin v1.0.0
```

## Faydalı Komutlar

```bash
# Build
dotnet build

# Test
dotnet test

# Format kontrolü
dotnet format --verify-no-changes

# Kod analizi
dotnet build /p:TreatWarningsAsErrors=true
```

