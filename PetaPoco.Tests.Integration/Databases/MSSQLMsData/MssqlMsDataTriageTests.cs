using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataTriageTests : BaseTriageTests
    {
        public MssqlMsDataTriageTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
