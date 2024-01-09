using MediatR;
using SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;

namespace SellingBuddy.OrderService.Domain.Events;

public class OrderStartedDomainEvent : INotification
{
    public string UserName { get; set; }

    public int CardTypeId { get; set; }

    public string CardNumber { get; set; }

    public string CardSecurityNumber { get; set; }

    public string CardHolderName { get; set; }

    public DateTime CardExpiration { get; set; }

    public Order Order { get; set; }

    public OrderStartedDomainEvent(
        string userName, int cardTypeId, string cardNumber, 
        string cardSecurityNumber, string cardHolderName, 
        DateTime cardExpiration, Order order)
    {
        UserName = userName;
        CardTypeId = cardTypeId;
        CardNumber = cardNumber;
        CardSecurityNumber = cardSecurityNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        Order = order;
    }
}
