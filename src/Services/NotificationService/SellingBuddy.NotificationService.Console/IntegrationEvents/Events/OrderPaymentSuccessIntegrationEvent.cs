using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.NotificationService.Console.IntegrationEvents.Events;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSuccessIntegrationEvent(int orderId) => OrderId = orderId;
}
