using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlTriageTests : BaseTriageTests
    {
        public MssqlTriageTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
