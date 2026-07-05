using IMBP.App.Core;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Specifications;
using Microsoft.AspNetCore.Mvc;
using IMBP.App.Domain.Models;

namespace IMBP.Web.Server.Controllers
{
    public class UserController(IUserService userService) : PortalController
    {
        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType<ServiceError>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
        public Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            return userService.Authenticate(request).ToControllerResponse();
        }
        
    }
}
