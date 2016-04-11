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
}
