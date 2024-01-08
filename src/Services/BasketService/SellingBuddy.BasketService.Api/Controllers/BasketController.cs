using Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SellingBuddy.BasketService.Api.Core.Application.Repository;
using SellingBuddy.BasketService.Api.Core.Application.Services;
using SellingBuddy.BasketService.Api.Core.Domain.Models;
using SellingBuddy.BasketService.Api.IntegrationEvents.Events;
using SellingBuddy.EventBus.Base.Abstraction;
using System.Net;

namespace SellingBuddy.BasketService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        IBasketRepository basketRepository, 
        IIdentityService identityService, 
        IEventBus eventBus, 
        ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult Get()
    {
        return Ok("Basket Service is App and Running!");
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBaskeyAsync([FromBody] CustomerBasket basket)
    {
        return Ok(await _basketRepository.UpdateBasketAsync(basket));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem item)
    {
        var userName = _identityService.GetUserName().ToString();

        var basket = await _basketRepository.GetBasketAsync(userName);

        if (basket == null)
        {
            basket = new CustomerBasket(userName);
        }

        basket.Items.Add(item);

        await _basketRepository.UpdateBasketAsync(basket);

        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;

        var basket = await _basketRepository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = _identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(
            userId, userName, basketCheckout.Buyer, basketCheckout.City, 
            basketCheckout.Street, basketCheckout.State, basketCheckout.Country, 
            basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName, 
            basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.CardExpiration, basket);

        try
        {
            // Listens itself to clean the basket
            // It is listened by OrderApi to start the process
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR: Publishing integration event with ID: {eventMessage.Id} from BasketService");

            throw;
        }

        return Accepted();
    }

    // DELETE -> api/v1/[controller]?{id=1}
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeteleteBasketByIdAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}
