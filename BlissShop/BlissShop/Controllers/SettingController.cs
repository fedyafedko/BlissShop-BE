using BlissShop.Abstraction;
using BlissShop.Common.DTO.Settings;
using BlissShop.Common.Extensions;
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

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateSettings(UpdateSettingDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _settingService.UpdateSettingsAsync(userId, dto);

            return result ? Ok() : BadRequest();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSettings()
        {
            var userId = HttpContext.GetUserId();
            var result = await _settingService.GetSettingsForUserAsync(userId);

            return Ok(result);
        }
    }
}
