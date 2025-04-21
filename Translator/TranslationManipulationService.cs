namespace Translator;

public class TranslationManipulationService : ITranslationManipulation
{
    public TranslationList AppendPlaintextTranslations(TranslationList translationCollection, string translation, string language, string separator)
    {
        var translationItems = translation.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        var itemsToUpdate = translationCollection.Translations
            .Where(x => !x.IgnoreTranslations)
            .Where(x => !x.Translations.Any(t => t.Language == language))
            .ToList();

        if (translationItems.Length != itemsToUpdate.Count)
        {
            throw new Exception("Incorrect number of translations found");
        }

        for (int i = 0; i < itemsToUpdate.Count; i++)
        {
            itemsToUpdate.ElementAt(i).Translations.Add(new TranslationValue
            {
                Mode = "Auto",
                Language = language,
                Value = translationItems[i]
            });
        }

        return translationCollection;
    }

    public string ExportToPlaintext(TranslationList collection, string fromLanguage, string toLanguage, string separator)
    {
        var toTranslateList = collection.Translations
            .Where(x => !x.IgnoreTranslations)
            .Select(x => ExportForAutomaticTranslation(x, fromLanguage, toLanguage));

        toTranslateList = toTranslateList.Where(x => !string.IsNullOrEmpty(x));
        return string.Join(separator, toTranslateList);
    }

    private string ExportForAutomaticTranslation(Translation firstTranslationItem, string fromLanguage, string toLanguage)
    {
        if (!string.IsNullOrEmpty(firstTranslationItem.Translations.FirstOrDefault(x => x.Language == toLanguage)?.Value))
        {
            return string.Empty;
        }

        return firstTranslationItem.Translations.FirstOrDefault(x => x.Language == fromLanguage).Value;
    }

    public string GenerateTranslation(TranslationList translationInput, string separator)
    {
        return $"<root>{separator}{string.Join(separator, translationInput.Translations.Select(x => Serialize(x, separator)))}{separator}</root>";
    }

    private string Serialize(Translation translationItem, string separator)
    {
        var ignoreTranslations = translationItem.IgnoreTranslations ? " ignoreTranslations=\"true\"" : string.Empty;
        return $"<data key=\"{translationItem.Key}\"{ignoreTranslations}>{separator}{string.Join(separator, translationItem.Translations.Select(x => Serialize(x)))}{separator}</data>";
    }

    private string Serialize(TranslationValue translation)
    {
        var mode = string.IsNullOrEmpty(translation.Mode) ? string.Empty : " mode=\"auto\"";
        return $"<value lang=\"{translation.Language}\"{mode}>{translation.Value.ReplaceReswUnsuportedCharacters()}</value>";
    }
}