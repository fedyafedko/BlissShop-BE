using BlissShop.Abstraction;
using BlissShop.Common.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SignUp(SignUpDTO dto)
        {
            var result = await _authService.SignUpAsync(dto);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SignIn(SignInDTO dto)
        {
            var result = await _authService.SignInAsync(dto);
            return Ok(result);
        }
    }
}
