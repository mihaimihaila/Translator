namespace Translator.Tests
{
    [TestClass]
    public class StringHelpersTests
    {
        [TestMethod]
        public void RemoveCSharpUnsupportedCharacters_WhenContainsUnsupportedCharacters_ShouldRemoveAllUnsupportedCharacters()
        {
            Assert.AreEqual("StartEnd", "Start -&.,·'!:/End".RemoveCSharpUnsupportedCharacters());
        }

        [TestMethod]
        public void ReplaceReswUnsuportedCharacters_WhenContainsUnsupportedCharacters_ShouldReplaceAllUnsupportedCharacters()
        {
            Assert.AreEqual("Start&amp;End", "Start&End".ReplaceReswUnsuportedCharacters());
        }
    }
}