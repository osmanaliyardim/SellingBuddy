using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Factory;
using SellingBuddy.EventBus.UnitTest.Events.EventHandlers;
using SellingBuddy.EventBus.UnitTest.Events.Events;

namespace SellingBuddy.EventBus.UnitTest;

public class EventBusTests
{
    private ServiceCollection services;

    public EventBusTests()
    {
        services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
    }

    [Fact]
    public void Subscribe_Event_On_RabbitMQ()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [Fact]
    public void Subscribe_Event_On_AzureSB()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureSBConfig(), sp);
        });

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        Task.Delay(2000).Wait();
    }

    [Fact]
    public void Send_Message_To_RabbitMQ()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    [Fact]
    public void Send_Message_To_AzureSB()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureSBConfig(), sp);
        });

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    private EventBusConfig GetAzureSBConfig()
    {
        return new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "SellingBuddy.EventBus.UnitTest",
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.AzureServiceBus,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString = "Endpoint=sb://sellingbuddy.servicebus.windows.net/:..."
        };
    }

    private EventBusConfig GetRabbitMQConfig()
    {
        return new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "SellingBuddy.EventBus.UnitTest",
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",

            // Defined as default so that we do not need to specify again
            //Connection = new ConnectionFactory()
            //{
            //    HostName = "localhost",
            //    Port = 15672,
            //    UserName = "guest",
            //    Password = "guest"
            //}
        };
    }
}