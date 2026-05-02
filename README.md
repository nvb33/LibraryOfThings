# Library of Things — Peer-to-Peer Rental Marketplace

A .NET MAUI Android application for peer-to-peer rental of everyday items, built as part of the Edinburgh Napier University SET09102 Software Engineering module.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=coverage)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)

---

## Project Overview

Library of Things allows users to:
- Browse, search, filter and sort items available for rent
- Search for items nearby using location with configurable radius
- Request, approve and manage rentals end-to-end
- Leave reviews after completed rentals
- View all reviews and average rating for any item

The app communicates with a shared REST API backend and follows a five-layer architecture: Views, ViewModels, Business Logic Services, Repositories and API Services.

---

## Repository Structure

```
LibraryOfThings/
├── .github/
│   └── workflows/
│       ├── build.yml               # CI/CD pipeline (build, test, SonarCloud, APK)
│       └── documentation.yml       # Doxygen documentation generation
├── StarterApp/                     # .NET MAUI Android app
│   ├── Converters/                 # Value converters for XAML bindings
│   ├── Data/
│   │   └── Repositories/           # Concrete repository implementations
│   ├── Services/                   # API, authentication, location, rental and review services
│   ├── ViewModels/                 # MVVM ViewModels
│   └── Views/                      # XAML pages and code-behind
├── StarterApp.Database/            # Shared models, DbContext and repository interfaces
│   ├── Data/
│   │   └── Repositories/           # IRepository<T> and domain-specific interfaces
│   └── Models/                     # Item, Rental, Review, User, Category etc.
├── StarterApp.Migrations/          # EF Core database migrations
├── StarterApp.Tests/               # xUnit test project (91 tests)
├── docker-compose.yml              
├── .gitignore
├── README.md
└── LibraryOfThings.sln
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Android SDK](https://developer.android.com/studio) (API level 21+)
- Visual Studio Code with C# Dev Kit extension

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/nvb33/LibraryOfThings.git
cd LibraryOfThings
```

### 2. Start the PostgreSQL database

```bash
docker-compose up -d
```

### 3. Configure database connection

Create `StarterApp.Database/appsettings.json` (excluded from version control):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost:5432;Username=app_user;Password=your_password;Database=appdb"
  }
}
```

### 4. Apply database migrations

```bash
dotnet ef database update --project StarterApp.Database --startup-project StarterApp.Migrations
```

### 5. Restore dependencies

```bash
dotnet restore
```

---

## Running the Application

### Build and deploy to Android device or emulator

Connect an Android device with USB debugging enabled, or start an emulator, then:

```bash
dotnet build StarterApp -f net10.0-android -c Debug
```

Install the generated APK:

```bash
adb install -r StarterApp/bin/Debug/net10.0-android/com.companyname.starterapp-Signed.apk
```

---

## Running the Tests

```bash
dotnet test StarterApp.Tests
```

Expected output: **91 tests, 0 failures**

To run with detailed output:

```bash
dotnet test StarterApp.Tests --verbosity normal
```

---

## API Documentation

The application connects to a shared REST API:

**Base URL:** `https://set09102-api.b-davison.workers.dev`

Key endpoints:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /auth/token | Authenticate and receive JWT token |
| POST | /auth/register | Register a new account |
| GET | /items | List all available items |
| GET | /items/nearby | Search items by location |
| POST | /items | Create a new item listing |
| POST | /rentals | Create a rental request |
| PATCH | /rentals/{id}/status | Update rental status |
| GET | /rentals/outgoing | Get rentals where current user is borrower |
| GET | /rentals/incoming | Get rental requests for current user's items |
| POST | /reviews | Submit a review |
| GET | /items/{id}/reviews | Get reviews for an item |

---

## Architecture Overview

The application follows a five-layer architecture:

```
┌─────────────────────────────────────────┐
│           Views (XAML pages)            │
│  ItemsListPage · RentalsPage · etc.     │
└──────────────────┬──────────────────────┘
                   │ data binding
┌──────────────────▼──────────────────────┐
│              ViewModels                 │
│  ItemsListViewModel · RentalsViewModel  │
└──────────────────┬──────────────────────┘
                   │ business rules
┌──────────────────▼──────────────────────┐
│        Business Logic Services          │
│     IRentalService · IReviewService     │
└──────────────────┬──────────────────────┘
                   │ data access
┌──────────────────▼──────────────────────┐
│              Repositories               │
│  IItemRepository · IRentalRepository    │
│         IReviewRepository               │
└──────────────────┬──────────────────────┘
                   │ HTTPS / JWT
┌──────────────────▼──────────────────────┐
│            API Services                 │
│      IApiService · ILocationService     │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│    REST API + PostgreSQL Database       │
│   set09102-api.b-davison.workers.dev    │
└─────────────────────────────────────────┘
```

**Key design decisions:**

- **API-first** — all item and rental data comes from the shared API, not the local database
- **Repository pattern** — `IRepository<T>` with domain-specific interfaces in `StarterApp.Database`, concrete implementations in `StarterApp`
- **Service layer** — `RentalService` and `ReviewService` enforce business rules (valid state transitions, review eligibility) before delegating to repositories
- **Dependency Injection** — all services, repositories and ViewModels registered in `MauiProgram.cs`
- **Interface-based design** — all services and repositories defined as interfaces, enabling mocking in unit tests

---

## CI/CD Pipeline

The project uses GitHub Actions with two workflows:

**`build.yml`** — triggers on pull requests and pushes to main:

1. Checkout code with full history
2. Setup .NET 10
3. Install SonarCloud scanner and coverage tool
4. Restore workloads and dependencies
5. Begin SonarCloud analysis
6. Build project
7. Run 91 unit tests with coverage collection
8. End SonarCloud analysis — results uploaded to sonarcloud.io
9. Build Android APK
10. Upload APK as downloadable artifact

**`documentation.yml`** — triggers on push to main:

1. Install Doxygen and Graphviz
2. Generate HTML documentation from XML comments
3. Upload HTML docs as artifact

---

## Code Quality

SonarCloud analyses every push to main and every pull request:

- Zero code smells
- Zero security vulnerabilities
- Zero reliability issues

Full report: https://sonarcloud.io/project/overview?id=nvb33_LibraryOfThings

---

## Running with Docker

The `docker-compose.yml` sets up a local PostgreSQL instance:

```bash
# Start database
docker-compose up -d

# Stop database
docker-compose down
```

The database connection string is configured via the `DATABASE_CONNECTION_STRING` environment variable or via `appsettings.json` (see Setup Instructions above).
