using IMBP.App.Domain.Models;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using IMBP.App.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace IMBP.App.Core.Services
{
    public class TranslationService(PortalContext context, ICacheService cacheService) : ITranslationService
    {
        private string GetTranslationsCacheKey(string language)
        {
            return string.Join(".", nameof(TranslationService), language);
        }
        public void ClearCache()
        {
            cacheService.Remove(GetTranslationsCacheKey("tr"));
            cacheService.Remove(GetTranslationsCacheKey("en"));
        }
        public async Task<string> GetTranslation(string language, string name, CancellationToken cancellationToken)
        {
            var translations = await GetTranslations(language, cancellationToken);
            return translations.FirstOrDefault(entity => entity.Name == name)?.Value ?? name;
        }
        public async Task<List<TranslationData>> GetTranslations(string language, CancellationToken cancellationToken)
        {

            return await cacheService.AddSliding(GetTranslationsCacheKey(language), async (entry) =>
            {
                return (await context.Translations
                                 .AsNoTracking()
                                 .Where(entity => entity.Language == language)
                                 .ToListAsync(cancellationToken: cancellationToken))
                                 .Select(entity => new TranslationData()
                                 {
                                     ID = entity.ID,
                                     Name = entity.Name,
                                     Value = entity.Value,
                                     Language = entity.Language
                                 })
                                 .ToList();
            }, TimeSpan.FromHours(1));
        }
        public async Task AddMissingTranslations(List<TranslationData>? translations, CancellationToken cancellationToken)
        {
            if (translations == null)
                return;

            foreach (var translation in translations)
            {
                var existing = await context.Translations.FirstOrDefaultAsync(entity => entity.Name == translation.Name && entity.Language == translation.Language, cancellationToken);

                if (existing == null)
                {
                    await context.Translations.AddAsync(new Translation
                    {
                        ID = Guid.NewGuid(),
                        Name = translation.Name,
                        Language = translation.Language,
                        Value = translation.Value,
                    }, cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            var languages = translations.Select(entity => entity.Language).Distinct();
            foreach (var language in languages)
            {
                cacheService.Remove(GetTranslationsCacheKey(language));
            }
        }
    }
}
