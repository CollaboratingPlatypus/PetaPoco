using System;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCeTests")]
    public class MssqlCeQueryTests2 : BaseDatabase
    {
        public MssqlCeQueryTests2()
            : base(new MssqlCeDBTestProvider())
        {
        }

        [Fact]
        public virtual void Query_ForEnumWithUnderlyingType_ShouldConvertValues()
        {
            foreach (var sqltype in new[] { "TINYINT", "SMALLINT", "INTEGER", "BIGINT" })
            {
                var t1 = DB.Single<T1>($"SELECT CONVERT({sqltype}, 1) AS [C]");
                t1.C.ShouldBe((E1)1);
                var t2 = DB.Single<T2>($"SELECT CONVERT({sqltype}, 1) AS [C]");
                t2.C.ShouldBe((E2)1);
                var t3 = DB.Single<T3>($"SELECT CONVERT({sqltype}, 1) AS [C]");
                t3.C.ShouldBe((E3)1);
                var t4 = DB.Single<T4>($"SELECT CONVERT({sqltype}, 1) AS [C]");
                t4.C.ShouldBe((E4)1);
            }
        }

        [Fact]
        public virtual void Query_ForNullableTypes_ShouldConvertValues()
        {
            // DateTime => DateTime?
            var t1 = DB.Single<TestClass>("SELECT GETDATE() AS [DateTimeValue]");
            t1.DateTimeValue.HasValue.ShouldBeTrue();

            // INTEGER => enum?
            var t2 = DB.Single<TestClass>("SELECT 1 AS [EnumValue]");
            t2.EnumValue.HasValue.ShouldBeTrue();
            t2.EnumValue.Value.ShouldBe((E3)1);

            // TEXT => enum?
            var t3 = DB.Single<TestClass>("SELECT 'Hello' AS [EnumValue]");
            t3.EnumValue.HasValue.ShouldBeTrue();
            t3.EnumValue.Value.ShouldBe(E3.Hello);

            // TEXT => Guid?
            var t4 = DB.Single<TestClass>("SELECT 'ffb68770-dfcc-49b2-b800-d499477af784' AS [GuidValue]");
            t4.GuidValue.HasValue.ShouldBeTrue();
            t4.GuidValue.Value.ShouldBe(Guid.Parse("ffb68770-dfcc-49b2-b800-d499477af784"));

            // INTEGER => int?
            var t5 = DB.Single<TestClass>("SELECT 1 AS [IntValue]");
            t5.IntValue.HasValue.ShouldBeTrue();
            t5.IntValue.Value.ShouldBe(1);

            // SMALLINT => int?
            var t6 = DB.Single<TestClass>("SELECT CONVERT(SMALLINT, 1) AS [IntValue]");
            t6.IntValue.HasValue.ShouldBeTrue();
            t6.IntValue.Value.ShouldBe(1);

            // BIGINT => int?
            var t7 = DB.Single<TestClass>("SELECT CONVERT(BIGINT, 1) AS [IntValue]");
            t7.IntValue.HasValue.ShouldBeTrue();
            t7.IntValue.Value.ShouldBe(1);
        }
    }

    internal enum E1 : byte
    {
    }

    internal class T1
    {
        public E1 C { get; set; }
    }

    internal enum E2 : short
    {
    }

    internal class T2
    {
        public E2 C { get; set; }
    }

    internal enum E3 : int
    {
        Hello = 1,
    }

    internal class T3
    {
        public E3 C { get; set; }
    }

    internal enum E4 : long
    {
    }

    internal class T4
    {
        public E4 C { get; set; }
    }

    internal class TestClass
    {
        public DateTime? DateTimeValue { get; set; }
        public E3? EnumValue { get; set; }
        public Guid? GuidValue { get; set; }
        public int? IntValue { get; set; }
    }
}
