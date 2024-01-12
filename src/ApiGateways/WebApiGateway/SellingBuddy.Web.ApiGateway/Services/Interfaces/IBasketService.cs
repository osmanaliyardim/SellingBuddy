using SellingBuddy.Web.ApiGateway.Models.Basket;

namespace SellingBuddy.Web.ApiGateway.Services.Interfaces;

public interface IBasketService
{
    Task<BasketData> GetById(string id);

    Task<BasketData> UpdateAsync(BasketData currentBasket);
}
