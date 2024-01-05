using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.PaymentService.Api.IntegrationEvents.Events;

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
