using SellingBuddy.OrderService.Domain.AggregateModels.BuyerAggregate;
using SellingBuddy.OrderService.Domain.Events;
using SellingBuddy.OrderService.Domain.SeedWork;

namespace SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;

public class Order : BaseEntity, IAggregateRoot
{
    public DateTime OrderDate { get; private set; }

    public int Quantity { get; private set; }

    public string Description { get; private set; }

    public Guid? BuyerId { get; private set; }

    public Buyer Buyer { get; private set; }

    public Address Address { get; private set; }

    private int orderStatusId;

    public OrderStatus OrderStatus { get; private set; }

    private readonly List<OrderItem> _orderItems;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Guid? PaymentMethodId { get; set; }

    protected Order()
    {
        Id = Guid.NewGuid();
        _orderItems = new List<OrderItem>();
    }

    public Order(
        string userName, Address address, int cartTypeId,
        string cardNumber, string CardSecurityNumber,
        string cardHolderName, DateTime cardExpiration,
        Guid? paymentMethodId, Guid? buyerId = null) : this()
    {
        BuyerId = buyerId;
        orderStatusId = OrderStatus.Submitted.Id;
        OrderDate = DateTime.UtcNow;
        Address = address;
        PaymentMethodId = paymentMethodId;

        AddOrderStartedDomainEvent(userName, cartTypeId, cardNumber, 
            CardSecurityNumber, cardHolderName, cardExpiration);
    }

    private void AddOrderStartedDomainEvent(
        string userName, int cartTypeId, 
        string cardNumber, string cardSecurityNumber, 
        string cardHolderName, DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(userName, cartTypeId,
                                                                  cardNumber, cardSecurityNumber,
                                                                  cardHolderName, cardExpiration, this);

        this.AddDomainEvent(orderStartedDomainEvent);
    }

    public void AddOrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1)
    {
        // ToDo: orderItem validations

        var orderItem = new OrderItem(productId, productName, pictureUrl, unitPrice, units);
        _orderItems.Add(orderItem);
    }

    public void SetBuyerId(Guid buyerId)
    {
        BuyerId = buyerId;
    }

    public void SetPaymentMethodId(Guid paymentMethodId)
    {
        PaymentMethodId = paymentMethodId;
    }
}
