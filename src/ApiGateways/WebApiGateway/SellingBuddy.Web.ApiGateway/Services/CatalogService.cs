using SellingBuddy.Web.ApiGateway.Extensions;
using SellingBuddy.Web.ApiGateway.Models.Catalog;
using SellingBuddy.Web.ApiGateway.Services.Interfaces;

namespace SellingBuddy.Web.ApiGateway.Services;

public class CatalogService : ICatalogService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CatalogService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CatalogItem> GetCatalogItemAsync(int id)
    {
        var catalogClient = _httpClientFactory.CreateClient("catalog");
        var response = await catalogClient.GetResponseAsync<CatalogItem>("/items/" + id);

        return response;
    }

    public Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
    {
        // ToDo

        return null;
    }
}
