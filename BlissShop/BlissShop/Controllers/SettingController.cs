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

        /// <summary>
        /// Send email support.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendEmailSupport(SupportRequest request)
        {
            var result = await _settingService.SendEmailSupport(request);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Update settings.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPut("[action]")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSettings(UpdateSettingDTO dto)
        {
            var userId = HttpContext.GetUserId();
            var result = await _settingService.UpdateSettingsAsync(userId, dto);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Get settings for user.
        /// </summary>
        /// <returns> This endpoint returns settings.</returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(SettingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSettings()
        {
            var userId = HttpContext.GetUserId();
            var result = await _settingService.GetSettingsForUserAsync(userId);

            return Ok(result);
        }
    }
}
