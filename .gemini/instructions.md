# Coding Standards & Guidelines

This document outlines the coding standards and guidelines applied in the PTN-SmartQuotationPricingEngine project. These rules should be strictly followed to ensure code consistency, reliability, and maintainability.

## 1. General Principles
- **DRY (Don't Repeat Yourself)**: Eliminate code duplication. Extract common logic into shared components, utilities, or base classes.
- **Single Responsibility Principle (SRP)**: Each class, component, and method should have one clear responsibility.
- **YAGNI (You Aren't Gonna Need It)**: Do not add functionality until it is necessary. Remove unused methods, classes, and files.

## 2. Backend (C# / .NET Core)
### 2.1 Patterns & Architecture
- **Clean Architecture & CQRS**: Maintain separation of concerns. Commands handle state mutations; Queries handle data retrieval. Handlers should reside in the `Application` layer.
- **Inheritance for Common Parameters**: Use base classes (e.g., `PaginatedQuery`) to standardize common request parameters like `Page`, `PageSize`, `SortColumn`, and `SortDirection`.
- **Records over Classes for DTOs/Messages**: Use `record` for immutability in Queries, Commands, and DTOs.
- **Init-only Properties**: Use `{ get; init; }` in DTOs instead of `set` to enforce immutability post-initialization.

### 2.2 Controllers & Endpoints
- **Routing & Versioning**: All endpoints must have an API version. Use `[ApiVersion("1.0")]` and `[Route("api/v{version:apiVersion}/[controller]")]`.
- **Security**: Apply `[Authorize]` at the controller or action level appropriately. Secure endpoints based on roles (e.g., `[Authorize(Roles = "Admin")]`).
- **Standardized Responses**: Use the `ApiResponse<T>` wrapper to ensure consistent API responses.
- **Error Handling**: Do not throw exceptions for business logic errors. Return `ApiResponse.Fail` with appropriate HTTP status codes (e.g., `StatusCode(result.StatusCode, result)`).

### 2.3 Entity Framework Core
- **AsNoTracking**: Always append `.AsNoTracking()` to LINQ queries when the data is read-only and no updates will be performed on the returned entities. This improves performance.

### 2.4 Localization
- **IStringLocalizer**: Use `IStringLocalizer<SharedResource>` for all user-facing strings, error messages, and email templates in the backend. Do not use hardcoded strings in Handlers or Controllers.

## 3. Frontend (React / Next.js)
### 3.1 Architecture & Services
- **Centralized API Definitions**: Keep all API endpoints in `src/lib/api-endpoints.ts`. Avoid hardcoding URLs in components.
- **Service Layer**: Extract all network requests into modular API clients in `src/lib/api/` (e.g., `requests.ts`, `products.ts`, `excel.ts`).
- **Shared Types**: Place shared TypeScript interfaces and types in `src/lib/types/api.ts` to ensure consistency between different modules.

### 3.2 Components
- **Extraction**: Extract reusable UI components (e.g., `<Pagination>`) into `src/components/common/`.
- **Localization**: Use `next-intl`'s `useTranslations` hook for all UI text, error toasts, and labels. No hardcoded strings should exist in the JSX.
- **Dead Code Elimination**: Regularly audit and remove unused files, hooks, and validations.
