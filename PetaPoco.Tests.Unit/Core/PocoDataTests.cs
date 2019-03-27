using System;
using PetaPoco.Core;
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
    }
}