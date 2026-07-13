namespace IMBP.App.Domain.Specifications
{
    public interface ICookieAuthService
    {
        void SetAuthCookies(string accessToken, string refreshToken, DateTime accessExpiresAt, DateTime refreshExpiresAt);
        void ClearAuthCookies();
        string? GetRefreshToken();
        Guid? GetSessionIdFromAccessToken();
    }
}
