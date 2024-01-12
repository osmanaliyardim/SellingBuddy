using MediatR;
using Microsoft.AspNetCore.Mvc;
using SellingBuddy.OrderService.Application.Features.Queries.GetOrderDeteailById;
using System.Net;

namespace SellingBuddy.OrderService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOrderDetailsById(Guid id)
    {
        var result = await _mediator.Send(new GetOrderDetailsQuery(id));

        return Ok(result);
    }
}
