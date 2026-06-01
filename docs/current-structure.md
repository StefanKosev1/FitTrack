# FitTrack Current Structure

This document captures the current project structure for architecture and class diagrams.

## Solution Overview

```text
FitTrack
|-- FitTrack.slnx
|-- Directory.Build.props
|-- docs
|   `-- current-structure.md
`-- src
    |-- FitTrack.Core
    |-- FitTrack.Base
    `-- FitTrack.Web
```

## Project Responsibilities

| Project | Responsibility |
| --- | --- |
| `FitTrack.Core` | Domain entities, repository/service contracts, and core business services. |
| `FitTrack.Base` | Infrastructure implementations such as SQL access, SQL query text, and repositories. |
| `FitTrack.Web` | ASP.NET Core Razor Pages UI, dependency injection setup, configuration, and static assets. |

## Project Dependencies

```mermaid
flowchart LR
    Web[FitTrack.Web] --> Core[FitTrack.Core]
    Web --> Base[FitTrack.Base]
    Base --> Core
```

`FitTrack.Core` is the innermost project and has no project references. `FitTrack.Base` depends on `FitTrack.Core` to implement its interfaces. `FitTrack.Web` references both projects so it can register implementations and use core services.

## Source Tree

```text
src
|-- FitTrack.Core
|   |-- Entities
|   |   |-- AuthResult.cs
|   |   |-- Membership.cs
|   |   |-- MembershipPlan.cs
|   |   |-- QRCode.cs
|   |   |-- User.cs
|   |   `-- UserDto.cs
|   |-- Interfaces
|   |   |-- Data
|   |   |   `-- IDbConnectionFactory.cs
|   |   |-- Repositories
|   |   |   |-- IMembershipRepository.cs
|   |   |   |-- IQRCodeRepository.cs
|   |   |   `-- IUserRepository.cs
|   |   `-- Services
|   |       |-- IAccessService.cs
|   |       |-- IAuthService.cs
|   |       `-- IMembershipService.cs
|   `-- Services
|       |-- AccessService.cs
|       |-- AuthService.cs
|       `-- MembershipService.cs
|-- FitTrack.Base
|   |-- Data
|   |   `-- SqlConnectionFactory.cs
|   |-- Queries
|   |   |-- MembershipSqlQueries.cs
|   |   |-- QRCodeSqlQueries.cs
|   |   `-- UserSqlQueries.cs
|   `-- Repositories
|       |-- InMemoryUserRepository.cs
|       |-- MembershipRepository.cs
|       |-- QRCodeRepository.cs
|       `-- UserRepository.cs
`-- FitTrack.Web
    |-- Configuration
    |   `-- DependencyInjection.cs
    |-- Pages
    |   |-- Account
    |   |   |-- Login.cshtml
    |   |   |-- Login.cshtml.cs
    |   |   |-- Logout.cshtml
    |   |   |-- Logout.cshtml.cs
    |   |   |-- Register.cshtml
    |   |   `-- Register.cshtml.cs
    |   |-- Shared
    |   |   |-- _Layout.cshtml
    |   |   `-- _ValidationScriptsPartial.cshtml
    |   |-- Error.cshtml
    |   |-- Error.cshtml.cs
    |   |-- Index.cshtml
    |   |-- Index.cshtml.cs
    |   |-- _ViewImports.cshtml
    |   `-- _ViewStart.cshtml
    |-- Properties
    |   `-- launchSettings.json
    |-- ViewModels
    |   `-- Account
    |       |-- LoginInputModel.cs
    |       `-- RegisterInputModel.cs
    |-- wwwroot
    |   |-- css
    |   |   `-- site.css
    |   |-- img
    |   |   `-- auth-graffiti-gym.png
    |   |-- js
    |   |   `-- site.js
    |   `-- favicon.ico
    |-- Program.cs
    |-- appsettings.Development.json
    `-- appsettings.json
```

## Core Layer

### Entities

| Entity | Current purpose |
| --- | --- |
| `User` | Account entity with `Id`, `FullName`, `Email`, password hash/salt, and creation timestamp. |
| `AuthResult` | Authentication operation result returned from login and registration. |
| `UserDto` | Lightweight user data transfer shape. |
| `Membership` | Placeholder entity for membership data. |
| `MembershipPlan` | Placeholder entity for membership plan data. |
| `QRCode` | Placeholder entity for QR code access data. |

```mermaid
classDiagram
    class User {
        Guid Id
        string FullName
        string Email
        string PasswordHash
        string PasswordSalt
        DateTime CreatedAtUtc
    }

    class AuthResult {
        bool IsSuccess
        string? ErrorMessage
        string? FullName
        string? Email
    }

    class UserDto {
        int Id
        string FullName
        string Email
        bool IsActive
    }

    class Membership
    class MembershipPlan
    class QRCode
```

### Interfaces And Services

```mermaid
classDiagram
    class IAuthService {
        LoginAsync(string email, string password) Task~AuthResult~
        RegisterAsync(string fullName, string email, string password) Task~AuthResult~
    }

    class AuthService
    IAuthService <|.. AuthService
    AuthService --> IUserRepository
    AuthService --> User
    AuthService --> AuthResult

    class IUserRepository {
        GetByEmailAsync(string email) Task~User?~
        CreateAsync(User user) Task~User~
    }

    class IMembershipService {
        GetActiveMembershipAsync(int userId) Task~Membership?~
    }

    class MembershipService
    IMembershipService <|.. MembershipService
    MembershipService --> Membership

    class IAccessService {
        CanUserAccessGymAsync(int userId) Task~bool~
    }

    class AccessService
    IAccessService <|.. AccessService
```

## Infrastructure Layer

```mermaid
classDiagram
    class IDbConnectionFactory {
        CreateOpenConnectionAsync() Task~DbConnection~
    }

    class SqlConnectionFactory
    IDbConnectionFactory <|.. SqlConnectionFactory

    class IUserRepository
    class UserRepository
    class InMemoryUserRepository
    IUserRepository <|.. UserRepository
    IUserRepository <|.. InMemoryUserRepository
    UserRepository --> IDbConnectionFactory
    UserRepository --> UserSqlQueries

    class IMembershipRepository
    class MembershipRepository
    IMembershipRepository <|.. MembershipRepository
    MembershipRepository --> MembershipSqlQueries

    class IQRCodeRepository
    class QRCodeRepository
    IQRCodeRepository <|.. QRCodeRepository
    QRCodeRepository --> QRCodeSqlQueries
```

## Web Layer

```mermaid
flowchart TD
    Program[Program.cs] --> DI[DependencyInjection.cs]
    DI --> AuthService[IAuthService -> AuthService]
    DI --> UserRepo{UseInMemoryUserRepository}
    UserRepo -->|true| MemoryRepo[IUserRepository -> InMemoryUserRepository]
    UserRepo -->|false| SqlFactory[IDbConnectionFactory -> SqlConnectionFactory]
    SqlFactory --> SqlRepo[IUserRepository -> UserRepository]

    LoginPage[Account/Login.cshtml.cs] --> IAuthService
    RegisterPage[Account/Register.cshtml.cs] --> IAuthService
    LogoutPage[Account/Logout.cshtml.cs] --> Session[ASP.NET session/auth state]
```

## Current Runtime Flow

### Register

```mermaid
sequenceDiagram
    participant Browser
    participant RegisterPage as Register PageModel
    participant AuthService
    participant UserRepository as IUserRepository

    Browser->>RegisterPage: Submit full name, email, password
    RegisterPage->>AuthService: RegisterAsync(...)
    AuthService->>UserRepository: GetByEmailAsync(email)
    UserRepository-->>AuthService: User? result
    AuthService->>AuthService: Hash password with PBKDF2
    AuthService->>UserRepository: CreateAsync(user)
    UserRepository-->>AuthService: Created user
    AuthService-->>RegisterPage: AuthResult
    RegisterPage-->>Browser: Redirect or show validation error
```

### Login

```mermaid
sequenceDiagram
    participant Browser
    participant LoginPage as Login PageModel
    participant AuthService
    participant UserRepository as IUserRepository

    Browser->>LoginPage: Submit email and password
    LoginPage->>AuthService: LoginAsync(email, password)
    AuthService->>UserRepository: GetByEmailAsync(email)
    UserRepository-->>AuthService: User? result
    AuthService->>AuthService: Verify PBKDF2 password hash
    AuthService-->>LoginPage: AuthResult
    LoginPage-->>Browser: Redirect or show validation error
```

## Dependency Injection Registrations

| Condition | Service | Implementation | Lifetime |
| --- | --- | --- | --- |
| `UseInMemoryUserRepository = true` | `IUserRepository` | `InMemoryUserRepository` | Singleton |
| Always in in-memory mode | `IAuthService` | `AuthService` | Scoped |
| `UseInMemoryUserRepository = false` | `IDbConnectionFactory` | `SqlConnectionFactory` | Singleton |
| `UseInMemoryUserRepository = false` | `IUserRepository` | `UserRepository` | Scoped |
| Always in SQL mode | `IAuthService` | `AuthService` | Scoped |

## Notes For Diagrams

- The `Models` folder has been removed from `FitTrack.Core`; `AuthResult` and `UserDto` now live with the other core entities under `FitTrack.Core.Entities`.
- `Membership`, `MembershipPlan`, and `QRCode` are currently empty placeholder entities.
- `MembershipRepository`, `QRCodeRepository`, `AccessService`, and `MembershipService` currently throw `NotImplementedException`.
- `UserRepository` is the current SQL-backed implementation of `IUserRepository`.
- `InMemoryUserRepository` is available for development/testing when configured through `UseInMemoryUserRepository`.
