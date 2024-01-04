using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.UnitTest.Events.Events;

namespace SellingBuddy.EventBus.UnitTest.Events.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        Console.WriteLine($"Handle method triggered with id:{@event.Id}");

        return Task.CompletedTask;
    }
}
