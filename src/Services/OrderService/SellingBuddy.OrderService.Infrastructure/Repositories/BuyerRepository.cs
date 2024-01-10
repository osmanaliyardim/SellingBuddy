using SellingBuddy.OrderService.Application.Interfaces.Repositories;
using SellingBuddy.OrderService.Domain.AggregateModels.BuyerAggregate;
using SellingBuddy.OrderService.Infrastructure.Context;

namespace SellingBuddy.OrderService.Infrastructure.Repositories;

public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
{
    public BuyerRepository(OrderDbContext dbContext) : base(dbContext)
    {

    }
}
