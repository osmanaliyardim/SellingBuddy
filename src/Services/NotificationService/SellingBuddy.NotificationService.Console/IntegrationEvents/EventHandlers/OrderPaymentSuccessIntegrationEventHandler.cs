using Microsoft.Extensions.Logging;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.NotificationService.Console.IntegrationEvents.Events;

namespace SellingBuddy.NotificationService.Console.IntegrationEvents.EventHandlers;

public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        // Send success notification via Sms, Email or Push..

        _logger.LogInformation($"Order Payment successfullly completed with OrderId: {@event.OrderId}");

        return Task.CompletedTask;
    }
}
