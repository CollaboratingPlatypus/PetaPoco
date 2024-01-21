using System;
using MySql.Data.MySqlClient;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlStoredProcTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
