using System.Reflection.Metadata.Ecma335;

namespace SellingBuddy.EventBus.Base;

public class SubscriptionInfo
{
    public Type HandleType { get; }

    public SubscriptionInfo(Type handleType)
    {
        HandleType = handleType ?? throw new ArgumentNullException(nameof(handleType));
    }

    public static SubscriptionInfo Typed(Type handlerType)
    {
        return new SubscriptionInfo(handlerType);
    }
}
