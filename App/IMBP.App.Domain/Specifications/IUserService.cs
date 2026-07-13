using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Models;

namespace IMBP.App.Domain.Specifications
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, bool rememberMe);
        Task<AuthenticationResponse> Refresh();
        Task<ServiceResponse> Logout();
        Task<AuthenticationResponse> GetMe();
    }
}
