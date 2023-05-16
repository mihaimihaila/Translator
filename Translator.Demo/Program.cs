using Translator;

/* Args
 * input.xml en-us Common Apps
 * input.xml fr-fr Common Apps
 */

var filename = args[0];
var language = args[1];
var name = args[2];
var namespaceName = args[3];

Console.WriteLine($"Generating translations for: {filename}");

ITranslationService translationService = new TranslationService();

var translationListContent = File.ReadAllText(filename);
var translation = translationService.ImportTranslationList(translationListContent);

var csharpEnumContent = translationService.ExportToCSharpEnum(translation, language, name, namespaceName);
File.WriteAllText($"{name}.cs", csharpEnumContent);

var xamlContent = translationService.ExportToXamlIdentityDictionary(translation);
File.WriteAllText($"{name}.xaml", xamlContent);

var reswContent = translationService.ExportToResw(translation, language);
File.WriteAllText($"{name}.resw", reswContent);

Console.WriteLine($"Done generating translations for: {filename}");