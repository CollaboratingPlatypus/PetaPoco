using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeTriageTests : TriageTests
    {
        public SqlServerCeTriageTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
