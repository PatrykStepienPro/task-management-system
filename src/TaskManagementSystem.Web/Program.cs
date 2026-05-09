using Microsoft.AspNetCore.Localization;
using TaskManagementSystem.Web.Components;
using TaskManagementSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { "en", "pl" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
    options.RequestCultureProviders = [new CookieRequestCultureProvider()];
});

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseRequestLocalization();
app.UseAntiforgery();

app.MapGet("/culture/set", (string culture, string redirectUri, HttpResponse response) =>
{
    var cookieValue = CookieRequestCultureProvider.MakeCookieValue(
        new RequestCulture(culture, culture));

    response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        cookieValue,
        new CookieOptions { MaxAge = TimeSpan.FromDays(30), IsEssential = true });

    return Results.LocalRedirect(redirectUri);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
