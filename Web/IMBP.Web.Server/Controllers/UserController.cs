using System.Security.Claims;
using IMBP.App.Core;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Models;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace IMBP.Web.Server.Controllers
{
    public class UserController(IUserService userService, IOptions<JwtSettings> jwtOptions) : PortalController
    {
        private readonly JwtSettings jwtSettings = jwtOptions.Value;

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        [EnableRateLimiting("authenticate")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ServiceError>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ServiceError>(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            var response = await userService.Authenticate(request, cancellationToken);
            if (string.IsNullOrWhiteSpace(response.ErrorCode) && !string.IsNullOrWhiteSpace(response.Token))
            {
                AuthCookie.Append(Response, jwtSettings, response.Token);
            }

            return response.ToControllerResponse();
        }

        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Logout()
        {
            AuthCookie.Delete(Response, jwtSettings);
            return NoContent();
        }

        [HttpGet]
        [Route("me")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var userUidValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userUidValue, out var userUid))
            {
                var unauthorized = new AuthenticationResponse();
                unauthorized.SetError(StatusCodes.Status401Unauthorized, "Authentication.Unauthorized");
                return Task.FromResult(unauthorized.ToControllerResponse());
            }

            return userService.GetCurrentUser(userUid, cancellationToken).ToControllerResponse();
        }
    }
}
