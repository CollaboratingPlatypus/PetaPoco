// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/22</date>

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetaPoco.Core.Inflection
{
    /// <summary>
    ///     Author: Originally written (I believe) by Andrew Peters
    ///     Source: Scott Kirkland (https://github.com/srkirkland/Inflector)
    /// </summary>
    public class EnglishInflector : IInflector
    {
        private static readonly List<Rule> Plurals = new List<Rule>();

        private static readonly List<Rule> Singulars = new List<Rule>();

        private static readonly List<string> Uncountables = new List<string>();

        static EnglishInflector()
        {
            AddPlural("$", "s");
            AddPlural("s$", "s");
            AddPlural("(ax|test)is$", "$1es");
            AddPlural("(octop|vir|alumn|fung)us$", "$1i");
            AddPlural("(alias|status)$", "$1es");
            AddPlural("(bu)s$", "$1ses");
            AddPlural("(buffal|tomat|volcan)o$", "$1oes");
            AddPlural("([ti])um$", "$1a");
            AddPlural("sis$", "ses");
            AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPlural("(hive)$", "$1s");
            AddPlural("([^aeiouy]|qu)y$", "$1ies");
            AddPlural("(x|ch|ss|sh)$", "$1es");
            AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
            AddPlural("([m|l])ouse$", "$1ice");
            AddPlural("^(ox)$", "$1en");
            AddPlural("(quiz)$", "$1zes");

            AddSingular("s$", "");
            AddSingular("(n)ews$", "$1ews");
            AddSingular("([ti])a$", "$1um");
            AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingular("(^analy)ses$", "$1sis");
            AddSingular("([^f])ves$", "$1fe");
            AddSingular("(hive)s$", "$1");
            AddSingular("(tive)s$", "$1");
            AddSingular("([lr])ves$", "$1f");
            AddSingular("([^aeiouy]|qu)ies$", "$1y");
            AddSingular("(s)eries$", "$1eries");
            AddSingular("(m)ovies$", "$1ovie");
            AddSingular("(x|ch|ss|sh)es$", "$1");
            AddSingular("([m|l])ice$", "$1ouse");
            AddSingular("(bus)es$", "$1");
            AddSingular("(o)es$", "$1");
            AddSingular("(shoe)s$", "$1");
            AddSingular("(cris|ax|test)es$", "$1is");
            AddSingular("(octop|vir|alumn|fung)i$", "$1us");
            AddSingular("(alias|status)es$", "$1");
            AddSingular("^(ox)en", "$1");
            AddSingular("(vert|ind)ices$", "$1ex");
            AddSingular("(matr)ices$", "$1ix");
            AddSingular("(quiz)zes$", "$1");

            AddIrregular("person", "people");
            AddIrregular("man", "men");
            AddIrregular("child", "children");
            AddIrregular("sex", "sexes");
            AddIrregular("move", "moves");
            AddIrregular("goose", "geese");
            AddIrregular("alumna", "alumnae");

            AddUncountable("equipment");
            AddUncountable("information");
            AddUncountable("rice");
            AddUncountable("money");
            AddUncountable("species");
            AddUncountable("series");
            AddUncountable("fish");
            AddUncountable("sheep");
            AddUncountable("deer");
            AddUncountable("aircraft");
        }

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
        public string Pluralise(string word)
        {
            return ApplyRules(Plurals, word);
        }

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
        public string Singularise(string word)
        {
            return ApplyRules(Singulars, word);
        }

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
        public string Titleise(string word)
        {
            return Regex.Replace(Humanise(Underscore(word)), @"\b([a-z])",
                match => match.Captures[0].Value.ToUpper());
        }

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
        public string Humanise(string lowercaseAndUnderscoredWord)
        {
            return Capitalise(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

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
        public string Pascalise(string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
                match => match.Groups[1].Value.ToUpper());
        }

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
        public string Camelise(string lowercaseAndUnderscoredWord)
        {
            return Uncapitalise(Pascalise(lowercaseAndUnderscoredWord));
        }

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
        public string Underscore(string pascalCasedWord)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                    "$1_$2"), @"[-\s]", "_").ToLower();
        }

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
        public string Capitalise(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

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
        public string Uncapitalise(string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

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
        public string Ordinalise(string number)
        {
            return Ordanise(int.Parse(number), number);
        }

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
        public string Ordinalise(int number)
        {
            return Ordanise(number, number.ToString());
        }

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
        public string Dasherise(string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        private static void AddIrregular(string singular, string plural)
        {
            AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
            AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
        }

        private static void AddUncountable(string word)
        {
            Uncountables.Add(word.ToLower());
        }

        private static void AddPlural(string rule, string replacement)
        {
            Plurals.Add(new Rule(rule, replacement));
        }

        private static void AddSingular(string rule, string replacement)
        {
            Singulars.Add(new Rule(rule, replacement));
        }

        private static string ApplyRules(IList<Rule> rules, string word)
        {
            var result = word;

            if (Uncountables.Contains(word.ToLower()))
                return result;

            for (var i = rules.Count - 1; i >= 0; i--)
            {
                if ((result = rules[i].Apply(word)) != null)
                {
                    break;
                }
            }

            return result;
        }

        private static string Ordanise(int number, string numberString)
        {
            var nMod100 = number%100;

            if (nMod100 >= 11 && nMod100 <= 13)
            {
                return numberString + "th";
            }

            switch (number%10)
            {
                case 1:
                    return numberString + "st";
                case 2:
                    return numberString + "nd";
                case 3:
                    return numberString + "rd";
                default:
                    return numberString + "th";
            }
        }

        private class Rule
        {
            private readonly Regex _regex;

            private readonly string _replacement;

            public Rule(string pattern, string replacement)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _replacement = replacement;
            }

            public string Apply(string word)
            {
                return !_regex.IsMatch(word) ? null : _regex.Replace(word, _replacement);
            }
        }
    }
}