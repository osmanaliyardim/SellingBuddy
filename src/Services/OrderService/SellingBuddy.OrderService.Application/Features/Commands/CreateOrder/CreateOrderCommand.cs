using MediatR;
using SellingBuddy.OrderService.Application.Dtos;
using SellingBuddy.OrderService.Domain.Models;

namespace SellingBuddy.OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<bool>
{
    // Address
    public string UserName { get; private set; }

    public string City { get; private set; }

    public string Street { get; private set; }

    public string State { get; private set; }

    public string Country { get; private set; }

    public string ZipCode { get; private set; }

    // Card info
    public string CardNumber { get; private set; }

    public string CardHolderName { get; private set; }

    public DateTime CardExpiration { get; private set; }

    public string CardSecurityNumber { get; private set; }

    public int CardTypeId { get; private set; }

    // Order info
    private readonly List<OrderItemDto> _orderItems;

    public IEnumerable<OrderItemDto> OrderItems => _orderItems;

    public CreateOrderCommand()
    {
        _orderItems = new List<OrderItemDto>();
    }

    public CreateOrderCommand(
        string userName, string city, string street, 
        string state, string country, string zipCode, 
        string cardNumber, string cardHolderName, 
        DateTime cardExpiration, string cardSecurityNumber, 
        int cardTypeId, List<BasketItem> basketItems) : this()
    {
        var dtoList = basketItems.Select(item => new OrderItemDto()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            PictureUrl = item.PictureUrl,
            UnitPrice = item.UnitPrice,
            Units = item.Quantity
        });

        _orderItems = dtoList.ToList();

        UserName = userName;
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
    }
}
