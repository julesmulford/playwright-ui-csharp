# playwright-ui-csharp

> **GitHub repo description:** Enterprise-grade Playwright UI test framework — .NET 8, NUnit, FluentAssertions, Serilog, Allure, Page Object Model targeting OrangeHRM.

A production-ready Web UI test automation framework built with Microsoft Playwright for .NET, demonstrating enterprise patterns including Page Object Model, builder-based test data, structured logging with per-test correlation IDs, Allure reporting with rich failure evidence (screenshots, video, traces), and safe parallel execution.

## Why NUnit over xUnit?

NUnit is chosen because:
1. **Parallel execution control** via `[Parallelizable]` with `ParallelScope.Fixtures` or `ParallelScope.Self`
2. **Rich lifecycle hooks**: `[OneTimeSetUp]`/`[OneTimeTearDown]` for browser, `[SetUp]`/`[TearDown]` for per-test context
3. **`[Retry]` attribute** for first-class retry support on known-flaky interactions
4. **Allure.NUnit** has native first-class integration
5. Mature, widely adopted in enterprise .NET automation

## Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Language | C# 12 | — |
| Runtime | .NET | 8 |
| Browser Automation | Microsoft.Playwright | 1.44.0 |
| Test Framework | NUnit | 4.1.0 |
| Assertions | FluentAssertions | 6.12.0 |
| Logging | Serilog | 3.1.1 |
| Reporting | Allure.NUnit | 2.12.1 |
| Configuration | Microsoft.Extensions.Configuration | 8.0.0 |

## Project Structure

```
playwright-ui-csharp/
├── .github/workflows/ci.yml
├── src/PlaywrightUI.Tests/
│   ├── Config/
│   │   ├── TestConfiguration.cs
│   │   ├── appsettings.json
│   │   └── appsettings.CI.json
│   ├── Constants/AppConstants.cs
│   ├── Models/
│   │   ├── Employee.cs
│   │   └── TestUser.cs
│   ├── Data/
│   │   ├── EmployeeBuilder.cs
│   │   └── TestDataFactory.cs
│   ├── Fixtures/BrowserFixture.cs
│   ├── Pages/
│   │   ├── BasePage.cs
│   │   ├── LoginPage.cs
│   │   ├── DashboardPage.cs
│   │   ├── EmployeeListPage.cs
│   │   └── AddEmployeePage.cs
│   ├── Components/SideMenuComponent.cs
│   ├── Utilities/
│   │   ├── AllureHelper.cs
│   │   └── WaitHelper.cs
│   └── Tests/
│       ├── LoginTests.cs
│       ├── DashboardTests.cs
│       ├── EmployeeTests.cs
│       └── NavigationTests.cs
├── allureConfig.json
├── .editorconfig
├── .gitignore
└── playwright-ui-csharp.sln
```

## Prerequisites

- .NET 8 SDK
- PowerShell (`pwsh`) for Playwright browser installation

## Setup

```bash
git clone https://github.com/YOUR_USERNAME/playwright-ui-csharp.git
cd playwright-ui-csharp
dotnet restore
dotnet build
pwsh src/PlaywrightUI.Tests/bin/Debug/net8.0/playwright.ps1 install --with-deps
```

## Running Tests

```bash
# All tests
dotnet test

# Smoke suite
dotnet test --filter "Category=Smoke"

# Regression suite
dotnet test --filter "Category=Regression"

# Headless (CI) mode
TEST_ENVIRONMENT=CI dotnet test

# Specific test
dotnet test --filter "FullyQualifiedName~LoginTests.Login_WithValidCredentials_ShouldReachDashboard"
```

## Allure Reporting

```bash
# Install Allure CLI
npm install -g allure-commandline

# Run tests then generate report
dotnet test
allure generate allure-results --clean -o allure-report
allure open allure-report
```

## CI/CD

GitHub Actions workflow (`.github/workflows/ci.yml`):
1. Checkout and setup .NET 8
2. Restore and build
3. Install Playwright browsers with OS dependencies
4. Run all tests in headless mode with Allure output
5. Upload `allure-results` and `TestResults` as artifacts

## Architecture Decisions

**Page Object Model**: Each page inherits `BasePage` which provides typed Playwright helpers, retry-safe click/fill, and screenshot capture. Pages expose high-level business actions, not raw Playwright calls.

**Fixture Design**: `BrowserFixture` uses NUnit lifecycle hooks. Browser is created once per test class (`[OneTimeSetUp]`). A fresh `IBrowserContext` and `IPage` are created per test (`[SetUp]`) providing full test isolation.

**Test Data**: `EmployeeBuilder` produces deterministic, unique records using a `Guid` suffix strategy. `TestDataFactory` provides named presets for common scenarios.

**Retry Strategy**: `[Retry(2)]` on tests that interact with eventually-consistent UI. Underlying locator strategy uses role/label/text selectors to avoid brittle CSS.

**Logging**: Serilog writes structured logs enriched with `{CorrelationId}` per test, enabling filtered log analysis per test execution.

## Scaling Notes

- Add more `appsettings.{env}.json` files for environment matrix; select via `TEST_ENVIRONMENT` env var
- For multi-browser runs, parameterise `Browser.Type` in config and run with separate `dotnet test` invocations
- For sharded CI execution, use `--filter` with NUnit categories per runner
