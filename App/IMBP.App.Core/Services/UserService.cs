using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Specifications;

namespace IMBP.App.Core.Services
{
    internal class UserService : IUserService
    {
        public Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
