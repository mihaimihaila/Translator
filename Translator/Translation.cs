namespace Translator;

public class Translation
{
    /// <summary>
    /// Ignores Translations. Used for strings that should not be translated e.g. {0}
    /// </summary>
    public bool IgnoreTranslations { get; set; }

    /// <summary>
    /// The translation key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// A list of translations
    /// </summary>
    public List<TranslationValue> Translations { get; set; }
}

public class TranslationValue
{
    /// <summary>
    /// A helper string indicating the translation mode e.g. auto, manual
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// The translation language e.g. en-us
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// The translation value for the corresponding language
    /// </summary>
    public string Value { get; set; }
}

public class TranslationList
{
    public List<Translation> Translations { get; set; }
}