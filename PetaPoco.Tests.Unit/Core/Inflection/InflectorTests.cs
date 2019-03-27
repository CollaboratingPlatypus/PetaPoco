using System;
using PetaPoco.Core.Inflection;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core.Inflection
{
    public class InflectorTests : IDisposable
    {
        public void Dispose()
        {
            Inflector.Instance = null;
        }

        [Fact]
        public void Instance_Default_ShouldBeEnglish()
        {
            Inflector.Instance.ShouldNotBeNull();
            Inflector.Instance.ShouldBeOfType<EnglishInflector>();
        }

        [Fact]
        public void Instance_CanBeReplaced_ShouldNotFail()
        {
            Inflector.Instance = new Wookiee();
            Inflector.Instance.ShouldNotBeNull();
            Inflector.Instance.ShouldBeOfType<Wookiee>();
            Inflector.Instance.Humanise("Test").ShouldBe(Wookiee.WookieeReponse);
        }

        private class Wookiee : IInflector
        {
            public const string WookieeReponse = "huuguughghg uughghhhgh";

            public string Pluralise(string word)
            {
                return WookieeReponse;
            }

            public string Singularise(string word)
            {
                return WookieeReponse;
            }

            public string Titleise(string word)
            {
                return WookieeReponse;
            }

            public string Humanise(string lowercaseAndUnderscoredWord)
            {
                return WookieeReponse;
            }

            public string Pascalise(string lowercaseAndUnderscoredWord)
            {
                return WookieeReponse;
            }

            public string Camelise(string lowercaseAndUnderscoredWord)
            {
                return WookieeReponse;
            }

            public string Underscore(string pascalCasedWord)
            {
                return WookieeReponse;
            }

            public string Capitalise(string word)
            {
                return WookieeReponse;
            }

            public string Uncapitalise(string word)
            {
                return WookieeReponse;
            }

            public string Ordinalise(string number)
            {
                return WookieeReponse;
            }

            public string Ordinalise(int number)
            {
                return WookieeReponse;
            }

            public string Dasherise(string underscoredWord)
            {
                return WookieeReponse;
            }
        }
    }
}