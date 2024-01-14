using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Factory;
using SellingBuddy.NotificationService.Console.IntegrationEvents.EventHandlers;
using SellingBuddy.NotificationService.Console.IntegrationEvents.Events;
using Serilog;

var serilogConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Configurations/serilog.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(serilogConfiguration)
    .CreateLogger();

ServiceCollection services = new ServiceCollection();

ConfigureServices(services);

var sp = services.BuildServiceProvider();

IEventBus eventBus = sp.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();

services.AddLogging(builder =>
{
    builder.AddSerilog(Log.Logger, true);
});

Console.WriteLine("Notification Service App and Running...");
Console.ReadLine();

void ConfigureServices(ServiceCollection services)
{
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventBusType = EventBusType.RabbitMQ,
            Connection = new ConnectionFactory()
            {
                HostName = "crabbitmq"
                //Port = is the same
            }
        };

        return EventBusFactory.Create(config, sp);
    });
}
