using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using PetaPoco;

namespace PetaPoco.Tests.Unit.Core
{
    public class AnsiStringTests
    {
        [Fact]
        public void Constructor_Returns_Valid_Object()
        {
            var input = "Some string value";
            var output = new AnsiString(input);

            output.ShouldBeOfType<AnsiString>();
            output.Value.ShouldBe(input);
        }

        [Fact]
        public void Explicit_Conversion_Works()
        {
            var input = "An ordinary string value";
            var output = (AnsiString)input;

            output.ShouldBeOfType<AnsiString>();
            output.Value.ShouldBe(input);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(List<string>))]
        public void Explicit_Conversion_Non_Strings_Should_Throw(Type type)
        {
            var input = Activator.CreateInstance(type);
            Action act = () => { var output = (AnsiString)input; };
            act.ShouldThrow<InvalidCastException>();
        }

        [Theory]
        [InlineData("njkasnd")]
        [InlineData(4720)]
        [InlineData(567.234)]
        public void Extension_Method_Works(object input)
        {
            var output = input.ToAnsiString();

            output.ShouldBeOfType<AnsiString>();
            output.Value.ShouldBe(input.ToString());
        }
    }
}
