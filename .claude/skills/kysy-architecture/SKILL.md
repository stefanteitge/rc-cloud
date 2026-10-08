---
name: kysy-architecture
description: The "kysy architecture" for ASP.NET Core backends - an onion architecture with DDD, immutable domain objects (private constructors, static factories, With… methods, collection objects like MeetingList), the specification pattern with query objects for repositories, FluentResults with custom error types, and minimal API endpoints backed by handler classes. Use whenever creating or changing backend C# code in a project that follows it (endpoints, handlers, domain types, repositories, application services, DTOs, DI registration) or when asked to scaffold a new feature, layer, or project in the kysy style.
---

# Kysy architecture for ASP.NET

An onion architecture with DDD. Dependencies point inward only. HTTP is exposed through
ASP.NET Core minimal APIs whose endpoints delegate to handler classes. Reads are expressed as
specifications wrapped in query objects. Domain objects are immutable.

The code examples use meetings, customers and regions (namespaces `MyCompany.MeetingManager.*`) purely for
illustration; replace `<Product>` and the feature names with your own.

Existing code in a project may predate these rules. Check the project's README (or equivalent
docs) for a list of known deviations: follow the rules here for new code, don't copy a
deviation, and migrate deviating code when you touch it.

## Layers and projects

```
            ┌──────────────────────────────────────────┐
            │  Host:  <Product>.WebApi  (minimal API)  │   Program.cs, Endpoints, Handlers, Dto
            │  ┌────────────────────────────────────┐  │
            │  │ Infrastructure (Mongo, files, HTTP)│  │   repository impls, persistence documents
            │  │  ┌──────────────────────────────┐  │  │
            │  │  │ Application (use cases)      │  │  │   services (business logic), ports for external systems
            │  │  │  ┌────────────────────────┐  │  │  │
            │  │  │  │ Domain                 │  │  │  │   entities, value objects, queries,
            │  │  │  │                        │  │  │  │   repository interfaces
            │  │  │  └────────────────────────┘  │  │  │
            │  │  └──────────────────────────────┘  │  │
            │  └────────────────────────────────────┘  │
            └──────────────────────────────────────────┘
```

| Project | References | Contains |
|---|---|---|
| `<Product>.Domain` | nothing (no NuGet except trivial ones like FluentResults) | entities, value objects, specifications, query objects, repository interfaces |
| `<Product>.Application` and `<Product>.Application.<Feature>` | Domain (+ provider libraries) | services (all business logic, imperative-verb names), application-level ports (`IReserveRoom`), feature-local domain types |
| `<Product>.Infrastructure` | Domain | repository implementations, persistence documents, settings |
| `<Product>.WebApi` (or `.Cli`, `.FunctionApi`) | all of the above | composition root, endpoints, handlers, DTOs |
| `<Product>.<Layer>.Tests` | the project under test | xUnit tests, `Data/` fixtures |

Rules:
- Domain never references Application, Infrastructure, ASP.NET, Mongo or serializers.
- Application never references Infrastructure; it talks to persistence only through Domain
  repository interfaces.
- Only the host project wires everything together.
- Large external integrations get their own Application project per source
  (`<Product>.Application.CongressCentre`, `.Hotel`, …) aggregated by `<Product>.Application`.

## Folder layout: feature first, then role

Inside every project, group by feature/aggregate first, then by role:

```
<Product>.Domain/
  Meetings/Meeting.cs
  Meetings/MeetingList.cs                     // collection object with domain filters
  Meetings/Specifications/MeetingSpecification.cs
  Meetings/Queries/MeetingQuery.cs
  Meetings/IMeetingRepository.cs
  Customers/Errors/InvalidCustomerNumberError.cs  // domain errors
  Common/Errors/NotFoundError.cs        // error base categories
  Regions/RegionReference.cs
<Product>.Application.CongressCentre/
  CongressCentreApplication.cs          // AddCongressCentre()
  Bookings/Domain/...                   // feature-local domain types
  Bookings/Services/ImportCongressCentreMeetings.cs
  Bookings/Errors/CongressCentreUnavailableError.cs  // use-case / technical errors
  Bookings/Services/IReserveRoom.cs     // port
  Bookings/Services/Impl/ReserveRoom.cs // implementation of the port
<Product>.Infrastructure/
  InfrastructureLayer.cs                // AddInfrastructure()
  Common/Configuration/MongoDbSettings.cs
  Meetings/MongoMeetingRepository.cs
  Meetings/MeetingsDocument.cs
  Customers/Mongo/... , Customers/File/...  // subfolder per technology when there are several
<Product>.WebApi/
  Program.cs
  Meetings/MeetingsEndpoints.cs               // AddMeetingsHandlers() + MapMeetingsEndpoints()
  Meetings/Handlers/ListUpcomingMeetingsHandler.cs
  Meetings/Handlers/CancelMeetingHandler.cs
  Meetings/Dto/MeetingPageDto.cs
```

Namespaces match folders (`csharp_style_namespace_declarations = file_scoped`).

## Domain

### Immutability (mandatory)

Every domain type (entity, aggregate, value object, collection object) follows these rules:

1. **Private constructor.** The constructor is always `private`. Because C# primary
   constructors can't be made private, write an explicit private constructor; positional
   records are not used for domain types.
2. **Static factory methods are the only way to create an instance.** Name them `Create(...)`
   by default and add intention-revealing variants where useful (`FromXxx(...)`, `Empty()`,
   well-known static instances). Validation and normalisation happen in the factory, so every
   instance that exists is valid. Persistence mappers (`ToDomain()`) and DTOs also go through
   the factory.
3. **Get-only properties.** No setters of any kind (`set`, `protected set`, `init`), no `void`
   mutator methods, no public mutable fields.
4. **Mutation = `With…` method returning a new object.** Every change is a method whose name
   starts with `With` (`WithNewName`, `WithRegion`, `WithAddedAlias`, `WithMeetings`). It returns a
   new instance (built via the private constructor) and leaves `this` untouched. Return `this`
   when nothing changes. Never use the C# `with` expression on domain types from outside — the
   `With…` method is the API.
5. **Collections are domain objects.** A collection of domain objects is never exposed as
   `List<T>`, `T[]` or `IEnumerable<T>` in the domain model; it is wrapped in its own immutable
   collection type (`MeetingList`, `CustomerList`, `AliasList`) that offers domain-meaningful queries
   (`UpcomingFrom(date)`, `InRegion(region)`, `FromSource(source)`, `OrderedByDate()`) and
   `With…` methods (`WithAdded`, `WithMerged`). Filtering methods return a new collection
   object of the same type. Internally it holds an `IReadOnlyList<T>`/`ImmutableArray<T>` and
   implements `IEnumerable<T>` for iteration.
6. Behaviour that only reads state lives on the domain object as expression-bodied members
   (`public bool IsMeeting() => …`).
7. References to other aggregates are `XxxReference` value objects holding the id, not the
   object graph.
8. **Constrained primitives are value objects.** Any simple value that carries a business
   rule (format, length, range, allowed characters, checksum) gets its own value object —
   `CustomerNumber`, `CountryCode`, `RoomCode`, `Url`, `Year`. A plain `string`/`int` is only
   acceptable for free-form values without rules (a customer's display name, a meeting title, a
   remark). If you catch yourself validating a primitive anywhere outside its factory, it
   should be a value object. See "Value objects for constrained primitives" below.

### Value objects for constrained primitives

They are the domain's guard against malicious input **and** malformed data already in the
database: nothing gets into the domain without passing through the factory.

- The private constructor takes the raw value; **all rules live in the static factory**.
- Because the input can be invalid, the factory returns **`Result<T>`** (FluentResults) with a
  custom `ValidationError` subclass (`InvalidCustomerNumberError`) instead of throwing. Name it
  `Create(...)`.
- Normalise before validating (trim, case) when the rule allows it; reject otherwise.
- Expose the raw value via a get-only `Value` property and override `ToString()`.
- Every boundary goes through `Create`: HTTP handlers (invalid → `400 Bad Request` /
  validation problem), scrapers parsing external pages, and Infrastructure `ToDomain()`
  mappers reading the database. A row that fails validation in `ToDomain()` is logged and
  skipped (or surfaced as a failed `Result`), never silently turned into a domain object.
- Entities take these value objects in their factories, not primitives — so an entity factory
  composed only of already-valid value objects can return the entity directly.

```csharp
namespace MyCompany.MeetingManager.Domain.Customers;

/// Customer number: exactly 5 characters, starting with a capital "C", e.g. "C0815".
public sealed partial record CustomerNumber
{
    private CustomerNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<CustomerNumber> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return InvalidCustomerNumberError.Missing();
        }

        var value = raw.Trim();
        if (!Pattern().IsMatch(value))
        {
            return InvalidCustomerNumberError.WrongFormat(value);
        }

        return new CustomerNumber(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex("^C[A-Z0-9]{4}$")]
    private static partial Regex Pattern();
}
```

Use at a boundary:

```csharp
// Handler
var number = CustomerNumber.Create(request.CustomerNumber);
if (number.IsFailed)
{
    return number.ToProblem();   // InvalidCustomerNumberError is a ValidationError → 400
}

// Infrastructure ToDomain()
public Result<Customer> ToDomain()
{
    var number = CustomerNumber.Create(Number);
    var region = RegionReference.Create(Region);

    var merged = Result.Merge(number.ToResult(), region.ToResult());
    return merged.IsFailed
        ? merged
        : Customer.Create(Name, number.Value, region.Value);
}
```

Value object:

```csharp
namespace MyCompany.MeetingManager.Domain.Regions;

public sealed record RegionReference
{
    public static readonly RegionReference West = new("west");

    public static readonly RegionReference East = new("east");

    private RegionReference(string id)
    {
        Id = id;
    }

    public string Id { get; }

    private static readonly RegionReference[] _all = [West, East];

    public static Result<RegionReference> Create(string? id)
    {
        var match = _all.FirstOrDefault(r => r.Id == id?.Trim().ToLowerInvariant());
        return match is null ? UnknownRegionError.For(id) : match;
    }

    public override string ToString() => Id;
}
```

Entity with `With…` methods:

```csharp
namespace MyCompany.MeetingManager.Domain.Customers;

public sealed class Customer
{
    private Customer(string name, AliasList aliases, CustomerNumber number, RegionReference? region)
    {
        Name = name;
        Aliases = aliases;
        Number = number;
        Region = region;
    }

    public string Name { get; }               // free text → plain string is fine

    public AliasList Aliases { get; }

    public CustomerNumber Number { get; }         // has rules → value object

    public RegionReference? Region { get; }

    public static Customer Create(string name, CustomerNumber number, RegionReference? region = null)
        => new(name.Trim(), AliasList.Empty(), number, region);

    public Customer WithNewName(string newName) =>
        newName == Name ? this : new Customer(newName, Aliases.WithAdded(Name), Number, Region);

    public Customer WithRegion(RegionReference region) =>
        region == Region ? this : new Customer(Name, Aliases, Number, region);
}
```

Collection object:

```csharp
namespace MyCompany.MeetingManager.Domain.Meetings;

public sealed class MeetingList : IEnumerable<Meeting>
{
    private readonly IReadOnlyList<Meeting> _meetings;

    private MeetingList(IReadOnlyList<Meeting> meetings)
    {
        _meetings = meetings;
    }

    public int Count => _meetings.Count;

    public static MeetingList Empty() => new([]);

    public static MeetingList Create(IEnumerable<Meeting> meetings) => new([.. meetings.OrderBy(r => r.Date)]);

    public MeetingList Matching(MeetingSpecification specification) => new([.. _meetings.Where(specification.IsSatisfiedBy)]);

    public MeetingList UpcomingFrom(DateOnly date) => Matching(MeetingSpecification.All().WithUpcomingFrom(date));

    public MeetingList InRegion(RegionReference region) => new([.. _meetings.Where(r => r.Regions.Contains(region))]);

    public bool ContainsSource(string source) => _meetings.Any(r => r.Source == source);

    public MeetingList WithMerged(MeetingList other) => Create(_meetings.Concat(other._meetings));

    public IEnumerator<Meeting> GetEnumerator() => _meetings.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

Equality: records get value equality for free; for classes implement `IEquatable<T>` only when
the domain needs it.

### Specifications and query objects (mandatory for reads)

Reads are expressed in two layers, both living in the Domain next to the aggregate:

| Concept | Answers | Example |
|---|---|---|
| **Specification** | *which* objects match | `MeetingSpecification.All().WithUpcomingFrom(today).WithRegion(RegionReference.West)` |
| **Query object** | *how* to fetch them: specification + ordering + paging/limit | `MeetingQuery.Matching(spec).WithOrder(MeetingOrder.ByDate).WithLimit(50)` |

Lookups by identity stay a plain `Get(XxxId id)`. Everything criteria-based goes through a
specification — repositories never get ad-hoc filter parameters (`FindByRegionAndDate(...)`).

#### Specifications

- **No generic base class, no `Specification<T>`, no `And`/`Or`/`Not` combinators.** Each
  query type has its own custom specification class: `MeetingQuery` → `MeetingSpecification`,
  `CustomerQuery` → `CustomerSpecification`. It lives in `<Feature>/Specifications/`.
- A specification is an immutable **set of optional criteria** for its aggregate. Each
  criterion is a get-only property (`null` = not restricted). All set criteria are combined
  with AND. When "any of" is needed, model it as a criterion holding a set
  (`WithRegions(params RegionReference[])`), not as a generic `Or`.
- Domain rules apply: private constructor, a starting factory (`All()`), intention-revealing
  factories for common cases (`Upcoming(venue, today)`), and `With…` methods to add or
  change a criterion. Criteria use value objects, not primitives.
- `IsSatisfiedBy(T)` on the specification evaluates the same criteria in memory. It is the
  reference semantics, used by collection objects (`MeetingList.Matching(spec)`), domain logic and
  tests.
- Adding a criterion means touching three places together: the property + `With…` method,
  `IsSatisfiedBy`, and each store's translator. The translator test (below) catches a miss.
- Don't expose `Expression<Func<TDomain, bool>>` for persistence — the database stores
  persistence documents, not domain objects.

```csharp
namespace MyCompany.MeetingManager.Domain.Meetings.Specifications;

public sealed class MeetingSpecification
{
    private MeetingSpecification(Venue? venue, DateOnly? upcomingFrom, IReadOnlySet<RegionReference>? regions)
    {
        Venue = venue;
        UpcomingFrom = upcomingFrom;
        Regions = regions;
    }

    public Venue? Venue { get; }

    public DateOnly? UpcomingFrom { get; }

    public IReadOnlySet<RegionReference>? Regions { get; }

    public static MeetingSpecification All() => new(null, null, null);

    public static MeetingSpecification Upcoming(Venue venue, DateOnly today) =>
        All().WithVenue(venue).WithUpcomingFrom(today);

    public MeetingSpecification WithVenue(Venue venue) => new(venue, UpcomingFrom, Regions);

    public MeetingSpecification WithUpcomingFrom(DateOnly date) => new(Venue, date, Regions);

    public MeetingSpecification WithRegions(params RegionReference[] regions) =>
        new(Venue, UpcomingFrom, regions.ToHashSet());

    public bool IsSatisfiedBy(Meeting meeting) =>
        (Venue is null || meeting.Venue == Venue)
        && (UpcomingFrom is null || meeting.Date >= UpcomingFrom)
        && (Regions is null || meeting.Regions.Any(Regions.Contains));
}
```

#### Query objects

A query object wraps its specification and adds fetch concerns. Same domain rules: private
constructor, static factory, `With…` methods.

**A read use case is a query, not a service.** "List upcoming meetings" has no business process
— it's a question. Give the query object an intention-revealing factory
(`MeetingQuery.Upcoming(venue, today)`) and let the handler pass it to the repository. The
"business logic" of a read (what counts as upcoming, default order, limits) lives in the
specification and query factories, i.e. in the domain, where it can be tested without I/O.
Only when a read needs more than one repository call, an external port, or a decision between
alternatives does it become an Application service.

```csharp
namespace MyCompany.MeetingManager.Domain.Meetings.Queries;

public sealed class MeetingQuery
{
    private MeetingQuery(MeetingSpecification specification, MeetingOrder order, int? limit)
    {
        Specification = specification;
        Order = order;
        Limit = limit;
    }

    public MeetingSpecification Specification { get; }

    public MeetingOrder Order { get; }

    public int? Limit { get; }

    public static MeetingQuery Matching(MeetingSpecification specification) => new(specification, MeetingOrder.ByDate, null);

    public static MeetingQuery Upcoming(Venue venue, DateOnly today) =>
        Matching(MeetingSpecification.Upcoming(venue, today));

    public MeetingQuery WithOrder(MeetingOrder order) => new(Specification, order, Limit);

    public MeetingQuery WithLimit(int limit) => new(Specification, Order, limit);
}
```

#### Repository interfaces

- Live in Domain next to the aggregate: `I<Aggregate>Repository` (qualify when there are several
  stores, e.g. `ICustomerFileRepository`, `ICustomerArchiveRepository`).
- Read methods: `Get(XxxId id)` for identity, `Find(XxxQuery query)` for everything else,
  optionally `Count(XxxSpecification)` / `Any(XxxSpecification)`.
- Return domain objects, never persistence documents. Multiple results come back as the
  collection object (`Task<MeetingList>`), not `List<Meeting>`. Use `Task<T?>` for "maybe",
  `Result` for writes.

```csharp
namespace MyCompany.MeetingManager.Domain.Meetings;

public interface IMeetingRepository
{
    Task<Meeting?> Get(MeetingId id);

    Task<MeetingList> Find(MeetingQuery query);

    Task<bool> Any(MeetingSpecification specification);

    Task<Result> Store(Meeting meeting);
}
```

#### Translating specifications in Infrastructure

- Each store has an `internal` translator per specification, e.g.
  `Meetings/MongoMeetingSpecificationTranslator`, that turns every set criterion into a native filter
  and ANDs them. An empty specification (`All()`) becomes the empty filter.
- Never fall back to loading everything and filtering in memory. If in-memory filtering is
  genuinely intended (tiny data set, a single small document), do it explicitly in
  the repository with `IsSatisfiedBy` and say so in a comment.
- Each translator gets a test asserting that the native filter and `IsSatisfiedBy` select the
  same items for a set of sample specifications covering every criterion.

```csharp
namespace MyCompany.MeetingManager.Infrastructure.Meetings;

internal static class MongoMeetingSpecificationTranslator
{
    private static readonly FilterDefinitionBuilder<MeetingDocument> _filter = Builders<MeetingDocument>.Filter;

    public static FilterDefinition<MeetingDocument> Translate(MeetingSpecification specification)
    {
        var filters = new List<FilterDefinition<MeetingDocument>>();

        if (specification.Venue is { } venue)
        {
            filters.Add(_filter.Eq(d => d.Venue, venue.Value));
        }

        if (specification.UpcomingFrom is { } from)
        {
            filters.Add(_filter.Gte(d => d.Date, from));
        }

        if (specification.Regions is { } regions)
        {
            filters.Add(_filter.AnyIn(d => d.Regions, regions.Select(r => r.Id)));
        }

        return filters.Count == 0 ? _filter.Empty : _filter.And(filters);
    }
}
```

#### Usage

```csharp
// Handler (read) or Application service
var spec = MeetingSpecification.Upcoming(Venue.MainHall, today).WithRegions(RegionReference.West);
var meetings = await repository.Find(MeetingQuery.Matching(spec).WithLimit(100));

// Same criteria in memory on a collection object
var westUpcoming = allMeetings.Matching(spec);
```

- Specifications and queries are never bound directly from HTTP (private constructors). The
  handler (for reads) or the service parses raw input into value objects and calls the
  factories.

## Errors: FluentResults with custom error types

Expected failures are never thrown and never plain strings. Every failure is a **custom error
class** derived from FluentResults' `Error`, returned in a `Result`/`Result<T>`. Exceptions are
reserved for programming errors and infrastructure faults nobody can handle.

- **Naming:** `<Subject><Problem>Error` — `MeetingNotFoundError`, `CustomerNotFoundError`,
  `InvalidCustomerNumberError`, `UnknownRegionError`, `DownloadError`, `DuplicateMeetingError`.
- **Never `Result.Fail("some text")`.** The type is the contract; the message is for humans.
  Callers branch on the type (`result.HasError<MeetingNotFoundError>()`), never on the message.
- **Same construction rules as domain types:** private constructor, static factory
  (`For(...)`, `Create(...)`), get-only properties carrying the structured context (ids,
  offending value, URL). Also add that context as metadata (`WithMetadata`) so it shows up in
  logs.
- **Small set of base categories** in `Domain/Common/Errors/`, so the host can map errors to
  HTTP status codes generically:
  - `NotFoundError` → 404
  - `ValidationError` → 400 / validation problem
  - `ConflictError` → 409
  - anything else → 500 (or 502 for upstream errors like `DownloadError`)
- **Where they live:** next to whatever produces them, in an `Errors/` folder per feature.
  Domain errors (`InvalidCustomerNumberError`) in Domain, use-case errors (`MeetingNotFoundError`
  from a service) in Application, technical errors (`DownloadError`) in the Application/provider
  project that does the I/O — `internal` if no caller outside needs to match on them.
- **Wrap causes:** when translating a lower-level failure, keep it with `.CausedBy(inner)`.
- **Combine** multiple validation failures with `Result.Merge(...)` so callers get all of them.

```csharp
namespace MyCompany.MeetingManager.Domain.Common.Errors;

public abstract class NotFoundError : Error
{
    protected NotFoundError(string message)
        : base(message)
    {
    }
}

// ValidationError, ConflictError analogous.
```

```csharp
namespace MyCompany.MeetingManager.Application.Meetings.Errors;

public sealed class MeetingNotFoundError : NotFoundError
{
    private MeetingNotFoundError(MeetingId meetingId)
        : base($"Meeting '{meetingId}' was not found.")
    {
        MeetingId = meetingId;
        WithMetadata(nameof(MeetingId), meetingId.Value);
    }

    public MeetingId MeetingId { get; }

    public static MeetingNotFoundError For(MeetingId meetingId) => new(meetingId);
}
```

```csharp
namespace MyCompany.MeetingManager.Domain.Customers.Errors;

public sealed class InvalidCustomerNumberError : ValidationError
{
    private InvalidCustomerNumberError(string? value, string reason)
        : base($"Customer number '{value}' is invalid: {reason}")
    {
        Value = value;
    }

    public string? Value { get; }

    public static InvalidCustomerNumberError Missing() => new(null, "it is required.");

    public static InvalidCustomerNumberError WrongFormat(string value) =>
        new(value, "it must be 5 characters starting with a capital 'C'.");
}
```

Mapping in the host — one shared extension, used by every handler:

```csharp
namespace MyCompany.MeetingManager.WebApi.Common;

public static class ResultHttpExtensions
{
    public static IResult ToProblem(this IResultBase result) => result.Errors.FirstOrDefault() switch
    {
        NotFoundError e => TypedResults.Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
        ValidationError => TypedResults.ValidationProblem(result.Errors
            .OfType<ValidationError>()
            .GroupBy(e => e.GetType().Name)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray())),
        ConflictError e => TypedResults.Problem(e.Message, statusCode: StatusCodes.Status409Conflict),
        var e => TypedResults.Problem(e?.Message, statusCode: StatusCodes.Status500InternalServerError),
    };
}
```

Handlers branch on specific types when an endpoint needs special behaviour
(`if (result.HasError<MeetingNotFoundError>()) …`) and otherwise return `result.ToProblem()`.

## Application

### Services

Services are where business processes live: use cases that change state, call external
systems, or coordinate several aggregates. Plain reads are queries (see "Specifications and
query objects") and don't get a service.

- **Location:** services live only in Application projects (`<Feature>/Services/`). Not in
  Domain, not in Infrastructure, not in the host. Handlers, CLI commands and Azure Functions
  never contain business logic; they call a service (or, for plain reads, a repository with a
  query object).
- **Naming:** the class name always **starts with an imperative verb**, followed by what it acts
  on: `ImportCongressCentreMeetings`, `RetrieveAllMeetings`, `ReserveRoom`, `GuessTopicFromTitle`,
  `UpdateCustomers`, `CancelMeeting`. Never `…Service`, `…Manager`, `…Helper`, or noun-only
  names like `MeetingProvider`.
- **One use case per service.** One public entry method named after the verb (`Import`,
  `Retrieve`, `Reserve`, `Guess`, `Update`, `Cancel`). If a second, unrelated public method
  shows up, split the class.
- **What goes where:**
  - *Domain objects* guard their own invariants: validation in factories, state changes via
    `With…`, rules as specifications, filtering on collection objects.
  - *Services* hold the business process: deciding what to load, which rules to apply in which
    order, combining aggregates, calling ports (repositories, external providers), and
    returning the result. They coordinate domain objects; they don't reimplement rules
    that belong to them.
  - *Queries* answer questions: specification + query object, executed by the repository.
  - *Handlers* only translate: HTTP input → service call or query → DTO.
- Dependencies come in through the primary constructor. Services can call other services
  (e.g. `RetrieveAllMeetings` uses the `Import…` services).

```csharp
namespace MyCompany.MeetingManager.Application.Meetings.Services;

public class CancelMeeting(IMeetingRepository repository, INotifyAttendees notifyAttendees, TimeProvider time)
{
    public async Task<Result<Meeting>> Cancel(MeetingId meetingId, CancellationReason reason)
    {
        var meeting = await repository.Get(meetingId);
        if (meeting is null)
        {
            return MeetingNotFoundError.For(meetingId);
        }

        var cancelled = meeting.WithCancellation(reason, time.GetUtcNow());   // domain guards the rules
        if (cancelled.IsFailed)
        {
            return cancelled;                                                 // e.g. MeetingAlreadyStartedError
        }

        var stored = await repository.Store(cancelled.Value);
        if (stored.IsFailed)
        {
            return stored;
        }

        await notifyAttendees.Notify(cancelled.Value);

        return cancelled;
    }
}
```

### Other Application conventions

- Interfaces only when there is a real seam (multiple implementations or a port to an external
  system): `IReserveRoom` in `Services/`, implementation in `Services/Impl/`.
- Expected failures are returned as `Result<T>` with custom error types — see "Errors:
  FluentResults with custom error types".
- Each Application project exposes one registration extension in `<Name>Application.cs`:
  ```csharp
  public static class CongressCentreApplication
  {
      public static IServiceCollection AddCongressCentre(this IServiceCollection services) => services
          .AddTransient<ImportCongressCentreMeetings>()
          .AddTransient<IReserveRoom, ReserveRoom>();
  }
  ```
  The umbrella `<Product>Application.AddApplication()` chains the feature registrations.
- Put `[assembly: InternalsVisibleTo("<Project>.Tests")]` in that file when tests need internals.
- Use `TimeProvider` (injected) instead of `DateTime.Now` so services are testable.

## Infrastructure

- Repository implementations are `internal` classes named `<Technology><Aggregate>Repository`
  (`MongoMeetingRepository`, `JsonCustomerFileRepository`).
- Persistence shapes are separate types (`MeetingsDocument`, `CustomerDocument`, `CustomerNode`,
  `CustomerJson`) with `FromDomain(...)` / `ToDomain()` mappers. `ToDomain()` rebuilds the domain
  object through its static factory and `With…` methods — never via reflection or a
  serializer writing into domain objects. Domain types never carry `[Bson…]`/`[Json…]`
  attributes.
- Specifications are turned into native filters by an `internal` per-specification translator
  (`Mongo<Aggregate>SpecificationTranslator`) — see "Translating specifications in
  Infrastructure" above.
- Shared plumbing in `Common/` (`MongoBaseRepository<T>`, `Common/Configuration/*Settings`).
- Settings are bound with `services.Configure<TSettings>(configuration.GetSection("…"))`.
- One registration entry point:
  ```csharp
  public static class InfrastructureLayer
  {
      public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
      { … return services.AddScoped<IMeetingRepository, MongoMeetingRepository>(); }
  }
  ```
- Log with structured templates: `logger.LogInformation("Stored meetings. ({Venue}/{Source})", …)`.

## Host: minimal API with handlers

`Program.cs` is composition only — configuration, DI, middleware, and one `Map…Endpoints()` call
per feature. **No endpoint logic in lambdas in `Program.cs`.**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddMeetingsHandlers()
    .AddCustomersHandlers();

var app = builder.Build();
app.UseCors();

app.MapMeetingsEndpoints();
app.MapCustomersEndpoints();

app.Run();
```

### Endpoint registration — `<Feature>/<Feature>Endpoints.cs`

```csharp
namespace MyCompany.MeetingManager.WebApi.Meetings;

public static class MeetingsEndpoints
{
    public static IServiceCollection AddMeetingsHandlers(this IServiceCollection services) => services
        .AddScoped<ListUpcomingMeetingsHandler>()
        .AddScoped<CancelMeetingHandler>();

    public static IEndpointRouteBuilder MapMeetingsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/meetings/upcoming", (ListUpcomingMeetingsHandler handler, string venue) => handler.Handle(venue));
        app.MapPost("/meetings/{id}/cancel", (CancelMeetingHandler handler, string id, CancelMeetingDto body) => handler.Handle(id, body));
        return app;
    }
}
```

### Handler — `<Feature>/Handlers/<Verb><Noun>Handler.cs`

- One class per endpoint, named after the operation (`ListUpcomingMeetingsHandler`,
  `CancelMeetingHandler`), with a single `Handle(...)` method.
- Dependencies via primary constructor:
  - **Command / process endpoint:** exactly one application service.
  - **Read endpoint:** the repository (plus e.g. `TimeProvider`), executing a query object.
- Raw request inputs (route values, query string, body DTOs) are `Handle` parameters, parsed
  into value objects via their factories.
- Return **typed results** (`TypedResults.Ok(...)`, `Results<Ok<TDto>, NotFound>`) so OpenAPI and
  tests see the contract; failures go through the shared `ToProblem()` mapping.
- **No business logic in handlers.** A handler only translates: parse input → call the service
  or run the query → map the `Result` to an HTTP result and the domain object to a DTO. A read
  handler uses the query's named factory (`MeetingQuery.Upcoming(...)`); it doesn't assemble
  criteria inline. Handlers never call `Store`/write methods directly — writes go through a
  service.

```csharp
namespace MyCompany.MeetingManager.WebApi.Meetings.Handlers;

// Read: query object straight to the repository
public class ListUpcomingMeetingsHandler(IMeetingRepository repository, TimeProvider time)
{
    public async Task<IResult> Handle(string venue)
    {
        var parsedVenue = Venue.Create(venue);
        if (parsedVenue.IsFailed)
        {
            return parsedVenue.ToProblem();
        }

        var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);
        var meetings = await repository.Find(MeetingQuery.Upcoming(parsedVenue.Value, today));

        return TypedResults.Ok(MeetingDto.FromDomain(meetings));
    }
}

// Command: delegate to the service
public class CancelMeetingHandler(CancelMeeting cancelMeeting)
{
    public async Task<IResult> Handle(string id, CancelMeetingDto body)
    {
        var meetingId = MeetingId.Create(id);
        var reason = CancellationReason.Create(body.Reason);

        var input = Result.Merge(meetingId.ToResult(), reason.ToResult());
        if (input.IsFailed)
        {
            return input.ToProblem();
        }

        var result = await cancelMeeting.Cancel(meetingId.Value, reason.Value);

        return result.IsSuccess
            ? TypedResults.Ok(MeetingDto.FromDomain(result.Value))
            : result.ToProblem();
    }
}
```

### DTOs — `<Feature>/Dto/<Name>Dto.cs`

- DTOs are serialization contracts, not domain types: primary constructor + get-only
  properties (or a positional `record`) is fine here.
- Mapping lives on the DTO as a static factory: `public static CustomerDto FromDomain(Customer customer) => …`,
  and for collections `public static List<CustomerDto> FromDomain(CustomerList customers) => …`.
  Handlers never return domain objects directly.
- Incoming DTOs map into the domain via `ToDomain()`, which calls the domain factory methods.

## Tests

- xUnit, one test project per production project, named `<Project>.Tests`, mirroring folders.
- Test names `Method_WithCondition_ExpectedOutcome`; body uses `// Arrange`, `// Act`,
  `// Assert`; system under test is `sut`.
- Recorded external input lives in `Data/<source>_<yyyy-MM-dd>.<ext>` and is copied to output.
- Handlers are tested by constructing them with fakes and asserting on the typed result.
- Query factories are tested like specifications: build the query, check its criteria / run
  `IsSatisfiedBy` on sample data.
- Specifications are unit-tested via `IsSatisfiedBy`; each store translator gets a test that
  the native filter and `IsSatisfiedBy` select the same items.

## Code style

Follow the project's `.editorconfig`. Defaults: file-scoped namespaces, 4-space indentation, `var` everywhere,
braces on new lines, `_camelCase` private fields, expression-bodied members where they fit,
collection expressions (`[]`, `[.. xs]`), nullable reference types enabled, max line ~120.

## Checklist for a new feature

1. Domain: aggregate / value objects with private constructors, `Create` factories and
   `With…` methods; value objects for every constrained primitive (factory returns
   `Result<T>`); a collection object (`XxxList`) with domain filters; specification(s) and query
   object(s); repository interface; custom error types for domain failures.
2. Reads: a named factory on the query object (`MeetingQuery.Upcoming(...)`) — no service.
   Application: one service per state-changing / process use case, named with an imperative
   verb (`CancelMeeting`), returning `Result<T>` with custom errors (`MeetingNotFoundError`);
   register in `<Name>Application.cs`.
3. Infrastructure: `internal` repository + persistence document with `FromDomain`/`ToDomain`;
   register in `InfrastructureLayer`.
4. WebApi: DTO with `FromDomain`, handler with typed results (errors via `ToProblem()`), `<Feature>Endpoints.cs` with
   `Add…Handlers()` / `Map…Endpoints()`, one line each in `Program.cs`.
5. Tests for domain behaviour, services and handlers.
