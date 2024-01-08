using Newtonsoft.Json;
using SellingBuddy.BasketService.Api.Core.Application.Repository;
using SellingBuddy.BasketService.Api.Core.Domain.Models;
using StackExchange.Redis;

namespace SellingBuddy.BasketService.Api.Infrastructure.Repository;

public class BasketRepository : IBasketRepository
{
    private readonly ILogger<BasketRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public BasketRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<BasketRepository>();
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }

    public async Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        var data = await _database.StringGetAsync(customerId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<CustomerBasket>(data);
    }

    public IEnumerable<string> GetUsers()
    {
        var server = GetServer();
        var data = server.Keys();

        return data?.Select(u => u.ToString());
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var created = await _database.StringSetAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));

        if (!created)
        {
            _logger.LogInformation("Problem occured when trying to persist the item");
            return null;
        }

        _logger.LogInformation("Basket item persisted successfully!");

        return await GetBasketAsync(basket.BuyerId);
    }

    private IServer GetServer()
    {
        var endpoint = _redis.GetEndPoints();

        return _redis.GetServer(endpoint.First());
    }
}
