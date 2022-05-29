using System;
using PetaPoco.Core;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class PocoDataTests
    {
        [Fact]
        public void GetFactory_GivenTypeWithNotPublicConstructor_ShouldThrow()
        {
            var pd = PocoData.ForObject(TestEntity.Instance, "Id", new ConventionMapper());

            Should.Throw<InvalidOperationException>(() => pd.GetFactory("", "", 1, 1, null, null));
        }

        [Fact]
        public void PD_ResultColumn_NotInAutoSelect()
        {
            var pd = PocoData.ForType(typeof(PocoWithResultColumn), new ConventionMapper());
            pd.QueryColumns.ShouldBe(new[] { "RegularProperty" });
        }

        [Fact]
        public void PD_IncludedResultColumn_InAutoSelect()
        {
            var pd = PocoData.ForType(typeof(PocoWithIncludedResultColumn), new ConventionMapper());
            pd.QueryColumns.ShouldBe(new[] { "RegularProperty", "ResultProperty" });
        }

        public class TestEntity
        {
            public static TestEntity Instance => new TestEntity("");

            private TestEntity(string arg1)
            {
            }
        }

        public class PocoWithResultColumn
        {
            public string RegularProperty { get; set; }

            [ResultColumn]
            public string ResultProperty { get; set; }
        }

        public class PocoWithIncludedResultColumn
        {
            public string RegularProperty { get; set; }

            [ResultColumn(IncludeInAutoSelect.Yes)]
            public string ResultProperty { get; set; }
        }

        [Theory]
        [InlineData(typeof(Child1))]
        [InlineData(typeof(Child2))]
        [InlineData(typeof(Child3))]
        [InlineData(typeof(Child4))]
        public void Atrributes_Should_Inherit(Type type)
        {
            var pd = PocoData.ForType(type, new ConventionMapper());
            pd.Columns.ShouldContainKey("Foo");
            pd.Columns.ShouldNotContainKey("Ignored");
        }
    }
}