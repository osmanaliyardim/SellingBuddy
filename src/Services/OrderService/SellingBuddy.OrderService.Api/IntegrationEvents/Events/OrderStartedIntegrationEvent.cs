using SellingBuddy.EventBus.Base.Events;
using SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;

namespace SellingBuddy.OrderService.Api.IntegrationEvents.Events;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    // User info
    public string UserId { get; private set; }

    public int OrderId { get; private set; }

    public OrderStartedIntegrationEvent(string userId, int orderId)
    {
        UserId = userId;
        OrderId = orderId;
    }
}
