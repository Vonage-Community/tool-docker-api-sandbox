using Vonage.Common.Monads;

namespace DockerApiSandbox.Api;

public interface IEnvironmentAdapter
{
    bool HasVariable(string key);
    Maybe<string> GetVariable(string key);
}

internal class EnvironmentAdapter : IEnvironmentAdapter
{
    public bool HasVariable(string key) => Environment.GetEnvironmentVariable(key) != null;

    public Maybe<string> GetVariable(string key) => Environment.GetEnvironmentVariable(key) ?? Maybe<string>.None;
}