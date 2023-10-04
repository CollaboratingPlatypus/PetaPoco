using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlTriageTests : BaseTriageTests
    {
        public MssqlTriageTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
