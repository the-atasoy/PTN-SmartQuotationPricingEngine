# Tasks — Database & Persistence

Agile task breakdown for database layer implementation. Depends on Epic 1 (Infrastructure) being complete.

---

## TASK-006 — Domain Entities
**Layer:** Backend — Domain  
**Description:**  
Create all entity classes in `Domain/Entities/`: `User`, `Customer`, `Product`, `Request`, `RequestItem`, `ProductPriceHistory`. Create enums in `Domain/Enums/`: `RequestStatus`, `ProductCategory`, `Currency`, `UserRole`. All entities use `Guid` as primary key. 

**DDD Principles to enforce:**
- Treat `Request` as an Aggregate Root.
- Use **private setters** (`private set;`) for entity properties to prevent external mutation.
- Use rich constructors or factory methods (e.g., `Request.Create(customer, ...)` to guarantee entities are always in a valid state.
- Expose intent-revealing methods to change state (e.g., `request.MarkAsSent()` or `request.AddItem(product, qty)`) instead of modifying properties directly from the Application layer.
- No EF Core attributes in the Domain layer — configuration is done in Infrastructure.

**Acceptance Criteria:**
- All entity classes and enums exist in the correct namespaces.
- Entities enforce invariants through private setters and intent-revealing methods.
- No EF Core or infrastructure references in the Domain project.

---

## TASK-007 — EF Core DbContext & Configuration
**Layer:** Backend — Infrastructure  
**Description:**  
Create `AppDbContext` in `Infrastructure/Persistence/`. Configure all entities using Fluent API inside `IEntityTypeConfiguration<T>` classes — one file per entity. All table and column names must use `snake_case`. Foreign key relationships must be explicitly configured. Define `IApplicationDbContext` interface in `Application/Interfaces/` exposing `DbSet<T>` properties and `SaveChangesAsync`.

**Acceptance Criteria:**
- `AppDbContext` implements `IApplicationDbContext`.
- All table names match the schema in `docs/backend/database.md`.
- All column names are `snake_case`.
- Foreign key constraints are defined for all relationships.

---

## TASK-008 — Migrations & Seed Data
**Layer:** Backend — Infrastructure  
**Description:**  
Generate the initial EF Core migration from the configured `AppDbContext`. Create `DataSeeder.cs` that seeds the database only when tables are empty. Seed data must include: 2 users (Admin + User role), 3 customers, 6 products (2× HMI, 2× Led Panel, 2× LCD), 4 products with pre-populated `last_request_price` and `last_request_date`, 6 `product_price_history` rows. Call `DataSeeder` from `Program.cs` after `app.MigrateDatabase()`.

**Acceptance Criteria:**
- Running `docker-compose up` applies the migration and seeds data automatically.
- Seeding is idempotent — running twice does not duplicate rows.
- All 6 products and 2 users exist in the database after first startup.
