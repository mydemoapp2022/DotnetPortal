using UI.EmployerPortal.Web.Features;
using UI.EmployerPortal.Web.Features.Dashboard;
using UI.EmployerPortal.Web.Features.EmployerRegistration.Services;
using UI.EmployerPortal.Web.Features.Landing;
using UI.EmployerPortal.Web.Features.Shared.Accounts.Services;
using UI.EmployerPortal.Web.Features.Shared.Session;
//using UI.EmployerPortal.Web.Startup;
//using UI.EmployerPortal.Web.Startup.ResiliencyProtocols;
//using UI.EmployerPortal.Web.Startup.WcfServiceClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication();
var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpContextAccessor();
//services.AddLogging();
//services.AddRetryPolicies();
//services.AddWcfServiceClients(builder.Configuration, builder.Environment.IsDevelopment());
//services.AddMyWisconsinId(builder.Configuration);

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveServerComponents();

//session related config
var sessionStorageType = builder.Configuration.GetValue<SessionStorageType>("Session:StorageType", SessionStorageType.Protected);
if (sessionStorageType == SessionStorageType.Distributed)
{
    var connectionString = builder.Configuration.GetConnectionString("SessionDb")
        ?? throw new InvalidOperationException("SessionDb connection string is required for distributed session storage.");
    builder.Services.AddSqlServerSessionStorage(connectionString);
}

builder.Services.AddSessionManagement(sessionStorageType);

//temporary service for demo purpose
services.AddSingleton<IAccountService, AccountService>();
services.AddScoped<ILandingOrchestrator, LandingOrchestrator>();
services.AddScoped<IDashboardOrchestrator, DashboardOrchestrator>();

// Add mock UserAccountService for development
services.AddScoped<IUserAccountService, MockUserAccountService>();
builder.Services.AddScoped<ISSNValidationService, SSNValidationService>();
//builder.AddOpenTelemetryInDevelopment();

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

app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.AddMyWisconsinIDEndpoints();

app.Run();
