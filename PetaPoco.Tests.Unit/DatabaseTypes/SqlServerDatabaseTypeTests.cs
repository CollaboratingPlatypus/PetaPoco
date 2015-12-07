// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

using PetaPoco.DatabaseTypes;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.DatabaseTypes
{
    public class SqlServerDatabaseTypeTests
    {
        private readonly SqlServerDatabaseType _type = new SqlServerDatabaseType();

        [Theory]
        [InlineData("column.name", "[column.name]")]
        [InlineData("column name", "[column name]")]
        public void EscapeSqlIdentifier_GivenInput_ShouldBeValid(string input, string expected)
        {
            _type.EscapeSqlIdentifier(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("column.name", "column.name")]
        [InlineData("column name", "[column name]")]
        public void EscapeTableName_GivenInput_ShouldBeValid(string input, string expected)
        {
            _type.EscapeTableName(input).ShouldBe(expected);
        }
    }
}