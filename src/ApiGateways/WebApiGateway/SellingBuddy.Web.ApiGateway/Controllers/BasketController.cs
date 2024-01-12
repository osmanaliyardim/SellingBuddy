using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellingBuddy.Web.ApiGateway.Models.Basket;
using SellingBuddy.Web.ApiGateway.Services.Interfaces;
using System.Net;

namespace SellingBuddy.Web.ApiGateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> AddBasketItemsAsync([FromBody] AddBasketItemRequest request)
        {
            if (request is null || request.Quantity == 0)
            {
                return BadRequest("Invalid payload!");
            }

            var item = await _catalogService.GetCatalogItemAsync(request.CatalogItemId);

            var currentBasket = await _basketService.GetById(request.BasketId);

            var product = currentBasket.Items.SingleOrDefault(i => i.ProductId == item.Id);

            if (product != null)
            {
                product.Quantity += request.Quantity;
            }
            else
            {
                currentBasket.Items.Add(new BasketDataItem()
                {
                    UnitPrice = item.Price,
                    PictureUrl = item.PictureUrl,
                    ProductId = item.Id,
                    Quantity = request.Quantity,
                    Id = Guid.NewGuid().ToString(),
                    ProductName = item.Name,
                });
            }

            await _basketService.UpdateAsync(currentBasket);

            return Ok();
        }
    }
}
