#region
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test;

public class TestApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly ITestOutputHelper helper;
    private readonly Dictionary<string, string> variables;

    internal TestApplicationFactory(Dictionary<string, string> variables, ITestOutputHelper helper)
    {
        this.variables = variables;
        this.helper = helper;
    }

    public static ApplicationBuilder<TStartup> Builder(ITestOutputHelper helper) =>
        new ApplicationBuilder<TStartup>(helper);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configurationBuilder = new ConfigurationBuilder().Build();
        builder.UseConfiguration(configurationBuilder);
        builder.ConfigureServices(services =>
        {
            services.AddSerilog(new LoggerConfiguration().WriteTo.TestOutput(this.helper).CreateLogger());
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEnvironmentAdapter));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddScoped<IEnvironmentAdapter>(_ => new EnvironmentAdapter(this.variables));
        });
        base.ConfigureWebHost(builder);
    }
}

public class ApplicationBuilder<TStartup>(ITestOutputHelper helper) where TStartup : class
{
    private readonly Dictionary<string, string> variables = new Dictionary<string, string>();

    public TestApplicationFactory<TStartup> Build() => new TestApplicationFactory<TStartup>(this.variables, helper);

    public ApplicationBuilder<TStartup> OverrideApplicationSpec(string value)
    {
        this.variables.Add("SPEC_APPLICATION", value);
        return this;
    }

    public ApplicationBuilder<TStartup> OverrideMessagesSpec(string value)
    {
        this.variables.Add("SPEC_MESSAGES", value);
        return this;
    }

    public ApplicationBuilder<TStartup> OverrideSmsSpec(string value)
    {
        this.variables.Add("SPEC_SMS", value);
        return this;
    }

    public ApplicationBuilder<TStartup> OverrideVerifySpec(string value)
    {
        this.variables.Add("SPEC_VERIFY", value);
        return this;
    }

    public ApplicationBuilder<TStartup> OverrideVoiceSpec(string value)
    {
        this.variables.Add("SPEC_VOICE", value);
        return this;
    }

    public ApplicationBuilder<TStartup> WithEnvironmentVariable(string key, string value)
    {
        this.variables.Add(key, value);
        return this;
    }
}