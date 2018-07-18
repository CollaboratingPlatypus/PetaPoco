// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCeTests")]
    public class MssqlCeExecuteTests : BaseExecuteTests
    {
        public MssqlCeExecuteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}