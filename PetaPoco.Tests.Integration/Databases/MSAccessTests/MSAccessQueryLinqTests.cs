using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessQueryLinqTests : BaseQueryLinqTests
    {
        public MsAccessQueryLinqTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}