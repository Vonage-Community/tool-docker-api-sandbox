using EnumsNET;

namespace DockerApiSandbox.Api.OperationIdentification;

public record ApiSpecification(SupportedApi SupportedApi, string Url)
{
    public ApiSpecification Overwrite(IEnvironmentAdapter environment)
    {
        var variable = environment.GetVariable(this.SupportedApi.AsString(EnumFormat.Description)!);
        return string.IsNullOrEmpty(variable) ? this : this with { Url = variable };
    }
}