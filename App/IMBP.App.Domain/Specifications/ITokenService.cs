using IMBP.App.Domain.Contracts;

namespace IMBP.App.Domain.Specifications
{
    public interface ITokenService
    {
        string CreateToken(TokenUser user);
    }
}
