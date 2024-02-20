using BlissShop.Abstraction;
using BlissShop.Common.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public AuthController(IAuthService authService, IEmailConfirmationService emailConfirmationService)
        {
            _authService = authService;
            _emailConfirmationService = emailConfirmationService;
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

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO dto)
        { 
            var result = await _emailConfirmationService.ConfirmEmailAsync(dto);
            return Ok(result);
        }
    }
}
