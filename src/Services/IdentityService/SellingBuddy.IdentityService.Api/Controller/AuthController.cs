using Microsoft.AspNetCore.Mvc;
using SellingBuddy.IdentityService.Api.Application.Models;
using SellingBuddy.IdentityService.Api.Application.Services;
using System.Net;

namespace SellingBuddy.IdentityService.Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        var result = await _identityService.Login(model);

        return Ok(result);
    }
}
