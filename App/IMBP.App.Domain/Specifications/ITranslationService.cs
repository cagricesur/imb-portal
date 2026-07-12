using IMBP.App.Domain.Models;

namespace IMBP.App.Domain.Specifications
{
    public interface ITranslationService
    {
        void ClearCache();
        Task<string> GetTranslation(string language, string name, CancellationToken cancellationToken);
        Task<List<TranslationData>> GetTranslations(string language, CancellationToken cancellationToken);
        Task AddMissingTranslations(List<TranslationData>? translations, CancellationToken cancellationToken);

    }
}
