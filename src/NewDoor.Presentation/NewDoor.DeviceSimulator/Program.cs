using NewDoor.DeviceSimulator.Components;
using NewDoor.DeviceSimulator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ApiClientService>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://newdoor-api.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<EventBufferService>();
builder.Services.AddSingleton<FakeTelemetryGeneratorService>();
builder.Services.AddSingleton<SimulationEngineService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
