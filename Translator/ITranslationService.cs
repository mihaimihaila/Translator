namespace Translator
{
    public interface ITranslationService
    {
        /// <summary>
        /// Imports a TranslationList from a XML document
        /// </summary>
        /// <param name="input">An XML document containing the translation list
        /// <returns>A translation list</returns>
        TranslationList ImportTranslationList(string input);
        
        /// <summary>
        /// Exports a translation list to .resw format
        /// </summary>
        /// <param name="translationList">The translation list</param>
        /// <param name="language">The language of the .resw translation. If a translation does not exist for the given language, the reference en-us translation is used.</param>
        /// <returns>A .resw string</returns>
        string ExportToResw(TranslationList translationList, string language);

        /// <summary>
        /// Generates a CSharp enum based on a translation list
        /// </summary>
        /// <param name="translationList">The translation list</param>
        /// <param name="language">The language on which to based the enum on e.g. en-us</param>
        /// <param name="name">The name of the generated enum</param>
        /// <param name="namespaceName">The namespace of the generated enum</param>
        /// <returns>A CSharp file containing a enum with all the translations as cases.</returns>
        string ExportToCSharpEnum(TranslationList translationList, string language, string name, string namespaceName);

        /// <summary>
        /// Generates a XAML Dictionary for the translation list
        /// </summary>
        /// <param name="translationList">The translation list</param>
        /// <returns>A XAML dictionary containing the translations as string resources. The value of each resource is the same as the key</returns>
        string ExportToXamlIdentityDictionary(TranslationList translationList);
    }
}