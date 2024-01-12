using SellingBuddy.BasketService.Api.Core.Application.Repository;
using SellingBuddy.BasketService.Api.Core.Application.Services;
using SellingBuddy.BasketService.Api.Extensions;
using SellingBuddy.BasketService.Api.Infrastructure.Repository;
using SellingBuddy.BasketService.Api.IntegrationEvents.EventHandlers;
using SellingBuddy.BasketService.Api.IntegrationEvents.Events;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Factory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ
    };

    return EventBusFactory.Create(config, sp);
});

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.RegisterWithConsul(lifetime);

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();