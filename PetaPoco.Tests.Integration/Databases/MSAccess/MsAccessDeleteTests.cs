// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/29</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccessTests")]
    public class MsAccessDeleteTests : BaseDeleteTests
    {
        public MsAccessDeleteTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}