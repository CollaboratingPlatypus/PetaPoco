using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataInsertTests : BaseInsertTests
    {
        public MssqlMsDataInsertTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}