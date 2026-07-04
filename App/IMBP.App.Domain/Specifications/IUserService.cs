using IMBP.App.Domain.Contracts;

namespace IMBP.App.Domain.Specifications
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request);
    }
}
