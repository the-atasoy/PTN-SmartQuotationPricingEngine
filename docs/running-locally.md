# Running Locally

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) & [Docker Compose](https://docs.docker.com/compose/install/) (v2+)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (only if running backend outside Docker)
- [Node.js 22+](https://nodejs.org/) (only if running frontend outside Docker)
- [PostgreSQL 16](https://www.postgresql.org/download/) (only if running database outside Docker)

---

## Option 1 — Docker Compose (Recommended)

The simplest way to run the entire stack. One command starts all services:

```bash
docker-compose up --build
```

This starts:
| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |
| PostgreSQL | localhost:5432 |
| Mailpit SMTP | localhost:1025 |
| Mailpit Web UI | http://localhost:8025 |

### Default Credentials

| User | Email | Password | Role |
|---|---|---|---|
| Admin | admin@piton.com.tr | Admin123! | Admin |
| User | user@piton.com.tr | User123! | User |

### Stopping

```bash
docker-compose down
```

To also remove the database volume:
```bash
docker-compose down -v
```

---

## Option 2 — Run Services Individually

Useful for development when you want hot-reload and debugging.

### 1. Start PostgreSQL & Mailpit via Docker

```bash
docker-compose up postgres mailpit
```

### 2. Run Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/API
```

Backend runs at `http://localhost:5000`. Migrations and seed data are applied automatically on startup.

### 3. Run Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend runs at `http://localhost:3000`.

### Environment Variables

#### Backend (`appsettings.Development.json` or env vars)
```
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=smart_quotation;Username=postgres;Password=postgres
Jwt__Secret=your-development-secret-key-min-32-chars
Jwt__Issuer=smart-quotation
Jwt__Audience=smart-quotation
Email__SmtpHost=localhost
Email__SmtpPort=1025
```

#### Frontend (`.env.local`)
```
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_DEFAULT_LOCALE=tr
```

---

## Verifying the Setup

1. Open http://localhost:3000 — you should see the login page.
2. Login with `admin@piton.com.tr` / `Admin123!`.
3. Browse products, add to cart, create a quotation request.
4. Check http://localhost:8025 — Mailpit shows received emails.
5. Verify database: `psql -h localhost -U postgres -d smart_quotation -c "SELECT count(*) FROM products;"`
