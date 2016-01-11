// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/11</date>

using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDbTests")]
    public class MariaDbUpdateTests : BaseUpdateTests
    {
        public MariaDbUpdateTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}