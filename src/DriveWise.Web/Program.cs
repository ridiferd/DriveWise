using DriveWise.Core.FuelEconomy;
using DriveWise.Core.Recommendation;
using DriveWise.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- DriveWise services -------------------------------------------------
// FuelEconomy:Mode = Resilient (live API with offline fallback, default) | Live | Offline
var mode = builder.Configuration["FuelEconomy:Mode"] ?? "Resilient";
var snapshotPath = Path.Combine(AppContext.BaseDirectory, "data", "vehicles-snapshot.json");

builder.Services.AddSingleton(_ => OfflineFuelEconomyClient.FromFile(snapshotPath));
builder.Services.AddHttpClient<FuelEconomyClient>(c =>
{
    c.BaseAddress = new Uri(FuelEconomyClient.BaseAddress);
    c.Timeout = TimeSpan.FromSeconds(8);
});
builder.Services.AddScoped<IFuelEconomyClient>(sp => mode switch
{
    "Offline" => sp.GetRequiredService<OfflineFuelEconomyClient>(),
    "Live" => sp.GetRequiredService<FuelEconomyClient>(),
    _ => ActivatorUtilities.CreateInstance<ResilientFuelEconomyClient>(sp)
});
builder.Services.AddSingleton<RecommendationEngine>();
// ------------------------------------------------------------------------

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
