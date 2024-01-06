using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.NotificationService.Console.IntegrationEvents.Events;

public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string ErrorMessage { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
    {
        ErrorMessage = errorMessage ?? string.Empty;
        OrderId = orderId;
    }
}
