namespace Translator
{
    public interface ITranslationManipulation
    {
        string GenerateTranslation(TranslationList translationList, string separator);
        string ExportToPlaintext(TranslationList translationList, string fromLanguage, string toLanguage, string separator);
        TranslationList AppendPlaintextTranslations(TranslationList translationList, string plaintextTranslations, string language, string separator);
    }
}