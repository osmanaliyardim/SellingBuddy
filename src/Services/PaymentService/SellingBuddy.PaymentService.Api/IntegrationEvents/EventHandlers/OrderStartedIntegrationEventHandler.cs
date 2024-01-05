using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Base.Events;
using SellingBuddy.PaymentService.Api.IntegrationEvents.Events;

namespace SellingBuddy.PaymentService.Api.IntegrationEvents.EventHandlers;

public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

    public OrderStartedIntegrationEventHandler(
        IConfiguration configuration, 
        IEventBus eventBus, 
        ILogger<OrderStartedIntegrationEventHandler> logger)
    {
        _configuration = configuration;
        _eventBus = eventBus;
        _logger = logger;
    }

    public Task Handle(OrderStartedIntegrationEvent @event)
    {
        // Fake payment process
        string keyword = "PaymentSuccess";
        bool paymentSuccessFlag = _configuration.GetValue<bool>(keyword);

        IntegrationEvent paymentEvent = paymentSuccessFlag
            ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
            : new OrderPaymentFailedIntegrationEvent(@event.OrderId, $"Order payment failed with OrderId:{@event.OrderId}");

        _logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentSuccess: {paymentSuccessFlag}, orderId: {@event.OrderId}");

        _eventBus.Publish(paymentEvent);

        return Task.CompletedTask;
    }
}
