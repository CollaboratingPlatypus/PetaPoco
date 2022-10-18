using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    [Trait("Category", "MssqlMsData")]
    public class MssqlMsDataUpdateTests : BaseUpdateTests
    {
        public MssqlMsDataUpdateTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
