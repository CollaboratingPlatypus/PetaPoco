using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlCeMiscellaneousTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
