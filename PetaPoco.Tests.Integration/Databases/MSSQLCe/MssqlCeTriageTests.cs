using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeTriageTests : BaseTriageTests
    {
        public MssqlCeTriageTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
