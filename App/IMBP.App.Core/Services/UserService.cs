using IMBP.App.Domain;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Models;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using IMBP.App.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IMBP.App.Core.Services
{
    internal class UserService(
        PortalContext dbContext,
        ISessionService sessionService,
        ICookieAuthService cookieAuthService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AuthenticationSettings> authenticationOptions,
        IOptions<JwtSettings> jwtOptions) : IUserService
    {
        private readonly AuthenticationSettings _authenticationSettings = authenticationOptions.Value;
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, bool rememberMe)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.UserName == request.UserName);

            if (user is null || user.Status != (byte)UserStatusEnum.Enabled)
            {
                return CreateErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "Authentication.InvalidCredentials");
            }

            if (!ValidatePassword(request.Password))
            {
                return CreateErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "Authentication.InvalidCredentials");
            }

            var userAgent = GetUserAgent();
            var sessionResult = await sessionService.CreateSessionAsync(user.ToAuthUser(), userAgent, rememberMe);

            SetAuthCookies(
                sessionResult.AccessToken,
                sessionResult.RefreshToken,
                sessionResult.RefreshExpiresAt);

            return MapUserProfile(user);
        }

        public async Task<AuthenticationResponse> Refresh()
        {
            var refreshToken = cookieAuthService.GetRefreshToken();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    "Authentication.RefreshTokenMissing");
            }

            var userAgent = GetUserAgent();
            var refreshResult = await sessionService.RefreshSessionAsync(refreshToken, userAgent);
            if (refreshResult is null)
            {
                cookieAuthService.ClearAuthCookies();
                return CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    "Authentication.RefreshTokenInvalid");
            }

            SetAuthCookies(
                refreshResult.AccessToken,
                refreshResult.RefreshToken,
                refreshResult.RefreshExpiresAt);

            return MapUserProfile(refreshResult.User);
        }

        public async Task<ServiceResponse> Logout()
        {
            var sessionId = cookieAuthService.GetSessionIdFromAccessToken();
            if (sessionId.HasValue)
            {
                await sessionService.RevokeSessionAsync(sessionId.Value);
            }
            else
            {
                var refreshToken = cookieAuthService.GetRefreshToken();
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var refreshSessionId = await sessionService.GetSessionIdByRefreshTokenAsync(refreshToken);
                    if (refreshSessionId.HasValue)
                    {
                        await sessionService.RevokeSessionAsync(refreshSessionId.Value);
                    }
                }
            }

            cookieAuthService.ClearAuthCookies();
            return new ServiceResponse();
        }

        public async Task<AuthenticationResponse> GetMe()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    "Authentication.Unauthorized");
            }

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.UID == userId.Value);

            if (user is null || user.Status != (byte)UserStatusEnum.Enabled)
            {
                return CreateErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    "Authentication.Unauthorized");
            }

            return MapUserProfile(user);
        }

        private bool ValidatePassword(string password)
        {
            if (_authenticationSettings.DevCredentials.Enabled)
            {
                return password == _authenticationSettings.DevCredentials.Password;
            }

            return false;
        }

        private void SetAuthCookies(string accessToken, string refreshToken, DateTime refreshExpiresAt)
        {
            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            cookieAuthService.SetAuthCookies(
                accessToken,
                refreshToken,
                accessExpiresAt,
                refreshExpiresAt);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdValue = httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId) ? userId : null;
        }

        private string GetUserAgent()
        {
            return httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;
        }

        private static AuthenticationResponse MapUserProfile(User user)
        {
            return new AuthenticationResponse
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
            };
        }

        private static AuthenticationResponse MapUserProfile(AuthUser user)
        {
            return new AuthenticationResponse
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
            };
        }

        private static AuthenticationResponse CreateErrorResponse(int statusCode, string errorCode)
        {
            var response = new AuthenticationResponse();
            response.SetError(statusCode, errorCode);
            return response;
        }
    }
}
