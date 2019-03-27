using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeQueryLinqTests : BaseQueryLinqTests
    {
        public MssqlCeQueryLinqTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}