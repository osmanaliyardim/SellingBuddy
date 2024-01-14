using Consul;

namespace SellingBuddy.IdentityService.Api.Extensions;

public static class ConsuleRegistration
{
    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));

        return services;
    }

    public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

        // Register service with consul
        var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
        var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
        var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

        var registration = new AgentServiceRegistration()
        {
            ID = serviceId ?? "IdentityService",
            Name = serviceName ?? "IdentityService",
            Address = uri.Host,
            Port = uri.Port,
            Tags = new[] { serviceName, serviceId }
        };

        logger.LogInformation("Registering service with Consul..");
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Deregistering service from Consul..");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}
