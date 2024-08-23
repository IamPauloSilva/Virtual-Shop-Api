using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using VShop.IdentityServer.Configuration;
using VShop.IdentityServer.Data;
using VShop.IdentityServer.SeedDatabase;
using VShop.IdentityServer.SeedDataBase;
using VShop.IdentityServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
builder.Services.AddControllersWithViews();

// Configuração do armazenamento das chaves de proteção de dados
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("./VShop.IdentityServer/keys"))
    .SetApplicationName("IdentityServer");

// Configuração do DbContext e IdentityServer
var connectionString = builder.Configuration["SQL_CONNECTION_STRING"]
    ?? throw new InvalidOperationException("Connection string 'mysql' not found.");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 38));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
})
.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
.AddInMemoryClients(IdentityConfiguration.Clients)
.AddAspNetIdentity<ApplicationUser>();

// Configuração dos cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.Configure<IdentityServerOptions>(options =>
{
    options.Authentication.CookieSameSiteMode = SameSiteMode.None;
});

// Adiciona serviços personalizados
builder.Services.AddScoped<IDatabaseSeedInitializer, DatabaseIdentityServerInitializer>();
builder.Services.AddScoped<IProfileService, ProfileAppService>();

var app = builder.Build();

// Configure the application for production environment
if (app.Environment.IsProduction())
{
    var port = builder.Configuration["PORT"];
    if (!string.IsNullOrEmpty(port))
    {
        // Use HTTPS for production
        app.Urls.Add($"https://*:{port}");
    }
}
else
{
    var port = builder.Configuration["PORT"];
    // For development or other environments
    app.Urls.Add($"http://*:{port}");
}

// Migração automática do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Aplica as migrações pendentes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

// Inicializa dados do banco de dados
SeedDatabaseIdentityServer(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabaseIdentityServer(IApplicationBuilder app)
{
    using (var serviceScope = app.ApplicationServices.CreateScope())
    {
        var initRolesUsers = serviceScope.ServiceProvider
                               .GetService<IDatabaseSeedInitializer>();

        initRolesUsers.InitializeSeedRoles();
        initRolesUsers.InitializeSeedUsers();
    }
}
