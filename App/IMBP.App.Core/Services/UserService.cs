using IMBP.App.Domain;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using IMBP.App.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMBP.App.Core.Services
{
    internal class UserService(
        PortalContext context,
        IActiveDirectoryService activeDirectoryService,
        ITokenService tokenService,
        ILogger<UserService> logger) : IUserService
    {
        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            var response = new AuthenticationResponse();

            ActiveDirectoryUser? adUser;
            try
            {
                adUser = await activeDirectoryService.ValidateCredentials(request.UserName, request.Password, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Directory unavailable during authentication");
                response.SetError(StatusCodes.Status503ServiceUnavailable, "Authentication.DirectoryUnavailable");
                return response;
            }

            if (adUser is null)
            {
                response.SetError(StatusCodes.Status401Unauthorized, "Authentication.InvalidCredentials");
                return response;
            }

            var user = await UpsertUser(adUser, cancellationToken);
            if (user.Status == (byte)UserStatusEnum.Disabled)
            {
                response.SetError(StatusCodes.Status403Forbidden, "Authentication.UserDisabled");
                return response;
            }

            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.MiddleName = user.MiddleName;
            response.LastName = user.LastName;
            response.Token = tokenService.CreateToken(new TokenUser
            {
                UID = user.UID,
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Role = user.Role,
            });

            return response;
        }

        public async Task<AuthenticationResponse> GetCurrentUser(Guid userUid, CancellationToken cancellationToken)
        {
            var response = new AuthenticationResponse();
            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.UID == userUid, cancellationToken);

            if (user is null)
            {
                response.SetError(StatusCodes.Status401Unauthorized, "Authentication.Unauthorized");
                return response;
            }

            if (user.Status == (byte)UserStatusEnum.Disabled)
            {
                response.SetError(StatusCodes.Status403Forbidden, "Authentication.UserDisabled");
                return response;
            }

            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.MiddleName = user.MiddleName;
            response.LastName = user.LastName;
            return response;
        }

        private async Task<User> UpsertUser(ActiveDirectoryUser adUser, CancellationToken cancellationToken)
        {
            var normalizedUserName = adUser.UserName.Trim();
            var user = await context.Users
                .FirstOrDefaultAsync(entity => entity.UserName == normalizedUserName, cancellationToken);

            if (user is null)
            {
                user = new User
                {
                    UID = Guid.NewGuid(),
                    CID = string.IsNullOrWhiteSpace(adUser.EmployeeId) ? normalizedUserName : adUser.EmployeeId.Trim(),
                    UserName = normalizedUserName,
                    FirstName = adUser.FirstName.Trim(),
                    MiddleName = string.IsNullOrWhiteSpace(adUser.MiddleName) ? null : adUser.MiddleName.Trim(),
                    LastName = adUser.LastName.Trim(),
                    RegistrationDate = DateTime.UtcNow,
                    Role = (byte)UserRoleEnum.Member,
                    Status = (byte)UserStatusEnum.Enabled,
                };

                await context.Users.AddAsync(user, cancellationToken);
            }
            else
            {
                user.FirstName = adUser.FirstName.Trim();
                user.MiddleName = string.IsNullOrWhiteSpace(adUser.MiddleName) ? null : adUser.MiddleName.Trim();
                user.LastName = adUser.LastName.Trim();

                if (!string.IsNullOrWhiteSpace(adUser.EmployeeId))
                {
                    user.CID = adUser.EmployeeId.Trim();
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
