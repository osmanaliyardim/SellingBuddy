using SellingBuddy.Web.ApiGateway.Extensions;
using SellingBuddy.Web.ApiGateway.Models.Basket;
using SellingBuddy.Web.ApiGateway.Services.Interfaces;

namespace SellingBuddy.Web.ApiGateway.Services;

public class BasketService : IBasketService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BasketService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<BasketData> GetById(string id)
    {
        var basketClient = _httpClientFactory.CreateClient("basket");
        var response = await basketClient.GetResponseAsync<BasketData>(id);

        return response ?? new BasketData(id);
    }

    public async Task<BasketData> UpdateAsync(BasketData currentBasket)
    {
        var basketClient = _httpClientFactory.CreateClient("basket");

        return await basketClient.PostGetResponseAsync<BasketData, BasketData>("update", currentBasket);
    }
}
