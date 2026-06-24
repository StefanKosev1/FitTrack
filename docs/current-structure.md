# FitTrack Current Structure

This document describes the current solution structure and the main class
relationships used by the application and its tests.

## Solution Overview

```text
FitTrack
|-- src
|   |-- FitTrack.Core
|   |-- FitTrack.Base
|   `-- FitTrack.Web
`-- tests
    |-- FitTrack.Core.Tests
    `-- FitTrack.Web.Tests
```

| Project | Responsibility |
| --- | --- |
| `FitTrack.Core` | Entities, DTOs, results, interfaces, validation, and business services. |
| `FitTrack.Base` | SQL Server connection, query definitions, and production repository implementations. |
| `FitTrack.Web` | Razor Pages, input models, authentication, configuration, and dependency injection. |
| `FitTrack.Core.Tests` | Unit tests with a `FakeUserRepository`. |
| `FitTrack.Web.Tests` | Web integration tests with in-memory repository implementations. |

## Project Dependencies

```mermaid
flowchart LR
    Web[FitTrack.Web] --> Core[FitTrack.Core]
    Web --> Base[FitTrack.Base]
    Base --> Core
    CoreTests[FitTrack.Core.Tests] --> Core
    WebTests[FitTrack.Web.Tests] --> Web
```

`FitTrack.Core` has no project references. Production repository
implementations belong to `FitTrack.Base`; fake and in-memory repositories
belong to the test projects.

## Current Source Tree

```text
src
|-- FitTrack.Core
|   |-- Dtos
|   |   |-- MembershipDto.cs
|   |   |-- MembershipPlanDto.cs
|   |   |-- QRCodeDto.cs
|   |   `-- UserDto.cs
|   |-- Entities
|   |   |-- Membership.cs
|   |   |-- MembershipPlan.cs
|   |   |-- QRCode.cs
|   |   `-- User.cs
|   |-- Interfaces
|   |   |-- Data
|   |   |   `-- IDbConnectionFactory.cs
|   |   |-- Repositories
|   |   |   |-- IMembershipRepository.cs
|   |   |   |-- IQRCodeRepository.cs
|   |   |   `-- IUserRepository.cs
|   |   `-- Services
|   |       |-- ILoginService.cs
|   |       |-- IMembershipService.cs
|   |       |-- IQRCodeService.cs
|   |       `-- IRegistrationService.cs
|   |-- Results
|   |   `-- AuthResult.cs
|   `-- Services
|       |-- AuthenticationInputValidator.cs
|       |-- LoginService.cs
|       |-- MembershipService.cs
|       |-- PasswordHasher.cs
|       |-- QRCodeService.cs
|       `-- RegistrationService.cs
|-- FitTrack.Base
|   |-- Data
|   |   `-- SqlConnectionFactory.cs
|   |-- Queries
|   |   |-- MembershipSqlQueries.cs
|   |   |-- QRCodeSqlQueries.cs
|   |   `-- UserSqlQueries.cs
|   `-- Repositories
|       |-- MembershipRepository.cs
|       |-- QRCodeRepository.cs
|       `-- UserRepository.cs
`-- FitTrack.Web
    |-- Configuration
    |   `-- DependencyInjection.cs
    |-- Pages
    |   |-- Account
    |   |-- Dashboard
    |   `-- Memberships
    |-- ViewModels
    |   `-- Account
    |       |-- LoginInputModel.cs
    |       `-- RegisterInputModel.cs
    `-- Program.cs

tests
|-- FitTrack.Core.Tests
|   |-- Entities
|   |-- Fakes
|   |   `-- FakeUserRepository.cs
|   `-- Services
|       |-- LoginServiceTests.cs
|       `-- RegistrationServiceTests.cs
`-- FitTrack.Web.Tests
    |-- Fakes
    |   |-- InMemoryMembershipRepository.cs
    |   |-- InMemoryQRCodeRepository.cs
    |   `-- InMemoryUserRepository.cs
    |-- FitTrackWebApplicationFactory.cs
    `-- FitTrackWebTests.cs
```

## Authentication Classes

There is no combined `AuthService`. Login and registration have separate
interfaces and services. They share static validation and password-hashing
helpers.

```mermaid
classDiagram
    class ILoginService {
        <<interface>>
        +LoginAsync(string email, string password) Task~AuthResult~
    }

    class IRegistrationService {
        <<interface>>
        +RegisterAsync(string fullName, string email, string password) Task~AuthResult~
    }

    class LoginService {
        -IUserRepository userRepository
        +LoginAsync(string email, string password) Task~AuthResult~
    }

    class RegistrationService {
        -IUserRepository userRepository
        +RegisterAsync(string fullName, string email, string password) Task~AuthResult~
    }

    class AuthenticationInputValidator {
        <<static>>
        +int MinimumPasswordLength
        +int MaximumPasswordLength
        +IsValidEmail(string email) bool
        +IsValidPassword(string password) bool
    }

    class PasswordHasher {
        <<static>>
        +HashPassword(string password)
        +VerifyPassword(string password, string salt, string hash) bool
    }

    class IUserRepository {
        <<interface>>
        +GetByEmailAsync(string email) Task~User?~
        +CreateAsync(User user) Task~User~
    }

    class AuthResult {
        +bool IsSuccess
        +Guid? UserId
        +string? ErrorMessage
        +string? FullName
        +string? Email
    }

    ILoginService <|.. LoginService
    IRegistrationService <|.. RegistrationService
    LoginService --> IUserRepository
    RegistrationService --> IUserRepository
    LoginService ..> AuthenticationInputValidator : validates input
    RegistrationService ..> AuthenticationInputValidator : validates input
    LoginService ..> PasswordHasher : verifies password
    RegistrationService ..> PasswordHasher : hashes password
    LoginService ..> AuthResult : returns
    RegistrationService ..> AuthResult : returns
```

`AuthenticationInputValidator` and `PasswordHasher` are internal static helper
classes. They are not injected services and do not need interfaces.

## Core Services

```mermaid
classDiagram
    class IMembershipService {
        <<interface>>
        +GetPlansAsync() Task
        +GetActiveMembershipAsync(Guid userId) Task
        +StartMembershipAsync(Guid userId, int planId) Task
    }

    class MembershipService
    class IMembershipRepository {
        <<interface>>
    }

    class IQRCodeService {
        <<interface>>
        +GetOrCreateForUserAsync(Guid userId) Task
    }

    class QRCodeService
    class IQRCodeRepository {
        <<interface>>
    }

    IMembershipService <|.. MembershipService
    IQRCodeService <|.. QRCodeService
    MembershipService --> IMembershipRepository
    QRCodeService --> IQRCodeRepository
    QRCodeService --> IMembershipRepository
```

## Production Repositories

```mermaid
classDiagram
    class IDbConnectionFactory {
        <<interface>>
        +CreateOpenConnectionAsync() Task~DbConnection~
    }

    class SqlConnectionFactory
    IDbConnectionFactory <|.. SqlConnectionFactory

    class IUserRepository {
        <<interface>>
    }
    class IMembershipRepository {
        <<interface>>
    }
    class IQRCodeRepository {
        <<interface>>
    }

    class UserRepository
    class MembershipRepository
    class QRCodeRepository

    IUserRepository <|.. UserRepository
    IMembershipRepository <|.. MembershipRepository
    IQRCodeRepository <|.. QRCodeRepository

    UserRepository --> IDbConnectionFactory
    MembershipRepository --> IDbConnectionFactory
    QRCodeRepository --> IDbConnectionFactory

    UserRepository ..> UserSqlQueries
    MembershipRepository ..> MembershipSqlQueries
    QRCodeRepository ..> QRCodeSqlQueries
```

The production repositories use SQL Server through `SqlConnectionFactory`.
They are the only repository implementations under `src`.

## Test Repositories

Test doubles are kept under `tests`, so they are not shipped as production
infrastructure.

```mermaid
classDiagram
    class IUserRepository {
        <<interface>>
    }
    class IMembershipRepository {
        <<interface>>
    }
    class IQRCodeRepository {
        <<interface>>
    }

    class FakeUserRepository {
        <<FitTrack.Core.Tests>>
    }

    class InMemoryUserRepository {
        <<FitTrack.Web.Tests>>
    }
    class InMemoryMembershipRepository {
        <<FitTrack.Web.Tests>>
    }
    class InMemoryQRCodeRepository {
        <<FitTrack.Web.Tests>>
    }

    IUserRepository <|.. FakeUserRepository
    IUserRepository <|.. InMemoryUserRepository
    IMembershipRepository <|.. InMemoryMembershipRepository
    IQRCodeRepository <|.. InMemoryQRCodeRepository
```

- `FakeUserRepository` supports focused Core unit tests.
- The three `InMemory...Repository` classes replace SQL repositories during
  web integration tests.
- `FitTrackWebApplicationFactory` performs those test-only dependency
  injection replacements.

## Web And Dependency Injection

```mermaid
flowchart TD
    Program[Program.cs] --> DI[DependencyInjection.cs]

    DI --> Login[ILoginService to LoginService]
    DI --> Registration[IRegistrationService to RegistrationService]
    DI --> Membership[IMembershipService to MembershipService]
    DI --> QRCode[IQRCodeService to QRCodeService]

    DI --> UserRepo[IUserRepository to UserRepository]
    DI --> MembershipRepo[IMembershipRepository to MembershipRepository]
    DI --> QRRepo[IQRCodeRepository to QRCodeRepository]
    DI --> SqlFactory[IDbConnectionFactory to SqlConnectionFactory]

    LoginPage[Account Login PageModel] --> ILoginService
    RegisterPage[Account Register PageModel] --> IRegistrationService
```

| Service | Production implementation | Lifetime |
| --- | --- | --- |
| `IDbConnectionFactory` | `SqlConnectionFactory` | Singleton |
| `IUserRepository` | `UserRepository` | Scoped |
| `IMembershipRepository` | `MembershipRepository` | Scoped |
| `IQRCodeRepository` | `QRCodeRepository` | Scoped |
| `ILoginService` | `LoginService` | Scoped |
| `IRegistrationService` | `RegistrationService` | Scoped |
| `IMembershipService` | `MembershipService` | Scoped |
| `IQRCodeService` | `QRCodeService` | Scoped |

## Registration Flow

```mermaid
sequenceDiagram
    participant Page as Register PageModel
    participant Service as RegistrationService
    participant Validator as AuthenticationInputValidator
    participant Hasher as PasswordHasher
    participant Repository as IUserRepository

    Page->>Service: RegisterAsync(fullName, email, password)
    Service->>Validator: Validate email and password
    Service->>Repository: GetByEmailAsync(email)
    Repository-->>Service: Existing user or null
    Service->>Hasher: HashPassword(password)
    Service->>Repository: CreateAsync(user)
    Repository-->>Service: Created user
    Service-->>Page: AuthResult
```

## Login Flow

```mermaid
sequenceDiagram
    participant Page as Login PageModel
    participant Service as LoginService
    participant Validator as AuthenticationInputValidator
    participant Hasher as PasswordHasher
    participant Repository as IUserRepository

    Page->>Service: LoginAsync(email, password)
    Service->>Validator: Validate email and password
    Service->>Repository: GetByEmailAsync(email)
    Repository-->>Service: User or null
    Service->>Hasher: VerifyPassword(password, salt, hash)
    Service-->>Page: AuthResult
```
