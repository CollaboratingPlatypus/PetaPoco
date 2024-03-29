﻿using System;
using MySql.Data.MySqlClient;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MariaDBStoredProcTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
