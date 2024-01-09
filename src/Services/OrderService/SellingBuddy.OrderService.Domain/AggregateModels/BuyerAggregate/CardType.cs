using SellingBuddy.OrderService.Domain.SeedWork;

namespace SellingBuddy.OrderService.Domain.AggregateModels.BuyerAggregate;

public class CardType : Enumeration
{
    public static CardType AmericanExpress = new(1, nameof(AmericanExpress));
    public static CardType Visa = new(2, nameof(Visa));
    public static CardType MasterCard = new(3, nameof(MasterCard));
    public static CardType Troy = new(4, nameof(Troy));

    public CardType(int id, string name) : base(id, name)
    {
        
    }
}
