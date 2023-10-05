﻿using System;
using MySql.Data.MySqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlStoredProcTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}