using MediatR;
using SellingBuddy.OrderService.Application.Interfaces.Repositories;
using SellingBuddy.OrderService.Domain.Events;

namespace SellingBuddy.OrderService.Application.DomainEventHandlers;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
    : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent buyerAndPaymentVerifiedEvent, CancellationToken cancellationToken)
    {
        // Set methods to validate
        var orderToUpdate = await _orderRepository.GetByIdAsync(buyerAndPaymentVerifiedEvent.OrderId);
        orderToUpdate.SetBuyerId(buyerAndPaymentVerifiedEvent.Buyer.Id);
        orderToUpdate.SetPaymentMethodId(buyerAndPaymentVerifiedEvent.Payment.Id);
    }
}
