using System.Xml.Linq;

namespace Translator;

public class TranslationService : ITranslationService
{
    public TranslationList ImportTranslationList(string input)
    {
        var document = XDocument.Parse(input);
        var descendants = document.Descendants("data");
        return new TranslationList
        {
            Translations = descendants
            .Select(x => ImportTranslation(x))
            .ToList()
        };
    }

    private Translation ImportTranslation(XElement element)
    {
        return new Translation
        {
            Key = element.Attribute("key").Value,
            IgnoreTranslations = element.Attribute("ignoreTranslations") != null && bool.Parse(element.Attribute("ignoreTranslations").Value),
            Translations = element.Descendants("value")
            .Select(v => new TranslationValue
            {
                Language = v.Attribute("lang").Value,
                Value = v.Value,
                Mode = v.Attribute("mode")?.Value
            })
            .ToList()
        };
    }

    public string ExportToResw(TranslationList translationList, string language)
    {
        var template = File.ReadAllText("ResourceTemplate.xml");
        return string.Format(template, string.Join(Environment.NewLine, translationList.Translations.Select(x => ExportToResw(x, language))));
    }

    private string ExportToResw(Translation translation, string language)
    {
        var referenceTranslation = translation.Translations.First(x => x.Language == "en-us");
        var languageTranslation = translation.Translations.FirstOrDefault(x => x.Language == language) ?? translation.Translations.First(t => t.Language == "en-us");

        var translationValue = string.IsNullOrEmpty(languageTranslation.Value) || translation.IgnoreTranslations ? referenceTranslation.Value : languageTranslation.Value;

        return $"<data name=\"{translation.Key}\" xml:space=\"preserve\">\n\t<value>{translationValue.ReplaceReswUnsuportedCharacters()}</value>\n</data>";
    }

    public string ExportToCSharpEnum(TranslationList translationList, string language, string name, string namespaceName)
    {
        var enumeCases = string.Join($"\n\t", translationList.Translations.Select(x => GenerateCSharpEnumCase(language, x)));
        return $"namespace {namespaceName};\n\npublic enum {name}Key\n{{\n\t{enumeCases}\n}}";
    }

    private string GenerateCSharpEnumCase(string language, Translation translation)
    {
        return $"{StringHelpers.RemoveCSharpUnsupportedCharacters(translation.Key)}, \t// {translation.Translations.First(t => t.Language == language).Value}";
    }

    public string ExportToXamlIdentityDictionary(TranslationList translationList)
    {
        var dictionaryItems = string.Join($"\n    ", translationList.Translations.Select(x => GenerateXAMLDictionaryItem(x)));
        var header = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
        var footer = "</ResourceDictionary>";
        return $"{header}\n    {dictionaryItems}\n{footer}";
    }

    private string GenerateXAMLDictionaryItem(Translation translation)
    {
        var key = StringHelpers.RemoveCSharpUnsupportedCharacters(translation.Key);
        return $"<x:String x:Key=\"{key}\">{key}</x:String>";
    }
}