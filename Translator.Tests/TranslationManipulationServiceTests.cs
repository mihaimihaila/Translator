namespace Translator.Tests
{
    [TestClass]
    public class TranslationManipulationServiceTests
    {
        private ITranslationManipulation translationManipulation;

        private TranslationList translationList;

        private string template;

        private readonly Translation backTranslationEnUs = new Translation
        {
            Key = "Back",
            Translations = new List<TranslationValue>
                {
                    new TranslationValue
                    {
                        Language = "en-us",
                        Value = "Back1"
                    }
                }
        };

        private readonly Translation blaTranslationEnUs = new Translation
        {
            Key = "Bla",
            Translations = new List<TranslationValue>
                {
                    new TranslationValue
                    {
                        Language = "en-us",
                        Value = "Bla1"
                    }
                }
        };

        private readonly Translation itemTranslationEnUsItIt = new Translation
        {
            Key = "Item",
            Translations = new List<TranslationValue>
                {
                    new TranslationValue
                    {
                        Language = "en-us",
                        Value = "Item1"
                    },
                    new TranslationValue
                    {
                        Language = "it-it",
                        Value = "Item2"
                    }
                }
        };


        [TestInitialize]
        public void TestInitialize()
        {
            template = File.ReadAllText("ResourceTemplate.xml");
            translationManipulation = new TranslationManipulationService();
            translationList = new TranslationList
            {
                Translations = new List<Translation>
                {
                    backTranslationEnUs,
                    itemTranslationEnUsItIt
                }
            };
        }

        [TestMethod]
        public void ExportToPlaintext_WhenItem_ShouldReturnCorrectResult()
        {
            var result = translationManipulation.ExportToPlaintext(new TranslationList
            {
                Translations = new List<Translation> { backTranslationEnUs }
            },
            "en-us", "it-it", "");
            Assert.AreEqual("Back1", result);
        }

        [TestMethod]
        public void ExportToPlaintext_WhenItemExists_ShouldReturnCorrectResult()
        {
            var result = translationManipulation.ExportToPlaintext(new TranslationList
            {
                Translations = new List<Translation>
                {
                    backTranslationEnUs
                }
            }, "en-us", "en-us", "");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ExportToPlaintext_WhenCollection_ShouldReturnCorrectResult()
        {
            var result = translationManipulation.ExportToPlaintext(new TranslationList
            {
                Translations = new List<Translation>
                {
                    backTranslationEnUs
                }
            }
            , "en-us", "it-it", "");
            Assert.AreEqual("Back1", result);
        }

        [TestMethod]
        public void ExportToPlaintext_WhenCollectionWithExistingTranslation_ShouldReturnCorrectResult()
        {
            var result = translationManipulation.ExportToPlaintext(new TranslationList
            {
                Translations = new List<Translation>
                {
                    backTranslationEnUs,
                    itemTranslationEnUsItIt,
                    blaTranslationEnUs
                }
            }
            , "en-us", "it-it", " ");
            Assert.AreEqual($"Back1 Bla1", result);
        }

        [TestMethod]
        public void AppendPlaintextTranslations_WhenCollectionWithExistingTranslation_ShouldReturnCorrectResult()
        {
            var result = translationManipulation.AppendPlaintextTranslations(new TranslationList
            {
                Translations = new List<Translation>
                {
                    backTranslationEnUs,
                    itemTranslationEnUsItIt,
                    blaTranslationEnUs
                }
            }, "Back3 Bla3", "it-it", " ");

            Assert.AreEqual(3, result.Translations.Count);
            Assert.AreEqual("Back", result.Translations[0].Key);
            Assert.AreEqual("Back1", result.Translations[0].Translations.Single(x => x.Language == "en-us").Value);
            Assert.AreEqual("Back3", result.Translations[0].Translations.Single(x => x.Language == "it-it").Value);

            Assert.AreEqual("Item", result.Translations[1].Key);
            Assert.AreEqual("Item1", result.Translations[1].Translations.Single(x => x.Language == "en-us").Value);
            Assert.AreEqual("Item2", result.Translations[1].Translations.Single(x => x.Language == "it-it").Value);

            Assert.AreEqual("Bla", result.Translations[2].Key);
            Assert.AreEqual("Bla1", result.Translations[2].Translations.Single(x => x.Language == "en-us").Value);
            Assert.AreEqual("Bla3", result.Translations[2].Translations.Single(x => x.Language == "it-it").Value);
        }

        [TestMethod]
        public void AppendPlaintextTranslations_WhenTranslating_ShouldReturnCorrectResult()
        {
            var translationService = new TranslationService();

            var language = "it-it";
            var input = "<root><data key='Back'> <value lang='en-us'>Back1</value> </data><data key='Cancel'><value lang='en-us'>Cancel1</value></data></root>";
            var translationInput = translationService.ImportTranslationList(input);

            var output = translationManipulation.ExportToPlaintext(translationInput, "en-us", language, " ");
            Assert.AreEqual("Back1 Cancel1", output);

            var translated = translationManipulation.AppendPlaintextTranslations(translationInput, "Back3 Cancel3", language, " ");
            var localization = translationService.ExportToResw(translated, language);
            var expected = string.Format(template, $"<data name=\"Back\" xml:space=\"preserve\">\n\t<value>Back3</value>\n</data>{Environment.NewLine}<data name=\"Cancel\" xml:space=\"preserve\">\n\t<value>Cancel3</value>\n</data>");

            Assert.AreEqual(expected, localization);
        }

        [TestMethod]
        public void GenerateTranslation_WhenSavingWithAutoInput_ShouldReturnCorrectResult()
        {
            var translationService = new TranslationService();
            var input = "<root><data key='Back'><value lang='en-us'>Back1</value></data><data key='Cancel'><value lang='en-us'>Cancel1</value></data></root>";
            var expectedOutput = "<root><data key=\"Back\"><value lang=\"en-us\">Back1</value><value lang=\"it-it\" mode=\"auto\">Back3</value></data><data key=\"Cancel\"><value lang=\"en-us\">Cancel1</value><value lang=\"it-it\" mode=\"auto\">Cancel3</value></data></root>";

            var translationInput = translationService.ImportTranslationList(input);
            _ = translationManipulation.AppendPlaintextTranslations(translationInput, "Back3 Cancel3", "it-it", " ");

            Assert.AreEqual(expectedOutput, translationManipulation.GenerateTranslation(translationInput, ""));
        }

        [TestMethod]
        public void GenerateTranslation_WhenSaving_ShouldReturnCorrectResult()
        {
            var translationService = new TranslationService();
            var input = "<root><data key=\"Back\"><value lang=\"en-us\">Back1</value></data><data key=\"Cancel\"><value lang=\"en-us\">Cancel1</value></data></root>";
            var translationInput = translationService.ImportTranslationList(input);
            Assert.AreEqual(input, translationManipulation.GenerateTranslation(translationInput, ""));
        }
    }
}