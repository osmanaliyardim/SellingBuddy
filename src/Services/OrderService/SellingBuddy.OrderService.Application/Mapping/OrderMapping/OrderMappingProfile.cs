using AutoMapper;
using SellingBuddy.OrderService.Application.Dtos;
using SellingBuddy.OrderService.Application.Features.Commands.CreateOrder;
using SellingBuddy.OrderService.Application.Features.Queries.ViewModels;
using SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;

namespace SellingBuddy.OrderService.Application.Mapping.OrderMapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, CreateOrderCommand>().ReverseMap();

        CreateMap<Domain.AggregateModels.OrderAggregate.OrderItem, OrderItemDto>().ReverseMap();

        CreateMap<Domain.AggregateModels.OrderAggregate.OrderItem, Features.Queries.ViewModels.OrderItem>();

        CreateMap<Order, OrderDetailViewModel>()
            .ForMember(x => x.City, y => y.MapFrom(z => z.Address.City))
            .ForMember(x => x.Country, y => y.MapFrom(z => z.Address.Country))
            .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street))
            .ForMember(x => x.ZipCode, y => y.MapFrom(z => z.Address.ZipCode))
            .ForMember(x => x.OrderDate, y => y.MapFrom(z => z.OrderDate))
            .ForMember(x => x.OrderNumber, y => y.MapFrom(z => z.Id.ToString()))
            .ForMember(x => x.Status, y => y.MapFrom(z => z.OrderStatus.Name))
            .ForMember(x => x.Total, y => y.MapFrom(z => z.OrderItems.Sum(i => i.Units * i.UnitPrice)))
            .ReverseMap();
    }
}
