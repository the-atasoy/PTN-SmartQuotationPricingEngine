# Tasks — README & Delivery

Agile task breakdown for final delivery artifacts. This should be the last epic completed.

---

## TASK-030 — README.md
**Layer:** Repository  
**Description:**  
Create a comprehensive `README.md` at the repository root covering: project description, tech stack overview, prerequisites (Docker, Docker Compose), getting started (single command: `docker-compose up --build`), environment variables reference, default login credentials (admin and user), API endpoint summary table, database schema diagram or description, GitHub Actions status badges, project structure overview.

**Acceptance Criteria:**
- A developer unfamiliar with the project can run the app using only the README.
- All default credentials are documented.
- All API endpoints are listed with HTTP method, path, required role, and brief description.

---

## TASK-031 — Demo Video Recording
**Layer:** Delivery  
**Description:**  
Record a short video demonstrating:
1. Starting the application with `docker-compose up --build`.
2. User flow: login → browse products → add to cart → create quotation → Excel download.
3. Admin flow: login → view requests → upload Excel → edit prices → send quotation.
4. Email verification in Mailpit.
5. Database schema walkthrough (tables, relationships, seed data).
6. Key technical decisions (architecture, localization approach, transaction handling).

**Acceptance Criteria:**
- Video covers all listed scenarios.
- Video is clear and narrated (or annotated).
- Video is included in or linked from the README.

---

## Task Summary

| Task | Epic | Layer | Title |
|---|---|---|---|
| TASK-001 | Infrastructure | Infra | Docker Compose Setup |
| TASK-002 | Infrastructure | Backend | Backend .NET Solution & Project Creation |
| TASK-003 | Infrastructure | Backend | ApiResponse, PaginatedResult & ValidationBehavior |
| TASK-004 | Infrastructure | Frontend | Frontend Next.js App Creation |
| TASK-005 | Infrastructure | Infra | CI/CD GitHub Actions |
| TASK-006 | Database | Backend | Domain Entities |
| TASK-007 | Database | Backend | EF Core DbContext & Configuration |
| TASK-008 | Database | Backend | Migrations & Seed Data |
| TASK-009 | Auth | Backend | JWT Token Service |
| TASK-010 | Auth | Backend | Login & Refresh Endpoints |
| TASK-011 | Auth | Frontend | Auth Context & Login Page |
| TASK-012 | Auth | Frontend | Route Guard Middleware |
| TASK-013 | Products | Backend | Get All Products Endpoint |
| TASK-014 | Products | Backend | Get Price History Endpoint |
| TASK-015 | Products | Frontend | Product Catalog Page |
| TASK-016 | Request Creation | Backend | Create Request Endpoint & Excel Export |
| TASK-017 | Request Creation | Frontend | Cart Page & Quotation Request |
| TASK-018 | Quotation Mgmt | Backend | Get All Requests Endpoint |
| TASK-019 | Quotation Mgmt | Backend | Get Request By ID Endpoint |
| TASK-020 | Quotation Mgmt | Backend | Parse Uploaded Excel Endpoint |
| TASK-021 | Quotation Mgmt | Backend | Send Quotation Endpoint |
| TASK-022 | Quotation Mgmt | Frontend | Admin Requests List Page |
| TASK-023 | Quotation Mgmt | Frontend | Admin Request Detail & Quotation Editor |
| TASK-024 | Quotation Mgmt | Frontend | Product Price History Page |
| TASK-025 | Email | Backend | Email Service Implementation |
| TASK-026 | Localization | Backend | Backend Localization Setup (JSON) |
| TASK-027 | Localization | Frontend | Frontend Localization Setup |
| TASK-028 | Layout | Frontend | Navbar Component |
| TASK-029 | Layout | Frontend | UI Components Library |
| TASK-030 | Delivery | Repo | README.md |
| TASK-031 | Delivery | Repo | Demo Video Recording |
