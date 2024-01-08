using System.Security.Claims;

namespace SellingBuddy.BasketService.Api.Core.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public IdentityService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetUserName()
    {
        return _contextAccessor.HttpContext.User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier).Value;
    }
}
