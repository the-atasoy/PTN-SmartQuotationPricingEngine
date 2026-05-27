# Backend — Architecture & Project Structure

## Overview

ASP.NET Core Web API built on **.NET 10** using **Onion Architecture**. All business logic flows through **MediatR** handlers. The backend exposes REST endpoints consumed by the Next.js frontend and the admin panel.

---

## Tech Stack

| Concern | Library / Tool |
|---|---|
| Runtime | .NET 10 |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 (Code First) |
| Database | PostgreSQL 16 |
| Mediator | MediatR 12 |
| Validation | FluentValidation (MediatR pipeline behavior) |
| Authentication | ASP.NET Core JWT Bearer |
| Password Hashing | BCrypt.Net-Next |
| Email | MailKit |
| Excel | EPPlus |
| Localization | ASP.NET Core `IStringLocalizer` + JSON resource files |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |

---

## Project Structure

```
backend/
├── SmartQuotation.sln
└── src/
    ├── Domain/
    │   ├── Entities/
    │   │   ├── Product.cs
    │   │   ├── Request.cs
    │   │   ├── RequestItem.cs
    │   │   ├── Customer.cs
    │   │   ├── ProductPriceHistory.cs
    │   │   └── User.cs
    │   └── Enums/
    │       ├── RequestStatus.cs        # Pending, Sent, Cancelled
    │       ├── ProductCategory.cs      # HMI, LedPanel, LCD
    │       ├── Currency.cs             # TRY, USD, EUR
    │       └── UserRole.cs             # User, Admin
    ├── Application/
    │   ├── Features/
    │   │   ├── Auth/
    │   │   │   └── Commands/
    │   │   │       ├── Login/
    │   │   │       │   ├── LoginCommand.cs
    │   │   │       │   ├── LoginCommandHandler.cs
    │   │   │       │   └── LoginCommandValidator.cs
    │   │   │       └── Refresh/
    │   │   │           ├── RefreshCommand.cs
    │   │   │           └── RefreshCommandHandler.cs
    │   │   ├── Requests/
    │   │   │   ├── Commands/
    │   │   │   │   ├── CreateRequest/
    │   │   │   │   │   ├── CreateRequestCommand.cs
    │   │   │   │   │   ├── CreateRequestCommandHandler.cs
    │   │   │   │   │   └── CreateRequestCommandValidator.cs
    │   │   │   │   └── SendQuotation/
    │   │   │   │       ├── SendQuotationCommand.cs
    │   │   │   │       ├── SendQuotationCommandHandler.cs
    │   │   │   │       └── SendQuotationCommandValidator.cs
    │   │   │   └── Queries/
    │   │   │       ├── GetAllRequests/
    │   │   │       │   ├── GetAllRequestsQuery.cs
    │   │   │       │   └── GetAllRequestsQueryHandler.cs
    │   │   │       └── GetRequestById/
    │   │   │           ├── GetRequestByIdQuery.cs
    │   │   │           └── GetRequestByIdQueryHandler.cs
    │   │   ├── Products/
    │   │   │   └── Queries/
    │   │   │       ├── GetAllProducts/
    │   │   │       │   ├── GetAllProductsQuery.cs
    │   │   │       │   └── GetAllProductsQueryHandler.cs
    │   │   │       └── GetPriceHistory/
    │   │   │           ├── GetPriceHistoryQuery.cs
    │   │   │           └── GetPriceHistoryQueryHandler.cs
    │   │   └── Excel/
    │   │       └── Commands/
    │   │           └── ParseUploadedExcel/
    │   │               ├── ParseUploadedExcelCommand.cs
    │   │               └── ParseUploadedExcelCommandHandler.cs
    │   ├── Common/
    │   │   ├── Behaviors/
    │   │   │   └── ValidationBehavior.cs  # MediatR pipeline — catches FluentValidation errors
    │   │   ├── Models/
    │   │   │   ├── ApiResponse.cs         # Unified API response envelope
    │   │   │   └── PaginatedResult.cs     # Generic paginated list wrapper
    │   │   ├── DTOs/
    │   │   │   ├── Auth/
    │   │   │   │   ├── LoginRequestDto.cs
    │   │   │   │   ├── LoginResponseDto.cs
    │   │   │   │   └── RefreshTokenDto.cs
    │   │   │   ├── Request/
    │   │   │   │   ├── CreateRequestDto.cs
    │   │   │   │   ├── RequestListItemDto.cs
    │   │   │   │   └── SendQuotationDto.cs
    │   │   │   ├── Product/
    │   │   │   │   ├── ProductDto.cs
    │   │   │   │   └── PriceHistoryDto.cs
    │   │   │   └── Excel/
    │   │   │       ├── ExcelRowDto.cs
    │   │   │       └── ParsedExcelResultDto.cs
    │   │   └── Exceptions/
    │   │       ├── NotFoundException.cs
    │   │       └── UnauthorizedException.cs
    │   └── Interfaces/
    │       ├── IEmailService.cs
    │       ├── IExcelService.cs
    │       ├── ITokenService.cs
    │       └── IApplicationDbContext.cs
    ├── Infrastructure/
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs
    │   │   ├── Migrations/
    │   │   └── Seed/
    │   │       └── DataSeeder.cs
    │   ├── Services/
    │   │   ├── EmailService.cs         # MailKit implementation
    │   │   ├── ExcelService.cs         # EPPlus implementation
    │   │   └── TokenService.cs         # JWT generation/validation
    │   └── DependencyInjection.cs
    └── API/
        ├── Controllers/
        │   ├── AuthController.cs
        │   ├── ProductsController.cs
        │   ├── RequestsController.cs
        │   └── ExcelController.cs
        ├── Middleware/
        │   └── ExceptionHandlerMiddleware.cs
        ├── Resources/
        │   ├── tr.json
        │   └── en.json
        ├── Program.cs
        └── appsettings.json
```

---

## Layer Responsibilities

### Domain
Pure C# classes — no framework dependencies. Contains entity models and enums. 
**DDD Approach:** Entities feature rich models using encapsulated properties (`private set`) and intent-revealing methods (e.g., `MarkAsSent()`) rather than anemic models with public setters. No EF Core attributes; all ORM configuration lives in Infrastructure.

### Application
Business logic via MediatR handlers. Defines interfaces (`IEmailService`, `IExcelService`, `ITokenService`, `IApplicationDbContext`) that Infrastructure implements. Contains DTOs, `ApiResponse` envelope, `PaginatedResult` wrapper, `ValidationBehavior` (MediatR pipeline that intercepts FluentValidation errors and returns them as `ApiResponse`), and custom exceptions.

### Infrastructure
Implements all interfaces from Application. Houses EF Core `DbContext`, migrations, seed data, MailKit email service, EPPlus Excel service, and JWT token service.

### API
Thin controller layer that delegates to MediatR. All endpoints return `ApiResponse<T>`. Contains `ExceptionHandlerMiddleware` (catches unhandled exceptions and wraps them into `ApiResponse`), localization resources, and `Program.cs` service configuration.
