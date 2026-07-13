using IMBP.App.Core;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Models;
using IMBP.App.Domain.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMBP.Web.Server.Controllers
{
    public class UserController(IUserService userService) : PortalController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public Task<IActionResult> Authenticate(AuthenticationRequest request, [FromQuery] bool rememberMe = false)
        {
            return userService.Authenticate(request, rememberMe).ToControllerResponse();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public Task<IActionResult> Refresh()
        {
            return userService.Refresh().ToControllerResponse();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> Logout()
        {
            return userService.Logout().ToControllerResponse();
        }

        [Authorize]
        [HttpGet]
        [Route("me")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public Task<IActionResult> GetMe()
        {
            return userService.GetMe().ToControllerResponse();
        }
    }
}
