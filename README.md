# Library of Things — Peer-to-Peer Rental Marketplace

A .NET MAUI Android application for peer-to-peer rental of everyday items, built as part of the Edinburgh Napier University SET09102 Software Engineering module.

## Project Overview

Library of Things allows users to:
- Browse and list items available for rent
- Search for items nearby using location
- Request, approve and manage rentals end-to-end
- Leave reviews after completed rentals

The app communicates with a shared REST API backend and follows the MVVM architecture pattern with dependency injection.

---

## Repository Structure

LibraryOfThings/
├── .github/
│   └── workflows/
│       ├── build.yml               # CI/CD pipeline (build, test, APK)
│       └── documentation.yml       # Doxygen documentation generation
├── StarterApp/                     # .NET MAUI Android app
│   ├── Converters/                 # Value converters for XAML bindings
│   ├── Services/                   # API service layer (IApiService, ApiService)
│   ├── ViewModels/                 # MVVM ViewModels
│   └── Views/                      # XAML pages and code-behind
├── StarterApp.Database/            # Shared models and DbContext
│   ├── Data/                       # AppDbContext
│   └── Models/                     # Item, Rental, Review, User etc.
├── StarterApp.Migrations/          # EF Core database migrations
├── StarterApp.Tests/               # xUnit test project
├── docker-compose.yml              # PostgreSQL container
├── .gitignore
├── README.md
└── LibraryOfThings.sln

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

### 3. Apply database migrations

```bash
dotnet ef database update --project StarterApp.Database --startup-project StarterApp.Migrations
```

### 4. Restore dependencies

```bash
dotnet restore
```

---

## Running the Application

### Build and deploy to Android device

Connect an Android device with USB debugging enabled, then:

```bash
dotnet build StarterApp -f net10.0-android -c Debug
```

Install the generated APK from:

StarterApp/bin/Debug/net10.0-android/com.companyname.starterapp-Signed.apk

---

## Running the Tests

```bash
dotnet test StarterApp.Tests
```

Expected output: **45 tests, 0 failures**

To run with detailed output:

```bash
dotnet test StarterApp.Tests --verbosity normal
```

---

## API Documentation

The application connects to a shared REST API:

**Base URL:** `https://set09102-api.b-davison.workers.dev`

Key endpoints:
- `POST /auth/login` — authenticate and receive JWT token
- `GET /items` — list all available items
- `GET /items/nearby` — search items by location
- `POST /rentals` — create a rental request
- `PATCH /rentals/{id}/status` — update rental status
- `POST /reviews` — submit a review
- `GET /items/{id}/reviews` — get reviews for an item

---

## Architecture Overview

The application follows the **MVVM (Model-View-ViewModel)** pattern:

Views (XAML)
↕ Data Binding
ViewModels (CommunityToolkit.Mvvm)
↕ Dependency Injection
Services (IApiService / ApiService)
↕ HTTP / JSON
REST API (set09102-api.b-davison.workers.dev)
↕
PostgreSQL Database

**Key design decisions:**
- **API-first** — all item and rental data comes from the shared API, not local database
- **Dependency Injection** — all services and ViewModels registered in `MauiProgram.cs`
- **Interface-based services** — `IApiService` and `IAuthenticationService` allow mocking in tests
- **Converters** — `BoolToColorConverter`, `EqualToStringConverter`, `StringToBoolConverter`, `InvertedBoolConverter` handle UI state without logic in Views

---

## CI/CD Pipeline

The project uses GitHub Actions with two workflows:

**`build.yml`** — triggers on pull requests:
1. Checkout code
2. Setup .NET 10
3. Restore workloads and dependencies
4. Build project
5. Run unit tests
6. Upload APK as artifact

**`documentation.yml`** — triggers on push to main:
1. Generate Doxygen documentation
2. Upload HTML docs as artifact

---

## Running with Docker

The `docker-compose.yml` sets up a local PostgreSQL instance:

```bash
# Start database
docker-compose up -d

# Stop database
docker-compose down
```

Connection string (development):

Host=localhost;Database=starterdb;Username=student_user;Password=password123;
## Code Quality

This project uses SonarCloud for continuous code quality analysis.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=coverage)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=nvb33_LibraryOfThings&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=nvb33_LibraryOfThings)

SonarCloud analyses every pull request automatically, checking for:
- Code smells and maintainability issues
- Security vulnerabilities
- Test coverage
- Duplicated code

To view the full quality report visit:
https://sonarcloud.io/project/overview?id=nvb33_LibraryOfThings
