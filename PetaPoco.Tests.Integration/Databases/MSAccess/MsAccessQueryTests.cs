using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
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

        public override Task PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            Should.Throw<NotSupportedException>(() => base.PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection().Wait());
            return Task.CompletedTask;
        }

        public override Task PageAsync_ForPocoSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            Should.Throw<NotSupportedException>(() => base.PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection().Wait());
            return Task.CompletedTask;
        }
    }
}