using BlissShop.Abstraction.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;

    public GoogleAuthController(IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> GoogleSignUp([FromHeader(Name = "Authorization-Code")] string authorizationCode, string role)
    {
        var result = await _googleAuthService.GoogleSignUpAsync(authorizationCode, role);
        return Ok(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> GoogleSignIn([FromHeader(Name = "Authorization-Code")] string authorizationCode)
    {
        var result = await _googleAuthService.GoogleSignInAsync(authorizationCode);
        return Ok(result);
    }
}
