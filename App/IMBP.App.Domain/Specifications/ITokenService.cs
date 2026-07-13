using IMBP.App.Domain.Models;

namespace IMBP.App.Domain.Specifications
{
    public interface ITokenService
    {
        string GenerateAccessToken(AuthUser user, Guid sessionId);
    }
}
