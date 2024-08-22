using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using VShop.Web.Services.CategoryService;
using VShop.Web.Services.ProductService;
using VShop.Web.Services;
using VShop.Web.Services.CartService;
using VShop.Web.Services.CouponService;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IProductInterface, ProductService>("ProductApi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:ProductApi"]);
    c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
    c.DefaultRequestHeaders.Add("Keep-Alive", "3600");
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-ProductApi");
});

builder.Services.AddHttpClient<ICartInterface, CartService>("CartApi",
    c => c.BaseAddress = new Uri(builder.Configuration["ServiceUri:CartApi"])
);
builder.Services.AddHttpClient<ICouponInterface, CouponService>("DiscountApi", c =>
   c.BaseAddress = new Uri(builder.Configuration["ServiceUri:DiscountApi"])
);

builder.Services.AddScoped<ICouponInterface, CouponService>();
builder.Services.AddScoped<ICartInterface, CartService>();
builder.Services.AddScoped<IProductInterface, ProductService>();
builder.Services.AddScoped<ICategoryInterface, CategoryService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, c =>
{
    c.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    c.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToAccessDenied = context =>
        {
            context.HttpContext.Response.Redirect(builder.Configuration["ServiceUri:IdentityServer"] + "/Account/AccessDenied");
            return Task.CompletedTask;
        }
    };
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnRemoteFailure = context =>
    {
        context.Response.Redirect("/");
        context.HandleResponse();
        return Task.FromResult(0);
    };

    options.Authority = builder.Configuration["ServiceUri:IdentityServer"];
    // Desabilita a exigência de HTTPS
    options.RequireHttpsMetadata = false;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "vshop";
    options.ClientSecret = builder.Configuration["Client:Secret"];
    options.ResponseType = "code";
    options.ClaimActions.MapJsonKey("role", "role", "role");
    options.ClaimActions.MapJsonKey("sub", "sub", "sub");
    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = "role";
    options.Scope.Add("vshop");
    options.SaveTokens = true;
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (builder.Environment.IsProduction())
{
    var port = builder.Configuration["PORT"];
    if (port is not null)
        builder.WebHost.UseUrls($"http://*:{port}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Remover HSTS se não estiver usando HTTPS
    // app.UseHsts();
}

// Remover redirecionamento para HTTPS se não estiver usando HTTPS
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
