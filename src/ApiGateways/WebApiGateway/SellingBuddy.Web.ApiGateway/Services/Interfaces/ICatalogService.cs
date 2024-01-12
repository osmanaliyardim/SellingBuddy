using SellingBuddy.Web.ApiGateway.Models.Catalog;

namespace SellingBuddy.Web.ApiGateway.Services.Interfaces;

public interface ICatalogService
{
    Task<CatalogItem> GetCatalogItemAsync(int id);

    Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
}
