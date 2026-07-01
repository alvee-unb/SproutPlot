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

## API

### Auth

| Method | Route                | Auth | Description                          |
| ------ | -------------------- | ---- | ------------------------------------ |
| POST   | `/api/auth/register` | —    | Create an account, returns a JWT     |
| POST   | `/api/auth/login`    | —    | Authenticate, returns a JWT          |
| GET    | `/api/auth/me`       | JWT  | Current user's id and email          |

### Gardens (owner-scoped)

| Method | Route                | Auth | Description                                   |
| ------ | -------------------- | ---- | --------------------------------------------- |
| GET    | `/api/gardens`       | JWT  | Paged list. Query: `page`, `pageSize`, `search`, `sortBy` (`Name`/`CreatedAt`), `descending` |
| GET    | `/api/gardens/{id}`  | JWT  | Get one garden                                |
| POST   | `/api/gardens`       | JWT  | Create a garden                               |
| PUT    | `/api/gardens/{id}`  | JWT  | Update a garden                               |
| DELETE | `/api/gardens/{id}`  | JWT  | Delete a garden                               |

All garden operations are scoped to the authenticated user; accessing another
user's garden returns 404.

### Plants & plant types

| Method | Route                              | Auth | Description                                  |
| ------ | ---------------------------------- | ---- | -------------------------------------------- |
| GET    | `/api/gardens/{gardenId}/plants`   | JWT  | Paged plants in a garden. Query: `page`, `pageSize`, `search`, `status`, `sortBy` (`Name`/`DatePlanted`/`CreatedAt`), `descending` |
| POST   | `/api/gardens/{gardenId}/plants`   | JWT  | Add a plant to the garden                    |
| GET    | `/api/plants/{id}`                 | JWT  | Get one plant                                |
| PUT    | `/api/plants/{id}`                 | JWT  | Update a plant                               |
| DELETE | `/api/plants/{id}`                 | JWT  | Delete a plant                               |
| GET    | `/api/plant-types`                 | JWT  | List seeded plant types (`search` optional)  |

Plants are scoped through their parent garden. `PlantType` is shared, seeded
reference data. Enums (plant status, category) are serialised as strings.

### Weather

| Method | Route                  | Auth | Description                                          |
| ------ | ---------------------- | ---- | ---------------------------------------------------- |
| GET    | `/api/weather`         | JWT  | Current conditions + 7-day forecast. Query: `latitude`, `longitude` |
| GET    | `/api/weather/search`  | JWT  | Geocode a place name to coordinates. Query: `name`   |

Weather comes from the free [Open-Meteo](https://open-meteo.com/) API (no key
required) and is cached per rounded coordinate in a `WeatherCache` table with a
short TTL. The dashboard uses browser geolocation or a city search to pick a
location.

### Watering

| Method | Route                                          | Auth | Description                                   |
| ------ | ---------------------------------------------- | ---- | --------------------------------------------- |
| POST   | `/api/gardens/{gardenId}/waterings`            | JWT  | Record a watering (optionally for one plant)  |
| GET    | `/api/gardens/{gardenId}/waterings`            | JWT  | Watering history (newest first, paged)        |
| GET    | `/api/gardens/{gardenId}/watering-recommendation` | JWT | Deterministic "should I water?" guidance   |
| DELETE | `/api/waterings/{id}`                          | JWT  | Delete a watering record                      |

The recommendation is **deterministic** (no AI): it combines the thirstiest
plant type in the garden, the season (from the garden's hemisphere), days since
the last watering, and — when the garden has coordinates — the rain forecast.
Significant forecast rain defers watering. Gardens carry optional
`latitude`/`longitude` to enable the rain-aware path.

### Tasks

| Method | Route                              | Auth | Description                                        |
| ------ | ---------------------------------- | ---- | -------------------------------------------------- |
| GET    | `/api/tasks`                       | JWT  | Tasks across all gardens. Query: `status`, `dueOnOrBefore`, `sortBy`, `descending`, paging |
| GET    | `/api/gardens/{gardenId}/tasks`    | JWT  | Tasks in one garden                                |
| POST   | `/api/gardens/{gardenId}/tasks`    | JWT  | Create a task (optionally targeting a plant)       |
| GET    | `/api/tasks/{id}`                  | JWT  | Get one task                                       |
| PUT    | `/api/tasks/{id}`                  | JWT  | Edit a task's details                              |
| POST   | `/api/tasks/{id}/complete`         | JWT  | Mark complete                                      |
| POST   | `/api/tasks/{id}/snooze`           | JWT  | Push the due date forward (`{ days }`)             |
| DELETE | `/api/tasks/{id}`                  | JWT  | Delete a task                                      |

Task types: Water, Fertilize, Weed, Mulch, Harvest, Prune, Trim, Repot, Pest
inspection, Disease inspection, Other. The dashboard groups pending tasks due
within a week into overdue / today / upcoming.

## Notes / roadmap

- JWT is stored in `localStorage` on the client for now; a refresh-token flow is
  a planned hardening step.
- Delivered slices: Auth foundation, Gardens (CRUD), Plants (CRUD + seeded
  types), Weather (Open-Meteo + caching), Watering (events + deterministic
  rain-aware recommendation), Tasks (CRUD + complete/snooze + dashboard).
- Feature slices to follow: Calendar, Notifications, then premium AI.
