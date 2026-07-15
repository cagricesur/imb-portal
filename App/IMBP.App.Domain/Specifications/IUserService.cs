using IMBP.App.Domain.Contracts;

namespace IMBP.App.Domain.Specifications
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, CancellationToken cancellationToken);
        Task<AuthenticationResponse> GetCurrentUser(Guid userUid, CancellationToken cancellationToken);
    }
}
