# rc-cloud Copilot Instructions

rc-cloud.de aggregates German (and limited BeNeLux) RC racing dates from multiple external sources (DMC, Myrcm, RCCO, RCK Challenge, RCK Kleinserie, LRP Offroad) and serves them via an Angular SPA backed by an ASP.NET Core Web API.

## Commands

### Frontend (`frontend/`)
```bash
npm run start          # dev server
npm run build          # dev build
npm run build:prod     # production build
npm test               # run all tests (Karma/Jasmine)
npm run lint           # ESLint + Prettier
```

### Backend (`backend/`)
```bash
# Build the API
dotnet build backend/RcCloud.WebApi

# Run all backend tests (each test project is independent)
dotnet test provider-dmc/RcCloud.Provider.Dmc.Tests
dotnet test backend/RcCloud.DateScraper.Application.Myrcm.Tests
dotnet test backend/RcCloud.DateScraper.Application.Rcco.Tests
dotnet test backend/RcCloud.DateScraper.Application.Rck.Tests
dotnet test backend/RcCloud.DateScraper.Application.Tamiya.Tests

# Run a single test class
dotnet test --filter "ClassName=DownloadDmcCalendarTests" provider-dmc/RcCloud.Provider.Dmc.Tests
```

## Architecture

### Backend architecture: kysy (mandatory)
All backend C# code (`backend/`, `provider-dmc/`) **must** follow the kysy architecture described in
[`.claude/skills/kysy-architecture/SKILL.md`](../.claude/skills/kysy-architecture/SKILL.md). Read it before
adding or changing backend code. In short:
- Onion architecture with DDD: Domain ← Application ← Infrastructure ← WebApi; dependencies point inward only.
- Immutable domain objects: private constructors, static factory methods (`Create`, …), changes only via
  `With…` methods that return a new object; collections wrapped in domain objects (`RaceList`).
- Value objects for every primitive with business rules; their factories validate and return `Result<T>`.
- Reads: per-query custom specifications wrapped in query objects (`RaceQuery.Upcoming(...)`), executed by
  the repository. Writes/processes: Application services named with an imperative verb (`UpdateClubs`).
- Errors: FluentResults with custom error types (`RaceNotFoundError`), never plain strings.
- Minimal API: `Program.cs` only wires things up; each endpoint is a handler class without business logic.

Existing code still deviates in places — see "Known deviations from the kysy architecture" in
[`README.md`](../README.md). Don't copy deviations; follow the skill for new code and migrate deviating code
you touch.

### Deployment
Both frontend and backend are deployed as **Docker containers** published to GitHub Container Registry (GHCR) and deployed via webhook. CI/CD:
- `.github/workflows/publish-frontend.yml` — builds and pushes the Angular/nginx container on changes to `frontend/`
- `.github/workflows/publish-webapi.yml` — builds and pushes the `RcCloud.WebApi` container on changes to `backend/` or `provider-dmc/`
- `.github/workflows/update-meetings.yml` — daily cron: calls `/update-clubs`, `/update-germany`, `/update-benelux` on `https://api.rc-cloud.de`
- `.github/workflows/update-dmc.yml` — daily cron: calls `/update-dmc` on `https://api.rc-cloud.de`

### Data flow
1. **Update**: GitHub Actions (or manual trigger) calls `POST /update-germany` → `RcCloud.WebApi` scrapes all providers → stores per-source `RaceCompilation` documents in MongoDB, then stores an `"aggregate"` document.
2. **Read**: `GET /germany` → loads `("germany", "aggregate")` from MongoDB; if no DMC races are in the aggregate, it also loads `("germany", "dmc")` and merges them server-side (DMC is updated on a separate schedule via `update-dmc.yml`).
3. **Frontend**: Angular fetches the API, groups races by date with `compileUpcomingDates()`, and displays them categorized by German region (north/south/east/west/central).

### Backend project structure
Each data source has its own Application project:
- `RcCloud.DateScraper.Application.Dmc` — DMC scraper
- `RcCloud.DateScraper.Application.Myrcm` — Myrcm scraper
- `RcCloud.DateScraper.Application.Rcco` — RC Car Online scraper
- `RcCloud.DateScraper.Application.Rck` — RCK Challenge, Kleinserie, and LRP Offroad scrapers
- `RcCloud.DateScraper.Application.Tamiya` — Tamiya scraper
- `provider-dmc/RcCloud.Provider.Dmc` — low-level DMC calendar download/parsing (separate solution, referenced by `backend/`)
- `RcCloud.DateScraper.Domain` — domain types: `RaceMeeting`, `Club`, `RegionReference`, `SeriesReference`, `SeasonReference`
- `RcCloud.DateScraper.Infrastructure` — MongoDB + JSON file repositories
- `RcCloud.WebApi` — ASP.NET Core minimal API HTTP endpoints (`Program.cs`)

### Club data
`db/club-db.json` is the canonical club database. It is loaded into MongoDB via the `/update-clubs` endpoint and then provided to scrapers at runtime via `IClubFileRepository`. Scrapers use `IGuessClub` to match scraped club names (including aliases) to known clubs and attach region info to races.

## Key Conventions

### Adding a new data source
1. Create `RcCloud.DateScraper.Application.XxxProvider` with a `ScrapeXxxRaces` service and a static `AddXxx(this IServiceCollection)` extension method.
2. Register it in `DateScraperApplication.AddScraping()`.
3. Call it from the service that updates the compilation (e.g. `UpdateGermanRaces`), which stores the per-source result and adds it to the aggregate. Don't put this logic into `Program.cs` or a handler (see the kysy architecture above). Until that service exists, the logic still lives in the `/update-germany` lambda in `backend/RcCloud.WebApi/Program.cs` (a known deviation).

### MongoDB storage key
Races are keyed by `(compilation, source)` — e.g., `("germany", "myrcm")`, `("germany", "aggregate")`. The `"aggregate"` document is the merged, date-sorted result of all active sources.

### Scraper pattern
Each scraper class is named `ScrapeXxxRaces`, uses AngleSharp for HTML parsing, returns `List<RaceMeeting>`, and uses `IGuessClub` + `GuessSeriesFromTitle` to enrich meetings with club and series info.

### Frontend structure
- `src/app/pages/races/components/` — `germany-race-list`, `benelux-race-list`, `race-meeting-list`
- `src/app/pages/clubs/` — club list page
- `src/app/shared/` — shared DTOs, services, and components (e.g., `race-meeting/`)
- `compileUpcomingDates(races, displayColumns)` in `shared/race-meeting/services/` groups flat race lists by date and region for display

### Regions
`RegionReference` values are lowercase strings: `north`, `south`, `east`, `west`, `central`. These are used as IDs in both C# and Angular.
