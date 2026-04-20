using SimsConstructor.Components;
using SimsConstructor.Options;
using SimsConstructor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RoomSettings>(builder.Configuration.GetSection(RoomSettings.SectionName));
builder.Services.AddSingleton<PlacementValidator>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
