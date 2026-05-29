# Deployment — GitHub Actions CI/CD

## Overview

Two separate CI workflows run on every push when relevant files change. Both workflows use **Docker** to build and validate the projects inside containers, ensuring the CI environment matches production.

---

## Backend CI

### `.github/workflows/backend.yml`

```yaml
name: Backend CI

on:
  push:
    paths: ['backend/**']
  pull_request:
    paths: ['backend/**']

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Build backend Docker image
        run: docker build -t smart-quotation-backend ./backend
```

### What it does
1. **Builds the backend Docker image** — validates the `Dockerfile` and compilation in the same environment as production.

---

## Frontend CI

### `.github/workflows/frontend.yml`

```yaml
name: Frontend CI

on:
  push:
    paths: ['frontend/**']
  pull_request:
    paths: ['frontend/**']

jobs:
  build-and-lint:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Build frontend Docker image
        run: docker build -t smart-quotation-frontend ./frontend

      - name: Run lint in container
        run: |
          docker run --rm \
            smart-quotation-frontend \
            npm run lint

      - name: Verify build output
        run: |
          docker run --rm \
            -e NEXT_PUBLIC_API_URL=http://backend:5100 \
            smart-quotation-frontend \
            npm run build
```

### What it does
1. **Builds the frontend Docker image** — validates the `Dockerfile` and dependency installation.
2. **Runs linting inside the container** — ensures code quality.
3. **Runs production build inside the container** — catches TypeScript and build errors.

---

## Why Docker in CI?

| Benefit | Description |
|---|---|
| **Environment parity** | CI builds match exactly what runs in production |
| **Dockerfile validation** | Catches Dockerfile issues before they reach staging/prod |
| **No tool version drift** | .NET SDK, Node.js versions are pinned in Dockerfiles, not in CI config |
| **Cache-friendly** | Docker layer caching can speed up subsequent runs |

---

## Adding CI Status Badges

Add these to your `README.md`:

```markdown
![Backend CI](https://github.com/the-atasoy/PTN-SmartQuotationPricingEngine/actions/workflows/backend.yml/badge.svg)
![Frontend CI](https://github.com/the-atasoy/PTN-SmartQuotationPricingEngine/actions/workflows/frontend.yml/badge.svg)
```

---

## Future Considerations

- **CD (Continuous Deployment):** Add deployment steps to push Docker images to a registry (e.g., GitHub Container Registry, Docker Hub) and deploy to a cloud provider.
- **Branch Protection:** Configure GitHub branch protection rules to require CI checks to pass before merging.
