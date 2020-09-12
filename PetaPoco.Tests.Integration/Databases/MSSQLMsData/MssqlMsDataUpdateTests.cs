using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataUpdateTests : BaseUpdateTests
    {
        public MssqlMsDataUpdateTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}