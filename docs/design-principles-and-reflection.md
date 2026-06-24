# FitTrack Design Principles And Reflection

## Purpose

This document explains the main software-design principles used in FitTrack:

- Interfaces
- SOLID principles
- Dependency injection and dependency inversion
- Encapsulation
- Data Transfer Objects (DTOs)

It also explains how the implementation changed during development, why the newer design is better, and where the design remains intentionally simple because FitTrack is a semester project rather than a production system.

## Project Layers

FitTrack is split into three projects:

| Project | Responsibility |
| --- | --- |
| `FitTrack.Core` | Entities, DTOs, interfaces, and business services. |
| `FitTrack.Base` | SQL queries, database connections, and repository implementations. |
| `FitTrack.Web` | Razor Pages, authentication, dependency registration, and presentation. |

The dependency direction is:

```text
FitTrack.Web  -> FitTrack.Core
FitTrack.Web  -> FitTrack.Base
FitTrack.Base -> FitTrack.Core
```

`FitTrack.Core` does not depend on the web application or the database implementation. This keeps the main business logic separate from infrastructure and presentation code.

## Initial Design And Main Changes

Not every principle was introduced at the same time. Interfaces, dependency injection, repositories, and DTOs were already used in the earlier design. The largest recent change was improving encapsulation and moving entity-construction rules out of services and repositories.

| Area | Initial situation | Current situation |
| --- | --- | --- |
| Interfaces | Services and repositories already had interfaces. | Interfaces remain the contracts between layers and are also used by the new tests. |
| Dependency injection | ASP.NET Core already selected service and repository implementations. | The same approach remains because it separates construction from use. |
| Encapsulation | Entities had public setters and were created with object initializers. | Entities have private constructors/setters and expose controlled `Create`, `Start`, and `Restore` methods. |
| DTOs | Services already returned DTOs for membership and QR-code data. | DTOs remain the public data returned to Razor Pages while entities are more strongly protected. |
| SOLID | Layer separation and dependency inversion already existed. | Single responsibility improved because entities now own their creation rules. |

This means the architecture was not completely replaced. It was improved by strengthening the weakest area while keeping the parts that already worked.

## Interfaces

### What An Interface Does

An interface defines what operations a class must provide without defining how those operations are performed.

For example, `IMembershipRepository` defines the storage operations required by the membership feature:

```csharp
public interface IMembershipRepository
{
    Task<IReadOnlyCollection<MembershipPlan>> GetPlansAsync();

    Task<Membership?> GetActiveByUserIdAsync(Guid userId);

    Task<Membership> CreateAsync(Membership membership);
}
```

The interface does not contain SQL or in-memory storage logic. That is provided by implementations:

```text
IMembershipRepository
    -> MembershipRepository
    -> InMemoryMembershipRepository
```

### How Interfaces Are Used

`MembershipService` depends on the interface:

```csharp
public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;

    public MembershipService(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }
}
```

The service does not know whether memberships are stored in SQL Server or memory. It only knows that the repository supports the operations declared by `IMembershipRepository`.

### Why This Is Better

Without an interface, the service would depend directly on a concrete repository:

```csharp
public MembershipService(MembershipRepository membershipRepository)
```

This would tightly connect the business service to SQL Server. It would also make testing harder because tests would need a real database.

With the interface, the implementation can be changed without rewriting the service:

```csharp
services.AddScoped<IMembershipRepository, MembershipRepository>();
```

or:

```csharp
services.AddSingleton<IMembershipRepository, InMemoryMembershipRepository>();
```

### Reflection

Interfaces are useful here because there are real alternative implementations: SQL repositories, in-memory repositories, and fake repositories used by tests. An interface should not be added automatically for every class. It is most useful when it creates a meaningful boundary or allows implementations to be replaced.

Interfaces were already part of the initial layered design. They were kept because they supported both the SQL implementation and the later manually written unit tests without requiring either the services or tests to change their contracts.

## Dependency Injection And Dependency Inversion

### Dependency Injection

Dependency injection means that a class receives the objects it needs instead of creating those objects itself.

For example, `MembershipService` receives its repository through its constructor:

```csharp
public MembershipService(IMembershipRepository membershipRepository)
{
    _membershipRepository = membershipRepository;
}
```

The service does not do this:

```csharp
var repository = new MembershipRepository(...);
```

The actual implementation is selected in `FitTrack.Web/Configuration/DependencyInjection.cs`:

```csharp
services.AddScoped<IMembershipRepository, MembershipRepository>();
services.AddScoped<IMembershipService, MembershipService>();
```

ASP.NET Core then creates `MembershipService` and supplies a `MembershipRepository`.

### Dependency Inversion

Dependency inversion means that higher-level business logic depends on abstractions rather than lower-level concrete implementations.

```text
Razor Page
    -> IMembershipService
    -> MembershipService
    -> IMembershipRepository
    -> MembershipRepository
    -> SQL Server
```

Important dependencies point toward the Core layer:

```text
MembershipRepository implements an interface from Core.
MembershipService depends on an interface from Core.
Razor Page depends on a service interface from Core.
```

The Core layer does not depend on `Microsoft.Data.SqlClient`, Razor Pages, or SQL query classes.

### Testing Benefit

The unit tests can inject a manually written fake repository:

```csharp
var repository = new FakeUserRepository();
var service = new RegistrationService(repository);
```

This tests the service without opening a database connection.

### Reflection

Dependency injection improves flexibility and testability. It also makes dependencies visible in constructors. The trade-off is that a beginner must follow registrations and interfaces to understand which concrete class runs. For FitTrack, the benefit is worth the extra structure because both SQL and in-memory implementations exist.

Dependency injection did not need a major redesign during the encapsulation change. The entity factories changed how services create entities, while dependency injection continued to control which services and repositories are supplied.

## Encapsulation

### Initial Implementation

Initially, entities had public setters:

```csharp
public class Membership
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
}
```

Services and repositories created entities with object initializers:

```csharp
var membership = new Membership
{
    Id = Guid.NewGuid(),
    UserId = userId,
    PlanId = selectedPlan.Id,
    PlanName = selectedPlan.Name,
    StartsAtUtc = startsAtUtc,
    EndsAtUtc = startsAtUtc.AddDays(selectedPlan.DurationInDays)
};
```

This worked, but any class could also create or modify invalid data:

```csharp
membership.UserId = Guid.Empty;
membership.PlanId = -5;
membership.EndsAtUtc = membership.StartsAtUtc.AddDays(-10);
```

The entity did not protect its own valid state.

### Current Implementation

The properties now have public getters and private setters:

```csharp
public Guid UserId { get; private set; }

public DateTime StartsAtUtc { get; private set; }

public DateTime EndsAtUtc { get; private set; }
```

Other classes can read these values but cannot freely modify them.

The constructor is private and validates the supplied values:

```csharp
private Membership(
    Guid id,
    Guid userId,
    int planId,
    string planName,
    DateTime startsAtUtc,
    DateTime endsAtUtc)
{
    if (userId == Guid.Empty)
    {
        throw new ArgumentException("User ID is required.", nameof(userId));
    }

    if (endsAtUtc <= startsAtUtc)
    {
        throw new ArgumentException(
            "Membership end date must be after its start date.",
            nameof(endsAtUtc));
    }

    UserId = userId;
    StartsAtUtc = startsAtUtc;
    EndsAtUtc = endsAtUtc;
}
```

New memberships must be created through `Membership.Start`:

```csharp
public static Membership Start(Guid userId, MembershipPlan plan)
{
    var startsAtUtc = DateTime.UtcNow;

    return new Membership(
        Guid.NewGuid(),
        userId,
        plan.Id,
        plan.Name,
        startsAtUtc,
        startsAtUtc.AddDays(plan.DurationInDays));
}
```

Existing memberships loaded from the database are reconstructed through `Membership.Restore`:

```csharp
public static Membership Restore(
    Guid id,
    Guid userId,
    int planId,
    string planName,
    DateTime startsAtUtc,
    DateTime endsAtUtc)
{
    return new Membership(id, userId, planId, planName, startsAtUtc, endsAtUtc);
}
```

Both methods call the same private constructor, so both newly created entities and database entities are validated.

### Start Versus Restore

```text
Membership.Start(...)
    -> Creates a new membership
    -> Generates a new ID
    -> Uses the current time
    -> Calculates the end date

Membership.Restore(...)
    -> Reconstructs an existing membership
    -> Keeps the database ID
    -> Keeps the database dates
    -> Still validates the values
```

### Flow After Encapsulation

```text
Razor Page handler
    -> IMembershipService.StartMembershipAsync(...)
    -> MembershipService finds the selected plan
    -> Membership.Start(...) creates and validates the entity
    -> IMembershipRepository.CreateAsync(...) saves the valid entity
    -> Service converts the entity to a DTO
    -> Razor Page displays the result
```

### Why This Is Better

- Validation is centralized inside the entity.
- Services no longer need to know every construction detail.
- Repositories cannot silently change entity values.
- Invalid entities are harder to create.
- The entity clearly communicates how it can be created.

### Reflection

This is better practice when entities contain important rules. It adds more code than public setters, so it may be unnecessary for very simple data-only classes. In FitTrack, memberships, users, plans, and QR codes have clear validity rules, so encapsulation improves the design.

## Data Transfer Objects

### What A DTO Is

A DTO is a simple object used to transfer selected data between layers.

For example:

```csharp
public class MembershipDto
{
    public string PlanName { get; set; } = string.Empty;

    public DateTime StartsAtUtc { get; set; }

    public DateTime EndsAtUtc { get; set; }
}
```

The DTO contains only the membership data required by the Razor Page. It does not contain repository details or entity creation logic.

### Entity-To-DTO Conversion

The conversion method is private because it is an internal helper used only by `MembershipService`:

```csharp
private static MembershipDto ToDto(Membership membership)
{
    return new MembershipDto
    {
        PlanName = membership.PlanName,
        StartsAtUtc = membership.StartsAtUtc,
        EndsAtUtc = membership.EndsAtUtc
    };
}
```

The service operation is public because the Razor Page must call it:

```csharp
public async Task<MembershipDto?> GetActiveMembershipAsync(Guid userId)
```

The Razor Page receives the DTO but does not call `ToDto`:

```text
Razor Page
    -> Calls GetActiveMembershipAsync(...)
    -> Service loads Membership entity
    -> Service privately calls ToDto(...)
    -> Razor Page receives MembershipDto
```

### Why DTOs Are Used

- The web layer receives only the data it needs.
- Sensitive entity properties are not accidentally exposed.
- Domain entities remain controlled by the Core layer.
- The UI is not tightly coupled to the database entity structure.

For example, `User` contains password hashes and salts. Those values should never be sent to the web page. A DTO can expose only safe user information.

### Reflection

DTOs are useful at layer boundaries. They would be unnecessary if every service and page needed exactly the same simple data, but FitTrack benefits from separating protected entities from data displayed by the UI.

DTOs existed before the encapsulation refactor. Their value became clearer after entities were protected: Razor Pages can still receive simple mutable display data without needing access to the protected domain entities.

## SOLID Principles

FitTrack uses SOLID as guidance rather than trying to implement every principle in an advanced way.

### Single Responsibility Principle

A class should have one main reason to change.

Examples:

| Class | Main responsibility |
| --- | --- |
| `MembershipService` | Membership business operations. |
| `MembershipRepository` | Store and load memberships using SQL. |
| `MembershipSqlQueries` | Hold membership SQL statements. |
| `Membership` | Protect membership data and rules. |
| Membership Razor Page | Handle membership-related web requests. |

Initially, services and repositories were also responsible for correctly constructing entities. Moving construction rules into entities improved this separation.

### Open/Closed Principle

Software should be open to extension but closed to unnecessary modification.

The repository interfaces allow another implementation to be added without changing the services:

```text
IMembershipRepository
    -> MembershipRepository
    -> InMemoryMembershipRepository
    -> Possible future repository
```

### Liskov Substitution Principle

Implementations of an interface should be usable wherever that interface is expected.

`MembershipService` can use either `MembershipRepository` or `InMemoryMembershipRepository` because both implement `IMembershipRepository`.

### Interface Segregation Principle

Interfaces should be focused rather than forcing classes to implement unrelated methods.

FitTrack uses separate interfaces:

```text
IUserRepository
IMembershipRepository
IQRCodeRepository
ILoginService
IRegistrationService
IMembershipService
IQRCodeService
```

For example, `IQRCodeRepository` does not need to implement user or membership operations.

### Dependency Inversion Principle

High-level services depend on repository interfaces rather than SQL repository classes:

```csharp
private readonly IUserRepository _userRepository;
```

The concrete implementation is selected by dependency injection.

### Reflection

The project follows SOLID at a suitable level for its size. Applying more abstractions would not automatically make it better. For example, adding an interface for every helper class would add complexity without a clear benefit. The current design mainly uses SOLID where it improves separation, replacement, and testing.

## Complete Membership Example

```text
User submits SelectedPlanId
    |
    v
Membership Razor Page
    -> Gets authenticated user ID from claims
    -> Calls IMembershipService.StartMembershipAsync(userId, planId)
    |
    v
MembershipService
    -> Calls IMembershipRepository.GetActiveByUserIdAsync(userId)
    -> Returns existing membership if one is active
    -> Calls IMembershipRepository.GetPlansAsync()
    -> Finds selected MembershipPlan
    -> Calls Membership.Start(userId, selectedPlan)
    |
    v
Membership.Start
    -> Generates ID and dates
    -> Calls private Membership constructor
    -> Constructor validates the membership
    |
    v
MembershipRepository
    -> Receives an already valid entity
    -> Reads its public getters
    -> Saves values using parameterized SQL
    |
    v
MembershipService
    -> Privately converts Membership to MembershipDto
    -> Returns DTO
    |
    v
Razor Page
    -> Displays success message and membership information
```

This flow combines interfaces, dependency injection, dependency inversion, encapsulation, DTOs, and separation of responsibilities.

## Final Reflection

The initial implementation was simpler and functional, but it relied on every service and repository to create valid entities correctly. The newer implementation makes responsibilities clearer:

- Entities protect their own valid state.
- Services coordinate business operations.
- Repositories handle persistence.
- Interfaces separate contracts from implementations.
- Dependency injection connects implementations at runtime.
- DTOs control what data leaves the Core layer.

The newer design is better practice because it reduces coupling and makes invalid states harder to create. It also supports unit testing without a real database. The trade-off is additional files and indirection, but the amount used in FitTrack is appropriate for demonstrating these principles without turning the semester project into an unnecessarily complex application.
