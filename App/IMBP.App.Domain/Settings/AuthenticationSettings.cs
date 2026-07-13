namespace IMBP.App.Domain.Settings
{
    public class DevCredentialsSettings
    {
        public bool Enabled { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class AuthenticationSettings
    {
        public const string Section = nameof(AuthenticationSettings);
        public DevCredentialsSettings DevCredentials { get; set; } = new();
    }
}
