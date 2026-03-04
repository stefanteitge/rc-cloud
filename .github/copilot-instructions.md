# rc-cloud Copilot Instructions

rc-cloud.de aggregates German (and limited BeNeLux) RC racing dates from multiple external sources (DMC, Myrcm, RCCO, RCK, Tamiya, etc.) and serves them via an Angular SPA backed by Azure Functions.

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
dotnet build backend/RcCloud.FunctionApi

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

### Deployment
Deployed as an **Azure Static Web App**: Angular frontend + C# Azure Functions API (`RcCloud.FunctionApi`). CI/CD via `.github/workflows/deploy-swa.yml`. Data is updated daily by workflows calling the live HTTP endpoints.

### Data flow
1. **Update**: GitHub Actions (or manual trigger) calls `POST /api/update-germany` → Azure Function scrapes all providers → stores per-source `RaceCompilation` documents in MongoDB, then stores an `"aggregate"` document.
2. **Read**: `GET /api/germany` → loads `("germany", "aggregate")` from MongoDB; if no DMC races are in the aggregate, it also loads `("germany", "dmc")` and merges them client-side (DMC is updated on a separate schedule via `update-dmc.yml`).
3. **Frontend**: Angular fetches the API, groups races by date with `compileUpcomingDates()`, and displays them categorized by German region (north/south/east/west/central).

### Backend project structure
Each data source has its own Application project:
- `RcCloud.DateScraper.Application.Dmc` — DMC scraper
- `RcCloud.DateScraper.Application.Myrcm` — Myrcm scraper
- `RcCloud.DateScraper.Application.Rcco` — RC Car Online scraper
- `RcCloud.DateScraper.Application.Rck` — RCK Challenge + Kleinserie scrapers
- `RcCloud.DateScraper.Application.Tamiya` — Tamiya scraper
- `provider-dmc/RcCloud.Provider.Dmc` — low-level DMC calendar download/parsing (separate solution, referenced by `backend/`)
- `RcCloud.DateScraper.Domain` — domain types: `RaceMeeting`, `Club`, `RegionReference`, `SeriesReference`, `SeasonReference`
- `RcCloud.DateScraper.Infrastructure` — MongoDB + JSON file repositories
- `RcCloud.FunctionApi` — Azure Functions HTTP endpoints

### Club data
`db/club-db.json` is the canonical club database. It is loaded into MongoDB via the `update-clubs` endpoint and then provided to scrapers at runtime via `IClubFileRepository`. Scrapers use `IGuessClub` to match scraped club names (including aliases) to known clubs and attach region info to races.

## Key Conventions

### Adding a new data source
1. Create `RcCloud.DateScraper.Application.XxxProvider` with a `ScrapeXxxRaces` service and a static `AddXxx(this IServiceCollection)` extension method.
2. Register it in `DateScraperApplication.AddScraping()`.
3. Call `await xxx.Scrape()` in `UpdateGermanyFunction` and store results with `await repo.Store(races, "germany", "xxx")`, then add to the `all` list for aggregation.

### MongoDB storage key
Races are keyed by `(compilation, source)` — e.g., `("germany", "myrcm")`, `("germany", "aggregate")`. The `"aggregate"` document is the merged, date-sorted result of all active sources.

### Scraper pattern
Each scraper class is named `ScrapeXxxRaces`, uses AngleSharp for HTML parsing, returns `List<RaceMeeting>`, and uses `IGuessClub` + `GuessSeriesFromTitle` to enrich meetings with club and series info.

### Frontend structure
- `src/app/pages/` — routed page components (`germany`, `benelux`, `clubs`)
- `src/app/shared/` — shared DTOs, services, and components (e.g., `race-meeting/`)
- `compileUpcomingDates(races, displayColumns)` in `shared/race-meeting/services/` groups flat race lists by date and region for display

### Regions
`RegionReference` values are lowercase strings: `north`, `south`, `east`, `west`, `central`. These are used as IDs in both C# and Angular.
