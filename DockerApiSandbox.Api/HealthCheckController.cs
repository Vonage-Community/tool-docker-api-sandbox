#region
using DockerApiSandbox.Api.OperationIdentification;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace DockerApiSandbox.Api;

[ApiController]
[Route("_")]
public class HealthCheckController(IEnvironmentAdapter environment, IDocumentStore documentStore) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult HealthCheck() => this.Ok(documentStore.GetLoadedApis().Select(supportedApi => supportedApi.ToString()).ToArray());

    [HttpGet("environment/{name}")]
    public IActionResult GetEnvironmentVariable(string name)
    {
        var value = environment.GetVariable(name);
        return value.Match(IActionResult (some) => this.Ok(some), this.NotFound);
    }
}