using SellingBuddy.OrderService.Api.Extensions;
using SellingBuddy.OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using SellingBuddy.OrderService.Application;
using SellingBuddy.OrderService.Infrastructure;
using SellingBuddy.OrderService.Infrastructure.Context;
using SellingBuddy.OrderService.Api.Extensions.Registration.ServiceDiscovery;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Factory;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.OrderService.Api.IntegrationEvents.Events;
using SellingBuddy.OrderService.Api.IntegrationEvents.EventHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureService(builder.Services);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContext>>();

    var dbContextSeeder = new OrderDbContextSeed();
    dbContextSeeder.SeedAsync(context, logger).Wait();
});

ConfigureEventBusForSubscription(app);

app.Run();

void ConfigureService(IServiceCollection services)
{
    services
        .AddLogging(configure => configure.AddConsole())
        .AddApplicationRegistration()
        .AddPersistenceRegistration(builder.Configuration)
        .ConfigureEventHandlers()
        .AddServiceDiscoveryRegistration(builder.Configuration);

    services.AddSingleton(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "OrderService",
            EventBusType = EventBusType.RabbitMQ
        };

        return EventBusFactory.Create(config, sp);
    });
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}
