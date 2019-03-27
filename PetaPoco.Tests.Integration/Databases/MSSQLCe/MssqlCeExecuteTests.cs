using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeExecuteTests : BaseExecuteTests
    {
        public MssqlCeExecuteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}