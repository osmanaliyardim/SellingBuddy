using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SellingBuddy.CatalogService.Api.Core.Application;
using SellingBuddy.CatalogService.Api.Core.Domain;
using SellingBuddy.CatalogService.Api.Infrastructure;
using SellingBuddy.CatalogService.Api.Infrastructure.Context;
using System.Net;

namespace SellingBuddy.CatalogService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly CatalogContext _catalogContext;
    private readonly CatalogSettings _catalogSettings;

    public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> catalogSettings)
    {
        _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        _catalogSettings = catalogSettings.Value;

        catalogContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    // GET -> api/v1/[controller]/items[?pageSize=3&pageIndex=10]
    [HttpGet]
    [Route("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var items = await GetItemsByIdAsync(ids);

            if (!items.Any())
            {
                return BadRequest("ids value is invalid. It must be comma-seperated list of numbers!");
            }

            return Ok(items);
        }

        var totalItems = await _catalogContext.CatalogItems
            .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items[?id=1]
    [HttpGet]
    [Route("items/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
    private async Task<ActionResult<CatalogItem>> ItemByIdAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid id");
        }

        var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

        var baseUri = _catalogSettings.PicBaseUrl;

        if (item != null)
        {
            item.PictureUri = baseUri + item.PictureFileName;

            return Ok(item);
        }

        return NotFound("No item with ID: " + id);
    }

    // GET -> api/v1/[controller]/items/withname/samplename[?pageSize=1&pageIndex=5]
    [HttpGet]
    [Route("items/withname/{name:minlength(1)}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsWithNameAsync(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems
            .Where(c => c.Name.StartsWith(name))
            .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(c => c.Name.StartsWith(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items/type/1/brand/[?1&pageSize=1&pageIndex=5]
    [HttpGet]
    [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsByTypeIdAndBrandIdAsync(int catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

        root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items/type/all/brand/[?1&pageSize=1&pageIndex=5]
    [HttpGet]
    [Route("items/type/all/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
    {
        var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/catalogtypes
    [HttpGet]
    [Route("catalogtype")]
    [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<CatalogType>>> CatalogTypesAsync()
    {
        return await _catalogContext.CatalogTypes.ToListAsync();
    }

    // GET -> api/v1/[controller]/catalogbrands
    [HttpGet]
    [Route("catalogbrands")]
    [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<CatalogBrand>>> CatalogBrandsAsync()
    {
        return await _catalogContext.CatalogBrands.ToListAsync();
    }

    // PUT -> api/v1/[controller]/items
    [HttpPut]
    [Route("items")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult> UpdateCatalogAsync([FromBody] CatalogItem catalogToUpdate)
    {
        var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == catalogToUpdate.Id);

        if (catalogItem == null) 
        {
            return NotFound(new { Message = $"Item with id {catalogToUpdate.Id} not found!" });
        }

        var oldPrice = catalogItem.Price;
        var raiseCatalogPriceChangedEvent = oldPrice != catalogToUpdate.Price;

        // Update current catalog
        catalogItem = catalogToUpdate;
        _catalogContext.CatalogItems.Update(catalogItem);

        await _catalogContext.SaveChangesAsync();

        //if (raiseCatalogPriceChangedEvent)
        //{
        //    var priceChangedEvent = new CatalogPriceChangedIntegrationEvent(catalogItem.Id, catalogToUpdate.Price, oldPrice);

        //    await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

        //    await _catalogIntegrationEventService.PublishThroughEventBustAsync(priceChangedEvent);
        //}
        //else
        //{
        //    // Just save the updated catalog because the Product's Price has not changed.
        //}

        return CreatedAtAction(nameof(ItemByIdAsync), new { id = catalogToUpdate.Id }, null);
    }

    // POST -> api/v1/[controller]/items
    [HttpPost]
    [Route("items")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult> CreateCatalogAsync([FromBody] CatalogItem catalogToAdd)
    {
        var catalog = new CatalogItem
        {
            CatalogBrandId = catalogToAdd.CatalogBrandId,
            CatalogTypeId = catalogToAdd.CatalogTypeId,
            Description = catalogToAdd.Description,
            Name = catalogToAdd.Name,
            PictureFileName = catalogToAdd.PictureFileName,
            Price = catalogToAdd.Price
        };

        await _catalogContext.CatalogItems.AddAsync(catalog);

        await _catalogContext.SaveChangesAsync();

        return CreatedAtAction(nameof(ItemByIdAsync), new { id = catalog.Id }, null);
    }

    // DELETE -> api/v1/[controller]/id
    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteCatalogAsync(int id)
    {
        var catalogToDelete = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

        if (catalogToDelete == null)
        {
            return NotFound($"Catalog with id:{id} could not found!");
        }

        _catalogContext.CatalogItems.Remove(catalogToDelete);

        await _catalogContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<List<CatalogItem>> GetItemsByIdAsync(string ids)
    {
        var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

        if (!numIds.All(nid => nid.Ok))
        {
            return new List<CatalogItem>();
        }

        var idsToSelect = numIds
            .Select(id => id.Value);

        var catalogs = await _catalogContext.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

        catalogs = ChangeUriPlaceHolder(catalogs);

        return catalogs;
    }

    private List<CatalogItem> ChangeUriPlaceHolder(List<CatalogItem> itemsOnPage)
    {
        var baseUri = _catalogSettings.PicBaseUrl;

        foreach (var item in itemsOnPage)
        {
            if (item != null)
            {
                item.PictureUri = baseUri + item.PictureFileName;
            }
        }

        return itemsOnPage;
    }
}
