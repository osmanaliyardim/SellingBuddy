using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using SellingBuddy.Web.ApiGateway.Infrastructure;
using SellingBuddy.Web.ApiGateway.Services;
using SellingBuddy.Web.ApiGateway.Services.Interfaces;

const string corsPolicy = "SellingBuddy.CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("Configurations/ocelot.json")
                            .AddEnvironmentVariables()
                            .Build();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOcelot(configuration).AddConsul();

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        corsPolicy,
        builder => builder.SetIsOriginAllowed((host) => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

ConfigureHttpClient(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.UseOcelot();

app.UseCors(corsPolicy);

app.Run();

void ConfigureHttpClient(IServiceCollection services)
{
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    services.AddHttpClient("basket", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["Urls:Basket"]);
    })
    .AddHttpMessageHandler<HttpClientDelegatingHandler>();

    services.AddHttpClient("catalog", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["Urls:Catalog"]);
    })
    .AddHttpMessageHandler<HttpClientDelegatingHandler>();
}