using SellingBuddy.IdentityService.Api.Application.Models;

namespace SellingBuddy.IdentityService.Api.Application.Services;

public interface IIdentityService
{
    Task<LoginResponseModel> Login(LoginRequestModel model);
}
