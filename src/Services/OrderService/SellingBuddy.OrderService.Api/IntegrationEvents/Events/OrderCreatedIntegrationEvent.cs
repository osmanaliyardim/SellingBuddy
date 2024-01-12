using SellingBuddy.EventBus.Base.Events;
using SellingBuddy.OrderService.Domain.Models;

namespace SellingBuddy.OrderService.Api.IntegrationEvents.Events;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    // User info
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Buyer { get; set; }

    // Order info
    public int OrderNumber { get; set; }

    // Address
    public string City { get; set; }

    public string Street { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string ZipCode { get; set; }

    // Card info
    public string CardNumber { get; set; }

    public string CardHolderName { get; set; }

    public string CardSecurityNumber { get; set; }

    public int CardTypeId { get; set; }

    public DateTime CardExpiration { get; set; }

    // Basket info
    public CustomerBasket Basket { get; set; }

    // Operational info
    public Guid RequestId { get; set; }

    public OrderCreatedIntegrationEvent(string userId, string userName,
    string buyer, string city, string street,
    string state, string country, string zipCode,
    string cardNumber, string cardHolderName, string cardSecurityNumber,
    int cardTypeId, DateTime cardExpiration,
    CustomerBasket basket, Guid requestId)
    {
        UserId = userId;
        UserName = userName;
        Buyer = buyer;
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
        CardExpiration = cardExpiration;
        Basket = basket;
        RequestId = requestId;
    }
}
