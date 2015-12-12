using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCeTests")]
    public class MssqlCeUpdateTests : BaseUpdateTests
    {
        public MssqlCeUpdateTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}