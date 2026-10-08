# rc-cloud.de

This is hosted at https://rc-cloud.de.

It aggregates German RC racing dates from [DMC](https://dmc-online.com/), [Myrcm](https://myrcm.ch), [RC Car Online](https://rccar-online.de), [RCK-Kleinserie](https://kleinserie.rck-solutions.de/indexgo.php), [RCK-Challenge](https://challenge.rck-solutions.de/indexgo.php) and [LRP Offroad Series](https://lrp.cc).

It also has limited support for BeNeLux RC racing dates.

## Technical stuff

The frontend is an Angular SPA served as a Docker container (nginx). The backend is an ASP.NET Core Web API (`RcCloud.WebApi`) also deployed as a Docker container. Both images are published to GitHub Container Registry and deployed via webhook.

It uses Domain Driven Design principles and makes effort to create clean and maintainable code.

Data is stored in MongoDB. The canonical club database lives in `db/club-db.json`.

### Known deviations from the kysy architecture

The backend follows the kysy architecture (see [`.claude/skills/kysy-architecture/SKILL.md`](.claude/skills/kysy-architecture/SKILL.md)).
Existing code predates parts of it. Don't replicate these; migrate them when touching the code:
- `backend/RcCloud.WebApi/Program.cs` contains endpoint logic in inline lambdas, including business
  logic (DMC merge, scrape-and-store orchestration) → plain reads (`/benelux`, `/clubs`) become
  query objects with named factories (`RaceQuery.Upcoming(...)`) run by thin handlers; `/germany`
  merges two sources, so it needs a service unless the DMC races get stored with the aggregate;
  the update endpoints delegate to services such as `UpdateGermanRaces` / `UpdateClubs`.
- All domain types use public (primary) constructors and positional records → private
  constructors + `Create` factories.
- `Club` has `protected set` and a `SetRealName` mutator, `List<string> Aliases` →
  `WithNewName`, `AliasList`.
- `RaceCompilation` has public setters and `List<RaceMeeting>` (the GET endpoint mutates it via
  `AddRange`) → immutable, `Races` becomes a `RaceList` with `UpcomingFrom`/`WithMerged`.
- Repositories and services return `List<RaceMeeting>` / `List<Club>` → `RaceList` / `ClubList`.
- Constrained values are raw primitives (`int? DmcClubNumber`, `int[] MyrcmClubNumbers`,
  `string? CountryCode`, `string? Source`) → value objects (`DmcClubNumber`, `CountryCode`, …)
  validated in their factories, including when read back from MongoDB.
- Repositories take positional strings (`Load(compilation, source)`) → `Get(id)` /
  `Find(query)` with specifications.
- `RacesDocument` stores domain `RaceMeeting` directly → introduce persistence nodes.
- `DateTime.Now` in handlers/services → `TimeProvider`.
- Errors only partly follow the custom-error rule: `DownloadError` / `ScrapeError` in
  `provider-dmc` use public primary constructors, sit in `Domain/` instead of `Errors/`, and don't
  derive from an error category; repository failures are signalled with `bool`; `/update-dmc`
  returns `BadRequest` with the first error's message instead of mapping by error type →
  private constructors + factories, `NotFoundError`/`ValidationError`/… categories,
  `Result` instead of `bool`, `ToProblem()` in handlers.
