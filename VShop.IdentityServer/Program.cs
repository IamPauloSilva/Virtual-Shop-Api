using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using VShop.IdentityServer.Configuration;
using VShop.IdentityServer.Data;
using VShop.IdentityServer.SeedDatabase;
using VShop.IdentityServer.SeedDataBase;
using VShop.IdentityServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adiciona vari�veis de ambiente � configura��o
var envVariables = Environment.GetEnvironmentVariables();
builder.Configuration.AddInMemoryCollection(envVariables.Cast<DictionaryEntry>()
                                      .ToDictionary(d => d.Key.ToString(),
                                                    d => d.Value.ToString()));

// Register DbContext with MySQL
var connectionString = builder.Configuration["SQL_CONNECTION_STRING"]
    ?? throw new InvalidOperationException("Connection string 'mysql' not found.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 38));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Configura o Identity e IdentityServer
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

// Configura��o do IdentityServer
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
.AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential(); // Para desenvolvimento, em produ��o use certificados v�lidos

builder.Services.AddScoped<IDatabaseSeedInitializer, DatabaseIdentityServerInitializer>();
builder.Services.AddScoped<IProfileService, ProfileAppService>();

var app = builder.Build();

if (builder.Environment.IsProduction())
{
    var port = builder.Configuration["PORT"];
    if (port is not null)
        builder.WebHost.UseUrls($"http://*:{port}");
    
}
// Migra��o autom�tica do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Aplica as migra��es pendentes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}


// Configure the HTTP request pipeline.
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
