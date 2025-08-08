#region
using DockerApiSandbox.Sample;
using DockerApiSandbox.Sample.Components;
using DockerApiSandbox.Sample.Webhooks;
using Vonage.Extensions;
#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddVonageClientScoped(builder.Configuration);
builder.Services.AddSingleton<IWebhookService, WebhookService>();
builder.Services.AddScoped<IMessageService, MessageService>();
var app = builder.Build();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapControllers();
app.MapBlazorHub("/blazor");
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();