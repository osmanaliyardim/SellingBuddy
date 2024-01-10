using MediatR;
using SellingBuddy.OrderService.Domain.SeedWork;
using SellingBuddy.OrderService.Infrastructure.Context;

namespace SellingBuddy.OrderService.Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderDbContext context)
    {
        var domainEntities = context.ChangeTracker
                                    .Entries<BaseEntity>()
                                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents)
                                         .ToList();

        domainEntities.ToList()
                      .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach ( var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
