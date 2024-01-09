using MediatR;
using SellingBuddy.OrderService.Application.Interfaces.Repositories;
using SellingBuddy.OrderService.Domain.AggregateModels.BuyerAggregate;
using SellingBuddy.OrderService.Domain.Events;

namespace SellingBuddy.OrderService.Application.DomainEventHandlers;

public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly IBuyerRepository _buyerRepository;

    public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }

    public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken)
    {
        var cardTypeId = (orderStartedEvent.CardTypeId != 0) ? orderStartedEvent.CardTypeId : 4;

        var buyer = await _buyerRepository.GetSingleAsync(b => b.Name == orderStartedEvent.UserName, b => b.PaymentMethods);

        bool buyerExists = buyer != null;

        if (!buyerExists)
        {
            buyer = new Buyer(orderStartedEvent.UserName);
        }

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                    $"Payment Method on {DateTime.UtcNow}",
                                    orderStartedEvent.CardNumber,
                                    orderStartedEvent.CardSecurityNumber,
                                    orderStartedEvent.CardHolderName,
                                    orderStartedEvent.CardExpiration,
                                    orderStartedEvent.Order.Id);

        var buyerUpdated = buyerExists
            ? _buyerRepository.Update(buyer) 
            : await _buyerRepository.AddAsync(buyer);

        await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // OrderStatus have been changed event may fired here
    }
}
