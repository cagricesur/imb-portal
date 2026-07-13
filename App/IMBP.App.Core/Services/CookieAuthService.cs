using System.IdentityModel.Tokens.Jwt;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IMBP.App.Core.Services
{
    internal class CookieAuthService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtSettings> jwtOptions) : ICookieAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public void SetAuthCookies(
            string accessToken,
            string refreshToken,
            DateTime accessExpiresAt,
            DateTime refreshExpiresAt)
        {
            var httpContext = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext is not available.");

            httpContext.Response.Cookies.Append(
                _jwtSettings.CookieNames.AccessToken,
                accessToken,
                CreateCookieOptions(accessExpiresAt));

            httpContext.Response.Cookies.Append(
                _jwtSettings.CookieNames.RefreshToken,
                refreshToken,
                CreateCookieOptions(refreshExpiresAt));
        }

        public void ClearAuthCookies()
        {
            var httpContext = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext is not available.");

            httpContext.Response.Cookies.Delete(_jwtSettings.CookieNames.AccessToken, CreateCookieOptions(DateTime.UtcNow));
            httpContext.Response.Cookies.Delete(_jwtSettings.CookieNames.RefreshToken, CreateCookieOptions(DateTime.UtcNow));
        }

        public string? GetRefreshToken()
        {
            return httpContextAccessor.HttpContext?.Request.Cookies[_jwtSettings.CookieNames.RefreshToken];
        }

        public Guid? GetSessionIdFromAccessToken()
        {
            var accessToken = httpContextAccessor.HttpContext?.Request.Cookies[_jwtSettings.CookieNames.AccessToken];
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);
                var sid = jwt.Claims.FirstOrDefault(claim => claim.Type == "sid")?.Value;
                return Guid.TryParse(sid, out var sessionId) ? sessionId : null;
            }
            catch
            {
                return null;
            }
        }

        private CookieOptions CreateCookieOptions(DateTime expiresAt)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = _jwtSettings.CookieSecure,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = expiresAt,
            };
        }
    }
}
