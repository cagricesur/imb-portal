using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IMBP.App.Core.Services
{
    [SupportedOSPlatform("windows")]
    internal class ActiveDirectoryService(
        IOptions<ActiveDirectorySettings> adOptions,
        ILogger<ActiveDirectoryService> logger) : IActiveDirectoryService
    {
        private readonly ActiveDirectorySettings settings = adOptions.Value;

        public Task<ActiveDirectoryUser?> ValidateCredentials(string userName, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult<ActiveDirectoryUser?>(null);
            }

            if (settings.UseStub)
            {
                return Task.FromResult<ActiveDirectoryUser?>(CreateStubUser(userName));
            }

            return Task.Run(() => ValidateAgainstDirectory(userName, password), cancellationToken);
        }

        private ActiveDirectoryUser? ValidateAgainstDirectory(string userName, string password)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, settings.Domain);
                if (!context.ValidateCredentials(userName, password, ContextOptions.Negotiate))
                {
                    return null;
                }

                using var principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName)
                    ?? UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, userName);

                if (principal is null)
                {
                    return null;
                }

                var samAccountName = principal.SamAccountName ?? userName;
                var givenName = principal.GivenName;
                var surname = principal.Surname;
                var displayName = principal.DisplayName;

                if (string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(displayName))
                {
                    var parts = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    givenName = parts.FirstOrDefault() ?? samAccountName;
                    surname = parts.Length > 1 ? parts.Last() : samAccountName;
                }

                return new ActiveDirectoryUser
                {
                    UserName = samAccountName,
                    FirstName = string.IsNullOrWhiteSpace(givenName) ? samAccountName : givenName,
                    LastName = string.IsNullOrWhiteSpace(surname) ? samAccountName : surname,
                    MiddleName = null,
                    EmployeeId = principal.EmployeeId,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Active Directory validation failed for user {UserName}", userName);
                throw;
            }
        }

        private static ActiveDirectoryUser CreateStubUser(string userName)
        {
            var normalized = userName.Contains('\\')
                ? userName[(userName.LastIndexOf('\\') + 1)..]
                : userName.Contains('@')
                    ? userName[..userName.IndexOf('@')]
                    : userName;

            return new ActiveDirectoryUser
            {
                UserName = normalized,
                FirstName = normalized,
                LastName = "User",
                EmployeeId = normalized,
            };
        }
    }
}
