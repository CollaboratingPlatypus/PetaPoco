namespace PetaPoco.Core.Inflection
{
    /// <summary>
    /// The IInflector interface specifies the inflection contract all inheriting class implementations must implement.
    /// </summary>
    public interface IInflector
    {
        /// <summary>
        /// Pluralises a word.
        /// </summary>
        /// <remarks>
        /// <c>fox => foxes</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Pluralise">
        /// <![CDATA[
        /// inflect.Pluralise("search").ShouldBe("searches");
        /// inflect.Pluralise("stack").ShouldBe("stacks");
        /// inflect.Pluralise("fish").ShouldBe("fish");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="word">The word to pluralise.</param>
        /// <returns>The pluralised word.</returns>
        string Pluralise(string word);

        /// <summary>
        /// Singularises a word.
        /// </summary>
        /// <remarks>
        /// <c>foxes => fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Singularise">
        /// <![CDATA[
        /// inflect.Singularise("searches").ShouldBe("search");
        /// inflect.Singularise("stacks").ShouldBe("stack");
        /// inflect.Singularise("fish").ShouldBe("fish");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="word">The word to singularise.</param>
        /// <returns>The singularised word.</returns>
        string Singularise(string word);

        /// <summary>
        /// Titleises the word using a "Title Case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the_brown_fox => The Brown Fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Titleise">
        /// <![CDATA[
        /// inflect.Titleise("some title").ShouldBe("Some Title");
        /// inflect.Titleise("some-title").ShouldBe("Some Title");
        /// inflect.Titleise("sometitle").ShouldBe("Sometitle");
        /// inflect.Titleise("some_title:_the_beginning").ShouldBe("Some Title: The Beginning");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="word">The word to titleise.</param>
        /// <returns>The titleised word.</returns>
        string Titleise(string word);

        /// <summary>
        /// Humanises the word using <see langword="abstract"/>"Sentence case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the_brown_fox => The brown fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Humanise">
        /// <![CDATA[
        /// inflect.Humanise("some_title").ShouldBe("Some title");
        /// inflect.Humanise("some-title").ShouldBe("Some-title");
        /// inflect.Humanise("Some_title").ShouldBe("Some title");
        /// inflect.Humanise("someTitle").ShouldBe("Sometitle");
        /// inflect.Humanise("someTitle_Another").ShouldBe("Sometitle another");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to humanise.</param>
        /// <returns>The humanised word.</returns>
        string Humanise(string lowercaseAndUnderscoredWord);

        /// <summary>
        /// Pascalises the word using a "PascalCase" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the_brown_fox => TheBrownFox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Pascalise">
        /// <![CDATA[
        /// inflect.Pascalise("customer").ShouldBe("Customer");
        /// inflect.Pascalise("customer_name").ShouldBe("CustomerName");
        /// inflect.Pascalise("customer name").ShouldBe("Customer name");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to pascalise.</param>
        /// <returns>The pascalised word.</returns>
        string Pascalise(string lowercaseAndUnderscoredWord);

        /// <summary>
        /// Camelises the word using a "camelCase" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the_brown_fox => theBrownFox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Camelise">
        /// <![CDATA[
        /// inflect.Camelise("Customer").ShouldBe("customer");
        /// inflect.Camelise("customer_name").ShouldBe("customerName");
        /// inflect.Camelise("customer_first_name").ShouldBe("customerFirstName");
        /// inflect.Camelise("customer name").ShouldBe("customer name");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="lowercaseAndUnderscoredWord">The word to camelise.</param>
        /// <returns>The camelised word.</returns>
        string Camelise(string lowercaseAndUnderscoredWord);

        /// <summary>
        /// Underscores and lowercases the word using a "snake_case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>TheBrownFox => the_brown_fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Underscore">
        /// <![CDATA[
        /// inflect.Underscore("SomeTitle").ShouldBe("some_title");
        /// inflect.Underscore("someTitle").ShouldBe("some_title");
        /// inflect.Underscore("some title that will be underscored").ShouldBe("some_title_that_will_be_underscored");
        /// inflect.Underscore("SomeTitleThatWillBeUnderscored").ShouldBe("some_title_that_will_be_underscored");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="pascalCasedWord">The word to underscore.</param>
        /// <returns>The underscored word.</returns>
        string Underscore(string pascalCasedWord);

        /// <summary>
        /// Capitalises the word using an "Initial upper case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the brown Fox => The brown fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Capitalise">
        /// <![CDATA[
        /// inflect.Capitalise("some title").ShouldBe("Some title");
        /// inflect.Capitalise("some Title").ShouldBe("Some title");
        /// inflect.Capitalise("SOMETITLE").ShouldBe("Sometitle");
        /// inflect.Capitalise("someTitle").ShouldBe("Sometitle");
        /// inflect.Capitalise("some title goes here").ShouldBe("Some title goes here");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="word">The word to capitalise.</param>
        /// <returns>The capitalised word.</returns>
        string Capitalise(string word);

        /// <summary>
        /// Uncapitalises the word using an "initial lower case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>The brown Fox => the brown Fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Uncapitalise">
        /// <![CDATA[
        /// inflect.Uncapitalise("Some title").ShouldBe("some title");
        /// inflect.Uncapitalise("Some Title").ShouldBe("some Title");
        /// inflect.Uncapitalise("SOMETITLE").ShouldBe("sOMETITLE");
        /// inflect.Uncapitalise("someTitle").ShouldBe("someTitle");
        /// inflect.Uncapitalise("Some title goes here").ShouldBe("some title goes here");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="word">The word to uncapitalise.</param>
        /// <returns>The uncapitalised word.</returns>
        string Uncapitalise(string word);

        /// <summary>
        /// Parses and Ordinalises the number string using a cardinal number (1,2,3...) to ordinal number (1st,2nd,3rd...) transformation.
        /// </summary>
        /// <remarks>
        /// <c>1 => 1st</c>, <c>2 => 2nd</c>, <c>3 => 3rd</c>...
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Ordinalise">
        /// <![CDATA[
        /// inflect.Ordinalise(0).ShouldBe("0th");
        /// inflect.Ordinalise(1).ShouldBe("1st");
        /// inflect.Ordinalise(2).ShouldBe("2nd");
        /// inflect.Ordinalise(3).ShouldBe("3rd");
        /// inflect.Ordinalise(101).ShouldBe("101st");
        /// inflect.Ordinalise(104).ShouldBe("104th");
        /// inflect.Ordinalise(1000).ShouldBe("1000th");
        /// inflect.Ordinalise(1001).ShouldBe("1001st");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="number">The number to ordinalise.</param>
        /// <returns>The ordinalised number.</returns>
        string Ordinalise(string number);

        /// <summary>
        /// Ordinalises the number using a cardinal number (1,2,3...) to ordinal number (1st,2nd,3rd...) transformation.
        /// </summary>
        /// <remarks>
        /// <c>1 => 1st</c>, <c>2 => 2nd</c>, <c>3 => 3rd</c>...
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Ordinalise">
        /// <![CDATA[
        /// inflect.Ordinalise("0").ShouldBe("0th");
        /// inflect.Ordinalise("1").ShouldBe("1st");
        /// inflect.Ordinalise("2").ShouldBe("2nd");
        /// inflect.Ordinalise("3").ShouldBe("3rd");
        /// inflect.Ordinalise("100").ShouldBe("100th");
        /// inflect.Ordinalise("101").ShouldBe("101st");
        /// inflect.Ordinalise("1000").ShouldBe("1000th");
        /// inflect.Ordinalise("1001").ShouldBe("1001st");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="number">The number to ordinalise.</param>
        /// <returns>The ordinalised number.</returns>
        string Ordinalise(int number);

        /// <summary>
        /// Dasherises the word using a "kebob-case" transformation.
        /// </summary>
        /// <remarks>
        /// <c>the_brown_fox => the-brown-fox</c>
        /// </remarks>
        /// <example>
        /// <code language="cs" title="Dasherise">
        /// <![CDATA[
        /// inflect.Dasherise("some_title").ShouldBe("some-title");
        /// inflect.Dasherise("some-title").ShouldBe("some-title");
        /// inflect.Dasherise("some_title_goes_here").ShouldBe("some-title-goes-here");
        /// inflect.Dasherise("some_title and_another").ShouldBe("some-title and-another");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="underscoredWord">The word to dasherise.</param>
        /// <returns>The dasherised word.</returns>
        string Dasherise(string underscoredWord);
    }
}
