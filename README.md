# Assignment & Submission Management System

A role-based web application for schools/colleges that lets Teachers create and manage assignments, Students view and submit their work, and Admins manage users, classes, and subjects — built as a fully decoupled REST API backend with a separate Next.js frontend.

## Overview

This system supports three roles — **Admin**, **Teacher**, and **Student** — each with distinct permissions enforced both at the API level (JWT + role-based authorization) and reflected in the frontend UI. Teachers create assignments as drafts, publish them when ready, and grade student submissions. Students see only published assignments for their own class, submit answers before a deadline, and can update their submission up until that deadline. Admins manage the underlying user/class/subject data that the rest of the system depends on.

## Main Features

- JWT-based authentication with role-based authorization (Admin / Teacher / Student)
- Admin: create/manage users (Teachers, Students), classes, subjects, and assign teachers to subjects
- Teacher: create, publish, and delete assignments; view and grade student submissions with marks + feedback
- Student: view assignments published for their class; submit or update an answer before the deadline; view marks and feedback once graded
- Business rules enforced server-side: draft assignments invisible to students, submission updates blocked after the deadline (late first-time submissions still accepted and marked "Late"), marks capped at an assignment's maximum, ownership checks (a Teacher can only manage their own assignments/subjects)
- Unit tests covering the core business rules above
- Swagger/OpenAPI documentation for the full API

## Technology Stack

**Backend:** ASP.NET Core Web API (.NET 8), Entity Framework Core, ASP.NET Core Identity, JWT Bearer Authentication, Microsoft SQL Server, N-Tier Architecture (Domain / Application / Infrastructure / API), Repository Pattern, Unit of Work, xUnit + Moq, Swagger/OpenAPI

**Frontend:** Next.js (App Router), React, TypeScript, Tailwind CSS, Axios

## Project Structure

```
AssignmentManagementSystem/
├── AssignmentSystem.Domain/          # Entities — no external dependencies
├── AssignmentSystem.Application/     # DTOs, service interfaces/implementations, mapping
├── AssignmentSystem.Infrastructure/  # EF Core DbContext, repositories, Identity, migrations
├── AssignmentSystem.API/             # Controllers, Program.cs, JWT/Swagger/CORS config
├── AssignmentSystem.Tests/           # xUnit unit tests
├── AssignmentManagementSystem.sln
└── assignment-system-frontend/       # Next.js/React/TypeScript frontend
```

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- Node.js (LTS) and npm
- Microsoft SQL Server (local instance, e.g. SQL Server Express or LocalDB)
- Visual Studio 2022 (backend) and VS Code (frontend), or your preferred equivalents

### Backend Setup

1. Open `AssignmentManagementSystem.sln` in Visual Studio.
2. Update the connection string in `AssignmentSystem.API/appsettings.json` under `ConnectionStrings:DefaultConnection` to point at your local SQL Server instance. See `.env.example` for the required format.
3. Set `AssignmentSystem.API` as the startup project.
4. Open the **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console), set the default project dropdown to `AssignmentSystem.Infrastructure`, and run:
   ```
   Update-Database -StartupProject AssignmentSystem.API
   ```
5. Run the project (F5). The API starts at `https://localhost:7015` and seeds three demo accounts (see **Demo Credentials** below) along with a sample Class, Subject, and Teacher-Subject assignment on first run.
6. Swagger UI is available at `https://localhost:7015/swagger`.

### Frontend Setup

1. Open a terminal in the `assignment-system-frontend` folder.
2. Install dependencies:
   ```
   npm install
   ```
3. Create a `.env.local` file in this folder (see `.env.example`):
   ```
   NEXT_PUBLIC_API_URL=https://localhost:7015/api
   ```
4. Run the dev server:
   ```
   npm run dev
   ```
5. Open `http://localhost:3000/login`.

**Note:** Both the backend (`https://localhost:7015`) and frontend (`http://localhost:3000`) must be running at the same time for the application to work.

### Running Tests

In Visual Studio: Test menu → Test Explorer → Run All.

Or from a terminal in the solution root:
```
dotnet test
```

## Demo Credentials

| Role    | Email               | Password    |
|---------|---------------------|-------------|
| Admin   | admin@demo.com       | Admin@123    |
| Teacher | teacher@demo.com     | Teacher@123  |
| Student | student@demo.com     | Student@123  |

## Assumptions

- `ClassId` on a user is only meaningful for the Student role; this is enforced at the application/service layer rather than the database schema, since a single-table Identity model (rather than separate Student/Teacher/Admin tables) was chosen for simplicity at this project's scale.
- A Teacher may only create assignments for subjects they are explicitly linked to via the `TeacherSubjects` table (managed by an Admin).
- A Student may have at most one Submission per Assignment; submitting again before the deadline updates the existing submission rather than creating a new one.
- Submitting after the deadline for the first time is still accepted and marked "Late"; updating an existing submission after the deadline is blocked.
- Grading is not restricted by deadline — a Teacher can grade a submission at any time.
- Password complexity requirements were relaxed (`RequireNonAlphanumeric = false`) to keep demo credentials simple; this would be reverted for a production deployment.
- Manual mapping (extension methods) is used instead of AutoMapper, after encountering a reflection-scanning runtime bug plus an unpatched high-severity DoS advisory (GHSA-rvv3-g6hj-g44x) present in all current free AutoMapper versions. For a DTO set this size, explicit mapping is simpler to debug and removes the dependency entirely.

## Known Limitations

- Admin functionality (user management, class/subject management, assigning teachers to subjects) is fully implemented and testable via the REST API and Swagger UI, but does not yet have a dedicated frontend UI. Frontend effort was prioritized on the Teacher and Student workflows, which represent the core day-to-day usage of the system.
- `GetAllAsync` for users performs a per-user role lookup (N+1 query pattern); acceptable at this project's scale, but would need a joined query for production-scale user counts.
- No automated integration or end-to-end tests — the test suite covers business-rule unit tests (with mocked dependencies); full request/response behavior was verified manually via Swagger and the frontend UI.
- No file upload support for submissions; submissions are plain text content.
- No pagination or advanced filtering on list endpoints.
- No real-time notifications.
- No Docker configuration.

## Possible Future Improvements

- Admin frontend UI (user/class/subject management)
- Pagination and filtering on list endpoints (e.g. filter assignments by status)
- Notifications for new assignments, grading, and approaching deadlines
- File upload support for submissions
- Dockerized setup for easier local onboarding
