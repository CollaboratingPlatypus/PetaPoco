using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataTriageTests : BaseTriageTests
    {
        public MssqlMsDataTriageTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
