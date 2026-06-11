# Add Product Test Suite

This skill adds full test coverage for a new or existing Vonage API product under `DockerApiSandbox.Api.Test/Products/`.

## Step 1 — Gather information

Ask the user for all of the following before touching any file:

1. **Product name** — the PascalCase name used for directories and the C# class (e.g. `NumberInsight`).
2. **OAS spec** — a file path, a URL, or a paste of the raw JSON/YAML. If a URL is given, fetch it.
3. **Is this API already in `SupportedApi`?** — check `DockerApiSandbox.Api/OperationIdentification/DocumentStore.cs`. If the product is NOT already there, also collect:
   - **Enum key** — `SupportedApi` enum member name, PascalCase (e.g. `NumberInsight`).
   - **Env-var name** — the `SPEC_` environment variable (e.g. `SPEC_NUMBER_INSIGHT`).
   - **Portal URL slug** — the path segment used in `https://developer.vonage.com/api/v1/developer/api/file/{slug}?format=json` (e.g. `number-insight`).
4. **Scope** — all endpoints (default) or a specific subset? If a subset, ask which ones.

Do not proceed until all required information is confirmed.

## Step 2 — Read and analyse the OAS spec

Parse the spec and build an endpoint inventory. For every operation (`paths → path → method`):

- HTTP method and full path (template variables like `{id}` are kept as-is in test URLs — replace them with a readable placeholder, e.g. `ID-123`).
- Authentication schemes (`securityRequirement` on the operation, or the global `security` block). Map to the test helper values:
  - `basic` / `basicAuth` / `http scheme: basic` → `"Basic"`
  - `bearer` / `bearerAuth` / `http scheme: bearer` → `"Bearer"`
  - `oauth2` / any OAuth2 flow → `"oauth2"`
  - No security → no `WithAuthorizationHeader` call.
- Whether a request body is required and its media type (JSON bodies are the norm).
- The success response code (use the lowest 2xx defined in the spec).
- Any `example` / `examples` / `x-example` values on the request body schema — these are the primary source for JSON test files.

Print a summary table so the user can review before any code is generated:

```
| Method | Path                        | Auth    | Body file               | Expected |
|--------|-----------------------------|---------|-------------------------|----------|
| GET    | /v1/foo/bars                | Basic   | —                       | 200      |
| POST   | /v1/foo/bars                | Basic   | CreateBar.json          | 201      |
| DELETE | /v1/foo/bars/{barId}        | Basic   | —                       | 204      |
```

Ask the user to confirm or correct the table before continuing.

## Step 3 — Wire up plumbing (new API only)

Skip this step if the API is already in `SupportedApi`.

### 3a — `DocumentStore.cs`

In `DockerApiSandbox.Api/OperationIdentification/DocumentStore.cs`, append a new member to the `SupportedApi` enum following the established pattern:

```csharp
[EnumMember(Value = "{EnumKey}")] [Description("{SPEC_ENV_VAR}")]
{EnumKey},
```

### 3b — `TestApplicationFactory.cs`

In `DockerApiSandbox.Api.Test/TestApplicationFactory.cs`, add a new `Override*Spec` method to `ApplicationBuilder<TStartup>`:

```csharp
public ApplicationBuilder<TStartup> Override{ProductName}Spec(string value)
{
    this.variables.Add("{SPEC_ENV_VAR}", value);
    return this;
}
```

### 3c — `appsettings.Development.json`

In `DockerApiSandbox.Api/appsettings.Development.json`, add an entry to the `specs` array:

```json
{
  "SupportedApi": "{EnumKey}",
  "Url": "https://developer.vonage.com/api/v1/developer/api/file/{slug}?format=json"
}
```

## Step 4 — Create the test class

Create the file `DockerApiSandbox.Api.Test/Products/{ProductName}/{ProductName}Test.cs`.

Follow these rules exactly:

- Namespace: `DockerApiSandbox.Api.Test.Products.{ProductName}`.
- Primary constructor: `(ITestOutputHelper helper)`.
- Single field: `private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper).Build();`.
- No spec override — product tests run against the live portal spec.
- Every test method is `async Task`.
- Group related operations under one `[Theory]` + `[InlineData]` method when they share the same URL pattern and differ only in body (e.g. multiple Create variants). Otherwise use `[Fact]`.
- Test naming convention: `{Operation}_{Condition}` — keep it concise and readable (e.g. `CreateCall_ShouldReturnCreated`, `GetAddresses`, `Cancel_ShouldReturnNoContent`).
- For path parameters, replace the template variable with a readable stub: `{callId}` → `CALL-123`, `{requestId}` → `REQ-123`, `{templateId}` → `TEM-123`, `{uuid}` → a lowercase UUID-shaped string, etc.
- Auth header value is always `"TEST"` — the sandbox accepts any non-empty string. Use `WithAuthorizationHeader("{scheme}")` where `{scheme}` is the string from Step 2.
- Body files are referenced via `Path.GetFullPath("Products/{ProductName}/Files/{FileName}.json")`.

Template:

```csharp
#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.{ProductName};

public class {ProductName}Test(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper).Build();

    // ... test methods
}
```

## Step 5 — Create JSON payload files

Create `DockerApiSandbox.Api.Test/Products/{ProductName}/Files/` and populate it.

For each operation that requires a request body:

1. **Extract from spec** — look in this priority order:
   - `requestBody.content["application/json"].examples` (take the first entry's `value`)
   - `requestBody.content["application/json"].example`
   - `requestBody.content["application/json"].schema.example`
   - The spec's top-level `components.examples`
2. **Derive from schema** — if no example exists, construct a minimal valid JSON object using the schema's `required` fields and `properties`. For required string fields, use a short descriptive placeholder (`"15550900000"` for phone numbers, `"Hello"` for text, etc.). For required integer/number fields, use the minimum value if specified, otherwise `1`.
3. **File naming** — match the test method name it belongs to (e.g. `CreateCall_AnswerUrl_Phone.json`). Use `PascalCase`. If a single body file covers multiple `[InlineData]` variants, the filename should reflect the variant.

Always write pretty-printed JSON (2-space indent).

## Step 6 — Update `README.md`

In `README.md`, add a new row to the "Supported APIs" table. Keep rows sorted alphabetically by API name:

```markdown
| {ProductName} API | ✅ |
```

## Step 7 — Update the `.csproj`

In `DockerApiSandbox.Api.Test/DockerApiSandbox.Api.Test.csproj`, add a `<None Update="...">` entry for every new JSON file created in Step 5. Add them inside the existing `<ItemGroup>` block that contains the other `<None Update>` entries:

```xml
<None Update="Products\{ProductName}\Files\{FileName}.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</None>
```

Use backslashes in the `Update` attribute value (Windows-style path separators are required by the MSBuild convention already in use here).

## Step 8 — Verify

Run the new tests:

```
dotnet test DockerApiSandbox.Api.Test --filter "FullyQualifiedName~Products.{ProductName}"
```

**If all tests pass:** report success.

**If a test fails**, first distinguish between a scaffolding mistake and a missing sandbox feature:

- **Scaffolding mistake** (wrong path, wrong method, wrong auth scheme, malformed JSON body) — fix the test or the JSON file and re-run. These are your errors, not sandbox limitations.
- **Missing sandbox feature** (correct path/method/body but the sandbox returns the wrong status or response) — this means the sandbox does not yet support something specific in this API's spec.

For a missing sandbox feature, **invoke `/implement-endpoint` directly** — do not ask the user to run it manually. Pass all diagnostic context in the args:
- The failing HTTP method and path
- The status code received vs. expected
- The product name (`{ProductName}`)
- The OAS spec path or URL (so `/implement-endpoint` can re-read it rather than work from a summary)
- Your hypothesis about which middleware layer is responsible

After `/implement-endpoint` completes, re-run the product tests. If a different endpoint is now failing (the fix exposed a second sandbox gap), invoke `/implement-endpoint` again for that endpoint. Repeat until either all tests pass or a fix attempt fails to make progress — in that case stop and report the remaining failure to the user rather than looping further.

Do not skip, comment out, or work around a failing test without explicit user approval.
