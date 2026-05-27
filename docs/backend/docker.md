# Backend — Docker

## Dockerfile

```dockerfile
# backend/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/API/API.csproj"
COPY . .
RUN dotnet build "src/API/API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/API/API.csproj" -c Release -o /app/publish

FROM base AS runner
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

---

## Environment Variables (Docker Compose)

```yaml
backend:
  build: ./backend
  ports:
    - "5000:5000"
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=smart_quotation;Username=postgres;Password=postgres
    - Jwt__Secret=<secret>
    - Jwt__Issuer=smart-quotation
    - Jwt__Audience=smart-quotation
    - Email__SmtpHost=mailpit
    - Email__SmtpPort=1025
  depends_on:
    postgres:
      condition: service_healthy
```

---

## Migrations

Migrations run automatically on startup using `app.MigrateDatabase()` extension before `app.Run()`. No manual migration step is needed when running via Docker.
