# Claude Context: Vonage API Sandbox

## Project Overview
**Name:** Vonage API Sandbox  
**Type:** API simulation/testing tool  
**Technology:** C#/.NET 8.0 with ASP.NET API  
**Purpose:** Provides a local Docker-based environment that simulates Vonage APIs without making real API calls  
**Repository:** https://github.com/Vonage-Community/tool-docker-api-sandbox

## What This Project Does
The Vonage API Sandbox is an API server with no predefined endpoints. Instead, it:

1. **Loads OpenAPI specifications** from the Vonage Developer Portal at startup (`https://developer.vonage.com/api/v1/developer/api/file/{product}`)
2. **Dynamically creates endpoints** based on these specifications
3. **Simulates API responses** using the spec definitions
4. **Validates requests** against the OpenAPI schemas
5. **Generates fake data** for responses when needed
6. **Handles callbacks** defined in specs by making HTTP requests to configured callback URLs

## Key Architecture
- **No real API calls**: Everything is mocked/simulated
- **OpenAPI-driven**: Behavior is defined by OpenAPI specs, not hardcoded
- **Docker-based**: Runs as a containerized service
- **Middleware pipeline**: Requests go through a series of ASP.NET middlewares
- **Single source of truth**: OpenAPI specs should be aligned with production environment

## Request Flow (Middleware Pipeline)
```
HTTP Request → Operation Identification → Authentication Verification → Input Validation → Output Generation → Callback (if needed) → Return Response
```

## Project Structure
```
DockerApiSandbox.Api/
├── AuthenticationVerification/     # Auth middleware
├── Callback/                       # Callback middleware  
├── InputValidation/                # Input validation middleware
├── OperationIdentification/        # Endpoint identification middleware
└── OutputGeneration/               # Fake data generation middleware

DockerApiSandbox.Api.Test/
├── Features/                       # TDD-style tests for individual features (uses local spec overrides)
└── Products/                       # End-to-end tests for entire products (uses live specs from portal)

DockerApiSandbox.Api.Sample/        # Blazor demo app for live testing
```
- Applications API ✅
- IdentityInsights API ✅
- Messages API ✅
- SimSwap API ✅
- SMS API ✅
- Verify V2 API ⚠️ (partial)
- Voice API ✅

## Docker Configuration
**Image:** `tr00d/openapiexperiment-api:latest`

**Key Environment Variables:**
- `PORT`: Port to expose the API (mandatory)
- `CLEAR_SPECS=true`: Remove default specs (optional)
- `SPEC_SMS`: Custom SMS API spec file/URL (optional)
- `SPEC_APPLICATION`: Custom Application API spec (optional)
- `SPEC_VOICE`: Custom Voice API spec (optional)
- `SMS_DLR`: SMS delivery receipt callback URL (optional)

**Network:** Must use `--network=host` on Linux or port binding on Windows

## Authentication
- No real credentials required
- For endpoints expecting Basic/Bearer auth, just include any non-empty header value
- The sandbox only validates that auth headers are present, not their actual values

## Target Users & Use Cases
**Vonage Employees:**
- Validating OpenAPI specs before General Availability
- Testing unreleased APIs
- Automation and CI/CD pipelines

**Vonage Customers:**
- Integrating Vonage APIs without making real API calls or incurring charges
- Development and testing environment
- Easy swap to production when ready (SDK endpoints are configurable)

## Common Integration Patterns
**For .NET Projects:** Use TestContainers to spin up the sandbox Docker container during integration tests. Configure your Vonage SDK to point to the container instead of production endpoints.

**Testing Approach:**
- **Feature Tests:** Use `WebApplicationFactory` with local spec file overrides for specific testing scenarios
- **Product Tests:** Test against live specs from production portal (accepts risk of portal downtime affecting tests)
- Both approaches launch the API locally in the test suite

## Local Development Setup
1. **Clone the repository**
2. **Create `appsettings.json`** with spec configuration (see `appsettings.Development.json` example)
3. **Run the project** - it will automatically load specs from the configured URLs
4. **Optional:** Use the Blazor sample app (`DockerApiSandbox.Api.Sample`) for live testing and demos

## Adding New API Support
1. **Add spec configuration** to `appsettings.json` specs array
2. **Create new SupportedApi** in DocumentStore
3. **Add tests** for the new API in the test suite
4. **API should work automatically** unless the OpenAPI spec contains custom features requiring additional support

## Design Decisions & Rationale

### Error Handling & Spec Validation
- **Decision**: Minimal OpenAPI spec validation at startup
- **Rationale**: Specs published on Vonage Developer Portal are assumed well-formed and validated
- **Implementation**: Failed specs are skipped with logging rather than crashing the sandbox
- **Trade-off**: Prioritizes startup reliability over comprehensive validation

### Observability & Monitoring
- **Current State**: Serilog logging covers middleware pipeline progression, failures, fake output generation, callbacks, and response times
- **OpenTelemetry Consideration**: Under evaluation with careful attention to PII/secrets in request bodies
- **Approach**: Structured logging enables easy filtering of sensitive data
- **Monitoring**: Request method/path/status logging without exposing sensitive payloads

### Performance & Caching
- **Decision**: Document store implements spec caching with TTL
- **Response Generation**: Intentionally uses random data (not cached) to simulate real-world variability
- **Rationale**: Testing should experience different response scenarios, not deterministic outputs

### Testing Philosophy
- **Deterministic Testing Challenge**: Random response generation makes exact output testing impractical
- **Focus**: Validates request schemas and response structure rather than exact content values
- **Callback Testing**: Uses same fake data generation logic as responses; determinism avoided intentionally
- **Two-tier Approach**: Feature tests (controlled specs) vs Product tests (live portal specs)

### Configuration Strategy
- **Environment Variables**: Chosen for Docker deployment simplicity and standard container practices
- **Runtime Configuration**: Currently requires restart for changes (startup-time configuration)
- **Scope**: Focused on essential behavior control rather than granular response manipulation

### Authentication Model
- **Decision**: Presence-only validation (any non-empty auth header accepted)
- **Rationale**: Sandbox purpose is integration testing, not security validation
- **Benefit**: Eliminates credential management complexity for development/testing scenarios