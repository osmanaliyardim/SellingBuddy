using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace SellingBuddy.CatalogService.Api.Extensions;

public static class HostExtension
{
    public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<TContext>>();

        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(8)
                });

            retry.Execute(() => InvokeSeeder(seeder, context, services));

            logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occured while migrating the database used on context {typeof(TContext).Name}");
        }

        return host;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) 
        where TContext : DbContext
    {
        context.Database.EnsureCreated();
        context.Database.Migrate();

        seeder(context, services);
    }
}
