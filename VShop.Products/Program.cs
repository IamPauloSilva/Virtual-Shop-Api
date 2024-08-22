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

// Adiciona variáveis de ambiente à configuração
var envVariables = Environment.GetEnvironmentVariables();
builder.Configuration.AddInMemoryCollection(envVariables.Cast<DictionaryEntry>()
                                      .ToDictionary(d => d.Key.ToString(),
                                                    d => d.Value.ToString()));

// Registra DbContext com MySQL
var connectionString = builder.Configuration["SQL_CONNECTION_STRING"]
    ?? throw new InvalidOperationException("Connection string 'mysql' not found.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 38));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Configura AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configura serviços e repositórios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductInterface, ProductService>();
builder.Services.AddScoped<ICategoryInterface, CategoryService>();

// Configura autenticação JWT
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

        // Desabilita a exigência de HTTPS apenas no ambiente de desenvolvimento
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });


// Configura autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "vshop");
    });
});

// Configura controladores e JSON Options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// Configura Swagger/OpenAPI
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

if (builder.Environment.IsProduction())
{
    var port = builder.Configuration["PORT"];
    if (port is not null)
        builder.WebHost.UseUrls($"http://*:{port}");
}

// Swagger apenas para desenvolvimento (opcional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
