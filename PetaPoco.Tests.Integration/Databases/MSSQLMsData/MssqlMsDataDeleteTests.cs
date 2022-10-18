using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    [Trait("Category", "MssqlMsData")]
    public class MssqlMsDataDeleteTests : BaseDeleteTests
    {
        public MssqlMsDataDeleteTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
