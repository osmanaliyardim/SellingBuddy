using SellingBuddy.EventBus.Base.Events;

namespace SellingBuddy.OrderService.Application.IntegrationEvents;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public string UserName { get; set; }

    public OrderStartedIntegrationEvent(string userName) => UserName = userName;
}
