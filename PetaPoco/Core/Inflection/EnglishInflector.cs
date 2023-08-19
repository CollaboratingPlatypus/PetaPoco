using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetaPoco.Core.Inflection
{
    /// <summary>
    /// The EnglishInflector class implements inflection for the English language.
    /// </summary>
    /// <remarks>
    /// Author: Originally written (I believe) by Andrew Peters
    /// <br/>Source: Scott Kirkland (<see href="https://github.com/srkirkland/Inflector"/>)
    /// </remarks>
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

        /// <inheritdoc/>
        public string Pluralise(string word)
        {
            return ApplyRules(Plurals, word);
        }

        /// <inheritdoc/>
        public string Singularise(string word)
        {
            return ApplyRules(Singulars, word);
        }

        /// <inheritdoc/>
        public string Titleise(string word)
        {
            return Regex.Replace(Humanise(Underscore(word)), @"\b([a-z])", match => match.Captures[0].Value.ToUpper());
        }

        /// <inheritdoc/>
        public string Humanise(string lowercaseAndUnderscoredWord)
        {
            return Capitalise(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

        /// <inheritdoc/>
        public string Pascalise(string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <inheritdoc/>
        public string Camelise(string lowercaseAndUnderscoredWord)
        {
            return Uncapitalise(Pascalise(lowercaseAndUnderscoredWord));
        }

        /// <inheritdoc/>
        public string Underscore(string pascalCasedWord)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(
                pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"), @"[-\s]", "_").ToLower();
        }

        /// <inheritdoc/>
        public string Capitalise(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

        /// <inheritdoc/>
        public string Uncapitalise(string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <inheritdoc/>
        public string Ordinalise(string number)
        {
            return Ordinalise(int.Parse(number), number);
        }

        /// <inheritdoc/>
        public string Ordinalise(int number)
        {
            return Ordinalise(number, number.ToString());
        }

        /// <inheritdoc/>
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

        private static string Ordinalise(int number, string numberString)
        {
            var nMod100 = number % 100;

            if (nMod100 >= 11 && nMod100 <= 13)
            {
                return numberString + "th";
            }

            switch (number % 10)
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
