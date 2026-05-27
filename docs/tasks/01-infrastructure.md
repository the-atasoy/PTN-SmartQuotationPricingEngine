# Tasks — Infrastructure & Environment

Agile task breakdown for infrastructure setup. Tasks are ordered by recommended implementation sequence.

---

## TASK-001 — Docker Compose Setup
**Layer:** Infrastructure  
**Description:**  
Create `docker-compose.yml` at the repository root defining four services: `postgres`, `mailpit`, `backend`, `frontend`. Each service must have health checks. `backend` and `frontend` must declare `depends_on` with condition `service_healthy` on `postgres`.

**Acceptance Criteria:**
- `docker-compose up --build` starts all four services without manual intervention.
- PostgreSQL is accessible at `localhost:5432`.
- Mailpit SMTP is accessible at `localhost:1025`; web UI at `localhost:8025`.
- Backend responds at `localhost:5000/health`.
- Frontend responds at `localhost:3000`.

---

## TASK-002 — Backend .NET Solution & Project Creation
**Layer:** Backend  
**Description:**  
Create the .NET 10 solution with four projects following Onion Architecture:

1. Create the solution file: `dotnet new sln -n SmartQuotation`
2. Create projects:
   - `dotnet new classlib -n Domain -o src/Domain`
   - `dotnet new classlib -n Application -o src/Application`
   - `dotnet new classlib -n Infrastructure -o src/Infrastructure`
   - `dotnet new webapi -n API -o src/API`
3. Add all projects to the solution.
4. Configure project references: API → Infrastructure → Application → Domain.
5. Install NuGet packages:
   - **Application:** `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`
   - **Infrastructure:** `Npgsql.EntityFrameworkCore.PostgreSQL`, `MailKit`, `EPPlus`, `BCrypt.Net-Next`, `My.Extensions.Localization.Json`
   - **API:** `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.EntityFrameworkCore.Design`
6. Create backend `Dockerfile` (multi-stage build).
7. Create `.gitignore` for .NET projects.

**Acceptance Criteria:**
- `dotnet build` succeeds with zero errors.
- All four projects exist with correct references.
- No business logic in `API` or `Infrastructure` projects at this stage.
- Dockerfile builds successfully.

---

## TASK-003 — ApiResponse, PaginatedResult & ValidationBehavior
**Layer:** Backend — Application  
**Description:**  
Create shared models and pipeline behavior in `Application/Common/`:

1. **`ApiResponse<T>`** (`Common/Models/ApiResponse.cs`): Generic API response envelope with properties: `Data`, `IsSuccessful`, `StatusCode`, `Message`, `Errors` (list of strings). Include static factory methods: `Success(data, statusCode)`, `Fail(message, statusCode)`, `ValidationFail(errors)`.
2. **`PaginatedResult<T>`** (`Common/Models/PaginatedResult.cs`): Generic paginated list with: `Items`, `Page`, `PageSize`, `TotalCount`, `TotalPages`, `HasNextPage`, `HasPreviousPage`. Include static factory method `Create(items, page, pageSize, totalCount)`.
3. **`ValidationBehavior<TRequest, TResponse>`** (`Common/Behaviors/ValidationBehavior.cs`): MediatR `IPipelineBehavior` that runs all registered `IValidator<TRequest>` validators before the handler. On validation failure, short-circuits and returns `ApiResponse` with `IsSuccessful = false`, `StatusCode = 422`, and error messages in `Errors`. On success, passes through to the next handler.

All MediatR handlers should return `ApiResponse<T>` (or `ApiResponse` for commands with no data).

**Acceptance Criteria:**
- `ApiResponse<T>` has working factory methods for success, failure, and validation failure.
- `PaginatedResult<T>` correctly computes `TotalPages`, `HasNextPage`, `HasPreviousPage`.
- `ValidationBehavior` intercepts invalid requests and returns `ApiResponse` without reaching the handler.
- `ValidationBehavior` is registered in the MediatR pipeline via DI.

---

## TASK-004 — Frontend Next.js App Creation
**Layer:** Frontend  
**Description:**  
Create the Next.js 15 application using App Router:

1. Create the app: `npx -y create-next-app@latest ./frontend --typescript --tailwind --eslint --app --src-dir --import-alias "@/*" --use-npm`
2. Install additional dependencies:
   - `npm install next-intl react-hook-form zod @hookform/resolvers lucide-react sonner`
3. Configure Tailwind CSS v4 in `tailwind.config.ts`.
4. Set up `next.config.ts` with `next-intl` plugin.
5. Create locale message files `messages/tr.json` and `messages/en.json` with all required keys.
6. Create `i18n.ts` configuration file.
7. Set up `[locale]` dynamic segment in the app directory.
8. Create frontend `Dockerfile` (multi-stage build).
9. Create `.env.local` with default environment variables.

**Acceptance Criteria:**
- `npm run dev` starts the app at `localhost:3000`.
- `npm run build` completes without errors.
- `npm run lint` passes with zero errors.
- Both locale message files exist with keys matching the spec.
- Dockerfile builds successfully.

---

## TASK-005 — CI/CD GitHub Actions
**Layer:** Infrastructure  
**Description:**  
Create two workflow files using Docker for CI builds:
- `.github/workflows/backend.yml`: triggers on push/PR when files under `backend/` change. Uses Docker to build the backend image.
- `.github/workflows/frontend.yml`: triggers on push/PR when files under `frontend/` change. Uses Docker to build the frontend image and run lint + build.

**Acceptance Criteria:**
- Both workflows appear in the GitHub Actions tab.
- Both workflows use Docker to build and validate.
- Both workflows pass on a clean push to `main`.
