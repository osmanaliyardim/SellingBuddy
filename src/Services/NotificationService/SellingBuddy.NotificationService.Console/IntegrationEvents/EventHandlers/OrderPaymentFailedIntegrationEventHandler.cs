using Microsoft.Extensions.Logging;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.NotificationService.Console.IntegrationEvents.Events;

namespace SellingBuddy.NotificationService.Console.IntegrationEvents.EventHandlers;

public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        // Send failed notification via Sms, Email or Push..

        _logger.LogInformation($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}
