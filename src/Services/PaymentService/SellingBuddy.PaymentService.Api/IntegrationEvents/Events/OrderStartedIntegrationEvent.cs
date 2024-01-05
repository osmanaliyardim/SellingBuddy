using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.PaymentService.Api.IntegrationEvents.Events;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; set; }

    public OrderStartedIntegrationEvent()
    {

    }

    public OrderStartedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
