// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/06</date>

using PetaPoco.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Providers
{
    public class SqlServerDatabaseProviderTests
    {
        private readonly SqlServerDatabaseProvider _provider = new SqlServerDatabaseProvider();

        [Theory]
        [InlineData("column.name", "[column.name]")]
        [InlineData("column name", "[column name]")]
        public void EscapeSqlIdentifier_GivenInput_ShouldBeExpected(string input, string expected)
        {
            _provider.EscapeSqlIdentifier(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("column.name", "column.name")]
        [InlineData("column name", "[column name]")]
        public void EscapeTableName_GivenInput_ShouldBeExpected(string input, string expected)
        {
            _provider.EscapeTableName(input).ShouldBe(expected);
        }
    }
}