using IMBP.App.Domain.Settings;
using Microsoft.AspNetCore.Http;

namespace IMBP.App.Core
{
    public static class AuthCookie
    {
        public static void Append(HttpResponse response, JwtSettings settings, string token)
        {
            response.Cookies.Append(settings.CookieName, token, CreateOptions());
        }

        public static void Delete(HttpResponse response, JwtSettings settings)
        {
            response.Cookies.Delete(settings.CookieName, CreateOptions());
        }

        private static CookieOptions CreateOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                IsEssential = true,
                // Session cookie: omit Expires/MaxAge so the browser clears it on close.
            };
        }
    }
}
