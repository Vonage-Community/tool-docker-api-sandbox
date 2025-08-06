namespace DockerApiSandbox.Api;

public interface IEnvironmentAdapter
{
    bool HasVariable(string key);
    string GetVariable(string key);
}

internal class EnvironmentAdapter : IEnvironmentAdapter
{
    public bool HasVariable(string key) => Environment.GetEnvironmentVariable(key) != null;

    public string GetVariable(string key) => Environment.GetEnvironmentVariable(key);
}