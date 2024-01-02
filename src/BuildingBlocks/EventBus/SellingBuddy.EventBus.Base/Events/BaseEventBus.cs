using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Base.SubManagers;

namespace SellingBuddy.EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider ServiceProvider;
    public readonly IEventBusSubscriptionManager SubscriptionManager;

    private EventBusConfig _eventBusConfig;

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
    {
        _eventBusConfig = eventBusConfig;
        ServiceProvider = serviceProvider;
        SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
    }

    public virtual string ProcessEventName(string eventName)
    {
        if (_eventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNamePrefix.ToArray());

        if (_eventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimEnd(_eventBusConfig.EventNameSuffix.ToArray());

        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }

    public virtual void Dispose()
    {
        _eventBusConfig = null;
    }

    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        var processed = false;

        if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);

            using var scope = ServiceProvider.CreateScope();
            
            foreach (var subscription in subscriptions)
            {
                var handler = ServiceProvider.GetService(subscription.HandleType);
                if (handler == null) continue;

                var eventType = SubscriptionManager.GetEventTypeByName($"{_eventBusConfig.EventNamePrefix}{eventName}{_eventBusConfig.EventNameSuffix}");
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
            }

            processed = true;
        }

        return processed;
    }

    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IntegrationEventHandler;

    public abstract void UnSubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IntegrationEventHandler;
}
