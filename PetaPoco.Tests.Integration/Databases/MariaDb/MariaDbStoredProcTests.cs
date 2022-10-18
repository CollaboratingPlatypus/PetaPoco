using System;
using MySql.Data.MySqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    [Trait("Category", "MariaDb")]
    public class MariaDbStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MariaDbStoredProcTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
