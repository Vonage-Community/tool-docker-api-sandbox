# Implement Feature Support (TDD Double Loop)

This skill fixes a failing product test for a specific endpoint using a TDD double loop:
the outer loop is the E2E product test, the inner loop is a focused feature test that isolates
the exact middleware behavior that needs to change.

Use this skill when:
- `/add-product` reports a failing product test
- A previously passing product test regresses
- You need to add support for a specific endpoint in isolation

## Step 1 — Gather context

Ask the user for:

1. **Failing endpoint** — HTTP method + full path (e.g. `POST /v1/verify/templates`).
2. **Test output** — the full failure message from `dotnet test` (status code received, exception, etc.).
3. **Product name** — the `Products/{ProductName}` folder the failing test lives in.
4. **OAS spec** — file path or URL for the API spec; fetch it if a URL is given.

Do not assume anything about the failure mode until you have the test output.

## Step 2 — Diagnose which middleware layer is failing

The sandbox processes every request through this ordered pipeline:

```
OperationIdentification → AuthenticationVerification → InputValidation → OutputGeneration → Callback
```

Map the observed failure to a layer using this table:

| Observed (received)         | Expected       | Likely layer              |
|-----------------------------|----------------|---------------------------|
| 404 Not Found               | Any 2xx        | OperationIdentification   |
| 401 Unauthorized            | Any 2xx        | AuthenticationVerification|
| 400 Bad Request             | Any 2xx        | InputValidation           |
| Wrong 2xx (e.g. 200 vs 201) | Specific 2xx   | OutputGeneration          |
| Empty / wrong body          | JSON response  | OutputGeneration          |
| 500 Internal Server Error   | Anything       | Unhandled exception — investigate logs |
| No callback delivered       | Callback fired | Callback                  |

Read the Serilog test output carefully — it logs `"Operation identified"`, `"Input validated"`, etc. at each stage. The last logged stage before silence or an error message reveals the failing layer.

Cross-check with the OAS spec to understand what the endpoint is supposed to do:
- Does it use an unusual auth scheme not seen in other endpoints?
- Does the request body use `oneOf`, `anyOf`, form encoding, or unusual content types?
- Does the response schema reference deep `$ref` chains or unusual types?
- Does the OAS define a callback/webhook?

State your diagnosis clearly and explain your reasoning before writing any code.

## Step 3 — Write a failing Feature test (inner loop, red)

Feature tests live in `DockerApiSandbox.Api.Test/Features/{Layer}/`.

Existing feature test directories and their scope:
- `Features/Identification/` — operation matching (path patterns, verb, query)
- `Features/Authentication/` — auth scheme validation (Basic, Bearer, OAuth2, missing auth)
- `Features/InputValidation/` — query params, request bodies (JSON, form), schema constraints
- `Features/OutputGeneration/` — response body generation, status code selection
- `Features/Callback/` — webhook delivery

### Choosing where to add the test

Add the test to the directory whose scope matches your diagnosis. If the failure spans two layers, start with the earlier one in the pipeline.

### Writing a minimal spec

Feature tests use a **local, minimal OAS spec** that contains only what is needed to reproduce the specific behavior. Create or extend `Features/{Layer}/Files/spec.json`.

The spec must:
- Define only the endpoints needed for the test (typically one).
- Include the exact auth scheme, body schema, or response schema that triggers the issue.
- Be valid OpenAPI 3.x JSON.

Refer to existing spec files in other Feature directories to understand the expected format and level of detail. Keep the spec as small as possible.

### Test class conventions

Feature tests follow the same infrastructure as product tests but use spec overrides:

```csharp
private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper)
    .OverrideApplicationSpec(Path.GetFullPath("Features/{Layer}/Files/spec.json"))
    .WithEnvironmentVariable("CLEAR_SPECS", "true")
    .Build();
```

- Always use `CLEAR_SPECS=true` so no other specs interfere.
- Use any `Override*Spec` method — it doesn't matter which API is overridden since all others are cleared.
- Name the test method to precisely describe the expected behavior: `ShouldReturn{Status}_Given{Condition}`.
- If the existing test class already covers adjacent behavior, add the new test method to it rather than creating a new class.

### Add the new JSON/spec file to the `.csproj`

Every new file under `Features/` must be registered in `DockerApiSandbox.Api.Test/DockerApiSandbox.Api.Test.csproj`:

```xml
<None Update="Features\{Layer}\Files\{FileName}">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

### Run the Feature test — expect it to fail (red)

```
dotnet test DockerApiSandbox.Api.Test --filter "FullyQualifiedName~Features.{Layer}"
```

If it passes immediately, the spec or test is not correctly reproducing the issue — revise before continuing.

## Step 4 — Implement the fix

### Locate the relevant implementation file

| Layer                    | Implementation file(s)                                                   |
|--------------------------|--------------------------------------------------------------------------|
| OperationIdentification  | `DockerApiSandbox.Api/OperationIdentification/OperationIdentificationMiddleware.cs`, `Product.cs` |
| AuthenticationVerification | `DockerApiSandbox.Api/AuthenticationVerification/AuthenticationVerificationMiddleware.cs` |
| InputValidation          | `DockerApiSandbox.Api/InputValidation/InputValidationMiddleware.cs`, `HttpRequestExtensions.cs`, `OpenApiParameterExtensions.cs` |
| OutputGeneration         | `DockerApiSandbox.Api/OutputGeneration/OutputGenerationMiddleware.cs`, `DataGenerator.cs`, `ResponseSchema.cs` |
| Callback                 | `DockerApiSandbox.Api/Callback/CallbackMiddleware.cs`, `EndpointExtensions.cs` |

Read the relevant files fully before writing a single line. Understand the existing pattern before extending it.

### Implementation rules

- Make the **minimum change** that makes the feature test pass. Do not refactor or add abstractions beyond what the task requires.
- Follow existing patterns exactly — naming, structure, style.
- Do not add comments unless the reason for the change is non-obvious.
- Do not add error handling for cases that cannot happen given the sandbox's design constraints (see `CLAUDE.md`).

### Run the Feature test — expect it to pass (green)

```
dotnet test DockerApiSandbox.Api.Test --filter "FullyQualifiedName~Features.{Layer}"
```

If it still fails, read the test output and revise the implementation. Do not proceed until this test is green.

## Step 5 — Close the outer loop

Run the original failing product test:

```
dotnet test DockerApiSandbox.Api.Test --filter "FullyQualifiedName~Products.{ProductName}"
```

If it passes: report success — both loops are closed.

If it still fails: the fix may have addressed only part of the issue, or there is a second failing layer. Return to Step 2 and repeat for the new failure.

## Step 6 — Run the full test suite

Once both loops are green, run the entire test suite to catch regressions:

```
dotnet test DockerApiSandbox.Api.Test
```

If any previously passing test now fails, investigate and fix before reporting done. Do not comment out or skip tests.

Report the final outcome: which feature test was added, what implementation change was made, and that the full suite passes.
