namespace IMBP.App.Domain.Models
{
    public class TranslationData
    {
        public Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Language { get; set; }
        public required string Value { get; set; }
    }
}
