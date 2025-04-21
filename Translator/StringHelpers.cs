namespace Translator;

public static class StringHelpers
{
    public static string RemoveCSharpUnsupportedCharacters(this string word)
    {
        var excludedCharacters = " -&.,·'!:/";
        foreach (var character in excludedCharacters)
        {
            word = word.Replace($"{character}", "");
        }

        return word;
    }

    public static string ReplaceReswUnsuportedCharacters(this string value)
    {
        return value.Replace("&", "&amp;");
    }
}