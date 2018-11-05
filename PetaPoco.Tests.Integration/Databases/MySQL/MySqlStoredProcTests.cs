// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using MySql.Data.MySqlClient;
using System;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySqlTests")]
    public class MySqlStoredProcTests: BaseStoredProcTests
    {
        public MySqlStoredProcTests()
            : base(new MySqlDBTestProvider())
        {
        }

        protected override Type DataParameterType => typeof(MySqlParameter);
    }
}
