namespace IMBP.App.Domain.Settings
{
    public class JwtCookieNames
    {
        public string AccessToken { get; set; } = "imb-portal-access";
        public string RefreshToken { get; set; } = "imb-portal-refresh";
    }

    public class JwtSettings
    {
        public const string Section = nameof(JwtSettings);
        public required string Secret { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 30;
        public bool EnforceSingleSession { get; set; } = true;
        public bool CookieSecure { get; set; } = true;
        public JwtCookieNames CookieNames { get; set; } = new();
    }
}
