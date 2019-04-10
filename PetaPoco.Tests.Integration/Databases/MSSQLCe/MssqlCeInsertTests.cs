using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeInsertTests : BaseInsertTests
    {
        public MssqlCeInsertTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}