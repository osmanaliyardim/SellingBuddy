using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.EventBus.UnitTest.Events.Events;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public int Id { get; set; }

    public OrderCreatedIntegrationEvent(int id)
    {
        Id = id;
    }
}
