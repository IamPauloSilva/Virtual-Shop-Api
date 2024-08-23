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

// Add environment variables to configuration
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

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure services and repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductInterface, ProductService>();
builder.Services.AddScoped<ICategoryInterface, CategoryService>();

// Configure authentication JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["VShop.IdentityServer:ApplicationUrl"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["VShop.IdentityServer:ApplicationUrl"]
        };

        // Disable HTTPS requirement for development environment
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "vshop");
    });
});

// Configure controllers and JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VShop.ProductApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"'Bearer' [space] your token",
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
               In = ParameterLocation.Header
            },
            new List<string> ()
         }
    });
});

var app = builder.Build();

// Apply migrations and create the database if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (builder.Environment.IsProduction())
{
    var port = builder.Configuration["PORT"];
    if (port is not null)
        builder.WebHost.UseUrls($"http://*:{port}");
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Swagger only for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure CORS (if needed)
// app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
