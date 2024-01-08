using SellingBuddy.BasketService.Api.Core.Application.Repository;
using SellingBuddy.BasketService.Api.IntegrationEvents.Events;
using SellingBuddy.EventBus.Base.Abstraction;

namespace SellingBuddy.BasketService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation($"--- Handling integration event: {@event.Id} at BasketService");

        await _basketRepository.DeleteBasketAsync(@event.UserId.ToString());
    }
}
