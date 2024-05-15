using BlissShop.Abstraction;
using BlissShop.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendEmailSupport(SupportRequest request)
        {
            var result = await _settingService.SendEmailSupport(request);

            return result ? Ok() : BadRequest();
        }
    }
}
