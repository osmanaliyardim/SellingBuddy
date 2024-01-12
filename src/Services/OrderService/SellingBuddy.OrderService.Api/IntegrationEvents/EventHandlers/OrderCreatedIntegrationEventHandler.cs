using MediatR;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.OrderService.Api.IntegrationEvents.Events;
using SellingBuddy.OrderService.Application.Features.Commands.CreateOrder;
using System.Reflection;

namespace SellingBuddy.OrderService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation($"Processing integration event: {@event.Id} at {Assembly.GetExecutingAssembly().GetName().Name} - ({nameof(OrderCreatedIntegrationEvent)})");
        
        var createOrderCommand = new CreateOrderCommand(
                @event.UserName, @event.City, @event.Street,
                @event.State, @event.Country, @event.ZipCode,
                @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
                @event.CardSecurityNumber, @event.CardTypeId, @event.Basket.Items
            );

        await _mediator.Send(createOrderCommand);
    }
}
