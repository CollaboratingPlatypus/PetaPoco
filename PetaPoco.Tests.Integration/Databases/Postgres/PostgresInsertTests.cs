// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("PostgresTests")]
    public class PostgresInsertTests : BaseInsertTests
    {
        public PostgresInsertTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}