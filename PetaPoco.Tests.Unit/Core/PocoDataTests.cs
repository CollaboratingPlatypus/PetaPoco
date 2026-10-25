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
        public void GetFactory_GivenTypeWithNoPublicConstructor_ShouldThrow()
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
        public void PD_IncludeResultColumn_InAutoSelect()
        {
            var pd = PocoData.ForType(typeof(PocoWithIncludeResultColumn), new ConventionMapper());
            pd.QueryColumns.ShouldBe(new[] { "RegularProperty", "ResultProperty" });
        }

        [Fact]
        public void ForType_GivenDifferentMapperTypes_ShouldCreateDistinctColumnMetadata()
        {
            PocoData.FlushCaches();

            var type = typeof(PocoWithMappedName);

            var pd1 = PocoData.ForType(type, new FirstNameMapper());
            var pd2 = PocoData.ForType(type, new SecondNameMapper());

            pd1.GetColumnName(nameof(PocoWithMappedName.Value)).ShouldBe("FirstName");
            pd2.GetColumnName(nameof(PocoWithMappedName.Value)).ShouldBe("SecondName");
            pd1.ShouldNotBeSameAs(pd2);
        }

        [Fact]
        public void ForType_GivenSameMapperType_ShouldReuseCachedColumnMetadata()
        {
            PocoData.FlushCaches();

            var type = typeof(PocoWithMappedName);

            var pd1 = PocoData.ForType(type, new FirstNameMapper());
            var pd2 = PocoData.ForType(type, new FirstNameMapper());

            pd1.ShouldBeSameAs(pd2);
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

        public class PocoWithIncludeResultColumn
        {
            public string RegularProperty { get; set; }

            [ResultColumn(IncludeInAutoSelect.Yes)]
            public string ResultProperty { get; set; }
        }

        public class PocoWithMappedName
        {
            public string Value { get; set; }
        }

        /// <summary>
        /// A convention mapper that maps the <see cref="PocoWithMappedName.Value"/> property onto a named column, leaving the
        /// rest of the convention mapping in place.
        /// </summary>
        public abstract class NamedColumnMapper : ConventionMapper
        {
            protected NamedColumnMapper(string columnName)
            {
                var original = MapColumn;
                MapColumn = (ci, t, pi) =>
                {
                    if (!original(ci, t, pi))
                        return false;

                    if (pi.Name == nameof(PocoWithMappedName.Value))
                        ci.ColumnName = columnName;

                    return true;
                };
            }
        }

        /// <summary>
        /// A convention mapper that maps the <see cref="PocoWithMappedName.Value"/> property onto the "FirstName" column.
        /// </summary>
        public class FirstNameMapper : NamedColumnMapper
        {
            public FirstNameMapper() : base("FirstName") { }
        }

        /// <summary>
        /// A convention mapper that maps the <see cref="PocoWithMappedName.Value"/> property onto the "SecondName" column.
        /// </summary>
        public class SecondNameMapper : NamedColumnMapper
        {
            public SecondNameMapper() : base("SecondName") { }
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
