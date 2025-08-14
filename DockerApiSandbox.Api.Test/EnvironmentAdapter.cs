using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.Test;

public record EnvironmentAdapter(Dictionary<string, string> Variables) : IEnvironmentAdapter
{
    public bool HasVariable(string key) => this.Variables.ContainsKey(key);

    public Maybe<string> GetVariable(string key) => this.HasVariable(key) ? this.Variables[key] : Maybe<string>.None;
}