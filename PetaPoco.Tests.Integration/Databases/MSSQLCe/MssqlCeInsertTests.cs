// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using PetaPoco.Tests.Integration.Databases.MSSQLCe;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCeTests")]
    public class MssqlCeInsertTests : BaseInsertTests
    {
        public MssqlCeInsertTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}