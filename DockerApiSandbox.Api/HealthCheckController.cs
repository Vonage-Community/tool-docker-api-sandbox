#region
using Microsoft.AspNetCore.Mvc;
#endregion

namespace DockerApiSandbox.Api;

[ApiController]
[Route("_")]
public class HealthCheckController(IEnvironmentAdapter environment) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult HealthCheck() => this.Ok();

    [HttpGet("environment/{name}")]
    public IActionResult GetEnvironmentVariable(string name)
    {
        var value = environment.GetVariable(name);
        return value is null ? this.NotFound() : this.Ok(value);
    }
}