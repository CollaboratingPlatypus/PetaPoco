// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using PetaPoco.Core.Inflection;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core.Inflection
{
    public class EnglishInflectorTests
    {
        private EnglishInflector inflect = new EnglishInflector();

        [Fact]
        public void Pluralise_GivenAWord_ShouldBeValid()
        {
            inflect.Pluralise("search").ShouldBe("searches");
            inflect.Pluralise("switch").ShouldBe("switches");
            inflect.Pluralise("fix").ShouldBe("fixes");
            inflect.Pluralise("box").ShouldBe("boxes");
            inflect.Pluralise("process").ShouldBe("processes");
            inflect.Pluralise("Cameliseress").ShouldBe("Cameliseresses");
            inflect.Pluralise("case").ShouldBe("cases");
            inflect.Pluralise("stack").ShouldBe("stacks");
            inflect.Pluralise("wish").ShouldBe("wishes");
            inflect.Pluralise("fish").ShouldBe("fish");

            inflect.Pluralise("category").ShouldBe("categories");
            inflect.Pluralise("query").ShouldBe("queries");
            inflect.Pluralise("ability").ShouldBe("abilities");
            inflect.Pluralise("agency").ShouldBe("agencies");
            inflect.Pluralise("movie").ShouldBe("movies");

            inflect.Pluralise("archive").ShouldBe("archives");

            inflect.Pluralise("index").ShouldBe("indices");

            inflect.Pluralise("wife").ShouldBe("wives");
            inflect.Pluralise("safe").ShouldBe("saves");
            inflect.Pluralise("half").ShouldBe("halves");

            inflect.Pluralise("move").ShouldBe("moves");

            inflect.Pluralise("salesperson").ShouldBe("salespeople");
            inflect.Pluralise("person").ShouldBe("people");

            inflect.Pluralise("spokesman").ShouldBe("spokesmen");
            inflect.Pluralise("man").ShouldBe("men");
            inflect.Pluralise("woman").ShouldBe("women");

            inflect.Pluralise("basis").ShouldBe("bases");
            inflect.Pluralise("diagnosis").ShouldBe("diagnoses");

            inflect.Pluralise("datum").ShouldBe("data");
            inflect.Pluralise("medium").ShouldBe("media");
            inflect.Pluralise("analysis").ShouldBe("analyses");

            inflect.Pluralise("node_child").ShouldBe("node_children");
            inflect.Pluralise("child").ShouldBe("children");

            inflect.Pluralise("experience").ShouldBe("experiences");
            inflect.Pluralise("day").ShouldBe("days");

            inflect.Pluralise("comment").ShouldBe("comments");
            inflect.Pluralise("foobar").ShouldBe("foobars");
            inflect.Pluralise("newsletter").ShouldBe("newsletters");

            inflect.Pluralise("old_news").ShouldBe("old_news");
            inflect.Pluralise("news").ShouldBe("news");

            inflect.Pluralise("series").ShouldBe("series");
            inflect.Pluralise("species").ShouldBe("species");

            inflect.Pluralise("quiz").ShouldBe("quizzes");

            inflect.Pluralise("perspective").ShouldBe("perspectives");

            inflect.Pluralise("ox").ShouldBe("oxen");
            inflect.Pluralise("photo").ShouldBe("photos");
            inflect.Pluralise("buffalo").ShouldBe("buffaloes");
            inflect.Pluralise("tomato").ShouldBe("tomatoes");
            inflect.Pluralise("dwarf").ShouldBe("dwarves");
            inflect.Pluralise("elf").ShouldBe("elves");
            inflect.Pluralise("information").ShouldBe("information");
            inflect.Pluralise("equipment").ShouldBe("equipment");
            inflect.Pluralise("bus").ShouldBe("buses");
            inflect.Pluralise("status").ShouldBe("statuses");
            inflect.Pluralise("status_code").ShouldBe("status_codes");
            inflect.Pluralise("mouse").ShouldBe("mice");

            inflect.Pluralise("louse").ShouldBe("lice");
            inflect.Pluralise("house").ShouldBe("houses");
            inflect.Pluralise("octopus").ShouldBe("octopi");
            inflect.Pluralise("virus").ShouldBe("viri");
            inflect.Pluralise("alias").ShouldBe("aliases");
            inflect.Pluralise("portfolio").ShouldBe("portfolios");

            inflect.Pluralise("vertex").ShouldBe("vertices");
            inflect.Pluralise("matrix").ShouldBe("matrices");

            inflect.Pluralise("axis").ShouldBe("axes");
            inflect.Pluralise("testis").ShouldBe("testes");
            inflect.Pluralise("crisis").ShouldBe("crises");

            inflect.Pluralise("rice").ShouldBe("rice");
            inflect.Pluralise("shoe").ShouldBe("shoes");

            inflect.Pluralise("horse").ShouldBe("horses");
            inflect.Pluralise("prize").ShouldBe("prizes");
            inflect.Pluralise("edge").ShouldBe("edges");

            /* Tests Cameliseed by Bas Jansen */
            inflect.Pluralise("goose").ShouldBe("geese");
            inflect.Pluralise("deer").ShouldBe("deer");
            inflect.Pluralise("sheep").ShouldBe("sheep");
            inflect.Pluralise("wolf").ShouldBe("wolves");
            inflect.Pluralise("volcano").ShouldBe("volcanoes");
            inflect.Pluralise("aircraft").ShouldBe("aircraft");
            inflect.Pluralise("alumna").ShouldBe("alumnae");
            inflect.Pluralise("alumnus").ShouldBe("alumni");
            inflect.Pluralise("fungus").ShouldBe("fungi");
        }

        [Fact]
        public void Singularise_GivenAWord_ShouldBeValid()
        {
            inflect.Singularise("searches").ShouldBe("search");
            inflect.Singularise("switches").ShouldBe("switch");
            inflect.Singularise("fixes").ShouldBe("fix");
            inflect.Singularise("boxes").ShouldBe("box");
            inflect.Singularise("processes").ShouldBe("process");
            inflect.Singularise("Cameliseresses").ShouldBe("Cameliseress");
            inflect.Singularise("cases").ShouldBe("case");
            inflect.Singularise("stacks").ShouldBe("stack");
            inflect.Singularise("wishes").ShouldBe("wish");
            inflect.Singularise("fish").ShouldBe("fish");

            inflect.Singularise("categories").ShouldBe("category");
            inflect.Singularise("queries").ShouldBe("query");
            inflect.Singularise("abilities").ShouldBe("ability");
            inflect.Singularise("agencies").ShouldBe("agency");
            inflect.Singularise("movies").ShouldBe("movie");

            inflect.Singularise("archives").ShouldBe("archive");

            inflect.Singularise("indices").ShouldBe("index");

            inflect.Singularise("wives").ShouldBe("wife");
            inflect.Singularise("saves").ShouldBe("safe");
            inflect.Singularise("halves").ShouldBe("half");

            inflect.Singularise("moves").ShouldBe("move");

            inflect.Singularise("salespeople").ShouldBe("salesperson");
            inflect.Singularise("people").ShouldBe("person");

            inflect.Singularise("spokesmen").ShouldBe("spokesman");
            inflect.Singularise("men").ShouldBe("man");
            inflect.Singularise("women").ShouldBe("woman");

            inflect.Singularise("bases").ShouldBe("basis");
            inflect.Singularise("diagnoses").ShouldBe("diagnosis");

            inflect.Singularise("data").ShouldBe("datum");
            inflect.Singularise("media").ShouldBe("medium");
            inflect.Singularise("analyses").ShouldBe("analysis");

            inflect.Singularise("node_children").ShouldBe("node_child");
            inflect.Singularise("children").ShouldBe("child");

            inflect.Singularise("experiences").ShouldBe("experience");
            inflect.Singularise("days").ShouldBe("day");

            inflect.Singularise("comments").ShouldBe("comment");
            inflect.Singularise("foobars").ShouldBe("foobar");
            inflect.Singularise("newsletters").ShouldBe("newsletter");

            inflect.Singularise("old_news").ShouldBe("old_news");
            inflect.Singularise("news").ShouldBe("news");

            inflect.Singularise("series").ShouldBe("series");
            inflect.Singularise("species").ShouldBe("species");

            inflect.Singularise("quizzes").ShouldBe("quiz");

            inflect.Singularise("perspectives").ShouldBe("perspective");

            inflect.Singularise("oxen").ShouldBe("ox");
            inflect.Singularise("photos").ShouldBe("photo");
            inflect.Singularise("buffaloes").ShouldBe("buffalo");
            inflect.Singularise("tomatoes").ShouldBe("tomato");
            inflect.Singularise("dwarves").ShouldBe("dwarf");
            inflect.Singularise("elves").ShouldBe("elf");
            inflect.Singularise("information").ShouldBe("information");
            inflect.Singularise("equipment").ShouldBe("equipment");
            inflect.Singularise("buses").ShouldBe("bus");
            inflect.Singularise("statuses").ShouldBe("status");
            inflect.Singularise("status_codes").ShouldBe("status_code");
            inflect.Singularise("mice").ShouldBe("mouse");

            inflect.Singularise("lice").ShouldBe("louse");
            inflect.Singularise("houses").ShouldBe("house");
            inflect.Singularise("octopi").ShouldBe("octopus");
            inflect.Singularise("viri").ShouldBe("virus");
            inflect.Singularise("aliases").ShouldBe("alias");
            inflect.Singularise("portfolios").ShouldBe("portfolio");

            inflect.Singularise("vertices").ShouldBe("vertex");
            inflect.Singularise("matrices").ShouldBe("matrix");

            inflect.Singularise("axes").ShouldBe("axis");
            inflect.Singularise("testes").ShouldBe("testis");
            inflect.Singularise("crises").ShouldBe("crisis");

            inflect.Singularise("rice").ShouldBe("rice");
            inflect.Singularise("shoes").ShouldBe("shoe");

            inflect.Singularise("horses").ShouldBe("horse");
            inflect.Singularise("prizes").ShouldBe("prize");
            inflect.Singularise("edges").ShouldBe("edge");

            /* Tests Cameliseed by Bas Jansen */
            inflect.Singularise("geese").ShouldBe("goose");
            inflect.Singularise("deer").ShouldBe("deer");
            inflect.Singularise("sheep").ShouldBe("sheep");
            inflect.Singularise("wolves").ShouldBe("wolf");
            inflect.Singularise("volcanoes").ShouldBe("volcano");
            inflect.Singularise("aircraft").ShouldBe("aircraft");
            inflect.Singularise("alumnae").ShouldBe("alumna");
            inflect.Singularise("alumni").ShouldBe("alumnus");
            inflect.Singularise("fungi").ShouldBe("fungus");
        }

        [Fact]
        public void Titleise_GivenAWord_ShouldBeValid()
        {
            inflect.Titleise("some title").ShouldBe("Some Title");
            inflect.Titleise("some-title").ShouldBe("Some Title");
            inflect.Titleise("sometitle").ShouldBe("Sometitle");
            inflect.Titleise("some-title: The beginning").ShouldBe("Some Title: The Beginning");
            inflect.Titleise("some_title:_the_beginning").ShouldBe("Some Title: The Beginning");
            inflect.Titleise("some title: The_beginning").ShouldBe("Some Title: The Beginning");
        }

        [Fact]
        public void Humanise_GivenContent_ShouldBeValid()
        {
            inflect.Humanise("some_title").ShouldBe("Some title");
            inflect.Humanise("some-title").ShouldBe("Some-title");
            inflect.Humanise("Some_title").ShouldBe("Some title");
            inflect.Humanise("someTitle").ShouldBe("Sometitle");
            inflect.Humanise("someTitle_Another").ShouldBe("Sometitle another");
        }

        [Fact]
        public void Pascalise_GivenContent_ShouldBeValid()
        {
            inflect.Pascalise("customer").ShouldBe("Customer");
            inflect.Pascalise("CUSTOMER").ShouldBe("CUSTOMER");
            inflect.Pascalise("CUStomer").ShouldBe("CUStomer");
            inflect.Pascalise("customer_name").ShouldBe("CustomerName");
            inflect.Pascalise("customer_first_name").ShouldBe("CustomerFirstName");
            inflect.Pascalise("customer_first_name_goes_here").ShouldBe("CustomerFirstNameGoesHere");
            inflect.Pascalise("customer name").ShouldBe("Customer name");
        }

        [Fact]
        public void Camelise_GivenContent_ShouldBeValid()
        {
            inflect.Camelise("Customer").ShouldBe("customer");
            inflect.Camelise("CUSTOMER").ShouldBe("cUSTOMER");
            inflect.Camelise("CUStomer").ShouldBe("cUStomer");
            inflect.Camelise("customer_name").ShouldBe("customerName");
            inflect.Camelise("customer_first_name").ShouldBe("customerFirstName");
            inflect.Camelise("customer_first_name_goes_here").ShouldBe("customerFirstNameGoesHere");
            inflect.Camelise("customer name").ShouldBe("customer name");
        }

        [Fact]
        public void Underscore_GivenContent_ShouldBeValid()
        {
            inflect.Underscore("SomeTitle").ShouldBe("some_title");
            inflect.Underscore("someTitle").ShouldBe("some_title");
            inflect.Underscore("some title").ShouldBe("some_title");
            inflect.Underscore("some title that will be underscored").ShouldBe("some_title_that_will_be_underscored");
            inflect.Underscore("SomeTitleThatWillBeUnderscored").ShouldBe("some_title_that_will_be_underscored");
        }

        [Fact]
        public void Capitalise_GivenContent_ShouldBeValid()
        {
            inflect.Capitalise("some title").ShouldBe("Some title");
            inflect.Capitalise("some Title").ShouldBe("Some title");
            inflect.Capitalise("SOMETITLE").ShouldBe("Sometitle");
            inflect.Capitalise("someTitle").ShouldBe("Sometitle");
            inflect.Capitalise("some title goes here").ShouldBe("Some title goes here");
            inflect.Capitalise("some TITLE").ShouldBe("Some title");
        }

        [Fact]
        public void Uncapitalise_GivenContent_ShouldBeValid()
        {
            inflect.Uncapitalise("Some title").ShouldBe("some title");
            inflect.Uncapitalise("Some Title").ShouldBe("some Title");
            inflect.Uncapitalise("SOMETITLE").ShouldBe("sOMETITLE");
            inflect.Uncapitalise("someTitle").ShouldBe("someTitle");
            inflect.Uncapitalise("Some title goes here").ShouldBe("some title goes here");
        }

        [Fact]
        public void Ordinalise_GivenNumber_ShouldBeValid()
        {
            inflect.Ordinalise(0).ShouldBe("0th");
            inflect.Ordinalise(1).ShouldBe("1st");
            inflect.Ordinalise(2).ShouldBe("2nd");
            inflect.Ordinalise(3).ShouldBe("3rd");
            inflect.Ordinalise(4).ShouldBe("4th");
            inflect.Ordinalise(5).ShouldBe("5th");
            inflect.Ordinalise(6).ShouldBe("6th");
            inflect.Ordinalise(7).ShouldBe("7th");
            inflect.Ordinalise(8).ShouldBe("8th");
            inflect.Ordinalise(9).ShouldBe("9th");
            inflect.Ordinalise(10).ShouldBe("10th");
            inflect.Ordinalise(11).ShouldBe("11th");
            inflect.Ordinalise(12).ShouldBe("12th");
            inflect.Ordinalise(13).ShouldBe("13th");
            inflect.Ordinalise(14).ShouldBe("14th");
            inflect.Ordinalise(20).ShouldBe("20th");
            inflect.Ordinalise(21).ShouldBe("21st");
            inflect.Ordinalise(22).ShouldBe("22nd");
            inflect.Ordinalise(23).ShouldBe("23rd");
            inflect.Ordinalise(24).ShouldBe("24th");
            inflect.Ordinalise(100).ShouldBe("100th");
            inflect.Ordinalise(101).ShouldBe("101st");
            inflect.Ordinalise(102).ShouldBe("102nd");
            inflect.Ordinalise(103).ShouldBe("103rd");
            inflect.Ordinalise(104).ShouldBe("104th");
            inflect.Ordinalise(110).ShouldBe("110th");
            inflect.Ordinalise(1000).ShouldBe("1000th");
            inflect.Ordinalise(1001).ShouldBe("1001st");
        }

        [Fact]
        public void Ordinalise_GivenStringNumber_ShouldBeValid()
        {
            inflect.Ordinalise("0").ShouldBe("0th");
            inflect.Ordinalise("1").ShouldBe("1st");
            inflect.Ordinalise("2").ShouldBe("2nd");
            inflect.Ordinalise("3").ShouldBe("3rd");
            inflect.Ordinalise("4").ShouldBe("4th");
            inflect.Ordinalise("5").ShouldBe("5th");
            inflect.Ordinalise("6").ShouldBe("6th");
            inflect.Ordinalise("7").ShouldBe("7th");
            inflect.Ordinalise("8").ShouldBe("8th");
            inflect.Ordinalise("9").ShouldBe("9th");
            inflect.Ordinalise("10").ShouldBe("10th");
            inflect.Ordinalise("11").ShouldBe("11th");
            inflect.Ordinalise("12").ShouldBe("12th");
            inflect.Ordinalise("13").ShouldBe("13th");
            inflect.Ordinalise("14").ShouldBe("14th");
            inflect.Ordinalise("20").ShouldBe("20th");
            inflect.Ordinalise("21").ShouldBe("21st");
            inflect.Ordinalise("22").ShouldBe("22nd");
            inflect.Ordinalise("23").ShouldBe("23rd");
            inflect.Ordinalise("24").ShouldBe("24th");
            inflect.Ordinalise("100").ShouldBe("100th");
            inflect.Ordinalise("101").ShouldBe("101st");
            inflect.Ordinalise("102").ShouldBe("102nd");
            inflect.Ordinalise("103").ShouldBe("103rd");
            inflect.Ordinalise("104").ShouldBe("104th");
            inflect.Ordinalise("110").ShouldBe("110th");
            inflect.Ordinalise("1000").ShouldBe("1000th");
            inflect.Ordinalise("1001").ShouldBe("1001st");
        }

        [Fact]
        public void Dasherise_GivenContent_ShouldBeValid()
        {
            inflect.Dasherise("some_title").ShouldBe("some-title");
            inflect.Dasherise("some-title").ShouldBe("some-title");
            inflect.Dasherise("some_title_goes_here").ShouldBe("some-title-goes-here");
            inflect.Dasherise("some_title and_another").ShouldBe("some-title and-another");
        }
    }
}