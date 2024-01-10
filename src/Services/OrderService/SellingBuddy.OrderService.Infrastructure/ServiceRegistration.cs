using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SellingBuddy.OrderService.Application.Interfaces.Repositories;
using SellingBuddy.OrderService.Infrastructure.Context;
using SellingBuddy.OrderService.Infrastructure.Repositories;

namespace SellingBuddy.OrderService.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(opt =>
        {
            opt.UseSqlServer(configuration["OrderDbConnectionString"]);
            opt.EnableSensitiveDataLogging();
        });

        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(configuration["OrderDbConnectionString"]);

        using var dbContext = new OrderDbContext(optionsBuilder.Options, null);
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();

        return services;
    }
}
