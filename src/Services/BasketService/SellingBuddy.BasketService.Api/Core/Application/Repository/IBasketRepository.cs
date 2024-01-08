using SellingBuddy.BasketService.Api.Core.Domain.Models;

namespace SellingBuddy.BasketService.Api.Core.Application.Repository;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);

    IEnumerable<string> GetUsers();

    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

    Task<bool> DeleteBasketAsync(string id);
}
