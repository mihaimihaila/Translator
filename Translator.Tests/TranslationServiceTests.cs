namespace Translator.Tests;

[TestClass]
public class TranslationServiceTests
{
    private ITranslationService translationService;

    private TranslationList translationList;

    private string template;

    private readonly Translation backTranslationEnUs = new()
    {
        Key = "Back",
        Translations = new List<TranslationValue>
            {
                new() {
                    Language = "en-us",
                    Value = "Back1"
                }
            }
    };

    private readonly Translation cancelTranslationEnUs = new()
    {
        Key = "Cancel",
        Translations = new List<TranslationValue>
            {
                new() {
                    Language = "en-us",
                    Value = "Cancel1"
                }
            }
    };

    private readonly Translation itemTranslationEnUsItIt = new()
    {
        Key = "Item",
        Translations = new List<TranslationValue>
            {
                new() {
                    Language = "en-us",
                    Value = "Item1"
                },
                new() {
                    Language = "it-it",
                    Value = "Item2"
                }
            }
    };


    [TestInitialize]
    public void TestInitialize()
    {
        template = File.ReadAllText("ResourceTemplate.xml");
        translationService = new TranslationService();
        translationList = new TranslationList
        {
            Translations = new List<Translation>
            {
                backTranslationEnUs,
                itemTranslationEnUsItIt
            }
        };
    }

    /*
        Input:
        <root>
            <data key='Back'>
                <value lang='en-us'>Back1</value>
            </data>
        </root>
     */
    [TestMethod]
    public void ImportTranslationList_WhenParsingATranslation_ShouldReturnCorrectResult()
    {
        var input = "<root><data key='Back'> <value lang='en-us'>Back1</value> </data></root>";
        var result = translationService.ImportTranslationList(input);
        Assert.AreEqual(1, result.Translations.Count);
        VerifyTranslation(backTranslationEnUs, result.Translations.First());
    }

    [TestMethod]
    public void TranslationService_WhenVerifyingTheFlow_ShouldGenerateContent()
    {
        var input = "<root><data key='Back'> <value lang='en-us'>Back1</value> </data></root>";
        var expectedEnumCode = $"namespace Namespace1;\n\npublic enum Strings1Key\n{{\n\tBack, \t// Back1\n}}";
        var expectedResw = string.Format(template, "<data name=\"Back\" xml:space=\"preserve\">\n\t<value>Back1</value>\n</data>");

        var translationList = translationService.ImportTranslationList(input);
        var enumCode = translationService.ExportToCSharpEnum(translationList, "en-us", "Strings1", "Namespace1");
        Assert.AreEqual(expectedEnumCode, enumCode);

        var resw = translationService.ExportToResw(translationList, "en-us");
        Assert.AreEqual(expectedResw, resw);
    }

    /*
        Input:
        <root>
            <data key='Back'>
                <value lang='en-us'>Back1</value>
            </data>
            <data key='Cancel'>
                <value lang='en-us'>Cancel1</value>
            </data>
        </root>
     */
    [TestMethod]
    public void ParsingInput_WhenParsingTwoTranslations_ShouldReturnCorrectResult()
    {
        var input = "<root><data key='Back'> <value lang='en-us'>Back1</value> </data><data key='Cancel'><value lang='en-us'>Cancel1</value></data></root>";
        var result = translationService.ImportTranslationList(input);
        Assert.AreEqual(2, result.Translations.Count);
        VerifyTranslation(backTranslationEnUs, result.Translations.First());
        VerifyTranslation(cancelTranslationEnUs, result.Translations.Skip(1).First());
    }

    /*
        Output:
        <data name="Back" xml:space="preserve">
            <value>Back1</value>
        </data>
     */

    [TestMethod]
    public void ExportToLocalizationFile_WhenTranslation_ShouldReturnCorrectResult()
    {
        var result = translationService.ExportToResw(new TranslationList { Translations = new List<Translation> { backTranslationEnUs } }, "en-us");
        var expected = string.Format(template, "<data name=\"Back\" xml:space=\"preserve\">\n\t<value>Back1</value>\n</data>");
        Assert.AreEqual(expected, result);
    }

    /*
        Output:
        <data name="Back" xml:space="preserve">
            <value>Back2</value>
        </data>
    */

    [TestMethod]
    public void ExportToLocalizationFile_WhenItemWithTwoTranslations_ShouldReturnCorrectResult()
    {
        var result = translationService.ExportToResw(new TranslationList { Translations = new List<Translation> { itemTranslationEnUsItIt } }, "it-it");
        var expected = string.Format(template, "<data name=\"Item\" xml:space=\"preserve\">\n\t<value>Item2</value>\n</data>");
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ExportToResw_WhenCalled_GeneratesEnglishTranslation()
    {
        var input = "<root><data key='Back'> <value lang='en-us'>Back1</value> </data><data key='Cancel'><value lang='en-us'>Cancel1</value></data></root>";
        var expected = string.Format(template, $"<data name=\"Back\" xml:space=\"preserve\">\n\t<value>Back1</value>\n</data>{Environment.NewLine}<data name=\"Cancel\" xml:space=\"preserve\">\n\t<value>Cancel1</value>\n</data>");
        var translation = translationService.ImportTranslationList(input);
        var localization = translationService.ExportToResw(translation, "en-us");

        Assert.AreEqual(expected, localization);
    }

    private static void VerifyTranslation(Translation expected, Translation actual)
    {
        Assert.AreEqual(expected.Key, actual.Key);
        Assert.AreEqual(expected.Translations.Count, actual.Translations.Count);
        Assert.AreEqual(expected.Translations.First().Language, actual.Translations.First().Language);
        Assert.AreEqual(expected.Translations.First().Value, actual.Translations.First().Value);
    }

    [TestMethod]
    public void ExportToXamlDictionary_WhenExportingOneTranslation_ShouldExportTheCorrectFormat()
    {
        var expected = $"<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n    <x:String x:Key=\"Back\">Back</x:String>\n</ResourceDictionary>";
        Assert.AreEqual(expected, translationService.ExportToXamlIdentityDictionary(new TranslationList
        {
            Translations = new List<Translation> { backTranslationEnUs }
        }));
    }

    [TestMethod]
    public void ExportToXamlDictionary_WhenExportingTwoTranslations_ShouldExportTheCorrectFormat()
    {
        var expected = $"<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n    <x:String x:Key=\"Back\">Back</x:String>\n    <x:String x:Key=\"Cancel\">Cancel</x:String>\n</ResourceDictionary>";
        Assert.AreEqual(expected, translationService.ExportToXamlIdentityDictionary(new TranslationList
        {
            Translations = new List<Translation> { backTranslationEnUs, cancelTranslationEnUs }
        }));
    }
}