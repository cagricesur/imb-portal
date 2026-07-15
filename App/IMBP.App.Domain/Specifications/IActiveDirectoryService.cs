using IMBP.App.Domain.Contracts;

namespace IMBP.App.Domain.Specifications
{
    public interface IActiveDirectoryService
    {
        Task<ActiveDirectoryUser?> ValidateCredentials(string userName, string password, CancellationToken cancellationToken);
    }
}
