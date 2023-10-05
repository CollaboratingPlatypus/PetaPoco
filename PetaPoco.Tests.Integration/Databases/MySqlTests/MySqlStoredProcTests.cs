using System;
using MySql.Data.MySqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlStoredProcTests()
            : base(new MySqlDbProviderFactory())
        {
        }
    }
}
