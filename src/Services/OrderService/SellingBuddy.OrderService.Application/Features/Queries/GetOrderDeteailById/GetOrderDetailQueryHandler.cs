using AutoMapper;
using MediatR;
using SellingBuddy.OrderService.Application.Features.Queries.ViewModels;
using SellingBuddy.OrderService.Application.Interfaces.Repositories;

namespace SellingBuddy.OrderService.Application.Features.Queries.GetOrderDeteailById;

public class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderDetailQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentException(nameof(orderRepository));
        _mapper = mapper;
    }

    public async Task<OrderDetailViewModel> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, i => i.OrderItems);

        var result = _mapper.Map<OrderDetailViewModel>(order);

        return result;
    }
}
