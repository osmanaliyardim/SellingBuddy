using SellingBuddy.EventBus.AzureServiceBus;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.RabbitMQ;

namespace SellingBuddy.EventBus.Factory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        return eventBusConfig.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus(serviceProvider, eventBusConfig),
            _ => new EventBusRabbitMQ(serviceProvider, eventBusConfig)
        };
    }
}