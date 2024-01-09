using SellingBuddy.OrderService.Domain.Events;
using SellingBuddy.OrderService.Domain.SeedWork;

namespace SellingBuddy.OrderService.Domain.AggregateModels.BuyerAggregate;

public class Buyer : BaseEntity, IAggregateRoot
{
    public string Name { get; set; }

    private List<PaymentMethod> _paymentMethods;

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer()
    {
        _paymentMethods = new List<PaymentMethod>();
    }

    public Buyer(string name) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public PaymentMethod VerifyOrAddPaymentMethod(
        int cartTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName,
        DateTime expiration, Guid orderId)
    {
        var existingPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cartTypeId, cardNumber, expiration));

        if (existingPayment != null)
        {
            // raise event
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

            return existingPayment;
        }

        var payment = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cartTypeId);

        _paymentMethods.Add(payment);

        // raise event
        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

        return payment;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) || 
                (obj is Buyer buyer
                && Id.Equals(buyer.Id)
                && Name == buyer.Name);
    }
}
