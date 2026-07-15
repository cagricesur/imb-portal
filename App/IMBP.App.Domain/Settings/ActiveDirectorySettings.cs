namespace IMBP.App.Domain.Settings
{
    public class ActiveDirectorySettings
    {
        public const string Section = nameof(ActiveDirectorySettings);

        public required string Domain { get; set; }

        /// <summary>
        /// When true, skips real AD bind and accepts any non-empty credentials (Development only).
        /// </summary>
        public bool UseStub { get; set; }
    }
}
