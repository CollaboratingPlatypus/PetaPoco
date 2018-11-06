using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using PetaPoco.Internal;

namespace PetaPoco.Tests.Unit.Utilities
{
    public class ParametersHelperTests
    {
        [Fact]
        public void EnsureParamPrefix_int_ShouldWork()
        {
            int input = 34;
            string output = input.EnsureParamPrefix("@");
            output.ShouldBe("@34");
        }

        [Theory]
        [InlineData("@foo")]
        [InlineData("$foo")]
        [InlineData(":foo")]
        [InlineData("^foo")]
        [InlineData("foo")]
        public void EnsureParamPrefix_string_ShouldWork(string input)
        {
            string output = input.EnsureParamPrefix("@");
            output.ShouldBe("@foo");
        }

        [Theory]
        [InlineData(":")]
        [InlineData("^")]        
        public void ReplaceParamPrefix_ShouldWork(string prefix)
        {
            var input = "This @foo is @bar";
            var expected = $"This {prefix}foo is {prefix}bar";
            var output = input.ReplaceParamPrefix(prefix);
            output.ShouldBe(expected);
        }
    }
}
