using EnumsNET;

namespace DockerApiSandbox.Api.OperationIdentification;

public record ApiSpecification(SupportedApi SupportedApi, string Url)
{
    public ApiSpecification Overwrite(IEnvironmentAdapter environment)
    {
        var variable = environment.GetVariable(this.SupportedApi.AsString(EnumFormat.Description)!);
        return variable.Match(some => this with {Url = some}, () => this); 
    }
}