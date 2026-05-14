using NewDoor.Web.Components;
using NewDoor.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient<ApiClientService>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://newdoor-api.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<EventBufferService>();
builder.Services.AddSingleton<TelemetryGeneratorService>();
builder.Services.AddSingleton<TelemetryClientService>();
builder.Services.AddSingleton<SimulationEngineService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
