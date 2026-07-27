# TestAutomationShowcase

A C#/.NET test automation project covering both **UI** and **API** testing, built with **Playwright**, **NUnit**, and **Reqnroll (BDD/Gherkin)**.

It runs against two public demo apps - [SauceDemo](https://www.saucedemo.com) for UI, [Restful-Booker](https://restful-booker.herokuapp.com) for API - but I built the framework the same way I'd build one for a real product: shared core library, dependency injection, layered config, a proper HTTP pipeline, and reporting from day one.

## Two suites, on purpose

`Tests` (plain NUnit) and `GherkinTests` (Reqnroll/BDD) test the exact same functionality, twice. That's intentional - I wanted one repo where I could show I'm equally comfortable in both styles.

| | NUnit (code-first) | Reqnroll/Gherkin (BDD) |
|---|---|---|
| Best for | fast, engineer-to-engineer tests, technical edge cases | living documentation, review with QA/BA/product |
| Readability for non-engineers | low, it's C# | high, plain-language scenarios |
| Speed to add a new test | fast, no feature file to keep in sync | slower, feature file + step definitions both need updating |
| Reuse across tests | helper methods/base classes | built in, via Given/When/Then steps and `Background` |
| Data-driven tests | `[TestCase]`/`[TestCaseSource]`, still C# | `Scenario Outline` + `Examples` tables, reads fine even to non-engineers |
| Refactoring safety | strong, compiler/IDE tooling all the way | weaker, step text is matched by string/regex and breaks quietly |
| Debugging | step straight into the test | one extra hop (step binding → page object/client) to trace through |
| Reporting | Allure, technical test names | Allure, readable scenario names, better for stakeholders |
| Onboarding | low if you know C#/NUnit | higher, needs some Gherkin discipline to avoid step sprawl |

My honest take: I default to NUnit for API/technical coverage where the audience is other engineers, and reach for Gherkin when non-technical people actually need to read and sign off on the scenarios, or when the business rules themselves are the thing worth documenting. Neither one wins outright - that's really the point of putting both here.

## Test design, not just test code

The tests themselves follow standard test design techniques, not just whatever came to mind first:

- **Equivalence partitioning** - login is tested once per class of user (valid, locked-out, wrong password, empty credentials), not a pile of usernames that would all fail the same way.
- **Boundary values / error guessing** - things like a booking ID that doesn't exist (`99999999`), or empty required UI fields, are picked on purpose to hit validation paths.
- **Decision-table-style data-driven tests** - the login feature's `Scenario Outline` + `Examples` map persona → expected error as a table, so a new negative case is a new row, not new code.
- **State transition / workflow coverage** - the UI suite runs a real journey (login → add to cart → checkout → confirmation) instead of testing each page as an island, which is where integration bugs actually show up.
- **Positive/negative pairing per operation** - every API mutation (create/update/partial-update/delete) has both an authenticated-success test and an unauthenticated/invalid-ID test next to it.
- **Verifying state, not just status codes** - several API scenarios re-fetch the resource after writing to it (*"Created booking can be retrieved"*, *"Updated booking can be retrieved"*), because a 200 doesn't always mean the write actually stuck.
- **Consistent Arrange-Act-Assert** - `[SetUp]`/`Background` for setup, one focused action, one assertion concern per test, across both suites, so a failure tells you something specific.

## Architecture

```
TestAutomationShowcase.Configuration   → strongly-typed settings, layered config sources
TestAutomationShowcase.Core            → shared library: API clients, Page Objects, models, HTTP pipeline
TestAutomationShowcase.Tests           → "classic" NUnit test suite (UI + API)
TestAutomationShowcase.GherkinTests    → BDD test suite (Reqnroll/Gherkin), same Core underneath
```

Both suites consume the same `Core` project rather than each keeping its own Page Objects/API clients. `Tests` wires things up with plain `Microsoft.Extensions.DependencyInjection`; `GherkinTests` wires the same classes through Reqnroll's `IObjectContainer`. Nothing UI- or API-specific is written twice - only the composition root and the test-writing style differ. See [Two suites, on purpose](#two-suites-on-purpose) above for why I set it up that way.

### HTTP layer: handlers instead of if-statements

`HttpClientFactory` builds one `HttpClient` per composition root, chaining two `DelegatingHandler`s:

```
AuthHandler → LoggingHandler → HttpClientHandler
```

- `AuthHandler` adds the auth cookie only for state-changing verbs (`PUT`/`DELETE`/`PATCH`), and can be skipped per-request via a typed `HttpRequestOptions.SkipAuthentication` flag - that's what the "unauthenticated request should fail" tests use.
- `LoggingHandler` logs every request/response through `ApiTestLogger` straight into the NUnit `TestContext`, so when something fails you can see the actual HTTP traffic in the test output/CI logs without digging.
- `TokenProvider` caches the token behind a `SemaphoreSlim` so parallel tests share one token instead of each one re-authenticating.

Auth and logging live in handlers rather than scattered through the API clients, so any new client gets both for free just by going through the factory/DI registration.

### API clients: typed, one method per endpoint

`BaseApiClient.SendAsync<T>` handles serialization, deserialization, and status codes in one place and hands back an `ApiResponse<T>` (status + typed value + raw body). On top of that, `BookingClient` is basically one line per endpoint - adding a new one shouldn't mean copy-pasting the whole request/response implementation.

### UI layer: Page Object Model, test-id first

Page objects (`LoginPage`, `ProductsPage`, `CartPage`, `CheckoutPage`) expose methods that say what they do (`LoginAsync`, not `Fill` then `Click`), and locate elements via Playwright's `data-test` attribute rather than CSS/XPath - the more resilient choice whenever the app already exposes test hooks.

### Config: layered, nothing secret in source control

`ConfigReader` builds config from `appsettings.json` → optional `appsettings.local.json` → environment variables, in that order, so local overrides and CI secrets both win over the committed defaults without any credentials sitting in the repo. Browser behavior (headless, slow-mo, timeouts) is config-driven too, so the same suite runs headless in CI and visibly on my machine.

### Reporting

Both suites are wired for Allure (`Allure.NUnit` / `Allure.Reqnroll`) with suite/feature tags, so results from either style land in one consistent report.

### CI

Every push and pull request runs both suites in GitHub Actions, builds a combined Allure report with run history, and publishes it to GitHub Pages - **[live report](https://iannik.github.io/TestAutomationShowcase)**. Pull requests also get a pass/fail test summary posted directly via `dorny/test-reporter`, so results show up as a check without anyone needing to open the Actions tab.

## Tech stack

| Concern            | Choice                                   |
|--------------------|-------------------------------------------|
| Language / runtime | C#, .NET 8                                |
| UI automation      | Playwright                                |
| API testing        | `HttpClient` + custom typed clients        |
| Test runners       | NUnit, Reqnroll (Gherkin/BDD)             |
| Reporting          | Allure                                    |
| DI                 | Microsoft.Extensions.DependencyInjection / Reqnroll `IObjectContainer` |
| Config             | `Microsoft.Extensions.Configuration`, layered JSON + env vars |
| CI/CD              | GitHub Actions - build, test, and Allure report publishing on every push/PR |

## Getting started

```bash
git clone https://github.com/iannik/TestAutomationShowcase.git
cd TestAutomationShowcase
dotnet restore
```

Add an `appsettings.local.json` next to `appsettings.json` in `TestAutomationShowcase.Configuration` (it's git-ignored) with real credentials for the demo sites:

```json
{
  "RestfulBookerCredentials": { "Username": "...", "Password": "..." },
  "SauceDemoCredentialsStandard": { "Username": "standard_user", "Password": "secret_sauce" },
  "SauceDemoCredentialsLocked": { "Username": "locked_out_user", "Password": "secret_sauce" }
}
```

Then run either suite:

```bash
dotnet test TestAutomationShowcase.Tests            # NUnit suite (UI + API)
dotnet test TestAutomationShowcase.GherkinTests      # Reqnroll/Gherkin suite (UI + API)
```

## What's covered

- **UI:** login (standard/locked-out/invalid credentials), cart, checkout - SauceDemo
- **API:** full CRUD plus auth-required/unauthenticated negative paths for bookings - Restful-Booker

## About me

Test Automation Engineer working across UI and API automation - C#/Playwright/NUnit/Reqnroll day to day, plus Python/Pytest/Selenium. Comfortable with backend and RBAC testing and wiring automation into CI/CD. ISTQB Certified Tester Foundation Level; PCEP Certified Entry-Level Python Programmer.