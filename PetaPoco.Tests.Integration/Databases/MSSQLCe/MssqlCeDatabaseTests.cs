using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeDatabaseTests : BaseDatabaseTests
    {
        public MssqlCeDatabaseTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}