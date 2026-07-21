FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/API/API.csproj"

COPY . .

RUN dotnet build "src/API/API.csproj" \
    -c Release \
    --no-restore 
    
FROM build AS publish

RUN dotnet publish "src/API/API.csproj" \
    -c Release \
    --no-build \
    -o /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl --fail http://localhost:5100/health || exit 1

ENTRYPOINT ["dotnet", "API.dll"]