using SellingBuddy.OrderService.Application.Interfaces.Repositories;
using SellingBuddy.OrderService.Domain.AggregateModels.OrderAggregate;
using SellingBuddy.OrderService.Infrastructure.Context;
using System.Linq.Expressions;

namespace SellingBuddy.OrderService.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
    {
        var entity = await base.GetByIdAsync(id, includes);

        if (entity == null)
        {
            entity = _dbContext.Orders.Local.FirstOrDefault(o => o.Id == id);
        }

        return entity;
    }
}
