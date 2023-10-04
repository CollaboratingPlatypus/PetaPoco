using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlDeleteTests : BaseDeleteTests
    {
        public MssqlDeleteTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
