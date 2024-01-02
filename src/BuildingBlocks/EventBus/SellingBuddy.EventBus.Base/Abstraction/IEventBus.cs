using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.EventBus.Base.Abstraction;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);

    void Subscribe<T, TH>() 
        where T : IntegrationEvent
        where TH : IntegrationEventHandler;

    void UnSubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IntegrationEventHandler;
}
