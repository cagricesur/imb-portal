using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMBP.App.Core
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PortalController : ControllerBase
    {
    }
}
