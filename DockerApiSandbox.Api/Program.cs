#region
using DockerApiSandbox.Api;
using DockerApiSandbox.Api.AuthenticationVerification;
using DockerApiSandbox.Api.Callback;
using DockerApiSandbox.Api.InputValidation;
using DockerApiSandbox.Api.OperationIdentification;
using DockerApiSandbox.Api.OutputGeneration;
using Serilog;
#endregion

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddScoped<DocumentStore>();
builder.Services.AddScoped<DocumentClient>();
builder.Services.AddScoped<IEnvironmentAdapter, EnvironmentAdapter>();
builder.Services.Configure<List<ApiSpecification>>(builder.Configuration.GetSection("specs"));
var app = builder.Build();
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/_"), branch =>
{
    branch.UseMiddleware<OperationIdentificationMiddleware>();
    branch.UseMiddleware<AuthenticationVerificationMiddleware>();
    branch.UseMiddleware<InputValidationMiddleware>();
    branch.UseMiddleware<OutputGenerationMiddleware>();
    branch.UseMiddleware<CallbackMiddleware>();
});
app.UseHttpsRedirection();
app.MapControllers();
var provider = app.Services.CreateScope().ServiceProvider;
var store = provider.GetRequiredService<DocumentStore>();
var environment = provider.GetRequiredService<IEnvironmentAdapter>();
await Task.WhenAll(store.LoadDocuments());
var port = environment.GetVariable("PORT");
port.Do(some => app.Run($"http://0.0.0.0:{port}"), () => app.Run());

namespace DockerApiSandbox.Api
{
    public class Program
    {
        protected Program()
        {
        }
    }
}