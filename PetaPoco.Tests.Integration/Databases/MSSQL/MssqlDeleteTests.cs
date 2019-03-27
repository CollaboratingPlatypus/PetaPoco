using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlDeleteTests : BaseDeleteTests
    {
        public MssqlDeleteTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}