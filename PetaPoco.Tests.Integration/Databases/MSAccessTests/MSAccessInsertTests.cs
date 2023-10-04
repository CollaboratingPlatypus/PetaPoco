using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessInsertTests : BaseInsertTests
    {
        public MsAccessInsertTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}