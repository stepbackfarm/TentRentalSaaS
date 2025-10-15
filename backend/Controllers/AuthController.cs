using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TentRentalSaaS.Api.DTOs;
using TentRentalSaaS.Api.Services;

namespace TentRentalSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> RequestLoginLink([FromBody] LoginRequestDto loginRequest)
        {
            await _authService.RequestLoginLinkAsync(loginRequest.Email);
            // Always return a generic success message to prevent email enumeration.
            return Ok(new { message = "If an account with this email exists, a login link has been sent." });
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            var portalData = await _authService.VerifyLoginTokenAsync(token);

            if (portalData == null)
            {
                return Unauthorized(new { message = "Invalid or expired login link." });
            }

            return Ok(portalData);
        }
    }
}
