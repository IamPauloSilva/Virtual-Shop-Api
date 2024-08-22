using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections;
using System.Text.Json.Serialization;
using VShop.Products.Context;
using VShop.Products.Mappings;
using VShop.Products.Repositorys;
using VShop.Products.Services.CategoryService;
using VShop.Products.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

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


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductInterface, ProductService>();
builder.Services.AddScoped<ICategoryInterface, CategoryService>();

builder.Services.AddAuthentication("Bearer")
       .AddJwtBearer("Bearer", options =>
       {
           options.Authority =
             builder.Configuration["VShop.IdentityServer:ApplicationUrl"];

           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateAudience = false
           };
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "vshop");
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VShop.ProductApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"'Bearer' [space] seu token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
         {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                  Type = ReferenceType.SecurityScheme,
                  Id = "Bearer"
               },
               Scheme = "oauth2",
               Name = "Bearer",
               In= ParameterLocation.Header
            },
            new List<string> ()
         }
    });
});


var app = builder.Build();
string port = builder.Configuration["PORT"];
if (builder.Environment.IsProduction() && port is not null)
    builder.WebHost.UseUrls($"http://*:{builder.Configuration["PORT"]}");


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
        // Em um cenário de produção, você pode querer encerrar a aplicação aqui
        // ou lançar a exceção para garantir que erros críticos não passem despercebidos.
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
