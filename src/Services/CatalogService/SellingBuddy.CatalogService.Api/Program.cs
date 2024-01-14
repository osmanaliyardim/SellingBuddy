using Microsoft.Extensions.FileProviders;
using SellingBuddy.CatalogService.Api.Extensions;
using SellingBuddy.CatalogService.Api.Infrastructure;
using SellingBuddy.CatalogService.Api.Infrastructure.Context;
using Serilog;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Configurations/appsettings.json", optional: false)
    .AddJsonFile($"Configurations/appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var serilogConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Configurations/serilog.json", optional: false)
    .AddJsonFile($"Configurations/serilog.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(serilogConfiguration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.GetName().Name,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "Pics",
    EnvironmentName = Environments.Development // -> Staging -> Production
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Host.ConfigureAppConfiguration(i => i.AddConfiguration(configuration));
builder.Host.ConfigureLogging(i => i.ClearProviders());
builder.Host.UseSerilog();

builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection(nameof(CatalogSettings)));

builder.Services.ConfigureConsul(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureServices((context, services) =>
{
    services.Configure<ServiceProviderOptions>(options =>
    {
        options.ValidateOnBuild = false;
        options.ValidateScopes = false;
    });
});

var app = builder.Build();

app.MigrateDbContext<CatalogContext>((context, services) =>
{
    var env = services.GetService<IWebHostEnvironment>();
    var logger = services.GetService<ILogger<CatalogContextSeed>>();

    new CatalogContextSeed()
        .SeedAsync(context, env, logger)
        .Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Pics")),
    RequestPath = "/pics"
});

app.UseAuthorization();

app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.RegisterWithConsul(lifetime, builder.Configuration);

app.Run();
