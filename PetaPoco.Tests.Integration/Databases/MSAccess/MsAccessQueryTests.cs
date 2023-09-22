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

        [Fact(Skip = "Paging not supported by provider.")]
        public override void PageCount_GivenSqlWithGroupBy_ShouldReturnEntireQueryCount() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSql_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForPocoGivenSql_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void FetchAsyncWithPaging_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSql_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForPocoGivenSql_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async void SkipAndTakeAsync_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;
    }
}
