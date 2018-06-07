// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/29</date>

using System;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccessTests")]
    public class MsAccessQueryTests : BaseQueryTests
    {
        public MsAccessQueryTests()
            : base(new MsAccessDBTestProvider())
        {
        }

        public override void Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            Should.Throw<NotSupportedException>(() => base.Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection());
        }

        public override void Page_ForPocoSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            Should.Throw<NotSupportedException>(() => base.Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection());
        }
    }
}