// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/23</date>

namespace PetaPoco.Core.Inflection
{
    /// <summary>
    ///     Specifies the inflection contract.
    /// </summary>
    public interface IInflector
    {
        /// <summary>
        ///     Pluralises a word.
        /// </summary>
        /// <example>
        ///     inflect.Pluralise("search").ShouldBe("searches");
        ///     inflect.Pluralise("stack").ShouldBe("stacks");
        ///     inflect.Pluralise("fish").ShouldBe("fish");
        /// </example>
        /// <param name="word">The word to pluralise.</param>
        /// <returns>The pluralised word.</returns>
        string Pluralise(string word);

        /// <summary>
        ///     Singularises a word.
        /// </summary>
        /// <example>
        ///     inflect.Singularise("searches").ShouldBe("search");
        ///     inflect.Singularise("stacks").ShouldBe("stack");
        ///     inflect.Singularise("fish").ShouldBe("fish");
        /// </example>
        /// <param name="word">The word to signularise.</param>
        /// <returns>The signularised word.</returns>
        string Singularise(string word);

        /// <summary>
        ///     Titleises the word. (title => Title, the_brown_fox => TheBrownFox)
        /// </summary>
        /// <example>
        ///     inflect.Titleise("some title").ShouldBe("Some Title");
        ///     inflect.Titleise("some-title").ShouldBe("Some Title");
        ///     inflect.Titleise("sometitle").ShouldBe("Sometitle");
        ///     inflect.Titleise("some_title:_the_beginning").ShouldBe("Some Title: The Beginning");
        /// </example>
        /// <param name="word">The word to titleise.</param>
        /// <returns>The titleised word.</returns>
        string Titleise(string word);

        /// <summary>
        ///     Humanizes the word.
        /// </summary>
        /// <example>
        ///     inflect.Humanise("some_title").ShouldBe("Some title");
        ///     inflect.Humanise("some-title").ShouldBe("Some-title");
        ///     inflect.Humanise("Some_title").ShouldBe("Some title");
        ///     inflect.Humanise("someTitle").ShouldBe("Sometitle");
        ///     inflect.Humanise("someTitle_Another").ShouldBe("Sometitle another");
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to humanise.</param>
        /// <returns>The humanized word.</returns>
        string Humanise(string lowercaseAndUnderscoredWord);

        /// <summary>
        ///     Pascalises the word.
        /// </summary>
        /// <example>
        ///     inflect.Pascalise("customer").ShouldBe("Customer");
        ///     inflect.Pascalise("customer_name").ShouldBe("CustomerName");
        ///     inflect.Pascalise("customer name").ShouldBe("Customer name");
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to pascalise.</param>
        /// <returns>The pascalied word.</returns>
        string Pascalise(string lowercaseAndUnderscoredWord);

        /// <summary>
        ///     Camelises the word.
        /// </summary>
        /// <example>
        ///     inflect.Camelise("Customer").ShouldBe("customer");
        ///     inflect.Camelise("customer_name").ShouldBe("customerName");
        ///     inflect.Camelise("customer_first_name").ShouldBe("customerFirstName");
        ///     inflect.Camelise("customer name").ShouldBe("customer name");
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to camelise.</param>
        /// <returns>The camelised word.</returns>
        string Camelise(string lowercaseAndUnderscoredWord);

        /// <summary>
        ///     Underscores the word.
        /// </summary>
        /// <example>
        ///     inflect.Underscore("SomeTitle").ShouldBe("some_title");
        ///     inflect.Underscore("someTitle").ShouldBe("some_title");
        ///     inflect.Underscore("some title that will be underscored").ShouldBe("some_title_that_will_be_underscored");
        ///     inflect.Underscore("SomeTitleThatWillBeUnderscored").ShouldBe("some_title_that_will_be_underscored");
        /// </example>
        /// <param name="pascalCasedWord">The word to underscore.</param>
        /// <returns>The underscored word.</returns>
        string Underscore(string pascalCasedWord);

        /// <summary>
        ///     Capitalises the word.
        /// </summary>
        /// <example>
        ///     inflect.Capitalise("some title").ShouldBe("Some title");
        ///     inflect.Capitalise("some Title").ShouldBe("Some title");
        ///     inflect.Capitalise("SOMETITLE").ShouldBe("Sometitle");
        ///     inflect.Capitalise("someTitle").ShouldBe("Sometitle");
        ///     inflect.Capitalise("some title goes here").ShouldBe("Some title goes here");
        /// </example>
        /// <param name="word">The word to capitalise.</param>
        /// <returns>The capitalised word.</returns>
        string Capitalise(string word);

        /// <summary>
        ///     Uncapitalises the word.
        /// </summary>
        /// <example>
        ///     inflect.Uncapitalise("Some title").ShouldBe("some title");
        ///     inflect.Uncapitalise("Some Title").ShouldBe("some Title");
        ///     inflect.Uncapitalise("SOMETITLE").ShouldBe("sOMETITLE");
        ///     inflect.Uncapitalise("someTitle").ShouldBe("someTitle");
        ///     inflect.Uncapitalise("Some title goes here").ShouldBe("some title goes here");
        /// </example>
        /// <param name="word">The word to uncapitalise.</param>
        /// <returns>The uncapitalised word.</returns>
        string Uncapitalise(string word);

        /// <summary>
        ///     Ordinalises the number.
        /// </summary>
        /// <example>
        ///     inflect.Ordinalise(0).ShouldBe("0th");
        ///     inflect.Ordinalise(1).ShouldBe("1st");
        ///     inflect.Ordinalise(2).ShouldBe("2nd");
        ///     inflect.Ordinalise(3).ShouldBe("3rd");
        ///     inflect.Ordinalise(101).ShouldBe("101st");
        ///     inflect.Ordinalise(104).ShouldBe("104th");
        ///     inflect.Ordinalise(1000).ShouldBe("1000th");
        ///     inflect.Ordinalise(1001).ShouldBe("1001st");
        /// </example>
        /// <param name="number">The number to ordinalise.</param>
        /// <returns>The ordinalised number.</returns>
        string Ordinalise(string number);

        /// <summary>
        ///     Ordinalises the number.
        /// </summary>
        /// <example>
        ///     inflect.Ordinalise("0").ShouldBe("0th");
        ///     inflect.Ordinalise("1").ShouldBe("1st");
        ///     inflect.Ordinalise("2").ShouldBe("2nd");
        ///     inflect.Ordinalise("3").ShouldBe("3rd");
        ///     inflect.Ordinalise("100").ShouldBe("100th");
        ///     inflect.Ordinalise("101").ShouldBe("101st");
        ///     inflect.Ordinalise("1000").ShouldBe("1000th");
        ///     inflect.Ordinalise("1001").ShouldBe("1001st");
        /// </example>
        /// <param name="number">The number to ordinalise.</param>
        /// <returns>The ordinalised number.</returns>
        string Ordinalise(int number);

        /// <summary>
        ///     Dasherises the word.
        /// </summary>
        /// <example>
        ///     inflect.Dasherise("some_title").ShouldBe("some-title");
        ///     inflect.Dasherise("some-title").ShouldBe("some-title");
        ///     inflect.Dasherise("some_title_goes_here").ShouldBe("some-title-goes-here");
        ///     inflect.Dasherise("some_title and_another").ShouldBe("some-title and-another");
        /// </example>
        /// <param name="underscoredWord">The word to dasherise.</param>
        /// <returns>The dasherised word.</returns>
        string Dasherise(string underscoredWord);
    }
}