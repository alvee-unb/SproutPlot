# SproutPlot

An AI-ready gardening tracker. Users manage gardens, plants, watering schedules,
tasks, weather-aware reminders, and seasonal maintenance. The core product works
fully without AI; premium AI assistance is an additive layer (see
[Architecture](#architecture)).

## Tech stack

| Layer      | Technology                                              |
| ---------- | ------------------------------------------------------- |
| Frontend   | React 19, TypeScript, Vite, Tailwind CSS 4, React Router |
| Backend    | ASP.NET Core 8 Web API, Clean Architecture               |
| Data       | PostgreSQL 16, EF Core 8 (code-first migrations)          |
| Auth       | ASP.NET Core Identity + JWT bearer tokens                |
| Validation | FluentValidation                                         |

## Architecture

The backend follows Clean Architecture with dependencies pointing inward:

```
SproutPlot.Api            → HTTP host: controllers, middleware, auth, DI wiring
   └── SproutPlot.Infrastructure  → EF Core, Identity, external services (weather, AI adapters)
          └── SproutPlot.Application  → use cases, DTOs, interfaces, validators
                 └── SproutPlot.Domain     → entities, enums, domain rules (no dependencies)
```

- **Deterministic business rules** (watering, reminders, scheduling) live in the
  Application/Domain layers and never depend on AI.
- **AI is a seam, not a dependency.** `IAiAdvisor` is defined in the Application
  layer with no implementation yet. When premium AI ships, its adapter lives in
  Infrastructure and consumes *structured, server-assembled context* — never raw
  user prompts, and never as a replacement for the deterministic rules.

The frontend mirrors this separation: `services/` (API access), `features/`
(stateful concerns like auth), `pages/`, `components/`, and `types/`.

## Prerequisites

- [.NET SDK 8](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/) (18+)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

## Getting started

### 1. Database

Create a local role and database (defaults expected by the dev connection string):

```sql
CREATE ROLE sproutplot LOGIN PASSWORD 'sproutplot_dev';
CREATE DATABASE sproutplot_dev OWNER sproutplot;
```

### 2. Backend configuration (secrets stay out of source control)

```bash
cd src/SproutPlot.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=sproutplot_dev;Username=sproutplot;Password=sproutplot_dev"
dotnet user-secrets set "Jwt:Secret" "<a-long-random-development-secret>"
```

Non-secret JWT settings (issuer, audience, expiry) live in `appsettings.json`.

### 3. Run the API

```bash
dotnet run --project src/SproutPlot.Api
```

- Pending EF migrations are applied automatically in Development.
- Swagger UI: `http://localhost:5268/swagger`

### 4. Run the frontend

```bash
cd client
npm install
npm run dev
```

- App: `http://localhost:5173`
- The Vite dev server proxies `/api` to the backend, so the SPA uses same-origin
  relative URLs (no CORS friction in development).

### Tests

```bash
dotnet test
```

## API (auth slice)

| Method | Route                | Auth | Description                          |
| ------ | -------------------- | ---- | ------------------------------------ |
| POST   | `/api/auth/register` | —    | Create an account, returns a JWT     |
| POST   | `/api/auth/login`    | —    | Authenticate, returns a JWT          |
| GET    | `/api/auth/me`       | JWT  | Current user's id and email          |

## Notes / roadmap

- JWT is stored in `localStorage` on the client for now; a refresh-token flow is
  a planned hardening step.
- Feature slices to follow: Gardens, Plants, Watering, Tasks, Weather, Calendar,
  Notifications, then premium AI.
