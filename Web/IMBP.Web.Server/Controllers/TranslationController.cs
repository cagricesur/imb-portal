using IMBP.App.Core;
using IMBP.App.Domain.Models;
using IMBP.App.Domain.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMBP.Web.Server.Controllers
{
    public class TranslationController(ITranslationService translationService) : PortalController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("list")]
        [ProducesResponseType<List<TranslationData>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTranslations([FromQuery] string language, CancellationToken cancellationToken)
        {
            var translations = await translationService.GetTranslations(language, cancellationToken);
            return new ObjectResult(translations)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        [HttpPost]
        [Route("add-missing")]
        [ProducesResponseType<OkResult>(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMissingTranslations(List<TranslationData> translations, CancellationToken cancellationToken)
        {
            await translationService.AddMissingTranslations(translations, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("clear-cache")]
        [ProducesResponseType<OkResult>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCache()
        {
            translationService.ClearCache();
            return Ok();
        }
    }
}
