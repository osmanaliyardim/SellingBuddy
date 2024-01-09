using MediatR;
using SellingBuddy.OrderService.Application.Features.Queries.ViewModels;

namespace SellingBuddy.OrderService.Application.Features.Queries.GetOrderDeteailById;

public class GetOrderDetailsQuery : IRequest<OrderDetailViewModel>
{
    public Guid OrderId { get; set; }

    public GetOrderDetailsQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
