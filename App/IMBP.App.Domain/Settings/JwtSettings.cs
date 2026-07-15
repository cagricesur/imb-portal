namespace IMBP.App.Domain.Settings
{
    public class JwtSettings
    {
        public const string Section = nameof(JwtSettings);

        public required string Secret { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int ExpirationMinutes { get; set; }
        public required string CookieName { get; set; }
    }
}
