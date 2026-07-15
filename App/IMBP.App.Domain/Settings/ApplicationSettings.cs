namespace IMBP.App.Domain.Settings
{
    public class ApplicationSettings
    {
        public const string Section = nameof(ApplicationSettings);
        public required Guid UID { get; set; }
    }
}
