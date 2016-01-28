// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/28</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySqlTests")]
    public class MySqlQueryTests : BaseQueryTests
    {
        public MySqlQueryTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}