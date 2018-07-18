// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("FirebirdTests")]
    public class FirebirdDatabaseTests : BaseDatabaseTests
    {
        public FirebirdDatabaseTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}